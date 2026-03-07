using HandiCraft.Application.DTOs.Orders;
using HandiCraft.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace HandiCraft.API.Controllers
{//ngrok http https://localhost:44314 --host-header=rewrite
    [Route("api/webhooks/paymob")]
    public class PaymobWebhookController : Controller
    {
        private readonly IPaymentServices _paymentServices;
        private readonly IPaymobServices _paymobServices;
        private readonly ILogger<PaymentController> _logger;
        public PaymobWebhookController(IPaymentServices paymentServices, ILogger<PaymentController> logger, IPaymobServices paymobServices)
        {
            _paymentServices = paymentServices;
            _logger = logger;
            _paymobServices = paymobServices;
        }
        [HttpPost("processed")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> PaymobCallback()
        {
            try
            {

                string rawBody = await new StreamReader(Request.Body).ReadToEndAsync();


                if (string.IsNullOrWhiteSpace(rawBody))
                {
                    return BadRequest("Empty body");
                }

                Dictionary<string, JsonElement> payload;
                try
                {
                    payload = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(rawBody);
                }
                catch (Exception jsonEx)
                {
                    return BadRequest("Invalid JSON format");
                }

                if (!payload.TryGetValue("obj", out var objElement) || objElement.ValueKind != JsonValueKind.Object)
                {
                    return BadRequest("Missing or invalid 'obj' in payload");
                }


                Dictionary<string, JsonElement> objDict;
                try
                {
                    objDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(objElement.GetRawText());
                }
                catch (Exception innerEx)
                {
                    return BadRequest("Invalid inner 'obj' format");
                }

                string transactionId = ExtractStringOrNumber(objDict, "id");
                string orderId = null;

                if (objDict.TryGetValue("order", out var orderEl) && orderEl.ValueKind == JsonValueKind.Object)
                {
                    orderId = ExtractStringOrNumberFromProperty(orderEl, "id");
                }

                bool success = objDict.TryGetValue("success", out var successEl) && successEl.ValueKind == JsonValueKind.True;

                int amountCents = ExtractInt(objDict, "amount_cents");

                string currency = ExtractString(objDict, "currency");

               
                var dto = new PaymobCallbackDto
                {
                    OrderId = orderId,
                    TransactionId = transactionId,
                    Success = success,
                    AmountCents = amountCents,
                    Currency = currency
                };

                await _paymentServices.HandlePaymentCallbackAsync(dto);

                return Ok("OK");
            }
            catch (Exception ex)
            {

                if (ex.InnerException != null)
                    Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                return Ok("OK - error logged");
            }
        }

        private string ExtractStringOrNumber(Dictionary<string, JsonElement> dict, string key)
        {
            if (!dict.TryGetValue(key, out var el)) return null;

            return el.ValueKind switch
            {
                JsonValueKind.Number => el.GetInt64().ToString(),
                JsonValueKind.String => el.GetString(),
                _ => el.ToString()
            };
        }

        private string ExtractStringOrNumberFromProperty(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var propEl)) return null;

            return propEl.ValueKind switch
            {
                JsonValueKind.Number => propEl.GetInt64().ToString(),
                JsonValueKind.String => propEl.GetString(),
                _ => propEl.ToString()
            };
        }

        private int ExtractInt(Dictionary<string, JsonElement> dict, string key)
        {
            if (!dict.TryGetValue(key, out var el) || el.ValueKind != JsonValueKind.Number)
                return 0;

            return el.GetInt32();
        }

        private string ExtractString(Dictionary<string, JsonElement> dict, string key)
        {
            if (!dict.TryGetValue(key, out var el) || el.ValueKind != JsonValueKind.String)
                return null;

            return el.GetString();
        }
        [HttpGet("/api/webhooks/paymob")]
        public IActionResult PaymobCallbackGet()
        {
            return Ok("Payment processed - thank you!");
        }

    }
}
