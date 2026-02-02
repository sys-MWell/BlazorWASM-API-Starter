using App.API.Logic.Helpers;
using App.Models.Dtos;
using App.Models.Models;
using App.API.Repository.ItemRepository;

namespace App.API.Logic.ItemLogic
{
    public class ItemQueryLogic(IItemQueryRepository itemQueryRepository) : IItemQueryLogic
    {
        private readonly IItemQueryRepository _itemQueryRepository = itemQueryRepository ?? throw new ArgumentNullException(nameof(itemQueryRepository));

        /// <summary>
        /// Retrieves a paginated list of item details.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of <see cref="ItemDetailsDto"/>.</returns>
        public async Task<ApiResponse<IEnumerable<ItemDetailsDto>>> GetItemsPagination(int pageNumber, int pageSize)
        {
            var repositoryResponse = await _itemQueryRepository.GetItemsPagination(pageNumber, pageSize);

            // Ensure nullability matches by explicitly handling potential nulls
            var successData = repositoryResponse.Data ?? [];

            return ApiResponseLogicHelper.HandleRepositoryResponse(repositoryResponse, successData);
        }

        /// <summary>
        /// Retrieves item details by the unique identifier of the item.
        /// </summary>
        /// <param name="itemId">The unique identifier of the item.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of <see cref="ItemDetailsDto"/>.</returns>
        public async Task<ApiResponse<IEnumerable<ItemDetailsDto>>> GetItemDetailsByItemId(int itemId)
        {
            var repositoryResponse = await _itemQueryRepository.GetItemDetailsByItemId(itemId);

            // Ensure nullability matches by explicitly handling potential nulls
            var successData = repositoryResponse.Data ?? [];

            return ApiResponseLogicHelper.HandleRepositoryResponse(repositoryResponse, successData);
        }

        /// <summary>
        /// Retrieves an item by its unique identifier.
        /// </summary>
        /// <param name="itemId">The unique identifier of the item to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of <see cref="Models.Models.Item"/>.</returns>
        public async Task<ApiResponse<IEnumerable<Item>>> GetItemByItemId(int itemId)
        {
            var repositoryResponse = await _itemQueryRepository.GetItemByItemId(itemId);

            // Ensure nullability matches by explicitly handling potential nulls
            var successData = repositoryResponse.Data ?? [];

            return ApiResponseLogicHelper.HandleRepositoryResponse(repositoryResponse, successData);
        }
    }
}
