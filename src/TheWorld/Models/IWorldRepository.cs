using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheWorld.Models
{
    public interface IWorldRepository
    {
        IEnumerable<Trip> GetAllTrips();
        void AddTrip(Trip trip);
        Task<bool> SaveChangesAsync();
        Trip GetTripByName(string tripName, string userName);
        void AddStop(string tripName, Stop newStop, string userName);
        IEnumerable<Trip> GetUserTripsWithStops(string name);
    }
}
