﻿using Microsoft.AspNetCore.Mvc;
using Online_store.Data;
using System.Web;

using Online_store.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Online_store.Controllers
{
    public class AccountController : Controller
    {
        private readonly OnlineStoreContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public AccountController(OnlineStoreContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager; 
            _signInManager = signInManager; 
        }



        public IActionResult Login(string returnUrl)
        {
            return View(new UserModel()
            {
                ReturnUrl = returnUrl
            }) ;
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserModel userModel)
        {
            if (!ModelState.IsValid)
                return View(userModel);
            var user = await _userManager.FindByNameAsync(userModel.Username);
            if(user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, userModel.PasswordHash, false, false);
                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(userModel.ReturnUrl))
                        return RedirectToAction("Index", "Home", new { logon = true });
                    return Redirect(userModel.ReturnUrl);
                }
            }
            ModelState.AddModelError("", "Username/password not found");
            return View(userModel); 
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserModel userModel)
        {
            if(ModelState.IsValid) {
                var user1 = new IdentityUser() { UserName = userModel.Username };
                var result = await _userManager.CreateAsync(user1, userModel.PasswordHash);
                if (result.Succeeded) {
                    return RedirectToAction("Index", "Home", new { register = true});
                } 
                else
                // displays error message to the user based on their incorrect input
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError("", $"{err.Description}");
                    }
                    }
                }
            return View(userModel);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { logoff = true });
        }
    }
}
