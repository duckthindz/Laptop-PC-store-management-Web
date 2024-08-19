using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;
using System.Data;
using System.Numerics;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/User")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(DataContext context,UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dataContext = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var userWithRoles = await (from u in _dataContext.Users
                                       join ur in _dataContext.UserRoles on u.Id equals ur.UserId
                                       join r in _roleManager.Roles on ur.RoleId equals r.Id
                                       select new { User = u, RoleName=r.Name})
                                     .ToListAsync();
            return View(userWithRoles);
        }
        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(new AppUserModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(AppUserModel user)
        {
            if (ModelState.IsValid)
            {
                var createUserResult = await _userManager.CreateAsync(user, user.PasswordHash); // tạo user
                if (createUserResult.Succeeded) 
                {
                    var createUser=await _userManager.FindByEmailAsync(user.Email); // tìm user dựa vào Email
                    var userId = createUser.Id;
                    var role=_roleManager.FindByIdAsync(user.RoleId); // lấy roleId
                    //Gán quyền
                    var addToRoleResult = await _userManager.AddToRoleAsync(createUser, role.Result.Name);
                    if(!addToRoleResult.Succeeded)
                    {
                        AddIdentityErrors(createUserResult);
                        return View(createUserResult);
                    }    

                    TempData["success"] = "User đã thêm thành công";
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    AddIdentityErrors(createUserResult);
                    return View(user);
                }
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
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }
        private void AddIdentityErrors(IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(string id, AppUserModel  user)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                //updata orther user properties (excluding password)
                existingUser.UserName = user.UserName;
                existingUser.Email=user.Email;
                existingUser.PhoneNumber=user.PhoneNumber;
                existingUser.RoleId=user.RoleId;

                var updateUserResult=await _userManager.UpdateAsync(existingUser); // thực hiện updata user
                if (updateUserResult.Succeeded)
                {
                    TempData["success"] = "Updata User thành công";
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    AddIdentityErrors(updateUserResult);
                    return View(existingUser);
                }
            }
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            //model validation failed
            TempData["error"] = "Model Validation failed";
            var error= ModelState.Values.SelectMany(v=>v.Errors.Select(e=>e.ErrorMessage)).ToList();
            string errorMessage = string.Join("\n", error);
            return View(existingUser);
        }
        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return View("Error");
            }
            TempData["success"] = "User đã xóa thành công";
            return RedirectToAction("Index");
        }
    }
}
