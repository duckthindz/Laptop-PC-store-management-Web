using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Brand")]
    [Authorize(Roles = "Admin,Author")]
	public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext Context)
        {
            _dataContext = Context;
        }
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Brands.OrderByDescending(p => p.Id).ToListAsync());
        }
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(BrandModel brand)
        {

            if (ModelState.IsValid)
            {
                //Add data
                TempData["success"] = "Model có hiệu lực";
                brand.Slug = brand.Name.Replace(" ", "-");
                //product.Description = DecodeHtmlEntities(product.Description);

                var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Thương Hiệu đã có trong database");
                    return View(brand);
                }
                _dataContext.Add(brand);
                _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm thương hiệu thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang bị lổi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            return View();
        }
        [HttpGet]
        [Route("Edit/{Id}")]
        public async Task<IActionResult> Edit(int Id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(Id);

            return View(brand);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id,BrandModel brand)
        {
            var exitsted_brand = _dataContext.Brands.Find(id);

            if (ModelState.IsValid)
            {
                //Add data
                TempData["success"] = "Câp nhật có hiệu lực";
                brand.Slug = brand.Name.Replace(" ", "-");
                //product.Description = DecodeHtmlEntities(product.Description);

                //update orther category properties
                exitsted_brand.Name = brand.Name;
                exitsted_brand.Description = brand.Description;
                exitsted_brand.Slug = brand.Slug;
                exitsted_brand.Status = brand.Status;

                _dataContext.Update(exitsted_brand);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật danh mục thành công";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang bị lổi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            return View();
        }
        [HttpGet]
        [Route("Delete/{Id}")]
        public async Task<IActionResult> Delete(int Id)
        {
            BrandModel brand = await _dataContext.Brands.FindAsync(Id);


            _dataContext.Brands.Remove(brand);
            await _dataContext.SaveChangesAsync();
            TempData["error"] = "Thương hiệu đã bị xóa";
            return RedirectToAction("Index");
        }
    }
}
