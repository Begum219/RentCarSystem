using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.FraudDetection.Models
{
    public class FraudAlert
    {
        public int Id {  get; set; }
        public string AlertType {  get; set; }= string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public int RiskScore { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }= DateTime.UtcNow;
        public bool IsResolved { get; set; }= false;
    }
}
