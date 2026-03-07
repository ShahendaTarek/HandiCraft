using HandiCraft.Domain.Identity;
using HandiCraft.Domain.UserConnections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Domain.UserAccount
{
    public class UserSettings
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool IsNotificationEnabled { get; set; } = true;
        public bool IsDarkMode { get; set; }
        public Language PreferredLanguage { get; set; }= Language.English;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        
    }
}
