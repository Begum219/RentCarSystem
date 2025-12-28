using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using RentCarSystem.Domain.Entities;
using RentCarSystem.Infrastructure.Context;

namespace RentCarSystem.API.Controllers.OData
{
    public class VehiclesController : ODataController
    {
        private readonly ApplicationDbContext _context;

        public VehiclesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [EnableQuery(MaxTop = 100)] // ← OData magic!
        public IQueryable<Vehicle> Get()
        {
            return _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Category);
        }
    }
}