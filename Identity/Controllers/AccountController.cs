﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Identity.Models;

namespace Identity.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _SignInManager;

        public AccountController(RoleManager<IdentityRole> roleManager,
                                    UserManager<IdentityUser> userManager,
                                    SignInManager<IdentityUser> signInManager)
        {
            _roleManger = roleManager;
            _userManager = userManager;
            _SignInManager = signInManager;
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult SigIn()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(LoginVM login)
        {
            if (ModelState.IsValid)
            {
                var SignInResultr = await _SignInManager.PasswordSignInAsync(login.UserName, login.Password, false, false);
                switch (SignInResultr.ToString())
                {
                    case "Succeeded":
                        return RedirectToAction("Index", "Home");

                    case "Failed":
                        ViewBag.msg = "Failed - Username of/and Password is incorrect";
                        break;
                    case "Lockedout":
                        ViewBag.msg = "Locked Out";
                        break;
                    default:
                        ViewBag.msg = SignInResultr.ToString();
                        break;
                }
            }
            return View(login);
        }

        public async Task<IActionResult> Logout()
        {
            await _SignInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }
        //[HttpPost]
        //public async Task<IActionResult> CreateUser(CreateUserVM  createUser)
        //{
        //    IdentityUser user = new IdentityUser() { UserName = createUser.UserName, Email = createUser.Email };
        //    var result = await _userManager.CreateAsync(user, createUser.Password);
        //    if (result.Succed)
        //    {
        //        ViewBag.Msg = "User was succesful Created";
        //        return RedirectToAction("CreateUser");
        //    }
        //    {
        //        ViewBag.errorlist = result.Errors;
        //    }
        //    return View(createUser);
        //}
        public IActionResult RoleList()
        {
            return View(_roleManger.Roles.ToList());
        }
        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return View();
            }
            var result = await _roleManger.CreateAsync(new IdentityRole(name));
            if (result.Succeeded)
            {
                return RedirectToAction("RoleList");

            }
            return View(name);
        }
        [HttpGet]
        public IActionResult AddUserToRole(string role)
        {
            ViewBag.Role = role;

            return View(_userManager.Users.ToList());
        }
        [HttpGet]
        public async Task<IActionResult> AddUserToRoleSave(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.AddToRoleAsync(user, role);

            return RedirectToAction(nameof(RoleList));
        }
    }
}