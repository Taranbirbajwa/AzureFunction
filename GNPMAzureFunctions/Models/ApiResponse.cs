using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNPMAzureFunctions.Models
{
    public class ApiResponse
    {
        public ResponseData? Response { get; set; }
    }

    public class ResponseData
    {

        public string? ResponseStatus { get; set; }
        public string? ResponseCode { get; set; }
        public string? ErrorMessage { get; set; }
     
    }
}
