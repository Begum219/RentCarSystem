using AutoMapper;
using RentCarSystem.Application.Common.Models.DTOs;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Domain.Enums;

namespace RentCarSystem.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Brand Mappings
            CreateMap<Brand, BrandDTO>();
            CreateMap<CreateBrandDTO, Brand>();

            // Category Mappings
            CreateMap<Category, CategoryDTO>();
            CreateMap<CreateCategoryDTO, Category>();

            // Location Mappings
            CreateMap<Location, LocationDTO>();
            CreateMap<CreateLocationDTO, Location>();

            // Vehicle Mappings
            CreateMap<Vehicle, VehicleDTO>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.FuelType, opt => opt.MapFrom(src => src.FuelType.ToString()))
                .ForMember(dest => dest.TransmissionType, opt => opt.MapFrom(src => src.TransmissionType.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateVehicleDTO, Vehicle>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (VehicleStatus)1)); // Available

            // User Mappings
            CreateMap<User, UserDTO>();
            CreateMap<UpdateUserDTO, User>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "Customer"));

            // Reservation Mappings
            CreateMap<Reservation, ReservationDTO>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.Vehicle.Model))
                .ForMember(dest => dest.VehiclePlate, opt => opt.MapFrom(src => src.Vehicle.PlateNumber))
                .ForMember(dest => dest.PickupLocationName, opt => opt.MapFrom(src => src.PickupLocation.Name))
                .ForMember(dest => dest.ReturnLocationName, opt => opt.MapFrom(src => src.ReturnLocation.Name))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.PickupDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.ReturnDate))
                .ForMember(dest => dest.ExtraCharges, opt => opt.MapFrom(src =>
                    src.ExtraServicesFee + src.InsuranceFee + src.ExtraKilometerFee + src.FuelDifferenceFee + src.LateFee))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.DiscountAmount))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateReservationDTO, Reservation>()
                .ForMember(dest => dest.PickupDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.ReturnDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ReservationStatus.Pending));

            // Payment Mappings
            CreateMap<Payment, PaymentDTO>()
                .ForMember(dest => dest.ReservationNumber, opt => opt.MapFrom(src => $"RES-{src.ReservationId:D6}"))
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.Reservation.User.FirstName + " " + src.Reservation.User.LastName))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Reservation.User.Email))
                .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.Reservation.Vehicle.Model))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Reservation.UserId))
                .ForMember(dest => dest.VehicleId, opt => opt.MapFrom(src => src.Reservation.VehicleId))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentType.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Success"))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<CreatePaymentDTO, Payment>()
                .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Review Mappings
            CreateMap<Review, ReviewDTO>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.Vehicle.Model));

            CreateMap<CreateReviewDTO, Review>();
            CreateMap<UpdateReviewDTO, Review>();

            // Maintenance Mappings YENİ
            CreateMap<Maintenance, MaintenanceDTO>()
                .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.Vehicle.Model))
                .ForMember(dest => dest.VehiclePlate, opt => opt.MapFrom(src => src.Vehicle.PlateNumber));

            CreateMap<CreateMaintenanceDTO, Maintenance>();

            // Insurance Mappings  YENİ
            CreateMap<Insurance, InsuranceDTO>()
                .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.Vehicle.Model))
                .ForMember(dest => dest.VehiclePlate, opt => opt.MapFrom(src => src.Vehicle.PlateNumber));

            CreateMap<CreateInsuranceDTO, Insurance>();

            // VehicleImage Mappings  YENİ
            CreateMap<VehicleImage, VehicleImageDTO>()
                .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.Vehicle.Model));

            CreateMap<CreateVehicleImageDTO, VehicleImage>();

            // RentalAgreement Mappings  YENİ
            CreateMap<RentalAgreement, RentalAgreementDTO>()
                .ForMember(dest => dest.ReservationNumber, opt => opt.MapFrom(src => $"RES-{src.ReservationId:D6}"))
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.Reservation.User.FirstName + " " + src.Reservation.User.LastName))
                .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.Reservation.Vehicle.Model))
                .ForMember(dest => dest.VehiclePlate, opt => opt.MapFrom(src => src.Reservation.Vehicle.PlateNumber));

            CreateMap<CreateRentalAgreementDTO, RentalAgreement>();

            CreateMap<UpdateRentalAgreementDTO, RentalAgreement>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // DamageReport Mappings YENİ
            CreateMap<DamageReport, DamageReportDTO>()
                .ForMember(dest => dest.ReservationNumber, opt => opt.MapFrom(src => $"RES-{src.ReservationId:D6}"))
                .ForMember(dest => dest.ContractNumber, opt => opt.MapFrom(src => src.RentalAgreement.ContractNumber))
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.Reservation.User.FirstName + " " + src.Reservation.User.LastName))
                .ForMember(dest => dest.VehicleModel, opt => opt.MapFrom(src => src.Reservation.Vehicle.Model))
                .ForMember(dest => dest.VehiclePlate, opt => opt.MapFrom(src => src.Reservation.Vehicle.PlateNumber));

            CreateMap<CreateDamageReportDTO, DamageReport>();

            CreateMap<UpdateDamageReportDTO, DamageReport>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // DamageImage Mappings YENİ
            CreateMap<DamageImage, DamageImageDTO>()
                .ForMember(dest => dest.DamageDescription, opt => opt.MapFrom(src => src.DamageReport.Description));

            CreateMap<CreateDamageImageDTO, DamageImage>();
        }
    }
}