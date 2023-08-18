using System;
using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
	public interface ICartService
	{
        Task<ResponseDto?> GetCartByUserIdAsync(string userId);
        Task<ResponseDto?> UpsertCartAsync(CartDto cartDto);
        Task<ResponseDto?> RemoveCartAsync(int icartDetailId);
        Task<ResponseDto?> ApplyCouponAync(CartDto couponDto);
    }
}

