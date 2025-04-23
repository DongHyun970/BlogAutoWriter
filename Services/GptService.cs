using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlogAutoWriter.Services
{
    public static class GptService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string ApiUrl = "https://api.openai.com/v1/chat/completions"; // 예시용 URL
        private const string ApiKey = "YOUR_API_KEY"; // 실제 키로 교체

        public static async Task<string> GenerateBlogContentAsync(string prompt)
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var requestJson = JsonSerializer.Serialize(requestBody);
            var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl);
            request.Headers.Add("Authorization", $"Bearer {ApiKey}");
            request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var result = doc.RootElement
                            .GetProperty("choices")[0]
                            .GetProperty("message")
                            .GetProperty("content")
                            .GetString();

            return result ?? "응답이 비어 있습니다.";
        }

        public static string BuildPrompt(string keyword)
        {
            return $@"
        '{keyword}'에 대해 블로그 글을 써줘.
        글은 서론, 본론1, 본론2, 결론의 구조로 구성해줘.

        각 섹션은 제목으로 시작하고, 본문은 2~3문단 이상 포함해.
        중요한 항목은 리스트로 정리하고,
        숫자나 비교 정보는 간단한 표로 작성해줘.

        HTML 태그는 쓰지 말고, 아래와 같은 형식으로 써줘:
        - 리스트는 `- 항목` 형식
        - 표는 `|`로 구분한 텍스트 테이블

        총 글자 수는 2000자 이상 4000자 이내로, 자연스럽고 친근한 말투로 써줘.
        ";
        }

    }
}