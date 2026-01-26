using App.API.Logic.Helpers;
using App.API.Repository.ItemTypeRepository;
using App.Models.Models;

namespace App.API.Logic.ItemTypeLogic
{
    public class ItemTypeQueryLogic(IItemTypeQueryRepository itemTypeRepository) : IItemTypeQueryLogic
    {
        private readonly IItemTypeQueryRepository _itemTypeRepository = itemTypeRepository ?? throw new ArgumentNullException(nameof(itemTypeRepository));

        /// <summary>
        /// Retrieves all item types from the system.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all item types.</returns>
        public async Task<ApiResponse<IEnumerable<ItemType>>> GetAllItemTypes()
        {
            var repositoryResponse = await _itemTypeRepository.GetAllItemTypes();

            // Ensure nullability matches by explicitly handling potential nulls
            var successData = repositoryResponse.Data ?? [];

            return ApiResponseLogicHelper.HandleRepositoryResponse(repositoryResponse, successData);
        }

        /// <summary>
        /// Retrieves a specific item type by its unique identifier.
        /// </summary>
        /// <param name="itemTypeID">The unique identifier of the item type to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of item types matching the specified ID.</returns>
        public async Task<ApiResponse<IEnumerable<ItemType>>> GetItemTypeById(int itemTypeID)
        {
            var repositoryResponse = await _itemTypeRepository.GetItemTypeById(itemTypeID);

            // Ensure nullability matches by explicitly handling potential nulls
            var successData = repositoryResponse.Data ?? [];

            return ApiResponseLogicHelper.HandleRepositoryResponse(repositoryResponse, successData);
        }
    }
}
