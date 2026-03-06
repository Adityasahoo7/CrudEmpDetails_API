using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudDemoPratice.Models.DTOs
{
    public class SalaryPredictionRequestDto
    {
        public int Age { get; set; }

        public DateOnly JoiningDate { get; set; }

        public string Department { get; set; } = string.Empty;
    }
}
