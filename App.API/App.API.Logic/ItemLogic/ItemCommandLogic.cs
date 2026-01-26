using App.API.Logic.Helpers;
using App.Models.Dtos;
using App.Models.Models;
using App.API.Repository.ItemRepository;

namespace App.API.Logic.ItemLogic
{
    public class ItemCommandLogic(IItemCommandRepository itemCommandRepository) : IItemCommandLogic
    {
        private readonly IItemCommandRepository _itemCommandRepository = itemCommandRepository ?? throw new ArgumentNullException(nameof(itemCommandRepository));

        /// <summary>
        /// Inserts a new item into the system.
        /// </summary>
        /// <param name="item">The item details to insert.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains an
        /// <see cref="ApiResponse{T}"/> with the new item's identifier.
        /// </returns>
        public async Task<ApiResponse<int>> InsertItem(ItemDto item)
        {
            var repositoryResponse = await _itemCommandRepository.InsertItem(item);
            return ApiResponseLogicHelper.HandleRepositoryResponse(repositoryResponse, repositoryResponse.Data);
        }

        /// <summary>
        /// Updates an item by its unique identifier.
        /// </summary>
        /// <param name="item">The unique identifier of the item to update.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of <see cref="Models.Models.Item"/>.</returns>
        public async Task<ApiResponse<string>> UpdateItemByItemId(Item item)
        {
            var repositoryResponse = await _itemCommandRepository.UpdateItemByItemId(item);

            // Handle repository response
            return ApiResponseLogicHelper.HandleRepositoryResponse(repositoryResponse, "Item updated successfully.");
        }

        /// <summary>
        /// Deletes an item by its unique identifier.
        /// </summary>
        /// <param name="itemId">The unique identifier of the item to delete.</param>
        /// <returns>An <see cref="ApiResponse{T}"/> indicating the result of the operation.</returns>
        public async Task<ApiResponse<string>> DeleteItem(int itemId)
        {
            var repositoryResponse = await _itemCommandRepository.DeleteItem(itemId);

            // Handle repository response
            return ApiResponseLogicHelper.HandleRepositoryResponse(repositoryResponse, "Item deleted successfully.");
        }
    }
}
