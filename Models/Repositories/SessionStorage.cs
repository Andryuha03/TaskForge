using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TaskForge.Models.Repositories
{
    public class SessionStorage : ISessionStorage
    {
        private readonly string _filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TaskForge",
            "session.dat");

        public SessionStorage()
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        public void SaveUserId(int userId)
        {
            var date = Encoding.UTF8.GetBytes(userId.ToString());
            var encrypted = ProtectedData.Protect(date, null, DataProtectionScope.CurrentUser);
            File.WriteAllBytes(_filePath, encrypted);
        }
        public int? LoadUserId()
        {
            if (!File.Exists(_filePath))
                return null;
            try
            {
                var encrypted = File.ReadAllBytes(_filePath);
                var decrypted = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
                var userIdStr = Encoding.UTF8.GetString(decrypted);
                return int.Parse(userIdStr);
            }
            catch
            {
                return null;
            }

        }
        public void Clear()
        {
            if (File.Exists(_filePath))
                File.Delete(_filePath);
        }
    }


    public interface ISessionStorage
    {
        void SaveUserId(int userId);
        int? LoadUserId();
        void Clear();
    }
}
