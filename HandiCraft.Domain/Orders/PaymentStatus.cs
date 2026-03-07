using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Domain.Orders
{
    public enum PaymentStatus
    {
        Pending,
        Succeeded,
        Failed,
        Canceled
    }
}
