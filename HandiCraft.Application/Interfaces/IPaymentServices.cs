using HandiCraft.Application.DTOs.Orders;
using HandiCraft.Domain.Orders;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HandiCraft.Application.Interfaces
{
    public interface IPaymentServices
    {
        Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentDto CreateDto);
        Task HandlePaymentCallbackAsync(PaymobCallbackDto callback);
        Task<PaymentStatusResponseDto> GetPaymentStatusAsync(int paymentId);
    }

}
