using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Route("Admin/Category")]
    [Authorize(Roles = "Admin,Author")]
    public class CategoryController : Controller
	{
		private readonly DataContext _dataContext;

		public CategoryController(DataContext Context)
		{
			_dataContext = Context;
		}
        //      [HttpGet]
        //      [Route("Index")]
        //      public async Task<IActionResult> Index()
        //{
        //	return View(await _dataContext.Categories.OrderByDescending(p => p.Id).ToListAsync());
        //}
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index(int pg=1)
        {
            List<CategoryModel> category = _dataContext.Categories.ToList();

            const int pageSize = 10;
            
            if(pg<1)
            {
                pg = 1;
            }
            int recsCount = category.Count();
            var pager=new Paginate(recsCount,pg,pageSize);

            int recSkip = (pg - 1) * pageSize;
            var data = category.Skip(recSkip).Take(pageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }
        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(CategoryModel category)
        {

            if (ModelState.IsValid)
            {
                //Add data
                TempData["success"] = "Model có hiệu lực";
                category.Slug = category.Name.Replace(" ", "-");
                //product.Description = DecodeHtmlEntities(product.Description);

                var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Danh Mục đã có trong database");
                    return View(category);
                }
                _dataContext.Add(category);
                _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm danh mục thành công";
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
            CategoryModel category = await _dataContext.Categories.FindAsync(Id);

            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id,CategoryModel category)
        {
            var exitsted_category = _dataContext.Categories.Find(id);

            if (ModelState.IsValid)
            {
                //Add data
                TempData["success"] = "Model có hiệu lực";
                category.Slug = category.Name.Replace(" ", "-");
                //product.Description = DecodeHtmlEntities(product.Description);

                //update orther category properties
                exitsted_category.Name=category.Name;
                exitsted_category.Description= category.Description;
                exitsted_category.Slug=category.Slug;
                exitsted_category.Status=category.Status;

                _dataContext.Update(exitsted_category);
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
            CategoryModel category = await _dataContext.Categories.FindAsync(Id);


            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();
            TempData["error"] = "Danh mục đã bị xóa";
            return RedirectToAction("Index");
        }
    }
}
