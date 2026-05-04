using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserLogin.Models;
using UserLogin.Models.ViewModels;
using UserLogin.Services;

namespace UserLogin.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;
        private readonly IEmailService _emailService;     
        private readonly OtpService _otpService;

        public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager, IEmailService emailService,    
            OtpService otpService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            _emailService = emailService;   
            _otpService = otpService;
        }

        // STEP 1: Show Forgot Password form (asks for email only)
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // STEP 1: Process email, generate and send OTP
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if user exists (but don't reveal this info)
            var user = await userManager.FindByEmailAsync(model.Email);

            // Generate OTP even if user doesn't exist (security - prevents email enumeration)
            string otp = _otpService.GenerateOtp();

            if (user != null)
            {
                // Store OTP for valid user
                _otpService.StoreOtp(model.Email, otp);
                // Send OTP email
                await _emailService.SendOtpEmailAsync(model.Email, otp);
            }
            else
            {
                // Still store OTP but don't send (or send to a dummy email)
                // This prevents attackers from knowing which emails exist
                _otpService.StoreOtp(model.Email, otp);
            }

            // Always redirect to OTP verification page
            TempData["ResetEmail"] = model.Email;
            return RedirectToAction("VerifyOtp", new { email = model.Email });
        }

        // STEP 2: Show OTP Verification form
        [AllowAnonymous]
        public IActionResult VerifyOtp(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            return View(new VerifyOtpViewModel { Email = email });
        }

        // STEP 2: Verify OTP
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyOtp(VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Verify the OTP
            bool isValid = _otpService.VerifyOtp(model.Email, model.Otp);

            if (isValid)
            {
                // OTP verified - redirect to reset password page
                TempData["ResetEmail"] = model.Email;
                return RedirectToAction("ResetPassword", new { email = model.Email });
            }
            else
            {
                ModelState.AddModelError("", "Invalid or expired OTP. Please request a new one.");
                return View(model);
            }
        }

        // STEP 3: Show Reset Password form (ONLY after OTP verified)
        [AllowAnonymous]
        public IActionResult ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            return View(new ResetPasswordViewModel { Email = email });
        }

        // STEP 3: Reset the password
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            // Remove current password and add new one
            var removeResult = await userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
            {
                foreach (var error in removeResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            var addResult = await userManager.AddPasswordAsync(user, model.NewPassword);
            if (addResult.Succeeded)
            {
                TempData["SuccessMessage"] = "Password reset successful! Please login with your new password.";
                return RedirectToAction("Login");
            }
            else
            {
                foreach (var error in addResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Email or password is incorrect");
                    return View(model);
                }
            }
            return View(model);
        }
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }


        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Users users = new Users
                {
                    FullName = model.Name,
                    Email = model.Email,
                    UserName = model.Email,
                };
                var result = await userManager.CreateAsync(users, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }
            return View(model);
        }
        [AllowAnonymous]
        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "Something is wrong!");
                    return View(model);
                }
                else
                {
                    return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
                }

            }
            return View(model);
        }

        //[AllowAnonymous]
        //public IActionResult ChangePassword(string username)
        //{
        //    if (string.IsNullOrEmpty(username))
        //    {
        //        return RedirectToAction("VerifyEmail", "Account");
        //    }
        //    return View(new ChangePasswordViewModel { Email = username });
        //}
        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await userManager.FindByEmailAsync(model.Email);
        //        if (user != null)
        //        {
        //            var result = await userManager.RemovePasswordAsync(user);
        //            if (result.Succeeded)
        //            {
        //                result = await userManager.AddPasswordAsync(user, model.NewPassword);
        //                return RedirectToAction("Login", "Account");
        //            }
        //            else
        //            {
        //                {
        //                    foreach (var error in result.Errors)
        //                    {
        //                        ModelState.AddModelError("", error.Description);
        //                    }
        //                    return View(model);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Email not Found");
        //            return View(model);
        //        }
        //    }
        //    else
        //    {
        //        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        //        {
        //            Console.WriteLine(error.ErrorMessage);
        //        }

        //        return View(model);
        //    }
        //}

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}