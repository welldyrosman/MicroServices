using System;
using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
	public interface ICouponService
	{
		Task<ResponseDto?> GetCoupon(string couponCode);
		Task<ResponseDto?> GetAllCouponAsync();
        Task<ResponseDto?> GetCouponByIdAsync(int id);
        Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto);
        Task<ResponseDto?> UpadateCouponAsync(CouponDto couponDto);
        Task<ResponseDto?> DeleteCouponAsync(int id);
    }
}

