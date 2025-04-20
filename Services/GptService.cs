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
    }
}