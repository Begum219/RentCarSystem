using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RentCarSystem.Application.Common.Models.DTOs;

namespace RentCarSystem.Application.Common.Interfaces
{
    public interface IRentalAgreementService
    {
        Task<List<RentalAgreementDTO>> GetAllAgreementsAsync();
        Task<RentalAgreementDTO?> GetAgreementByIdAsync(int id);
        Task<RentalAgreementDTO?> GetAgreementByReservationAsync(int reservationId);
        Task<List<RentalAgreementDTO>> GetActiveAgreementsAsync();
        Task<List<RentalAgreementDTO>> GetCompletedAgreementsAsync();

        Task<RentalAgreementDTO> CreateAgreementAsync(CreateRentalAgreementDTO dto);
        Task<RentalAgreementDTO> UpdateAgreementAsync(int id, UpdateRentalAgreementDTO dto);
        Task<bool> CompleteAgreementAsync(int id, UpdateRentalAgreementDTO dto);
        Task<bool> DeleteAgreementAsync(int id);
    }
}