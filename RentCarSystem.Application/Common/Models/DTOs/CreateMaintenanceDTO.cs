using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class CreateMaintenanceDTO
    {
        public int VehicleId { get; set; }              
        public string MaintenanceType { get; set; }  =string.Empty;   
        public DateTime StartDate { get; set; }         
        public DateTime? EndDate { get; set; }          
        public decimal Cost { get; set; }             
        public string? Description { get; set; }        
        public string? ServiceProvider { get; set; }  
        public bool IsCompleted { get; set; }           
    }
}
