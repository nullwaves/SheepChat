using SheepChat.Server.Data.Models;
using System;
using System.Linq;

namespace SheepChat.Server.Data.Managers
{
    /// <summary>
    /// Extension of the ModelManager with User specific functions.
    /// </summary>
    public class UserManager : ModelManager<User>
    {
        /// <summary>
        /// Create a new user withe supplied name and password.
        /// </summary>
        /// <param name="filledUser">User object with username and raw password filled.</param>
        /// <returns>ID of new user or -1</returns>
        public new long Create(User filledUser)
        {
            User user = new User
            {
                Username = filledUser.Username,
                Registered = DateTime.Now,
                UserRole = (long)UserRole.Member
            };

            user = SetPassword(user, filledUser.Password);

            var nameTaken = repository.Filter(x => x.Username == user.Username).Count() > 0;
            return nameTaken ? -1 : repository.Create(user);
        }

        /// <summary>
        /// Find a user by their username
        /// </summary>
        /// <param name="username">Username to search for</param>
        /// <returns>User with the given username.</returns>
        public User FindByUsername(string username)
        {
            return repository.Filter(x => x.Username == username).FirstOrDefault();
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

            string hash = BCrypt.Net.BCrypt.HashPassword(password);
            user.Password = hash;
            return user;
        }

        /// <summary>
        /// Attempt to authenticate a user with a given password.
        /// </summary>
        /// <param name="user">User attempting to authenticate</param>
        /// <param name="password">Raw password input</param>
        /// <returns>Authenticated user or null if the password is incorrect.</returns>
        public User Authenticate(User user, string password)
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