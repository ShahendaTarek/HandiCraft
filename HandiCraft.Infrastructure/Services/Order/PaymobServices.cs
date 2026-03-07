using HandiCraft.Application.DTOs.Orders;
using HandiCraft.Application.Interfaces;
using HandiCraft.Domain.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HandiCraft.Infrastructure.Services.Order
{
    public class PaymobServices: IPaymobServices
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly string _hmacSecret;

        public PaymobServices(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _hmacSecret = config["Paymob:HmacSecret"]
                 ?? throw new Exception("Paymob HMAC secret not configured");
        }
       

        public async Task<(string paymentUrl, string paymobOrderId,string paymentkey )>CreatePaymentAsync(Payment payment)
        {
            var authResponse = await _httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/auth/tokens",
                new { api_key = _config["Paymob:ApiKey"] });

            var authJson = await authResponse.Content.ReadFromJsonAsync<PaymobAuthResponseDto>();

            var orderResponse = await _httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/ecommerce/orders",
                new
                {
                    auth_token = authJson.Token,
                    amount_cents = (int)(payment.Amount * 100),
                    currency = payment.Currency,
                    delivery_needed = false,
                    items = new List<object>()
                });

            var orderJson =
                await orderResponse.Content.ReadFromJsonAsync<PaymobOrderResponseDto>();

            var paymentKeyResponse = await _httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/acceptance/payment_keys",
                new
                {
                    auth_token = authJson.Token,
                    amount_cents = (int)(payment.Amount * 100),
                    expiration = 3600,
                    order_id = orderJson.Id,
                    currency = payment.Currency,
                    integration_id = int.Parse(_config["Paymob:IntegrationId"]),
                    billing_data = new
                    {
                        apartment = "NA",
                        email = "test@test.com",
                        floor = "NA",
                        first_name = "User",
                        street = "NA",
                        building = "NA",
                        phone_number = "01000000000",
                        shipping_method = "NA",
                        postal_code = "12345",
                        city = "Cairo",
                        country = "EG",
                        last_name = "Customer",
                        state = "Cairo"
                    }
                });
            paymentKeyResponse.EnsureSuccessStatusCode();

            var paymentKeyJson =
                await paymentKeyResponse.Content.ReadFromJsonAsync<PaymentKeyResponseDto>();

            var iframeId = _config["Paymob:IframeId"];

            var paymentUrl =
                $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentKeyJson.Token}";

           return (
             paymentUrl,
             orderJson.Id.ToString(),
             paymentKeyJson.Token
           );
        }

        private Dictionary<string, string> Flatten(JsonElement element, string prefix = "", Dictionary<string, string>? result = null)
        {
            result ??= new Dictionary<string, string>();
            foreach (var property in element.EnumerateObject())
            {
                var key = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";
                switch (property.Value.ValueKind)
                {
                    case JsonValueKind.Object:
                        Flatten(property.Value, key, result);
                        break;
                    case JsonValueKind.Array: 
                        break;
                    default:
                        result[key] = property.Value.ToString();
                        break;
                }
            }
            return result;
        }


        private string GenerateHmac(string data)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_hmacSecret));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));

            return BitConverter
                .ToString(hashBytes)
                .Replace("-", "")
                .ToLower();
        }
        private static readonly string[] FieldsOrder =
  {
        "amount_cents",
        "created_at",
        "currency",
        "error_occured",
        "has_parent_transaction",
        "id",
        "integration_id",
        "is_3d_secure",
        "is_auth",
        "is_capture",
        "is_refunded",
        "is_standalone_payment",
        "is_voided",
        "order.id",
        "owner",
        "pending",
        "source_data.pan",
        "source_data.sub_type",
        "source_data.type",
        "success"
    };
        private string BuildConcatenatedString(Dictionary<string, string> flatData)
        {
            var sb = new StringBuilder();

            foreach (var field in FieldsOrder)
            {
                sb.Append(flatData.ContainsKey(field)
                    ? flatData[field]
                    : "");
            }

            return sb.ToString();
        }

        public bool VerifyHmac(IDictionary<string, string> data, string receivedHmac)
        {
            var fields = new[]
            {
        "amount_cents",
        "created_at",
        "currency",
        "error_occured",
        "has_parent_transaction",
        "id",
        "integration_id",
        "is_3d_secure",
        "is_auth",
        "is_capture",
        "is_refunded",
        "is_standalone_payment",
        "is_voided",
        "order",                       // ← fixed: not "order.id"
        "owner",
        "pending",
        "source_data.pan",             // ← fixed: dot, not underscore
        "source_data.sub_type",        // ← fixed
        "source_data.type",            // ← fixed
        "success"
    };

            var concatenated = string.Concat(fields.Select(f => data.TryGetValue(f, out var val) ? val : ""));

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_hmacSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenated));
            var computed = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

            // Optional debug
            // Console.WriteLine($"Computed: {computed}");
            // Console.WriteLine($"Received: {receivedHmac}");

            return computed == receivedHmac;
        }
    }
}

