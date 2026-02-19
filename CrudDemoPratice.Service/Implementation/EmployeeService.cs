using CrudDemoPratice.Models.DTOs;
using CrudDemoPratice.Models.Models;
using CrudDemoPratice.Repository.Interface;
using CrudDemoPratice.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudDemoPratice.Service.Implementation
{
    public class EmployeeService:IEmployeeService
    {
        private readonly IEmployeeRepo _repo;
        public EmployeeService(IEmployeeRepo repo)
        {
                _repo = repo;
        }

        public async Task<List<GetAllEmployeeDTO>> GetAllEmployeeService() {

            var emp = await _repo.GetAllAsyncRepo();

            return emp.Select(e => new GetAllEmployeeDTO
            {
                Id = e.Id,
                Name = e.Name,
                Salary = e.Salary,
                Phone = e.Phone,//Return the phone number
                Email = e.Email,
                Age = e.Age,
                Department = e.Department
                //I am not return phone number because of some security reasons
            }).ToList();
        
        
        }

        public async Task<GetAllEmployeeDTO> GetEmployeeByIdService(int id) {

            var emp = await _repo.GetByIdAsyncRepo(id);
            if(emp == null)
            {
                throw new Exception("Employee not found");
            }

            return new GetAllEmployeeDTO
            {
                Id = emp.Id,
                Name = emp.Name,
                Salary = emp.Salary,
                Phone = emp.Phone,
                Email = emp.Email,
                Age = emp.Age,
                Department = emp.Department


            };

        }

        public async Task AddEmployeeService(CreateEmployeeDTO dto) { 
        
            var emp = new Employee
            {
                Name = dto.Name,
                Salary = dto.Salary,
                Phone = dto.Phone,
                Email = dto.Email,
                Age = dto.Age,
                Department = dto.Department
            };

            await _repo.AddAsyncRepo(emp);

        }

        public async Task UpdateEmployeeService(UpdateEmployeeDTO dto) {

            var emp = await _repo.GetByIdAsyncRepo(dto.Id);
            if(emp == null)
            {
                throw new Exception("Employee not found");
            }

            emp.Name = dto.Name;
            emp.Salary = dto.Salary;
            emp.Phone = dto.Phone;
            emp.Email = dto.Email;
            emp.Age = dto.Age;
            emp.Department = dto.Department;

            await _repo.UpdateAsyncRepo(emp);

        }

        public async Task DeleteEmployeeService(int id)
        {
            await _repo.DeleteAsyncRepo(id);
        }






    }
}
