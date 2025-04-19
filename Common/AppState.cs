using BlogAutoWriter.Models;

namespace BlogAutoWriter.Common
{
    public static class AppState
    {
        public static UserInfo? CurrentUser { get; set; }
    }
}
