using System;

namespace devboost.dronedelivery.Model
{
    public class Pedido
    {
        public Guid Id { get; set; }
        public int Peso { get; set; }
        //public SqlGeography LatLong { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime DataHora { get; set; }
        public int? DroneId { get; set; }
        public Drone Drone { get; set; }

        public StatusPedido StatusPedido { get; set; }
    }

    public enum StatusPedido
    {
        aguardandoAprovacao = 0,
        reprovado = 1,
        aguardandoPedido = 2,
        saiuPraEntrega = 3,
        entregue = 4,
        naoHaDronesDisponiveis = 5
    }
}
