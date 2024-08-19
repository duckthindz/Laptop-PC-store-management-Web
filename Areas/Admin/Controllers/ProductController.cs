using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;
using System.Text.RegularExpressions;
using System.Web;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Route("Admin/Product")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Products.OrderByDescending(p=>p.Id).Include(p=>p.Category).Include(p => p.Brand).ToListAsync());
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
            return View();
        }
        public string DecodeHtmlEntities(string encodedString)
        {
            return Regex.Replace(HttpUtility.HtmlDecode(encodedString), "<.*?>", String.Empty);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
            
            if(ModelState.IsValid)
            {
                //Add data
                TempData["success"] = "Model có hiệu lực";
                product.Slug = product.Name.Replace(" ", "-");
                product.Description=DecodeHtmlEntities(product.Description);

                var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                if(slug!=null)
                {
                    ModelState.AddModelError("", "Sản phẩm đã có trong database");
                    return View(product);
                }
                else
                {
                    if(product.ImageUpload!=null)
                    {
                        string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath,"media/products");
                        string imageName=Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                        string filePath = Path.Combine(uploadsDir, imageName);

                        FileStream fs = new FileStream(filePath, FileMode.Create);
                        await product.ImageUpload.CopyToAsync(fs);
                        fs.Close();
                        product.Images = imageName;
                    }
                    _dataContext.Add(product);
                    _dataContext.SaveChangesAsync();
                    TempData["success"] = "Thêm sản phẩm thành công";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang bị lổi";
                List<string> errors = new List<string>();
                foreach(var value in ModelState.Values)
                {
                    foreach(var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage=string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            return View();
        }
        public async Task<IActionResult> Edit(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            var exitsed_product = _dataContext.Products.Find(product.Id);

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.Replace(" ", "-");
                product.Description = DecodeHtmlEntities(product.Description);
                if (product.ImageUpload != null)
                {
                    //upload new image
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    //Delelte old Picture
                    string oldfilePath = Path.Combine(uploadsDir, exitsed_product.Images);

                    try
                    {
                        if (System.IO.File.Exists(oldfilePath))
                        {
                            System.IO.File.Delete(oldfilePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An error occcurred while deleting the product image");
                    }

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    exitsed_product.Images = imageName;

                    
                }
                //Update orther product properties
                exitsed_product.Name = product.Name;
                exitsed_product.Description = product.Description;
                exitsed_product.Price = product.Price;
                exitsed_product.CategoryId = product.CategoryId;
                exitsed_product.BrandId = product.BrandId;
                //..other properties

                _dataContext.Update(exitsed_product); //Update the exiting product object

                await _dataContext.SaveChangesAsync();
                TempData["success"] = "cập nhật sản phẩm thành công";
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
        public async Task<IActionResult> Delete(int Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);

            if (product == null)
            {
                return NotFound(); //Handle product not found
            }

            string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
            string oldfilePath = Path.Combine(uploadsDir, product.Images);

            try 
            {
                if (System.IO.File.Exists(oldfilePath))
                {
                    System.IO.File.Delete(oldfilePath);
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", "An error occcurred while deleting the product image");
            }
            
            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();
            TempData["error"] = "Sản phẩm đã bị xóa";
            return RedirectToAction("Index");
        }
    }
}
