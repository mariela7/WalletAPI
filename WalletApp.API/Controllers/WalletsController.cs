using Microsoft.AspNetCore.Mvc;
using WalletApp.Application.Interfaces;
using WalletApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;

namespace WalletApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletsController(IWalletService walletService)
        {
            _walletService = walletService;
        }


        /// <summary>
        /// Obtiene la lista de todas las billeteras registradas.
        /// </summary>
        /// <returns>Una lista de objetos Wallet.</returns>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Wallet>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll()
        {
            var wallets = await _walletService.GetAllAsync();
            return Ok(wallets);
        }


        /// <summary>
        /// Obtiene una billetera por su identificador único.
        /// </summary>
        /// <param name="id">Identificador de la billetera.</param>
        /// <returns>El objeto Wallet correspondiente, si es que existe.</returns>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Wallet), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(int id)
        {
            var wallet = await _walletService.GetByIdAsync(id);
            if (wallet == null) 
            { 
                return NotFound(); 
            }
            return Ok(wallet);
        }


        /// <summary>
        /// Crea una nueva billetera.
        /// </summary>
        /// <param name="wallet">Objeto Wallet con los datos de la nueva billetera.</param>
        /// <returns>La billetera creada con su identificador.</returns>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(Wallet), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create(Wallet wallet)
        {
            if (string.IsNullOrWhiteSpace(wallet.DocumentId)
                || string.IsNullOrWhiteSpace(wallet.Name)
                || wallet.Balance < 0)
            {
                return BadRequest("Los datos proporcionados no están correctos. No es posible añadir la cuenta.");
            }

            var created = await _walletService.CreateAsync(wallet);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }


        /// <summary>
        /// Actualiza una billetera existente.
        /// </summary>
        /// <param name="id">Identificador de la billetera a actualizar.</param>
        /// <param name="wallet">Objeto Wallet con los datos actualizados, incluyendo el identificador de la billetera.</param>
        /// <response code="204">NoContent: si se actualizó</response>
        /// <response code="404">NotFound: Si la billetera no existe</response>
        /// <response code="400">BadRequest: Si los datos enviados no son válidos</response>
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(int id, Wallet wallet)
        {
            if (wallet.Id == 0 || id != wallet.Id
                || string.IsNullOrWhiteSpace(wallet.DocumentId)
                || string.IsNullOrWhiteSpace(wallet.Name)
                || wallet.Balance < 0)
            {
                return BadRequest("Los datos proporcionados no están correctos. No es posible actualizar la cuenta."); 
            }

            var updated = await _walletService.UpdateAsync(wallet);
            return updated ? NoContent() : NotFound();
        }


        /// <summary>
        /// Elimina una billetera existente por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la billetera a eliminar.</param>
        /// <response code="204">NoContent: si se eliminó correctamente</response>
        /// <response code="404">NotFound: Si la billetera no existe</response>
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _walletService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
