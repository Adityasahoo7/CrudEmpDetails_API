using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudDemoPratice.Models.MLModels
{
    public class SalaryTrainingData
    {
        public float Age { get; set; }

        public float ExperienceYears { get; set; }

        public string Department { get; set; } = string.Empty;

        public float Salary { get; set; }   // Target column
    }
}
