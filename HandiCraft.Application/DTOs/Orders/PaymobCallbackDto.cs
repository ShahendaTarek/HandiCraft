using HandiCraft.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.DTOs.Orders
{
    public class PaymobCallbackDto
    {
        public string OrderId { get; set; } = string.Empty;         
        public string TransactionId { get; set; } = string.Empty;   
        public bool Success { get; set; }                          
        public string SuccessRaw { get; set; } = string.Empty;      

        public int AmountCents { get; set; }                        
        public string Currency { get; set; } = string.Empty;        

        public string Hmac { get; set; } = string.Empty;          

        public string CreatedAtRaw { get; set; } = string.Empty;    
       
        public string SourceType { get; set; } = string.Empty;   
        public string SourceSubType { get; set; } = string.Empty;   
        public string SourcePan { get; set; } = string.Empty;       

        public string ErrorOccured { get; set; } = string.Empty;    // "error_occured"
        public string Pending { get; set; } = string.Empty;         // "pending"
        public string Is3dSecure { get; set; } = string.Empty;      // "is_3d_secure"
        public string IsAuth { get; set; } = string.Empty;          // "is_auth"
        public string IsCapture { get; set; } = string.Empty;       // "is_capture"
        public string IsRefunded { get; set; } = string.Empty;      // "is_refunded"
        public string IsVoided { get; set; } = string.Empty;        // "is_voided"

        public string IntegrationId { get; set; } = string.Empty;   // "integration_id"
        public string HasParentTransaction { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
    }
}
