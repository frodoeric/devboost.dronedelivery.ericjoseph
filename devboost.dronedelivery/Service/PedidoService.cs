using devboost.dronedelivery.DTO;
using devboost.dronedelivery.Model;
using devboost.dronedelivery.Repository;
using GeoLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace devboost.dronedelivery.Service
{
    public class PedidoService
    {
        //readonly static DbGeography LOCATION_ORIGINB = DbGeography.FromText("POINT(-23.5880684 -46.6564195)");
        const double longitude = -23.5880684;
        const double latitude = -46.6564195;

        readonly PedidoRepository _pedidoRepository;
        readonly DroneRepository _droneRepository;

        public PedidoService(PedidoRepository pedidoRepository, DroneRepository droneRepository)
        {
            _pedidoRepository = pedidoRepository;
            _droneRepository = droneRepository;
        }

        public async Task<List<Drone>> BuscarDrone()
        {
            return await _droneRepository.GetAll();
        }

        public async Task<Pedido> RealizarPedido(PedidoDTO pedidoDto)
        {
            //var longlat = $"POINT({pedidoDto.Latitude} {pedidoDto.Longitude})".Replace(",", ".");
            try
            {
                var pedido = new Pedido();
                pedido.Id = Guid.NewGuid();
                pedido.DataHora = DateTime.Now;
                //pedido.LatLong = DbGeography.FromText(longlat);
                pedido.Latitude = pedidoDto.Latitude;
                pedido.Longitude = pedidoDto.Longitude;
                pedido.Peso = pedidoDto.Peso;
                pedido.StatusPedido = StatusPedido.aguardandoAprovacao;

                //using (var trans = new TransactionScope())
                //{
                    //Verificar a distância entre Origem e Destindo (Pedido)
                    var distance = new Coordinates(latitude, longitude)
                   .DistanceTo(
                       new Coordinates(pedido.Latitude, pedido.Longitude),
                       UnitOfLength.Kilometers
                   );
                    var drones = await _droneRepository.GetDisponiveis();
                    //Verificar drones, que possuem autonomia de ida e volta

                    //Qual automomia atual do drone = (Autonomia * Carga) / 100
                    //Temos que pegar os Drones com AA >= Distancia do Pedido * 2
                    var dronesDispAutonomia = drones?.Where(x => (((x.Autonomia * x.Carga) / 100) * x.Velocidade) >= (distance * 2))?.ToList();

                    //Dos Drones com autonomia, quais podem carregar o peso do pedido
                    var dronesComCapacidade = dronesDispAutonomia?.Where(x => x.Capacidade >= pedido.Peso)?.ToList();

                    //Caso dronesComCapacidade não seja nulo e contenha objetos (drone), pode ser responsável pela entrega
                    if (dronesComCapacidade != null && dronesComCapacidade.Count() > 0)
                    {
                        var drone = dronesComCapacidade.FirstOrDefault();
                        pedido.Drone = drone;
                        pedido.StatusPedido = StatusPedido.despachado;
                        drone.StatusDrone = StatusDrone.emTrajeto;
                        await _pedidoRepository.AddPedido(pedido);
                        await _droneRepository.UpdateDrone(drone);
                    }
                    else
                    {
                        pedido.StatusPedido = StatusPedido.reprovado;
                        await _pedidoRepository.AddPedido(pedido);
                    }
                    //trans.Complete();
                    return pedido;
                //}
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
    }
}
