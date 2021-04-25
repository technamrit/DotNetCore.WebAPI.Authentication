using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCore.Microservices.Authentication.Models
{
    public class BaseResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
