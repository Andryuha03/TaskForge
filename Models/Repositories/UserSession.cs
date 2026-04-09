using MaterialDesignColors.Recommended;
using TaskForge.Models.Entities;

namespace TaskForge.Models.Repositories
{
    public interface IUserSession
    {
        int CurrentUserId { get; }
        string CurrentUserName { get; }
        string CurrentUserEmail { get; }
        int CurrentUserLevel { get; }
        int CurrentUserTotalEx {  get; }
        DateTime CurrentUserCreatedAt { get; }
        void SetCurrentUser(User user);
        void Clear();
        
    }
    public class UserSession : IUserSession
    {
        public int CurrentUserId { get; private set; }
        public string CurrentUserName { get; private set; } = string.Empty;
        public string CurrentUserEmail { get; private set; } = string.Empty;
        public int CurrentUserLevel {  get; private set; }
        public int CurrentUserTotalEx {  get; private set; }
        public DateTime CurrentUserCreatedAt {  get; private set; }
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
