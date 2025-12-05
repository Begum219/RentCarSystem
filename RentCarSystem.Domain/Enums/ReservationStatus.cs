using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Domain.Enums
{
    public enum ReservationStatus
    {
        Pending = 1,        //Beklemede
        Confirmed =2,   //Onaylandı
        Active=3,       // Aktif (Araç teslim edildi)
        Completed=4,   //Tamamlandı
        Cancelled=5   //İptal edildi
    }
}
