namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class FilterVehiclesDTO
    {
        public string? SearchTerm { get; set; } // Model, plaka vb.
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Color { get; set; }
        public string? Status { get; set; } // Available, Rented, etc.

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}