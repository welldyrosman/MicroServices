using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Mango.Web.Models;
using Newtonsoft.Json;
using Mango.Web.Service.IService;
using Mango.Web.Service;
using Microsoft.AspNetCore.Authorization;
using IdentityModel;

namespace Mango.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _ProductService;
    private readonly ICartService _CartService;
    public HomeController(ILogger<HomeController> logger,IProductService productService,ICartService cartService)
    {
        _CartService = cartService;
        _ProductService = productService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        List<ProductDto>? list = new();
        ResponseDto? response = await _ProductService.GetAllProductAsync();
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Query Sukses";
            list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return View(list);
    }
    [Authorize]
    [HttpPost]
    [ActionName("ProductDetails")]
    public async Task<IActionResult> ProductDetails(ProductDto productDto)
    {
        CartDto cartDto = new CartDto()
        {
            CartHeader = new CartHeaderDto
            {
                UserId = User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value
            }
        };
        CartDetailsDto cartDetails = new CartDetailsDto()
        {
            Count = productDto.Count,
            ProductId = productDto.ProductId,
        };

        List<CartDetailsDto> cartDetailsDtos = new() { cartDetails };
        cartDto.CartDetails = cartDetailsDtos;

        ResponseDto? response = await _CartService.UpsertCartAsync(cartDto);

        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Item has been added to the Shopping Cart";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["error"] = response?.Message;
        }

        return View(productDto);
    }
    [Authorize]
    public async Task<IActionResult> ProductDetails(int productId)
    {
        ProductDto? product = new();
        ResponseDto? response = await _ProductService.GetProductByIdAsync(productId);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Query Sukses";
            product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return View(product);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

