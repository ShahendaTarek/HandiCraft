using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.DTOs.Notification
{
    public class NotificationDto
    {
        public string UserId { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
    }
}
