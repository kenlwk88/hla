using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLA.Integration.Innov8tif.WebServer.Domain
{
    public static class ErrorResponse
    {
        public static dynamic Unknown()
        {
            return new { status = "error", message = "UNKNOWN_ERROR" };
        }
    }
}
