using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WalletApp.Application.Exceptions;
using WalletApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using WalletApp.Domain.Entities;

namespace WalletApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovementsController : ControllerBase
    {
        private readonly IMovementService _movementService;

        public MovementsController(IMovementService movementService)
        {
            _movementService = movementService;
        }


        /// <summary>
        /// Realiza una transferencia de saldo entre dos billeteras.
        /// </summary>
        /// <param name="request">
        /// Objeto que contiene el identificador de la billetera origen, la billetera destino y el monto a transferir.
        /// </param>
        /// <response code="200">OK: transferencia realizada con éxito</response>
        /// <response code="400">BadRequest: si los datos son inválidos o no hay saldo suficiente</response>
        /// <response code="401">Unauthorized: si no estás autenticado</response>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
        {
            if (request.Amount <= 0)
            {
                return BadRequest("El monto debe ser mayor a cero.");
            }

            try
            {
                var success = await _movementService.TransferAsync(
                    request.OriginWalletId,
                    request.DestinationWalletId,
                    request.Amount
                );

                return Ok("Transferencia realizada con éxito.");
            }
            catch (BusinessException ex)
            {
                return BadRequest(ex.Message);
            }            
        }


        /// <summary>
        /// Obtiene el historial de movimientos de una billetera específica.
        /// </summary>
        /// <param name="walletId">Identificador de la billetera.</param>
        /// <response code="200">OK: historial de movimientos encontrado</response>
        /// <response code="401">Unauthorized: si no estás autenticado</response>
        [ProducesResponseType(typeof(IEnumerable<Movement>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpGet("wallet/{walletId}")]
        public async Task<IActionResult> GetByWallet(int walletId)
        {
            var movements = await _movementService.GetMovementsByWalletIdAsync(walletId);
            return Ok(movements);
        }
    }

    public class TransferRequest
    {
        public int OriginWalletId { get; set; }
        public int DestinationWalletId { get; set; }
        public decimal Amount { get; set; }
    }
}
