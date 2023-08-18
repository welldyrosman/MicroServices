using System;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mango.Services.ShoppingCartAPI.Service
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<CouponDto> GetCoupon(string couponcode)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            //HttpRequestMessage message = new();
            //message.Headers.Add("Accept", "application/json");
            //message.Headers.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/coupon/GetByCode/{couponcode}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp != null && resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(resp.Result));
            }
            return new CouponDto();
        }
    }
}

