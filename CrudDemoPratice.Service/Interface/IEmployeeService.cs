using CrudDemoPratice.Models.DTOs;
using CrudDemoPratice.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudDemoPratice.Service.Interface
{
    public interface IEmployeeService
    {
        Task<List<GetAllEmployeeDTO>> GetAllEmployeeService();
        Task<GetAllEmployeeDTO> GetEmployeeByIdService(int id);
        Task AddEmployeeService(CreateEmployeeDTO createEmployeeDTO);

        Task UpdateEmployeeService(UpdateEmployeeDTO updateEmployeeDTO);

        Task DeleteEmployeeService(int id);



    }
}
