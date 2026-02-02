using App.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.API.Logic.ItemTypeLogic
{
    /// <summary>
    /// Defines commands for creating, updating, and deleting item types.
    /// </summary>
    public interface IItemTypeCommandLogic
    {
        /// <summary>
        /// Deletes an existing item type by its unique identifier.
        /// </summary>
        /// <param name="itemTypeID">The unique identifier of the item type to delete.</param>
        /// <returns>An ApiResponse indicating success or failure of the delete operation.</returns>
        Task<ApiResponse<string>> DeleteItemType(int itemTypeID);

        /// <summary>
        /// Inserts a new item type into the system.
        /// </summary>
        /// <param name="itemType">The item type to insert.</param>
        /// <returns>An ApiResponse indicating success or failure of the insert operation.</returns>
        Task<ApiResponse<int>> InsertItemType(ItemType itemType);

        /// <summary>
        /// Updates an existing item type in the system.
        /// </summary>
        /// <param name="itemType">The item type with updated values.</param>
        /// <returns>An ApiResponse indicating success or failure of the update operation.</returns>
        Task<ApiResponse<string>> UpdateItemType(ItemType itemType);
    }
}
