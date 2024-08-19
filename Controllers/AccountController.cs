using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopping_Tutorial.Areas.Admin.Repository;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Models.ViewModels;

namespace Shopping_Tutorial.Controllers
{
	public class AccountController : Controller
	{
		private UserManager<AppUserModel> _userManager;
		private SignInManager<AppUserModel> _signInManager;
		private readonly IEmailSender _emailSender;
		public AccountController(UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, IEmailSender emailSender)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
		}
		
		public IActionResult Login(string returnUrl)
		{
			return View(new LoginViewModel { ReturnUrl = returnUrl });
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel loginVM)
		{
			if(ModelState.IsValid)
			{
				Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.UserName, loginVM.Password, false, false);
				if(result.Succeeded)
				{
					TempData["success"] = "Đăng nhập thành công";
							//Phần gữi xác nhận mail
					//var receiver = "ducthinh14122003@gmail.com";
					//var subject = "Đăng nhập trên thiết bị thành công";
					//var message = "Đăng nhập thành công, triển khai trải nghiệm nhé!";
					//await _emailSender.SendEmailAsync(receiver,subject, message);
					return Redirect(loginVM.ReturnUrl ?? "/");
				}
				ModelState.AddModelError("", "Username hoặc Password bị sai");
			}
			return View(loginVM);
		}
		public async Task<IActionResult> Create()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(UserModel user)
		{
			if(ModelState.IsValid)
			{
				AppUserModel newUser = new AppUserModel { UserName=user.UserName,Email=user.Email};
				IdentityResult result = await _userManager.CreateAsync(newUser,user.Password);
				if(result.Succeeded)
				{
					TempData["success"] = "Tạo User Thành Công";
					return Redirect("/account");
				}
				foreach(IdentityError error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return View(user);
		}
		public async Task<IActionResult> Logout(string returnUrl = "/")
		{
			await _signInManager.SignOutAsync();
			return Redirect(returnUrl);
		}

	}
}
