using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Mango.Web.Controllers
{
    
    public class ProductController : Controller
    {
        // GET: /<controller>/
        private readonly IProductService _ProductService;
        public ProductController(IProductService ProductService) {
            _ProductService = ProductService;
        }

        public async Task<IActionResult> ProductIndex()
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

        public IActionResult ProductCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDto ProductDto)
        {
            if(ModelState.IsValid)
            {
                ResponseDto? response = await _ProductService.CreateProductAsync(ProductDto);
                if(response!=null && response.IsSuccess)
                {
                    TempData["success"] = "PRODUCT BERHASIL DIBUAT";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }

            }
            return View(ProductDto);
        }
        public async Task<IActionResult> ProductEdit(int id)
        {
            ResponseDto? response = await _ProductService.GetProductByIdAsync(id);
            if (response != null && response.IsSuccess)
            {
                ProductDto? ProductDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(ProductDto);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductDto ProductDto)
        {
            ResponseDto? response = await _ProductService.UpadateProductAsync(ProductDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "PRODUCT BERHASIL DI EDIT";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(ProductDto);
        }
        public async Task<IActionResult> ProductDelete(int id)
        {
            ResponseDto? response = await _ProductService.GetProductByIdAsync(id);
            if (response != null && response.IsSuccess)
            {
                ProductDto? ProductDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(ProductDto);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDto ProductDto)
        {
            ResponseDto? response = await _ProductService.DeleteProductAsync(ProductDto.ProductId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "PRODUCT BERHASIL DI HAPUS";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(ProductDto);
        }
    }
}

