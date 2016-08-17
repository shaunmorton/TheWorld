using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Route("api/trips")]
    public class TripsController : Controller
    {
        private ILogger<TripsController> _logger;
        private IWorldRepository _repository;

        public TripsController(IWorldRepository repository, ILogger<TripsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            try
            {
                var data = _repository.GetAllTrips();
                return Ok(AutoMapper.Mapper.Map<IEnumerable<TripViewModel>>(data));
            }
            catch (Exception ex)
            {
                _logger.LogError("Data Retrieval Failed", ex);
                return BadRequest("Bad Stuff Happened");
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody]TripViewModel trip)
        {
            if (ModelState.IsValid)
            {
                var newTrip = AutoMapper.Mapper.Map<Trip>(trip);
                _repository.AddTrip(newTrip);
                
                if (await _repository.SaveChangesAsync())
                {
                    return Created($"api/trips/{trip.Name}", AutoMapper.Mapper.Map<TripViewModel>(newTrip));
                }
            }
            else
            {
                return BadRequest("Failed to save changes to the database.");
            }

            return BadRequest("Failed to save the Trip.");
            
        }
    }
}
