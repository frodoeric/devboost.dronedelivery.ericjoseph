using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devboost.dronedelivery.Model
{
    public class Drone
    {
        public int Id { get; set; }
        public int Capacidade { get; set; }
        public int Velocidade { get; set; }
        public int Autonomia { get; set; }
        public int Carga { get; set; }
        public List<Pedido> Pedidos { get; set; }
        public StatusDrone StatusDrone { get; set; }

    }

    public enum StatusDrone
    {
        indisponivel = 0,
        disponivel = 1,  
        emTrajeto = 2,
        disponivelParaEntrega = 3
        
    }
}
