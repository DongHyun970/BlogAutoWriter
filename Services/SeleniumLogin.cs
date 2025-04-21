using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;

namespace BlogAutoWriter.Services
{
    public static class SeleniumLogin
    {
        private static ChromeDriver? driver;

        private static void InitDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            driver = new ChromeDriver(options);
        }

        public static void LoginWithKakao()
        {
            InitDriver();
            driver!.Navigate().GoToUrl("https://www.tistory.com/auth/login");

            Thread.Sleep(1000); // 페이지 로딩 대기
            Console.WriteLine("🟡 카카오 로그인 페이지에 진입했습니다.");
            // TODO: 아이디, 비번 자동 입력 및 로그인 후 티스토리 리다이렉트 구현
        }

        public static void LoginWithTistory()
        {
            InitDriver();
            driver!.Navigate().GoToUrl("https://www.tistory.com/auth/login");

            Thread.Sleep(1000);
            Console.WriteLine("🟡 티스토리 이메일 로그인 페이지 진입 완료.");
            // TODO: 티스토리 이메일 로그인 자동화 구현
        }

        public static void Quit()
        {
            driver?.Quit();
        }
    }
}
