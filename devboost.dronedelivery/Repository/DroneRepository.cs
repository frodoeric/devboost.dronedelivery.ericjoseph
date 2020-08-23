using devboost.dronedelivery.Model;
using devboost.dronedelivery.Repository.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace devboost.dronedelivery.Repository
{
    public class DroneRepository
    {
        readonly DataContext _dataContext;

        public DroneRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Drone>> GetDisponiveis()
        {
            return await _dataContext.Drone.Include(x => x.Pedidos).Where(x => (int)x.StatusDrone != (int)StatusDrone.indisponivel).ToListAsync();
        }

        public async Task UpdateDrone(Drone drone)
        {
            _dataContext.Drone.Update(drone);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<List<Drone>> GetAll()
        {
            var drones = await _dataContext.Drone.Include(x => x.Pedidos).ToListAsync();
            return drones;
        }
    }
}
