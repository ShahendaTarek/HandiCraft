using HandiCraft.Application.DTOs.Orders;
using HandiCraft.Domain.Orders;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HandiCraft.Application.Interfaces
{
    public interface IPaymobServices
    {
        Task<(string paymentUrl, string paymobOrderId,string paymentkey)> CreatePaymentAsync(Payment payment);
        bool VerifyHmac(IDictionary<string, string> data, string receivedHmac);

    }
}
