using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Route("/api/trips/{tripName}/stops")]
    [Authorize]
    public class StopsController : Controller
    {
        private GeoService _geoservice;
        private ILogger<StopsController> _logger;
        private IWorldRepository _repository;

        public StopsController(IWorldRepository repository, ILogger<StopsController> logger, GeoService geoservice)
        {
            _repository = repository;
            _logger = logger;
            _geoservice = geoservice;
        }

        [HttpGet("")]
        public IActionResult Get(string tripName)
        {
            try
            {
                var trip = _repository.GetTripByName(tripName, User.Identity.Name);
                return Ok(AutoMapper.Mapper.Map<IEnumerable<StopViewModel>>(trip.Stops.OrderBy(s => s.Order).ToList()));
            }
            catch (Exception ex)
            {

                _logger.LogError("Failed to get Stops", ex);
            }

            return BadRequest("Failed to get Stops");
        }

        [HttpPost("")]
        public async Task<IActionResult> Post(string tripName, [FromBody]StopViewModel stop)
        {
            try
            {
                //If the new stop is valid
                if (ModelState.IsValid)
                {
                    var newStop = AutoMapper.Mapper.Map<Stop>(stop);

                    //use GeoService to get the coordinates
                    var result = await _geoservice.GetCoordsAsync(newStop.Name);
                    if (!result.Success)
                    {
                        _logger.LogError(result.Message);
                    }
                    else
                    {
                        newStop.Latitude = result.Latitude;
                        newStop.Longitude = result.Longitude;

                        //Save to the Database
                        _repository.AddStop(tripName, newStop, User.Identity.Name);


                        if (await _repository.SaveChangesAsync())
                        {
                            return Created($"/api/trip/{tripName}/stops/{newStop}",
                                AutoMapper.Mapper.Map<StopViewModel>(newStop));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to create new Stop: {0}", ex);
            }

            return BadRequest("Failed to create new Stoppppp");
        }
    }
}
