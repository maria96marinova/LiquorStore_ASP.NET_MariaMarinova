using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using LiquorStore.Web.Models;
using LiquorStore.Domain.Models;
using LiquorStore.Domain;
using AutoMapper;

namespace LiquorStore.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly ApplicationDbContext dbContext;
        
        public AccountController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult All()
         => View(this.dbContext.Users
             .Select(u => new UserViewModel
             {
                 Id = u.Id,
                 FirstName = u.FirstName,
                 LastName = u.LastName,
                 Address = u.Address
             }));

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(string id)
        {
            var userForEdit = this.dbContext.Users.Where(u => u.Id == id)
                              .Select(u => new UserEditFormModel
                              {
                                  Id = id,
                                  FirstName = u.FirstName,
                                  LastName = u.LastName,
                                  Address = u.Address
                              }).FirstOrDefault();

            return View(userForEdit);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(string id, UserEditFormModel model)
        {
            var user = this.dbContext.Users.FirstOrDefault(u => u.Id == id);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;

            this.dbContext.SaveChanges();

            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(string id)
        {
            var user = this.dbContext.Users.FirstOrDefault(u => u.Id == id);
            this.dbContext.Users.Remove(user);
            this.dbContext.SaveChanges();

            return RedirectToAction(nameof(All));
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditPersonalInfo(string id)
        {
            if (this.HttpContext.User.Identity.GetUserId() != id)
            {
                TempData["error"] = "You can only edit your own information";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var user = Mapper.Map<ApplicationUser, UserViewModel>(this.dbContext.Users
                                                    .FirstOrDefault(u => u.Id == id));

                return View(user);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditPersonalInfo(string id, FormCollection collection)
        {
            if (this.HttpContext.User.Identity.GetUserId() != id)
            {
                TempData["error"] = "You can only edit your own information";
            }

            else
            { 
                var user = this.dbContext.Users.FirstOrDefault(u => u.Id == id);
                user.FirstName = collection["FirstName"];
                user.LastName = collection["LastName"];
                user.Address = collection["Address"];
                this.dbContext.SaveChanges();
                TempData["success"] = "Successfully edited information";
            }

            return RedirectToAction("Index", "Home");
        }


        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }
        
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { FirstName = model.FirstName,
                    LastName = model.LastName, UserName = model.Email, Email = model.Email, Address = model.Address };

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}