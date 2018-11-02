using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication2.Controllers
{
    public class UserAdminController : Controller
    {
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public UserAdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public async Task<ActionResult> Index()
        {
            return View(await _userManager.Users.ToListAsync());
        }



        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                throw new ApplicationException();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new ApplicationException();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,

                RolesList = _roleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    throw new ApplicationException();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;


                var userRoles = await _userManager.GetRolesAsync(user);

                selectedRole = selectedRole ?? new string[] { };

                var result = await _userManager.AddToRolesAsync(user, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    AddErrors(result);
                    return View();
                }
                result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    AddErrors(result);
                    return View();
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }





        //
        // GET: /Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                throw new ApplicationException();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new ApplicationException();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    throw new ApplicationException();
                }

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    throw new ApplicationException();
                }
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    AddErrors(result);
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }


        //
        // GET: /Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                throw new ApplicationException();
            }
            var user = await _userManager.FindByIdAsync(id);

            ViewBag.RoleNames = await _userManager.GetRolesAsync(user);

            return View(user);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
