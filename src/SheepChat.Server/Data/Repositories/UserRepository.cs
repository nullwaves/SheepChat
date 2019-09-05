using SheepChat.Server.Data.Models;
using System;
using System.Linq;
using BCrypt;

namespace SheepChat.Server.Data.Repositories
{
    public static class UserRepository
    {
        public static IOrderedQueryable<User> All
        {
            get
            {
                using (var repo = DataManager.OpenDocumentSession<User>())
                {
                    return repo.Query<User>();
                }
            }
        }

        public static void Save(User user) => DocumentRepository<User>.Save(user);

        public static User Load(string id) => DocumentRepository<User>.Load(id);

        public static User Create(string username, string password)
        {
            using (var repo = DataManager.OpenDocumentSession<User>())
            {
                User user = new User {
                    Username = username,
                    Registered = DateTime.Now,
                    UserRole = UserRole.Member
                };

                user = SetPassword(user, password);
                return user;
            }
        }

        public static User SetPassword(User user, string password)
        {
            using (var repo = DataManager.OpenDocumentSession<User>())
            {
                string hash = BCryptHelper.HashPassword(password, BCryptHelper.GenerateSalt());
                user.Password = hash;
                repo.Upsert(user);
                return user;
            }
        }

        public static User Authenticate(string username, string password)
        {
            using (var repo = DataManager.OpenDocumentSession<User>())
            {
                var user = (from u in repo.Query<User>()
                            where u.Username.Equals(username)
                            select u).FirstOrDefault();

                if (user == null) return null;
                if (BCryptHelper.CheckPassword(password, user.Password))
                {
                    user.LastLogin = DateTime.Now;
                    return user;
                }
                return null;
            }
        }
    }
}
