using Markdig;

namespace BlogAutoWriter.Services
{
    public static class MarkdownToHtmlConverter
    {
        public static string Convert(string markdown)
        {
            // 기본 마크다운을 HTML로 변환
            var pipeline = new MarkdownPipelineBuilder()
                            .UseAdvancedExtensions()
                            .Build();

            string html = Markdown.ToHtml(markdown, pipeline);

            // 커스텀 CSS 클래스를 삽입 (옵션)
            html = html
                .Replace("<table>", "<table class='baw-table'>")
                .Replace("<h1>", "<h1 class='baw-h1'>")
                .Replace("<h2>", "<h2 class='baw-h2'>");

            return html;
        }
    }
}
