using Azure;
using App.API.Logic.ItemTypeLogic;
using App.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using App.API.Helpers;

namespace App.API.Controllers
{
    /// <summary>
    /// Controller for managing item types.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ItemTypesController : ControllerBase
    {
        private readonly ILogger<ItemTypesController> _logger;
        private readonly IItemTypeQueryLogic _itemTypeQueryLogic;
        private readonly IItemTypeCommandLogic _itemTypeCommandLogic;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemTypesController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for logging.</param>
        /// <param name="itemTypeQueryLogic">Logic layer for item query operations.</param>
        /// <param name="itemTypeCommandLogic">Logic layer for item command operations.</param>
        public ItemTypesController(ILogger<ItemTypesController> logger, IItemTypeQueryLogic itemTypeQueryLogic, IItemTypeCommandLogic itemTypeCommandLogic)
        {
            _logger = logger;
            _itemTypeQueryLogic = itemTypeQueryLogic;
            _itemTypeCommandLogic = itemTypeCommandLogic;
        }

        /// <summary>
        /// Retrieves all item types.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a list of all item types.
        /// </returns>
        [Authorize]
        [HttpGet("GetAllItemTypes", Name = "GetAllItemTypes")]
        [ProducesResponseType(typeof(IEnumerable<ItemType>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllItemTypes()
        {
            _logger.LogInformation("GetAllItemTypes called");

            var response = await _itemTypeQueryLogic.GetAllItemTypes();

            return !response.IsSuccess
                ? response.ToActionResult(this)
                : Ok(response.Data);
        }

        /// <summary>
        /// Retrieves an item type by its unique identifier.
        /// </summary>
        /// <param name="itemTypeId">The unique identifier of the item type to retrieve.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the item type details if found, or a 404 Not Found status if no item type exists with the given identifier.
        /// </returns>
        [Authorize]
        [HttpGet("GetItemTypeById", Name = "GetItemTypeByID")]
        [ProducesResponseType(typeof(IEnumerable<ItemType>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetItemTypeById([Required] int itemTypeId)
        {
            _logger.LogInformation("GetItemTypeByID called");

            var response = await _itemTypeQueryLogic.GetItemTypeById(itemTypeId);

            return !response.IsSuccess
                ? response.ToActionResult(this)
                : Ok(response.Data);
        }

        /// <param name="itemType">The <see cref="ItemType"/> payload containing the type name and optional description.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the operation result message on success, or an appropriate error response on failure.
        /// </returns>
        [Authorize]
        [HttpPost("InsertItemType", Name = "InsertItemType")]
        [ProducesResponseType(typeof(IEnumerable<ItemType>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertItemType([FromBody] ItemType itemType)
        {
            _logger.LogInformation("InsertItemType called");
            var response = await _itemTypeCommandLogic.InsertItemType(itemType);

            return !response.IsSuccess
                ? response.ToActionResult(this)
                : Ok(response.Data);
        }

        /// <summary>
        /// Updates an existing item type.
        /// </summary>
        /// <param name="itemType">The <see cref="ItemType"/> payload containing the identifier and updated values.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the operation result message on success, or an appropriate error response on failure.
        /// </returns>
        [Authorize]
        [HttpPut("UpdateItemType", Name = "UpdateItemType")]
        [ProducesResponseType(typeof(IEnumerable<ItemType>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateItemType([FromBody] ItemType itemType)
        {
            _logger.LogInformation("UpdateItemType called");
            var response = await _itemTypeCommandLogic.UpdateItemType(itemType);
            return !response.IsSuccess
                ? response.ToActionResult(this)
                : Ok(response.Data);
        }

        /// <summary>
        /// Deletes an existing item type by its unique identifier.
        /// </summary>
        /// <param name="itemTypeID">The unique identifier of the item type to delete.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the operation result message on success, or an appropriate error response on failure.
        /// </returns>
        [Authorize]
        [HttpDelete("DeleteItemType", Name = "DeleteItemType")]
        [ProducesResponseType(typeof(IEnumerable<ItemType>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteItemType([FromQuery][Required] int itemTypeID)
        {
            _logger.LogInformation("DeleteItemType called");
            var response = await _itemTypeCommandLogic.DeleteItemType(itemTypeID);
            return !response.IsSuccess
                ? response.ToActionResult(this)
                : Ok(response.Data);
        }
    }
}
