using CrudDemoPratice.Models.Models;
using CrudDemoPratice.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudDemoPratice.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDBContext _context;
       

        public UserRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByCredentialsAsync(string username, string password)
        {
            try
            {
              
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    return null;
                }
                if (user.Password == password)
                {
                    return user;
                }
                else
                {
                    return null;
                }
                //if (user != null && user.Password == password)
                //{
                //    return user;
                //}
                //else
                //{
                //    return null;
                //}
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        //Method for signup a user 
        public async Task<bool> RegisterUserAsync(User user)
        {
            try
            {

              
                var exists = await _context.Users.AnyAsync(u => u.Username == user.Username);
                if (exists)
                {
                    return false; // Already exists
                }


                // DB re insert kara
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
               
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

    }

}
