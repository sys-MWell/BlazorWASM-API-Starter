using App.API.Logic.Helpers;
using App.API.Repository.ItemRepository;
using App.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.API.Repository.ItemTypeRepository;

namespace App.API.Logic.ItemTypeLogic
{
    public class ItemTypeCommandLogic(IItemTypeCommandRepository itemTypeCommandRepository) : IItemTypeCommandLogic
    {
        private readonly IItemTypeCommandRepository _itemCommandRepository = itemTypeCommandRepository ?? throw new ArgumentNullException(nameof(itemTypeCommandRepository));

        public async Task<ApiResponse<int>> InsertItemType(ItemType itemType)
        {
            var repositoryResponse = await _itemCommandRepository.InsertItemType(itemType);
            return ApiResponseLogicHelper.HandleRepositoryResponse(repositoryResponse, repositoryResponse.Data);
        }

        public Task<ApiResponse<string>> UpdateItemType(ItemType itemType)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<string>> DeleteItemType(int itemTypeID)
        {
            var repositoryResponse = await _itemCommandRepository.DeleteItemType(itemTypeID);
            return ApiResponseLogicHelper.HandleRepositoryResponse(repositoryResponse, repositoryResponse.Data);
        }
    }
}
