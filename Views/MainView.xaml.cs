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

            if (string.IsNullOrWhiteSpace(keyword) || string.IsNullOrWhiteSpace(style))
            {
                MessageBox.Show("키워드와 스타일을 모두 입력해주세요.");
                return;
            }

            // 로딩 메시지 출력
            PreviewTextBlock.Text = "GPT가 글을 생성 중입니다...";

            try
            {
                string prompt = $"'{keyword}' 키워드로 {style} 스타일의 블로그 글을 작성해줘.";
                string result = await GptService.GenerateBlogContentAsync(prompt);
                PreviewTextBlock.Text = result;
            }
            catch (System.Exception ex)
            {
                PreviewTextBlock.Text = "글 생성 중 오류가 발생했습니다.\n" + ex.Message;
            }
        }
    }
}