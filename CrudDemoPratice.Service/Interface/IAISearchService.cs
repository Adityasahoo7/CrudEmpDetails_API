using CrudDemoPratice.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudDemoPratice.Service.Interface
{
    public interface IAISearchService
    {
        Task<List<GetAllEmployeeDTO>> SearchAsync(string naturalLanguageQuery);
    }

}
