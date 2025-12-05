namespace RentCarSystem.Domain.Enums
{
    public enum DepositStatus
    {
        Pending = 1,    // Beklemede
        Held = 2,       // Bloke edildi
        Returned = 3,   // İade edildi
        Deducted = 4    // Kesildi (hasar varsa)
    }
}