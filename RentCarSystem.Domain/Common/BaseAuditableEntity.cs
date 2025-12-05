using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Domain.Common
{
    public class BaseAuditableEntity : BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy {  get; set; }
        public DateTime? UpdatedAt { get; set; }    
        public string? UpdatedBy { get;set; }
        public byte[] RowVersion { get; set; } = null!;   //Optimistic Locking için          
    }
}
