using RentCarSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }= string.Empty;
        public string? Description { get; set; }
        //Navigation Property
        public bool IsActive { get; set; } = true;
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

       
    }
}
