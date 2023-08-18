using System;
using Mango.Web.Models;
using Mango.Web.Utility;

namespace Mango.Web.Service.IService
{
	public class CartService:ICartService
	{
        private readonly IBaseService _baseService;
        public CartService(IBaseService baseService)
		{
            _baseService = baseService;
        }

        public async Task<ResponseDto?> ApplyCouponAync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.ShoppingCartAPIBase + "/api/cart/ApplyCoupon"
            });
        }

        public async Task<ResponseDto?> GetCartByUserIdAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ShoppingCartAPIBase + "/api/cart/GetCart/"+userId
            });
        }

        public async Task<ResponseDto?> RemoveCartAsync(int icartDetailId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = icartDetailId,
                Url = SD.ShoppingCartAPIBase + "/api/cart/RemoveCart"
            });
        }

        public async Task<ResponseDto?> UpsertCartAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.ShoppingCartAPIBase + "/api/cart/CartUpsert"
            });
        }
    }
}

