using CrudDemoPratice.Models.DTOs;
using CrudDemoPratice.Models.Models;
using CrudDemoPratice.Repository.Interface;
using CrudDemoPratice.Service.Interface;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.ObjectPool;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

        

        public async Task<(List<Employee>, int)> GetFilteredEmployees(EmployeeFilterRequestDTO request)
        {
            var employees = await _repo.GetFilteredEmployees(request);
            return (employees, employees.Count);
        }


        public async Task<byte[]> ExportEmployeesToExcel(EmployeeFilterRequestDTO request)
        {
            var employees = await _repo.GetFilteredEmployees(request);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employees");

                // Header
                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Salary";
                worksheet.Cells[1,4].Value= "Phone";
                worksheet.Cells[1, 5].Value = "Email";
                worksheet.Cells[1, 6].Value = "Age";
                worksheet.Cells[1, 7].Value = "Department";
                worksheet.Cells[1, 8].Value = "Joining Date";

                // Data
                for (int i = 0; i < employees.Count; i++)
                {
                    var e = employees[i];
                    worksheet.Cells[i + 2, 1].Value = e.Id;
                    worksheet.Cells[i + 2, 2].Value = e.Name;
                   
                    worksheet.Cells[i + 2, 3].Value = e.Salary;

                    worksheet.Cells[i + 2, 4].Value = await Convertnumber(e.Phone);



                    worksheet.Cells[i + 2, 3].Value = e.JoiningDate.ToString("yyyy-MM-dd");
                }

                return package.GetAsByteArray();
            }
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
                Department = e.Department,
                JoiningDate= e.JoiningDate
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
                Department = emp.Department,
                JoiningDate = emp.JoiningDate


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
                Department = dto.Department,
                JoiningDate = dto.JoiningDate
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
            emp.JoiningDate = dto.JoiningDate;

            await _repo.UpdateAsyncRepo(emp);

        }

        public async Task DeleteEmployeeService(int id)
        {
            await _repo.DeleteAsyncRepo(id);
        }


        public async Task<string> Convertnumber(string number)
        {
            string str = "";
            for (int i = 0; i < number.Length; i++)
            {
                if (i < number.Length - 4)
                {
                    str += "X";
                }
                else
                {
                    str += number[i];
                }
            }
            return str;
        }
        public async Task<string> ConvertEmail(string email) {

            var parts = email.Split('@');
            var username = parts[0];
            var domain = parts[1];

            return username.Substring(0,3)+"****@"+domain;


        }


        



    }
}
