// ✅ 통합 버전: 쿠키 기반 로그인 + 게시까지 완전체
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace TistoryAutoPoster
{
    public static class IntegratedPublisher
    {
        private const string CookieFilePath = "cookies.json";

        public static void LoginAndPublishPost(string email, string password, string title, string htmlContent, string tags, string categoryName)
        {
            string extractedTitle = title;
            string extractedContent = htmlContent;

            if (string.IsNullOrEmpty(extractedTitle))
            {
                var match = Regex.Match(htmlContent, "<h1[^>]*>(.*?)</h1>", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    extractedTitle = match.Groups[1].Value.Trim();
                    extractedContent = Regex.Replace(htmlContent, "<h1[^>]*>.*?</h1>", "", RegexOptions.IgnoreCase).Trim();
                }
            }

            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            using var driver = new ChromeDriver(options);
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            try
            {
                driver.Navigate().GoToUrl("https://www.tistory.com");

                // ✅ 쿠키가 있으면 바로 로드 시도
                if (File.Exists(CookieFilePath))
                {
                    var cookies = JsonSerializer.Deserialize<CookieData[]>(File.ReadAllText(CookieFilePath));
                    foreach (var cookie in cookies!)
                    {
                        driver.Manage().Cookies.AddCookie(new Cookie(cookie.name, cookie.value, cookie.domain, cookie.path, cookie.expiry));
                    }
                    driver.Navigate().Refresh();
                    Thread.Sleep(3000);
                }

                // ✅ 로그인 여부 확인
                bool loggedIn = false;
                try
                {
                    driver.Navigate().GoToUrl("https://www.tistory.com/");
                    wait.Until(d => d.FindElement(By.XPath("//*[@id='mArticle']/div/div[2]/div/div[1]/div[2]/a[1]")));
                    loggedIn = true;
                }
                catch
                {
                    loggedIn = false;
                }

                if (!loggedIn)
                {
                    // 🔐 로그인 진행
                    driver.Navigate().GoToUrl("https://www.tistory.com/auth/login");
                    Thread.Sleep(500);

                    wait.Until(d => d.FindElement(By.XPath("//*[@id='cMain']/div/div/div/div/a[2]/span[2]"))).Click();
                    Thread.Sleep(500);

                    wait.Until(d => d.FindElement(By.Id("loginId--1"))).SendKeys(email);
                    wait.Until(d => d.FindElement(By.Id("password--2"))).SendKeys(password);
                    wait.Until(d => d.FindElement(By.XPath("//*[@id='mainContent']/div/div/form/div[4]/button[1]"))).Click();
                    Thread.Sleep(5000);

                    // ✅ 로그인 성공 후 쿠키 저장
                    var allCookies = driver.Manage().Cookies.AllCookies;
                    var cookieData = allCookies.Select(c => new CookieData
                    {
                        domain = c.Domain ?? "",
                        name = c.Name,
                        value = c.Value,
                        path = c.Path ?? "/",
                        expiry = c.Expiry
                    });
                    File.WriteAllText(CookieFilePath, JsonSerializer.Serialize(cookieData, new JsonSerializerOptions { WriteIndented = true }));
                }

                // ✍️ 새 글 작성 진입
                driver.Navigate().GoToUrl("https://www.tistory.com/");
                Thread.Sleep(500);
                wait.Until(d => d.FindElement(By.XPath("//*[@id='mArticle']/div/div[2]/div/div[1]/div[2]/a[1]"))).Click();
                Thread.Sleep(2000);

                driver.SwitchTo().Window(driver.WindowHandles[1]);
                new Actions(driver).SendKeys(Keys.Escape).Perform();
                Thread.Sleep(500);

                // 🔧 HTML 모드 전환
                wait.Until(d => d.FindElement(By.Id("editor-mode-layer-btn-open"))).Click();
                Thread.Sleep(500);
                wait.Until(d => d.FindElement(By.Id("editor-mode-html-text"))).Click();
                Thread.Sleep(1000);
                try { driver.SwitchTo().Alert().Accept(); } catch { }
                Thread.Sleep(500);

                // ✏️ 제목 입력
                wait.Until(d => d.FindElement(By.Id("post-title-inp"))).SendKeys(extractedTitle);

                // 📂 카테고리 선택
                wait.Until(d => d.FindElement(By.Id("category-btn"))).Click();
                Thread.Sleep(500);
                string categoryXPath = categoryName.Contains("/")
                    ? $"//span[text()='- {categoryName.Split('/')[1]}']"
                    : $"//span[text()='{categoryName}']";
                wait.Until(d => d.FindElement(By.XPath(categoryXPath))).Click();

                // 📝 HTML 삽입 (CodeMirror + TinyMCE)
                var jsExecutor = (IJavaScriptExecutor)driver;
                string updateScript = @"
                    var content = arguments[0];
                    var cm = document.querySelector('.CodeMirror').CodeMirror;
                    if (cm) { cm.setValue(content); cm.save(); cm.refresh(); }
                    if (window.tinymce && tinymce.activeEditor) {
                        tinymce.activeEditor.setContent(content);
                        tinymce.activeEditor.save();
                    }
                    var hiddenEditor = document.getElementById('editor-tistory');
                    if (hiddenEditor) {
                        hiddenEditor.value = content;
                        ['change','input','blur'].forEach(ev => {
                            hiddenEditor.dispatchEvent(new Event(ev, { bubbles: true }));
                        });
                    }";
                jsExecutor.ExecuteScript(updateScript, extractedContent);
                Thread.Sleep(1000);

                // 🏷️ 태그 입력
                wait.Until(d => d.FindElement(By.Id("tagText"))).SendKeys(tags + Keys.Enter);

                // 🚀 게시 버튼 클릭
                wait.Until(d => d.FindElement(By.Id("publish-layer-btn"))).Click();
                Thread.Sleep(500);
                wait.Until(d => d.FindElement(By.Id("open20"))).Click();
                Thread.Sleep(500);
                wait.Until(d => d.FindElement(By.Id("publish-btn"))).Click();

                MessageBox.Show("✅ 글 발행 완료!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ 게시 중 오류 발생: " + ex.Message);
            }
            finally
            {
                Thread.Sleep(2000);
                driver.Quit();
            }
        }

        private class CookieData
        {
            public string name { get; set; } = "";
            public string value { get; set; } = "";
            public string domain { get; set; } = "";
            public string path { get; set; } = "";
            public DateTime? expiry { get; set; }
        }

    }
}
