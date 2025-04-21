using System.Windows;
using System.Windows.Controls;
using BlogAutoWriter.Services;

namespace BlogAutoWriter.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            string keyword = KeywordBox.Text.Trim();
            var selected = StyleSelector.SelectedItem as ComboBoxItem;
            string style = selected?.Content?.ToString() ?? "";

            var loginItem = BlogLoginMethodComboBox.SelectedItem as ComboBoxItem;
            string loginMethod = loginItem?.Content?.ToString() ?? "카카오 로그인";

            if (string.IsNullOrWhiteSpace(keyword) || string.IsNullOrWhiteSpace(style))
            {
                MessageBox.Show("키워드와 스타일을 모두 입력해주세요.");
                return;
            }

            PreviewTextBlock.Text = "GPT가 글을 생성 중입니다...";

            try
            {
                string prompt = $"'{keyword}' 키워드로 {style} 스타일의 블로그 글을 작성해줘.";
                string result = await GptService.GenerateBlogContentAsync(prompt);
                PreviewTextBlock.Text = result;

                // ✨ 선택된 로그인 방식에 따라 셀레니움 자동화 분기
                if (loginMethod == "카카오 로그인")
                {
                    MessageBox.Show("카카오 로그인 자동화 경로로 이동합니다. (예정)");
                    // SeleniumLogin.LoginWithKakao();
                }
                else if (loginMethod == "티스토리 이메일 로그인")
                {
                    MessageBox.Show("티스토리 로그인 자동화 경로로 이동합니다. (예정)");
                    // SeleniumLogin.LoginWithTistory();
                }
            }
            catch (System.Exception ex)
            {
                PreviewTextBlock.Text = "글 생성 중 오류가 발생했습니다.\n" + ex.Message;
            }
        }
    }
}
