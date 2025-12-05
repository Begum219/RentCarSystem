namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class MaintenanceDTO
    {
        public int Id { get; set; }

        // Vehicle bilgileri
        public int VehicleId { get; set; }
        public string VehicleModel { get; set; } = string.Empty;
        public string VehiclePlate { get; set; }= string.Empty;       

        // Maintenance bilgileri
        public string MaintenanceType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }             
        public DateTime? EndDate { get; set; }          
        public decimal Cost { get; set; }               
        public string? Description { get; set; }        
        public string? ServiceProvider { get; set; }   
        public bool IsCompleted { get; set; }          

        public DateTime CreatedAt { get; set; }
    }
}