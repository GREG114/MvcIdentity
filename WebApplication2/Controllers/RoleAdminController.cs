using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "管理员")]
    public class RoleAdminController : Controller
    {

        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public RoleAdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public ActionResult Index()
        {
            return View(_roleManager.Roles);
        }


        //    [Authorize(Roles = "管理员")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Roles/Create
        [HttpPost]
        public async Task<ActionResult> Create(IdentityRole roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole(roleViewModel.Name);

                var roleresult = await _roleManager.CreateAsync(role);
                if (!roleresult.Succeeded)
                {
                    AddErrors(roleresult);
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }




        //
        // GET: /Roles/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                throw new ApplicationException();
            }
            var role = await _roleManager.FindByIdAsync(id);
            // Get the list of Users in this Role
            var users = new List<IdentityUser>();

            // Get the list of Users in this Role
            foreach (var user in _userManager.Users.ToList())
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    users.Add(user);
                }
            }

            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();
            return View(role);
        }

        //
        // GET: /Roles/Edit/Admin
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                throw new ApplicationException();
            }
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                throw new ApplicationException();
            }
            IdentityRole roleModel = new IdentityRole { Id = role.Id, Name = role.Name };

            return View(roleModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(IdentityRole roleModel)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(roleModel.Id);
                role.Name = roleModel.Name;

                await _roleManager.UpdateAsync(role);
                return RedirectToAction("Index");
            }
            return View();
        }


        //
        // GET: /Roles/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                throw new ApplicationException();
            }
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                throw new ApplicationException();
            }
            return View(role);
        }

        //
        // POST: /Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id, string deleteUser)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    throw new ApplicationException();
                }
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    throw new ApplicationException();
                }
                IdentityResult result;
                if (deleteUser != null)
                {
                    result = await _roleManager.DeleteAsync(role);
                }
                else
                {
                    result = await _roleManager.DeleteAsync(role);
                }
                if (!result.Succeeded)
                {
                    AddErrors(result);
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }

        //不知道是个干什么的，报错用的？
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
