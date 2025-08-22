using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Domain.Posts
{
    public class Media
    {
        public int Id { get; set; }
        public string Url { get; set; } = null!;
        public MediaType MediaType { get; set; }

        public Guid PostId { get; set; }
        public Post Post { get; set; }
    }
}
