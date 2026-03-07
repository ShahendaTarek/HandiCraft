using HandiCraft.Domain.UserAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.DTOs.Settings
{
    public class UpdateSettingsDto
    {
        public bool IsNotificationEnabled { get; set; }
        public bool IsDarkMode { get; set; }
        public Language PreferredLanguage { get; set; }
    }
}
