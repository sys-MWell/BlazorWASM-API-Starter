using App.Models.Dtos;
using App.Models.Models;

namespace App.API.Logic.ItemLogic
{
    public interface IItemCommandLogic
    {
        /// <summary>
        /// Inserts a new item into the system.
        /// </summary>
        /// <param name="item">The item details to insert.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="ApiResponse{T}"/> with a message or identifier.</returns>
        Task<ApiResponse<int>> InsertItem(ItemDto item);

        /// <summary>
        /// Updates an item by its unique identifier.
        /// </summary>
        /// <param name="item">The unique identifier of the item to update.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of <see cref="Item"/>.</returns>
        Task<ApiResponse<string>> UpdateItemByItemId(Item item);

        /// <summary>
        /// Deletes an item by its unique identifier.
        /// </summary>
        /// <param name="itemId">The unique identifier of the item to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<ApiResponse<string>> DeleteItem(int itemId);
    }
}
