using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Domain.Enums
{
    public enum PaymentType
    {
        Reservation=1,     //Rezervasyon ödemesi
        Deposit=2,         //depozito
        ExtraCharge=3,   //ekstra ücret
        Refund=4      //iade
    }
}
