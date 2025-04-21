using System;
using System.Windows;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace BlogAutoWriter.Services
{
    public class LoginService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private const string LoginUrl = "https://script.google.com/macros/s/AKfycbwUxlxvupeXpJ3PB-uTzeuX4dqro7DTI6N8IJAyQqnRJitXpcv6KGfoWV7L_FMaECRj/exec"; // 실제 URL로 교체

    public class LoginResult
    {
        public bool Success { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

        // 날짜 문자열로 받아서 파싱
        public string StartDateRaw { get; set; } = string.Empty;
        public int ValidDays { get; set; }

        [JsonIgnore]
        public DateTime StartDate => DateTime.TryParse(StartDateRaw, out var dt) ? dt : DateTime.Now;
    }


        public static async Task<LoginResult> LoginAsync(string userid, string plainPassword)
        {
            var passwordHash = ComputeSha256Hash(plainPassword);

            var payload = new
            {
                userid = userid,
                password = passwordHash
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(LoginUrl, content);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                // 👇 여기에 추가
                MessageBox.Show("💬 서버 응답:\n" + json);

                var result = JsonSerializer.Deserialize<LoginResult>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result ?? new LoginResult { Success = false, Reason = "invalid_response" };
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    Success = false,
                    Reason = "exception: " + ex.Message
                };
            }
        }

        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}