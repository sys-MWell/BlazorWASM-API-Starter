using App.Models.Dtos;
using App.Models.Models;

namespace App.API.Logic.ItemLogic
{
    public interface IItemQueryLogic
    {
        /// <summary>
        /// Retrieves a paginated list of item details.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of <see cref="ItemDetailsDto"/>.</returns>
        Task<ApiResponse<IEnumerable<ItemDetailsDto>>> GetItemsPagination(int pageNumber, int pageSize);

        /// <summary>
        /// Retrieves item details by the unique identifier of the item.
        /// </summary>
        /// <param name="itemId">The unique identifier of the item.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of <see cref="ItemDetailsDto"/>.</returns>
        Task<ApiResponse<IEnumerable<ItemDetailsDto>>> GetItemDetailsByItemId(int itemId);

        /// <summary>
        /// Retrieves an item by its unique identifier.
        /// </summary>
        /// <param name="itemId">The unique identifier of the item to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of <see cref="Item"/>.</returns>
        Task<ApiResponse<IEnumerable<Item>>> GetItemByItemId(int itemId);
    }
}
