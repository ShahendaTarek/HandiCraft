using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Presentation.ErrorHandling
{
    public  class ValidationError:Response
    {
        public IEnumerable<string> Errors { get; set; }
        public ValidationError() : base(400)
        {
            Errors = new List<string>();
        }
    }
}
