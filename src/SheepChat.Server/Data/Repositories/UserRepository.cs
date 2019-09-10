using SheepChat.Server.Data.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SheepChat.Server.Data.Repositories
{
    public static class UserRepository
    {
        public static IEnumerable<User> All
        {
            get
            {
                using (var repo = DataManager.OpenDocumentSession<User>())
                {
                    return repo.Query();
                }
            }
        }

        public static void Save(User user) => DocumentRepository<User>.Save(user);

        public static User Load(string id) => DocumentRepository<User>.Load(id);

        public static User Create(string username, string password)
        {
            User user = new User
            {
                Username = username,
                Registered = DateTime.Now,
                UserRole = UserRole.Member
            };

            user = SetPassword(user, password);

            using (var repo = DataManager.OpenDocumentSession<User>())
            {
                var nameTaken = (from u in repo.Query() where u.Username.ToLower().Equals(user.Username.ToLower()) select u).FirstOrDefault();
                if (nameTaken == null)
                {
                    repo.Insert(user);
                }
                else
                {
                    return null;
                }
            }

            return user;
        }

        public static User FindByUsername(string username)
        {
            using (var repo = DataManager.OpenDocumentSession<User>())
            {
                var user = (from u in repo.Query()
                            where u.Username.Equals(username)
                            select u).FirstOrDefault();
                return user;
            }
        }

        public static User SetPassword(User user, string password)
        {
            using (var repo = DataManager.OpenDocumentSession<User>())
            {
                string hash = BCrypt.Net.BCrypt.HashPassword(password);
                user.Password = hash;
                return user;
            }
        }

        public static User Authenticate(User user, string password)
        {
            if (user == null) return null;
            if (BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                user.LastLogin = DateTime.Now;
                return user;
            }
            return null;
        }
    }
}
