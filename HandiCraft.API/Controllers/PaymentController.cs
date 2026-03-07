using HandiCraft.Application.DTOs.Orders;
using HandiCraft.Application.Interfaces;
using HandiCraft.Infrastructure.Services.Order;
using HandiCraft.Presentation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace HandiCraft.API.Controllers
{
   
    public class PaymentController :APIControllerBase
    {
        private readonly IPaymentServices _paymentServices;
        private readonly IPaymobServices _paymobServices;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentServices paymentServices, IPaymobServices paymobServices, ILogger<PaymentController> logger)
        {
            _paymentServices = paymentServices;
            _paymobServices = paymobServices;
            _logger = logger;
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment(CreatePaymentDto dto)
        {
            var result = await _paymentServices.CreatePaymentAsync(dto);
            return Ok(result);
        }
        [HttpPost("callback")]
        [IgnoreAntiforgeryToken]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> PaymobCallback()
        {
            try
            {
                Debug.WriteLine("=== PAYMOB PROCESSED CALLBACK RECEIVED ===");

                // Read raw body safely without relying on Request.Form immediately
                string rawBody = string.Empty;
                using (var reader = new StreamReader(Request.Body))
                {
                    rawBody = await reader.ReadToEndAsync();
                }

                Debug.WriteLine($"Raw body length: {rawBody.Length}");
                Debug.WriteLine($"Raw body preview: {rawBody.Substring(0, Math.Min(200, rawBody.Length))}...");

                if (string.IsNullOrWhiteSpace(rawBody))
                {
                    Debug.WriteLine("Empty body received");
                    return BadRequest("Empty callback body");
                }

                // Parse as form-urlencoded manually (safe fallback)
                var formValues = new Dictionary<string, string>();
                try
                {
                    var pairs = rawBody.Split('&');
                    foreach (var pair in pairs)
                    {
                        var kv = pair.Split('=', 2);
                        if (kv.Length == 2)
                        {
                            var key = Uri.UnescapeDataString(kv[0]);
                            var value = Uri.UnescapeDataString(kv[1]);
                            formValues[key] = value;
                        }
                    }
                }
                catch (Exception parseEx)
                {
                    Debug.WriteLine($"Body parse failed: {parseEx.Message}");
                    return BadRequest("Invalid form data format");
                }

                // Now use formValues like a dictionary
                Debug.WriteLine("Parsed fields:");
                foreach (var kv in formValues)
                {
                    Debug.WriteLine($"{kv.Key}: {kv.Value}");
                }

                var receivedHmac = formValues.GetValueOrDefault("hmac", string.Empty);

                if (string.IsNullOrEmpty(receivedHmac))
                {
                    Debug.WriteLine("Missing HMAC");
                    return BadRequest("Missing HMAC");
                }

                // For VerifyHmac: adapt it to take Dictionary<string, string> instead of IFormCollection
                // Or recreate IFormCollection if needed (but dictionary is simpler)
                bool isValid = _paymobServices.VerifyHmac(formValues, receivedHmac);  // Implement helper below

                if (!isValid)
                {
                    Debug.WriteLine("Invalid HMAC");
                    return Unauthorized("Invalid HMAC");
                }

                var dto = new PaymobCallbackDto
                {
                    OrderId = formValues.GetValueOrDefault("order", string.Empty),
                    TransactionId = formValues.GetValueOrDefault("id", string.Empty),
                    Success = formValues.GetValueOrDefault("success", string.Empty)
                                  .Equals("true", StringComparison.OrdinalIgnoreCase),
                    AmountCents = int.TryParse(formValues.GetValueOrDefault("amount_cents"), out var amt) ? amt : 0,
                    Currency = formValues.GetValueOrDefault("currency", string.Empty),
                };

                Debug.WriteLine($"Processing OrderId: '{dto.OrderId}'");

                await _paymentServices.HandlePaymentCallbackAsync(dto);

                return Ok("OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CRASH: {ex.Message}\nStack: {ex.StackTrace}");
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

       


        [HttpGet("callback")]
        public IActionResult PaymobCallbackGet()
        {
            return Ok("Callback endpoint alive");
        }

        [HttpGet("{paymentId}/status")]
        public async Task<IActionResult> GetPaymentStatus(int paymentId)
        {
            var result = await _paymentServices.GetPaymentStatusAsync(paymentId);
            return Ok(result);
        }
    }
}

