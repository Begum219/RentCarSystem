namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class UpdateReservationDTO
    {
        public DateTime? ActualReturnDate { get; set; }
        public int? Status { get; set; }  // 1=Pending, 2=Confirmed, 3=Active, 4=Completed, 5=Cancelled
        public decimal? ExtraCharges { get; set; }
    }
}