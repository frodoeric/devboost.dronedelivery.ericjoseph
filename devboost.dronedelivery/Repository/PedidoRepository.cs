using devboost.dronedelivery.Model;
using devboost.dronedelivery.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace devboost.dronedelivery.Repository
{
    public class PedidoRepository
    {
        readonly DataContext _dataContext;

        public PedidoRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task AddPedido(Pedido pedido)
        {
            _dataContext.Pedido.Add(pedido);
            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdatePedido(Pedido pedido)
        {
            _dataContext.Pedido.Update(pedido);
            await _dataContext.SaveChangesAsync();
        }

    }
}
