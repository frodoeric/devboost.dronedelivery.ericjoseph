using devboost.dronedelivery.DTO;
using devboost.dronedelivery.Model;
using devboost.dronedelivery.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace devboost.dronedelivery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DroneController : ControllerBase
    {
        readonly PedidoService _pedidoService;

        public DroneController(PedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var result = await _pedidoService.BuscarDrone();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PedidoDTO pedidoDto)
        {
            try
            {
                var result = await _pedidoService.RealizarPedido(pedidoDto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        //[HttpPost("LiberarDronesParaEntrega")]
        //public async Task<ActionResult> LiberarDronesParaEntrega()
        //{
        //    try
        //    {
        //        var drone = await _pedidoService.BuscarDrone();
        //        //var result = await _pedidoService.LiberarDrones(drone);

        //        //return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex);
        //    }

        //}

    }
}
