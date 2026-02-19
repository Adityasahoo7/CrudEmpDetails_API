using CrudDemoPratice.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudDemoPratice.Repository.Interface
{
    public interface IEmployeeRepo
    {
        Task<List<Employee>> GetAllAsyncRepo();
        Task<Employee> GetByIdAsyncRepo(int id);
        Task AddAsyncRepo(Employee employee);
        Task UpdateAsyncRepo(Employee employee);
        Task DeleteAsyncRepo(int id);
    }
}
