using System.ComponentModel.DataAnnotations;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class CreateVehicleDTO
    {
        [Required(ErrorMessage = "BrandId is required")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "CategoryId is required")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Model is required")]
        [StringLength(100, ErrorMessage = "Model cannot exceed 100 characters")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Year is required")]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
        public int Year { get; set; }

        [Required(ErrorMessage = "PlateNumber is required")]
        [RegularExpression(@"^\d{2}[A-Z]{1,3}\d{2,4}$", ErrorMessage = "Invalid plate number format (e.g., 34ABC123)")]
        public string PlateNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Color is required")]
        [StringLength(50, ErrorMessage = "Color cannot exceed 50 characters")]
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "FuelType is required")]
        public int FuelType { get; set; }

        [Required(ErrorMessage = "TransmissionType is required")]
        public int TransmissionType { get; set; }

        [Required(ErrorMessage = "SeatCount is required")]
        [Range(2, 50, ErrorMessage = "SeatCount must be between 2 and 50")]
        public int SeatCount { get; set; }

        [Range(2, 10, ErrorMessage = "DoorCount must be between 2 and 10")]
        public int DoorCount { get; set; }

        [Range(0, 10000, ErrorMessage = "LuggageCapacity must be between 0 and 10000")]
        public int LuggageCapacity { get; set; }

        public bool HasAirConditioning { get; set; }
        public bool HasGPS { get; set; }
        public bool HasBluetooth { get; set; }
        public bool HasParkingSensor { get; set; }
        public bool HasRearCamera { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "CurrentKilometer must be positive")]
        public int CurrentKilometer { get; set; }

        [Required(ErrorMessage = "DailyPrice is required")]
        [Range(0, double.MaxValue, ErrorMessage = "DailyPrice must be positive")]
        public decimal DailyPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "HourlyPrice must be positive")]
        public decimal HourlyPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "WeeklyPrice must be positive")]
        public decimal WeeklyPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "MonthlyPrice must be positive")]
        public decimal MonthlyPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "DepositAmount must be positive")]
        public decimal DepositAmount { get; set; }

        [Range(18, 100, ErrorMessage = "MinDriverAge must be between 18 and 100")]
        public int MinDriverAge { get; set; }

        [Range(0, 50, ErrorMessage = "MinDriverLicenseYear must be between 0 and 50")]
        public int MinDriverLicenseYear { get; set; }

        [Range(0, 10000, ErrorMessage = "DailyKilometerLimit must be between 0 and 10000")]
        public int DailyKilometerLimit { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "ExtraKilometerFee must be positive")]
        public decimal ExtraKilometerFee { get; set; }
    }
}