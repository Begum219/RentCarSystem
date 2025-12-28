namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class CreateReservationWithPaymentDTO
    {
        public int VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PickupLocationId { get; set; }
        public int ReturnLocationId { get; set; }
        public decimal Discount { get; set; }
        public decimal PaymentAmount { get; set; }
        public string? IdempotencyKey { get; set; }


    }
}