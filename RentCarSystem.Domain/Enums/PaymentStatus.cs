namespace RentCarSystem.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending = 1,      // Bekliyor
        Completed = 2,    // Tamamlandı
        Failed = 3,       // Başarısız
        Refunded = 4,     // İade edildi
        Cancelled = 5     // İptal edildi
    }
}