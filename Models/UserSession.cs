using System;

namespace BlogAutoWriter.Models
{
    public class UserSession
    {
        public string UserId { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public int ValidDays { get; set; }

        public bool IsExpired()
        {
            return DateTime.Now > StartDate.AddDays(ValidDays);
        }

        public TimeSpan RemainingTime()
        {
            return StartDate.AddDays(ValidDays) - DateTime.Now;
        }
    }

    public static class SessionManager
    {
        public static UserSession? CurrentUser { get; set; }

        public static bool IsSessionExpired =>
            CurrentUser == null || CurrentUser.IsExpired();
    }
}