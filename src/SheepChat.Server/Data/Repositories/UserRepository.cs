using SheepChat.Server.Data.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SheepChat.Server.Data.Repositories
{
    /// <summary>
    /// A user repository that extends (without inheriting) the functionality of a basic DocumentRepository
    /// </summary>
    public static class UserRepository
    {
        /// <summary>
        /// Get all users from the repository.
        /// </summary>
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

        /// <summary>
        /// Save a user in the repository. This is an ambiguous function that doesn't distinguis between existing and new users.
        /// </summary>
        /// <param name="user">User to save.</param>
        public static void Save(User user) => DocumentRepository<User>.Save(user);

        /// <summary>
        /// Load a user by it's ID
        /// </summary>
        /// <param name="id">ID of the user to load</param>
        /// <returns>User with the specified ID or null</returns>
        public static User Load(int id) => DocumentRepository<User>.Load(id);

        /// <summary>
        /// Create a new user withe supplied name and password.
        /// </summary>
        /// <param name="username">Username of the new user.</param>
        /// <param name="password">Raw password string of the new user.</param>
        /// <returns>A new user document or null if username already exists</returns>
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

        /// <summary>
        /// Find a user by their username
        /// </summary>
        /// <param name="username">Username to search for</param>
        /// <returns>User with the given username.</returns>
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

        /// <summary>
        /// Update a user's password.
        /// </summary>
        /// <param name="user">User to update the password of</param>
        /// <param name="password">Raw password string</param>
        /// <returns>Updated and saved user document with new hashed password.</returns>
        public static User SetPassword(User user, string password)
        {
            if (password.Length < 1)
            {
                throw new ArgumentException("Password is empty", "password", new ArgumentOutOfRangeException("password"));
            }

            using (var repo = DataManager.OpenDocumentSession<User>())
            {
                string hash = BCrypt.Net.BCrypt.HashPassword(password);
                user.Password = hash;
                return user;
            }
        }

        /// <summary>
        /// Attempt to authenticate a user with a given password.
        /// </summary>
        /// <param name="user">User attempting to authenticate</param>
        /// <param name="password">Raw password input</param>
        /// <returns>Authenticated user or null if the password is incorrect.</returns>
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
