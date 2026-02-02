using App.API.Helpers;
using App.API.Logic.ItemLogic;
using App.Models.Dtos;
using App.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace App.API.Controllers
{
    /// <summary>
    /// Controller for managing items.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ILogger<ItemController> _logger;
        private readonly IItemCommandLogic _itemCommandLogic;
        private readonly IItemQueryLogic _itemQueryLogic;
        
        /// <summary>
        /// Initialises a new instance of the <see cref="ItemController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for logging.</param>
        /// <param name="itemCommandLogic">Logic layer for item command operations.</param>
        /// <param name="itemQueryLogic">Logic layer for item query operations.</param>
        public ItemController(ILogger<ItemController> logger, IItemCommandLogic itemCommandLogic, IItemQueryLogic itemQueryLogic)
        {
            _logger = logger;
            _itemCommandLogic = itemCommandLogic;
            _itemQueryLogic = itemQueryLogic;
        }

        /// <summary>
        /// Retrieves items with pagination.
        /// </summary>
        /// <returns>A list of item details.</returns>
        [Authorize]
        [HttpGet("GetItemDetailsPaginator", Name = "GetAllInvoiceData")]
        [ProducesResponseType(typeof(IEnumerable<ItemDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetItemDetailsPaginatorAsync(
            [FromQuery][Required] int pageNumber,
            [FromQuery][Required] int pageSize)
        {
            Pagination pagination = new Pagination
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            _logger.LogInformation("GetItemDetailsPaginatorAsync called with pageNumber: {pageNumber}, pageSize: {pageSize}", pagination.PageNumber, pagination.PageSize);

            var response = await _itemQueryLogic.GetItemsPagination(pagination.PageNumber, pagination.PageSize);
            return response.ToActionResult(this);
        }

        /// <summary>
        /// Retrieves item details by item Id.
        /// </summary>
        /// <param name="itemId">The unique identifier of the item to retrieve.</param>
        /// <returns>Action result.</returns>
        [Authorize]
        [HttpGet("GetItemDetailsByItemId", Name = "GetItemDetailsByItemId")]
        [ProducesResponseType(typeof(IEnumerable<ItemDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetItemDetailsByItemId([FromQuery][Required] int itemId)
        {
            _logger.LogInformation("GetItemDetailsByItemId called");
            var response = await _itemQueryLogic.GetItemDetailsByItemId(itemId);
            return response.ToActionResult(this);
        }

        /// <summary>
        /// Retrieves an item by its unique identifier.
        /// </summary>
        /// <param name="itemId">The unique identifier of the item to retrieve.</param>
        /// <returns>Action result.</returns>
        [Authorize]
        [HttpGet("GetItemByItemId", Name = "GetItemByItemId")]
        [ProducesResponseType(typeof(IEnumerable<Item>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetItemByItemId([FromQuery][Required] int itemId)
        {
            _logger.LogInformation("GetItemByItemId called");
            var response = await _itemQueryLogic.GetItemByItemId(itemId);
            return response.ToActionResult(this);
        }

        [Authorize]
        [HttpPut("InsertItem", Name = "InsertItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertItemByItemId([FromBody] ItemDto item)
        {
            _logger.LogInformation("InsertItem called");
            var response = await _itemCommandLogic.InsertItem(item);
            return response.ToActionResult(this);
        }

        /// <summary>
        /// Updates an item by its unique identifier within body.
        /// </summary>
        /// <param name="itemId">The unique identifier of the item to update.</param>
        /// <param name="item">The item model containing updated data.</param>
        /// <returns>Action result.</returns>
        [Authorize]
        [HttpPut("UpdateItemByItemID/{itemId}", Name = "UpdateItemByItemId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateItemByItemId(int itemId, [FromBody] Item item)
        {
            _logger.LogInformation("UpdateItemByItemId called");
            var response = await _itemCommandLogic.UpdateItemByItemId(item);
            return response.ToActionResult(this);
        }

        /// <summary>
        /// Deletes an item and its dependencies.
        /// </summary>
        /// <param name="itemId">Id of the item to delete.</param>
        /// <returns>HTTP status code indicating the result of the operation.</returns>
        [Authorize]
        [HttpDelete("DeleteItem/{itemId}", Name = "DeleteItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteItem(int itemId)
        {
            _logger.LogInformation("DeleteItem called");
            var response = await _itemCommandLogic.DeleteItem(itemId);
            return response.ToActionResult(this);
        }
    }
}
