using MaterialDesignColors.Recommended;
using TaskForge.Models.Entities;

namespace TaskForge.Models.Repositories
{
    public interface IUserSession
    {
        int CurrentUserId { get; set; }
        string CurrentUserName { get; set; }
        string CurrentUserEmail { get; set; }
        int CurrentUserLevel { get; set; }
        int CurrentUserTotalEx {  get; set; }
        DateTime CurrentUserCreatedAt { get; set; }
        void SetCurrentUser(User user);
        void Clear();
        
    }
    public class UserSession : IUserSession
    {
        public int CurrentUserId { get; set; }
        public string CurrentUserName { get; set; } = string.Empty;
        public string CurrentUserEmail { get; set; } = string.Empty;
        public int CurrentUserLevel {  get; set; }
        public int CurrentUserTotalEx {  get; set; }
        public DateTime CurrentUserCreatedAt {  get; set; }
        public bool IsLoggedIn => CurrentUserId > 0;


        public void SetCurrentUser(User user)
        {
            CurrentUserId = user.Id;
            CurrentUserName = user.Name;
            CurrentUserEmail = user.Email;
            CurrentUserLevel = user.Level;
            CurrentUserTotalEx = user.Total_exp;
            CurrentUserCreatedAt = user.Created_at;
        }

        public void Clear()
        {
            CurrentUserId = 0;
            CurrentUserName = string.Empty;
            CurrentUserEmail = string.Empty;
            CurrentUserLevel = 0;
            CurrentUserTotalEx = 0;
            CurrentUserCreatedAt = DateTime.Now;
        }
    }
}
