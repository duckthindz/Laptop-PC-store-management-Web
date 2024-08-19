using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Order")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;
        public OrderController(DataContext context)
        {
            _dataContext = context;
        }
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Orders.OrderByDescending(o=>o.Id).ToListAsync());
        }
        //public IActionResult ViewOrder()
        //{
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [HttpGet]
        [Route("ViewOrder")]
        public async Task<IActionResult> ViewOrder(string ordercode)
        {
            var DetailsOrder=await _dataContext.OrderDetails.Include(od=>od.Product).Where(od=>od.OrderCode==ordercode).ToListAsync();
            return View(DetailsOrder);
        }
        [HttpPost]
        [Route("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder(string ordercode, int status)
        {
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
            if (order == null)
            {
                return NotFound();
            }
            order.Status = status;
            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Orther status updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occuurred while updating the order status");
            }
        }
        [HttpGet]
        [Route("Delete/{ordercode}")]
        public async Task<IActionResult> Delete(string ordercode)
        {
            var order =await _dataContext.Orders.FindAsync(ordercode);
            if(order==null)
            {
                return NotFound();
            }
            _dataContext.Remove(order);
            await _dataContext.SaveChangesAsync();
            TempData["error"] = "Đơn hàng đã bị xóa";
            return RedirectToAction("Index");
        }
    }
}
