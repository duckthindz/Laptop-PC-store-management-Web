using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Role")]
    [Authorize(Roles = "Admin,Author")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _roleManager.Roles.OrderByDescending(r=>r.Id).ToListAsync());
        }
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            return View(new IdentityRole());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                var createRoleResult = await _roleManager.CreateAsync(role);
                if(createRoleResult.Succeeded)
                {
                    TempData["success"] = "Theem quyền thành công";
                    return RedirectToAction("Index", "Role");
                }
                else
                {
                    AddIdentityError(createRoleResult);
                    return View(role);
                }
            }
            else
            {
                TempData["error"] = "Model đang bị lổi";
                List<string> errors=new List<string>();
                foreach(var value in ModelState.Values)
                {
                    foreach(var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n");
                return BadRequest(errorMessage);
            }
            return View(role);
        }
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var role=await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(string id, IdentityRole role)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var existingRole=await _roleManager.FindByIdAsync(id);
            if (existingRole == null) 
            {
                return NotFound();   
            }
            if(ModelState.IsValid)
            {
                //update orther role
                existingRole.Name = role.Name;
                var updateRoleResult=await _roleManager.UpdateAsync(existingRole);
                if(updateRoleResult.Succeeded)
                {
                    TempData["success"] = "Updata Role thành công";
                    return RedirectToAction("Index", "Role");
                }
                else
                {
                    AddIdentityError(updateRoleResult);
                    return View(existingRole);
                }
            }
            TempData["error"] = "Model Validation failed";
            var error = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            string errorMessage = string.Join("\n", error);
            return View(existingRole);
        }
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var role=await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            var deleteResult= await _roleManager.DeleteAsync(role);
            if (!deleteResult.Succeeded)
            {
                return View("Error");
            }
            TempData["success"] = "Role đã xóa thành công";
            return RedirectToAction("Index");
        }
        private void AddIdentityError(IdentityResult identityResult)
        {
            foreach(var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
