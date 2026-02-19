using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudDemoPratice.Models.DTOs.AISearch
{
    public class AIQueryMetadataDto
    {
        public List<FilterCondition> Filters { get; set; }
        public string SortBy { get; set; }
        public bool SortDescending { get; set; }
    }

    public class FilterCondition
    {
        public string Column { get; set; }
        public string Operator { get; set; } // >, <, =, Contains
        public string Value { get; set; }
    }
}
