using MaterialDesignColors.Recommended;
using TaskForge.Models.Entities;

namespace TaskForge.Models.Repositories
{
    public static class UserSession
    {
        public static int CurrentUserId { get; set; }
        public static string CurrentUserName { get; set; }
        public static string CurrentPassword { get; set; }
        public static string CurrentEmail {  get; set; }

        public static void SetCurrentUser(User user)
        {
            CurrentUserId = user.Id;
            CurrentUserName = user.Name;
            CurrentPassword = user.Password;
            CurrentEmail = user.Email;
        }

        public static void Clear()
        {
            CurrentUserId = 0;
            CurrentUserName = null;
            CurrentPassword = null;
            CurrentEmail = null;
        }

        public static bool IsLoggedIn => CurrentUserId > 0;
    }
}
