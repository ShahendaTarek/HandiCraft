using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Domain.Notifications
{
    public  class UserDevice
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string DeviceToken { get; set; } 
        public DateTime LastUpdated { get; set; }
    }
}
