using CrudDemoPratice.Models.Models;
using CrudDemoPratice.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudDemoPratice.Repository.Implementation
{
    public class EmployeeRepo:IEmployeeRepo
    {
        private readonly AppDBContext _context;

        public EmployeeRepo(AppDBContext context)
        {
                _context = context;
        }

        public async Task<List<Employee>> GetAllAsyncRepo() {
            return await _context.Employeesds.ToListAsync();
        
        }
        public async Task<Employee> GetByIdAsyncRepo(int id) {
              return await _context.Employeesds.FindAsync(id);
         
        }
        public async Task AddAsyncRepo(Employee emp)
        {
            _context.Employeesds.Add(emp);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsyncRepo(Employee emp) { 
        _context.Employeesds.Update(emp);
            await _context.SaveChangesAsync();

        }

        public async Task DeleteAsyncRepo(int id) {

            var emp = await _context.Employeesds.FindAsync(id);

            if(emp != null)
            {
                _context.Employeesds.Remove(emp);
                await _context.SaveChangesAsync();
            }
        
        }
    }
}
