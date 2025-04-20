using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlogAutoWriter.Services
{
    public class LoginService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private const string LoginUrl = "https://script.google.com/macros/s/AKfycbwUxlxvupeXpJ3PB-uTzeuX4dqro7DTI6N8IJAyQqnRJitXpcv6KGfoWV7L_FMaECRj/exec"; // TODO: 실제 URL로 교체

        public class LoginResult
        {
            public bool Success { get; set; }
            public string Reason { get; set; }
            public string Grade { get; set; }
        }

        public static async Task<LoginResult> LoginAsync(string userid, string passwordHash)
        {
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
                var result = JsonSerializer.Deserialize<LoginResult>(json);
                return result;
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    Success = false,
                    Reason = "network_error: " + ex.Message
                };
            }
        }
    }
}
