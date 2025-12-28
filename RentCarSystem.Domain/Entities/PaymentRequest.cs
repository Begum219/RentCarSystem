using RentCarSystem.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace RentCarSystem.Domain.Entities
{
    
    public class PaymentRequest : BaseAuditableEntity
    {
        
        public string IdempotencyKey { get; set; } = string.Empty;

        public int UserId { get; set; }
        public Guid PublicId { get; set; } = Guid.NewGuid();


        public string RequestBody { get; set; } = string.Empty;

   
        public string ResponseBody { get; set; } = string.Empty;

      
        public bool IsSuccessful { get; set; }

        
        public int? ReservationId { get; set; }

        public string? TransactionId { get; set; }

        // Navigation Property
        public virtual User? User { get; set; }
        public virtual Reservation? Reservation { get; set; }

       
        
    }
}