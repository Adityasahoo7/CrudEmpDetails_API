using CrudDemoPratice.Models.DTOs;
using CrudDemoPratice.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CrudDemoPratice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("GetAllEmployees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeService.GetAllEmployeeService();
            return Ok(employees);
        }

        [HttpGet("GetEmployeeById/{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdService(id);
            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }
            return Ok(employee);
        }


        [HttpPost("AddEmployee")]
        public async Task<IActionResult> CreateEmp(CreateEmployeeDTO dto)
        {
            await _employeeService.AddEmployeeService(dto);
            return Ok("Employee created successfully.");
        }

        [HttpPut("UpdateEmployee")]

        public async Task<IActionResult> UpdateEmp(UpdateEmployeeDTO dto) {

            await _employeeService.UpdateEmployeeService(dto);
            return Ok("Employee Updated Successfully");

        }

        [HttpDelete("DeleteEmployee/{id}")]
        public async Task<IActionResult> DeleteEmp(int id) {

            await _employeeService.DeleteEmployeeService(id);
            return Ok("Employee Deleted Successfully");
        
        }

    }
}
