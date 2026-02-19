using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrudDemoPratice.Models.DTOs;
using CrudDemoPratice.Models.Models;
using CrudDemoPratice.Repository;
using CrudDemoPratice.Service.AI.Interface;
using CrudDemoPratice.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace CrudDemoPratice.Service.Implementation
{


    public class AISearchService : IAISearchService
    {
        private readonly IAIQueryInterpreter _ai;
        private readonly AppDBContext _context;

        public AISearchService(IAIQueryInterpreter ai, AppDBContext context)
        {
            _ai = ai;
            _context = context;
        }

        public async Task<List<GetAllEmployeeDTO>> SearchAsync(string queryText)
        {
            var aiQuery = await _ai.InterpretAsync(queryText);

            if (aiQuery == null)
                return new List<GetAllEmployeeDTO>();

            IQueryable<Employee> query = _context.Employeesds;

            if (aiQuery.Filters != null)
            {
                foreach (var f in aiQuery.Filters)
                {
                    if (f == null || string.IsNullOrEmpty(f.Column))
                        continue;

                    switch (f.Column.ToLower())
                    {
                        case "salary":
                            if (int.TryParse(f.Value, out int salary))
                            {
                                query = f.Operator switch
                                {
                                    ">" => query.Where(e => e.Salary > salary),
                                    "<" => query.Where(e => e.Salary < salary),
                                    "=" => query.Where(e => e.Salary == salary),
                                    _ => query
                                };
                            }
                            break;

                        case "age":
                            if (int.TryParse(f.Value, out int age))
                            {
                                query = f.Operator switch
                                {
                                    ">" => query.Where(e => e.Age > age),
                                    "<" => query.Where(e => e.Age < age),
                                    "=" => query.Where(e => e.Age == age),
                                    _ => query
                                };
                            }
                            break;

                        case "name":
                            if (f.Operator == "contains")
                                query = query.Where(e =>
                                    e.Name.ToLower().Contains(f.Value.ToLower()));
                            break;

                        case "department":
                            query = query.Where(e => e.Department == f.Value);
                            break;
                    }
                }
            }

            // Sorting
            if (!string.IsNullOrEmpty(aiQuery.SortBy))
            {
                switch (aiQuery.SortBy.ToLower())
                {
                    case "salary":
                        query = aiQuery.SortDescending
                            ? query.OrderByDescending(e => e.Salary)
                            : query.OrderBy(e => e.Salary);
                        break;

                    case "age":
                        query = aiQuery.SortDescending
                            ? query.OrderByDescending(e => e.Age)
                            : query.OrderBy(e => e.Age);
                        break;

                    case "name":
                        query = aiQuery.SortDescending
                            ? query.OrderByDescending(e => e.Name)
                            : query.OrderBy(e => e.Name);
                        break;
                }
            }

            return await query.Select(e => new GetAllEmployeeDTO
            {
                Id = e.Id,
                Name = e.Name,
                Salary = e.Salary,
                Phone = e.Phone,
                Email = e.Email,
                Age = e.Age,
                Department = e.Department
            }).ToListAsync();
        }


    }

}
