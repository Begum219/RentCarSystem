using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentCarSystem.Application.Common.Models.DTOs
{
    public class UpdateRentalAgreementDTO
    {
        // İade Bilgileri
        public int? ReturnKilometer { get; set; }
        public int? FuelLevelAtReturn { get; set; }
        public string? ReturnNotes { get; set; }
    }
}
