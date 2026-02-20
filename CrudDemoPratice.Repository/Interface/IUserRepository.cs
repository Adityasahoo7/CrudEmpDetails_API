using CrudDemoPratice.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudDemoPratice.Repository.Interface
{
    public interface IUserRepository
    {
        Task<User?> GetUserByCredentialsAsync(string username, string password);
        Task<bool> RegisterUserAsync(User user);//Method for Signup a user
    }
}
