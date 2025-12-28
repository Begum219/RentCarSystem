using RentCarSystem.Domain.Common;
using RentCarSystem.Domain.Enums;

namespace RentCarSystem.Domain.Entities
{
    public class Vehicle : BaseAuditableEntity
    {
        
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        
        
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string? ChassisNumber { get; set; }
        public string Color { get; set; } = string.Empty;

     
        public FuelType FuelType { get; set; }
        public TransmissionType TransmissionType { get; set; }
        public int SeatCount { get; set; }
        public int DoorCount { get; set; }
        public int LuggageCapacity { get; set; } // Litre

        
        public bool HasAirConditioning { get; set; }
        public bool HasGPS { get; set; }
        public bool HasBluetooth { get; set; }
        public bool HasParkingSensor { get; set; }
        public bool HasRearCamera { get; set; }

        
        public int CurrentKilometer { get; set; }
        public VehicleStatus Status { get; set; } = VehicleStatus.Available;

        
        public decimal DailyPrice { get; set; }
        public decimal? HourlyPrice { get; set; }
        public decimal? WeeklyPrice { get; set; }
        public decimal? MonthlyPrice { get; set; }
        public decimal DepositAmount { get; set; }

        
        public int MinDriverAge { get; set; } = 21;
        public int MinDriverLicenseYear { get; set; } = 1;
        public int DailyKilometerLimit { get; set; } = 200;
        public decimal ExtraKilometerFee { get; set; } = 2.5m;

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public Brand Brand { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public ICollection<VehicleImage> VehicleImages { get; set; } = new List<VehicleImage>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
        public ICollection<Insurance> Insurances { get; set; } = new List<Insurance>();
    }
}