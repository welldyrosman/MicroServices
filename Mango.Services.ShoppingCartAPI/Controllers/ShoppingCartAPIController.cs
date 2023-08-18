using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class ShoppingCartAPIController : ControllerBase
    {
        private ResponseDto _responseDto;
        private IMapper _mapper;
        private readonly AppDbContext _appDb;
        private IProductService _productService;
        private ICouponService _couponService;
        // GET: /<controller>/
        public ShoppingCartAPIController(AppDbContext appDb, IMapper mapper,IProductService productService, ICouponService couponService)
        {
            _couponService = couponService;
            _appDb = appDb;
            _mapper = mapper;
            _productService = productService;
            this._responseDto = new ResponseDto();
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var cart = await _appDb.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                
                if (cart == null)
                {
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _appDb.CartHeaders.Add(cartHeader);
                    await _appDb.SaveChangesAsync();

                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _appDb.CardDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _appDb.SaveChangesAsync();
                }
                else
                {
                    var cartDetail = await _appDb.CardDetails.AsNoTracking().FirstOrDefaultAsync(u =>
                    u.ProductId == cartDto.CartDetails.First().ProductId &&
                    u.CartHeaderId == cart.CartHeaderId);

                    if (cartDetail == null)
                    {
                        cartDto.CartDetails.First().CartHeaderId = cart.CartHeaderId;
                        _appDb.CardDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _appDb.SaveChangesAsync();
                    }
                    else
                    {
                        CouponDto couponfromdb = await _couponService.GetCoupon(cartDto.CartHeader.CouponCode);
                        
                        cartDto.CartDetails.First().Count += cartDetail.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetail.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailId = cartDetail.CartDetailId;
                        _appDb.CardDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _appDb.SaveChangesAsync();
                    }
                }
                _responseDto.Result = cartDto;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message.ToString();
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailId)
        {
            try
            {
                CartDetails cartDetail = _appDb.CardDetails.AsNoTracking().First(u =>
                   u.CartDetailId == cartDetailId);

                int totalCountCartItem = _appDb.CardDetails.Where(u => u.CartHeaderId == cartDetail.CartHeaderId).Count();
                _appDb.CardDetails.Remove(cartDetail);

                if (totalCountCartItem == 1)
                {
                    var cartToRemove = await _appDb.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetail.CartHeaderId);
                    _appDb.CartHeaders.Remove(cartToRemove);
                }
                await _appDb.SaveChangesAsync();
                _responseDto.Result = true;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message.ToString();
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }

        [HttpGet("GetCart/{userId}")]
        [Authorize]
        public async Task<ResponseDto>GetCart(string userId)
        {
            try
            {
                CartDto carts = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_appDb.CartHeaders.First(u => u.UserId == userId))
                };
                carts.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_appDb.CardDetails
                    .Where(u => u.CartHeader.CartHeaderId == carts.CartHeader.CartHeaderId));
                IEnumerable<ProductDto> productDtos = await _productService.GetProducts();
                foreach(var item in carts.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                    carts.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }
                if (!string.IsNullOrEmpty(carts.CartHeader.CouponCode))
                {
                   CouponDto coupon = await _couponService.GetCoupon(carts.CartHeader.CouponCode);
                    if (coupon != null && carts.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        carts.CartHeader.CartTotal -= coupon.DiscountAmount;
                        carts.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }
                _responseDto.Result = carts;
            }catch(Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message.ToString();
            }
            return _responseDto;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartDb = _appDb.CartHeaders.First(u => u.UserId == cartDto.CartHeader.UserId);
                cartDb.CouponCode = cartDto.CartHeader.CouponCode;
                _appDb.CartHeaders.Update(cartDb);
                await _appDb.SaveChangesAsync();
                _responseDto.Result = true;
            }catch(Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }
        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartDb = _appDb.CartHeaders.First(u => u.UserId == cartDto.CartHeader.UserId);
                cartDb.CouponCode = "";
                _appDb.CartHeaders.Update(cartDb);
                await _appDb.SaveChangesAsync();
                _responseDto.Result = true;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }
    }
}

