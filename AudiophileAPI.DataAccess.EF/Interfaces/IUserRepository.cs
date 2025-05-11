using AudiophileAPI.DataAccess.EF.Models;
using AudiophileAPI.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudiophileAPI.DataAccess.EF.Repositories
{
    public interface IUsersRepository
    {
        Task<IEnumerable<User>> GetAllUsers();

        Task<User> GetUserById(int id);

        Task<User> CreateUser(UsersDTO user);

        Task<User> UpdateUser(int userId, string firstName, string lastName, string email, string passwordHashed, string role);

        Task DeleteUser(int id);
    }
}