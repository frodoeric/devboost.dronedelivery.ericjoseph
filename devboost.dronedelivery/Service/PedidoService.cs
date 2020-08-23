using devboost.dronedelivery.DTO;
using devboost.dronedelivery.Model;
using devboost.dronedelivery.Repository;
using GeoLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;

namespace devboost.dronedelivery.Service
{
    public class PedidoService
    {
        //readonly static DbGeography LOCATION_ORIGINB = DbGeography.FromText("POINT(-23.5880684 -46.6564195)");
        const double latitude = -23.5880684;
        const double longitude = -46.6564195;

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
                        pedido.StatusPedido = StatusPedido.aguardandoPedido;

                        drone = AtualizaStatusDrone(drone, distance, pedido.Peso);
                        
                        await _pedidoRepository.AddPedido(pedido);
                        await _droneRepository.UpdateDrone(drone);
                    }
                    else
                    {
                        pedido.StatusPedido = StatusPedido.naoHaDronesDisponiveis;
                        await _pedidoRepository.AddPedido(pedido);

                        throw new Exception("Não há mais drones disponíveis");
                    }
                    //trans.Complete();
                    return pedido;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task LiberarDrones(List<Drone> drones)
        {

            //await _droneRepository.UpdateDrone(drones);
        }

        private Drone AtualizaStatusDrone(Drone drone, double distance, int peso)
        {
            drone.StatusDrone = StatusDrone.disponivelParaEntrega;

            var autonimiaDistancia = ((drone.Autonomia * drone.Carga) / 100) * drone.Velocidade;
            var distanciaPercorrida = autonimiaDistancia - distance;
            var cargaUsada = distanciaPercorrida / drone.Velocidade;
            
            drone.Carga -= Convert.ToInt32(cargaUsada);
            drone.Capacidade -= peso;

            return drone;
        }
    }
}
