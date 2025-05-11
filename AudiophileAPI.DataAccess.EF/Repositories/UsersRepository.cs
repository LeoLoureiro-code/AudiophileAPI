using AudiophileAPI.DataAccess.EF.Context;
using AudiophileAPI.DataAccess.EF.Models;
using AudiophileAPI.DataAccess.EF.Services;
using AudiophileAPI.DTO;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudiophileAPI.DataAccess.EF.Repositories
{
    public class UsersRepository: IUsersRepository
    {
        private readonly AudiophileAPIDbContext _context;

        public UsersRepository(AudiophileAPIDbContext context)
        { 
            _context = context; 
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserById (int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            return user;
        }

        public async Task<User> CreateUser(UsersDTO user)
        {
            var passwordService = new PasswordService();
            string hashed = passwordService.HashPassword(user.Password);

            user.Password = hashed;

            var userEntity = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PasswordHash = hashed,
                Role = user.Role
            };

            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();
            return userEntity;
        }

        public async Task<User> UpdateUser(int userId, string firstName, string lastName, string email, string passwordHashed, string role)
        {
            User? existingUser = await _context.Users.FindAsync(userId);

            if (existingUser == null)
            {
                throw new Exception("User not found");
            }

            var passwordService = new PasswordService();
            string hashed = passwordService.HashPassword(passwordHashed);

            existingUser.FirstName = firstName;
            existingUser.LastName = lastName;
            existingUser.Email = email;
            existingUser.PasswordHash = hashed;
            existingUser.Role = role;

            await _context.SaveChangesAsync();
            return existingUser;
        }

        public async Task DeleteUser(int id)
        {
            User? user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

    }
}
