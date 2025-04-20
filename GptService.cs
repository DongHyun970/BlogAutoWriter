using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BlogAutoWriter.Models;

namespace BlogAutoWriter.Services
{
    public class GptService
    {
        private static readonly HttpClient _httpClient = new();

        public async Task<List<string>> GetRecommendedKeywords()
        {
            var apiKey = AppSettings.Current.OpenAiApiKey;
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new Exception("OpenAI API 키가 설정되지 않았습니다.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "user", content = "블로그 주제로 쓸만한 인기 키워드 5개를 추천해줘." }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var json = await response.Content.ReadAsStringAsync();

            var doc = JsonDocument.Parse(json);
            var text = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            return ParseKeywords(text ?? "");
        }

        private List<string> ParseKeywords(string text)
        {
            var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var result = new List<string>();

            foreach (var line in lines)
            {
                var keyword = line.Trim().TrimStart('-', '*', '0', '1', '2', '3', '4', '5', '.', ' ');
                if (!string.IsNullOrWhiteSpace(keyword))
                    result.Add(keyword);
            }

            return result;
        }
    }
}
