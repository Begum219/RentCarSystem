using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Domain.Enums
{
    public enum VehicleStatus
    {
        Available =1 ,         //Müsait
        Rented=2 ,            //Kiralanmış
        Maintenance =3 ,      //Bakımda
        OutOfService=4        //Hizmet dışı


    }
}
