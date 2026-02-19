using CrudDemoPratice.Models.DTOs.AISearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudDemoPratice.Service.AI.Interface
{
    public interface IAIQueryInterpreter
    {
        Task<AIQueryMetadataDto> InterpretAsync(string naturalLanguageQuery);
    }

}
