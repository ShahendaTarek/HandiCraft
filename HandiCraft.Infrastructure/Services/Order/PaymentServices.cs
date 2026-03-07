using AutoMapper;
using HandiCraft.Application.DTOs.Orders;
using HandiCraft.Application.Interfaces;
using HandiCraft.Domain.Orders;
using HandiCraft.Infrastructure.Services.Order;
using HandiCraft.Presistance.context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using static Org.BouncyCastle.Math.EC.ECCurve;

public class PaymentServices : IPaymentServices
{
    private readonly HandiCraftDbContext _context;
    private readonly IPaymobServices _paymobService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    public PaymentServices(
        HandiCraftDbContext context,
        IPaymobServices paymobService,
        IMapper mapper,
        IConfiguration config)
    {
        _context = context;
        _paymobService = paymobService;
        _mapper = mapper;
        _config = config;
    }

    public async Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentDto dto)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.DeliveryMethod)
            .FirstOrDefaultAsync(o => o.Id == dto.OrderId);

        if (order == null)
            throw new InvalidOperationException("Order not found");

        if (order.IsPaid)
            throw new InvalidOperationException("Order already paid");

        if (order.Items == null || !order.Items.Any())
            throw new InvalidOperationException("Order has no items");

        var payment = _mapper.Map<Payment>(dto);
        payment.OrderId = order.Id;
        payment.Amount = order.Total();
        payment.Status = PaymentStatus.Pending;
        payment.PaymentMethod = dto.PaymentMethod;
        payment.IntegrationId = _config.GetValue<int>("Paymob:IntegrationId");

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        var paymobResult = await _paymobService.CreatePaymentAsync(payment);

        payment.PaymobOrderId = paymobResult.paymobOrderId;
        payment.PaymentKey = paymobResult.paymentkey;

        await _context.SaveChangesAsync();

        var response = _mapper.Map<PaymentResponseDto>(payment);
        response.PaymentUrl = paymobResult.paymentUrl;

        return response;
    }

    public async Task HandlePaymentCallbackAsync(PaymobCallbackDto dto)
    {
        if (dto == null)
        {
            return;
        }

        var orderId = dto.OrderId?.Trim();

        if (string.IsNullOrWhiteSpace(orderId))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(dto?.OrderId))
        {
            return;
        }



        try
        {
            var payment = await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.PaymobOrderId == orderId);

            if (payment == null)
            {
                return;
            }


            if (payment.IsPaid)
            {
                return;
            }

            if (dto.Success)
            {
                payment.Status = PaymentStatus.Succeeded;
                payment.IsPaid = true;
                payment.TransactionId = dto.TransactionId;
                payment.PaidAt = DateTime.UtcNow;

                if (payment.Order != null)
                {
                    payment.Order.IsPaid = true;
                }
                else
                {
                    Debug.WriteLine("WARNING: Payment has no linked Order (Include failed?)");
                }
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
            }

            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException dbEx)
        {
            Debug.WriteLine($"  Message: {dbEx.Message}");
            Debug.WriteLine($"  Inner: {dbEx.InnerException?.Message}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"  Message: {ex.Message}");
            Debug.WriteLine($"  StackTrace: {ex.StackTrace}");
        }
    }

    public async Task<PaymentStatusResponseDto> GetPaymentStatusAsync(int paymentId)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.Id == paymentId);

        if (payment == null)
            throw new Exception("Payment not found");

        return new PaymentStatusResponseDto
        {
            PaymentId = payment.Id,
            Status = payment.Status.ToString(),
            IsPaid = payment.IsPaid,
            Amount = payment.Amount,
            Currency = payment.Currency
        };
    }
}
