using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Presentation
{
    public class Response
    {
        public int statusCode { get; set; }
        public string? Message { get; set; }
        public Response(int statuscode, string? message = null)
        {
            statusCode = statuscode;
            Message = message ?? GetDefaultMessageForeStatusCode(statusCode);
        }

        private string? GetDefaultMessageForeStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "Not Authorized",
                404 => "Not Found",
                500 => "Intenral Server Error",
                _ => null

            };
        }
    }
}
