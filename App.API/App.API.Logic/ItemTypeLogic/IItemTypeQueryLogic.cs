using App.Models.Models;

namespace App.API.Logic.ItemTypeLogic
{
    public interface IItemTypeQueryLogic
    {
        /// <summary>
        /// Retrieves all item types from the system.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all item types.</returns>
        Task<ApiResponse<IEnumerable<ItemType>>> GetAllItemTypes();

        /// <summary>
        /// Retrieves a specific item type by its unique identifier.
        /// </summary>
        /// <param name="itemTypeID">The unique identifier of the item type to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of item types matching the specified ID.</returns>
        Task<ApiResponse<IEnumerable<ItemType>>> GetItemTypeById(int itemTypeID);
    }
}
