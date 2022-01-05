using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OnlineExam.Data;
using OnlineExam.Data.Services;
using OnlineExam.Helpers;
using OnlineExam.Models;
using OnlineExam.ViewModels;
using OtpNet;
using PagedList;

namespace OnlineExam.Controllers
{
    [AppAuthorization]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        ApplicationDbContext db = new ApplicationDbContext();
        ITempUserService _tempUserService;
        private readonly IStudentWiseBatchService _studentWiseBatchService;
        private readonly OtpConfiguration _otpConfig;
        Message _message = new Message();
        public AccountController()
        {
            _otpConfig = new OtpConfiguration(200, 6);
            _tempUserService = new TempUserService(db);
            _studentWiseBatchService = new StudentWiseBatchService(db);
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginOtp(LoginViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }
            ApplicationUser user;
            if (model.Email.Contains("@"))
                user = UserManager.FindByEmail(model.Email);
            else
                user = UserManager.FindByName(model.Email);

            var key = KeyGeneration.GenerateRandomKey(10);
            string randkey = Convert.ToBase64String(key);
            var totp = new Totp(key, step: _otpConfig.Time, totpSize: _otpConfig.Length, timeCorrection: null);
            var otp = totp.ComputeTotp(DateTime.UtcNow);

            if (user != null)
            {
                var userPassword = await UserManager.FindAsync(user.UserName, model.Password);
                if (userPassword != null)
                {
                    userPassword.Otp = otp;
                    userPassword.OtpKey = randkey;

                    var res = await UserManager.UpdateAsync(userPassword);

                    if (user.UserName.Contains('@'))
                        return View(model);

                    //string message = "Use " + otp + " as your OTP to login to E-Exam, ODC Soft. Thank you.";
                    //var SmsSendStatus = SMSGateway.SendSMS(message, user.UserName);
                    //if (SmsSendStatus.status_code == 200)
                    //{
                    //    _message.success(this, "An OTP Send to your mobile number");
                    //}
                    //else
                    //{
                    //    ModelState.AddModelError("", SmsSendStatus.error_message);
                    //}
                    return View(model);
                }
            }
            ModelState.AddModelError("", "Invalid login attempt.");
            return View("Login", model);
        }

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

            ApplicationUser user;
            //string phonenumberPattern = @"(^([+]{1}[8]{2}|0088)?(01){1}[3-9]{1}\d{8})$";
            //var matsh=Regex.Match(model.Email, phonenumberPattern);
            //if (Regex.IsMatch(model.Email, phonenumberPattern))
            //    //user=db.Users.Where(u => u.PhoneNumber == model.Email).FirstOrDefault();
            //    user = UserManager.Users.Where(u => u.PhoneNumber == model.Email).FirstOrDefault();
            //else
            if (model.Email.Contains("@"))
                user = UserManager.FindByEmail(model.Email);
            else
                user = UserManager.FindByName(model.Email);
            //var res=UserManager.find
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true

            if (user != null)
            {
                var userPassword = await UserManager.FindAsync(user.UserName, model.Password);
                if (userPassword != null)
                {

                    bool isOTPok = userPassword.Otp.Equals(model.Otp);
                    if (isOTPok)
                    {
                        byte[] Bytekey = Convert.FromBase64String(userPassword.OtpKey);
                        var totp = new Totp(Bytekey, step: _otpConfig.Time, totpSize: _otpConfig.Length, timeCorrection: null);
                        long timeStepMatched;
                        bool verify = totp.VerifyTotp(DateTime.UtcNow, model.Otp, out timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);

                        if (verify)
                        {
                            await SignInAsync(user, true);
                            _message.success(this, "Login Successfully");
                            return RedirectToLocal(returnUrl);
                        }
                        else
                        {
                            ModelState.AddModelError("", "OTP timeout");
                            return View("LoginOtp", model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "OTP is incorrect");
                        return View("LoginOtp", model);
                    }
                }
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);


            #region old login
            //var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: false);
            //switch (result)
            //{
            //    case SignInStatus.Success:
            //        return RedirectToLocal(returnUrl);
            //    case SignInStatus.LockedOut:
            //        return View("Lockout");
            //    case SignInStatus.RequiresVerification:
            //        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
            //    case SignInStatus.Failure:
            //    default:
            //        ModelState.AddModelError("", "Invalid login attempt.");
            //        return View(model);
            //} 
            #endregion
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
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
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterOtp(RegisterViewModel model)
        {
            var key = KeyGeneration.GenerateRandomKey(10);
            string randkey = Convert.ToBase64String(key);
            var totp = new Totp(key, step: _otpConfig.Time, totpSize: _otpConfig.Length, timeCorrection: null);
            var otp = totp.ComputeTotp(DateTime.UtcNow);
            var userWithUserName = UserManager.Users.Where(u => u.StudentId == model.UserName).FirstOrDefault();
            var userWithMobileNo = UserManager.Users.Where(u => u.UserName == model.MobileNo).FirstOrDefault();
            var userWithEmail = UserManager.Users.Where(u => u.Email == model.Email).FirstOrDefault();
            int error = 0;
            if (userWithMobileNo != null)
            {
                error++;
                ModelState.AddModelError("", "User already exist with this mobile no.");
            }
            if (userWithEmail != null)
            {
                error++;
                ModelState.AddModelError("", "User already exist with this email.");
            }
            if (userWithUserName != null)
            {
                error++;
                ModelState.AddModelError("", "User already exist with this Student ID.");
            }
            if (error == 0)
            {
                var tempUser = _tempUserService.GetByregisterInfo(model);
                if (tempUser != null)
                {
                    tempUser.Otp = otp;
                    tempUser.OtpKey = randkey;
                    _tempUserService.Update(tempUser);
                }
                else
                {
                    TempUser temp = new TempUser();
                    temp.Email = model.Email;
                    temp.UserName = model.UserName;
                    temp.MobileNo = model.MobileNo;
                    temp.Password = model.Password;
                    temp.Otp = otp;
                    temp.OtpKey = randkey;
                    _tempUserService.Add(temp);
                }
                if (model.UserName.Contains('@'))
                    return View(model);

                string message = "Use " + otp + " as your OTP to login to E-Exam, ODC Soft. Thank you.";
                var SmsSendStatus = SMSGateway.SendSMS(message, model.MobileNo);
                if (SmsSendStatus.status_code == 200)
                {
                    _message.success(this, "An OTP Send to your mobile number");
                }
                else
                {
                    ModelState.AddModelError("", SmsSendStatus.error_message);
                }
                return View(model);
            }
            return View("Register", model);
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
                var tempUser = _tempUserService.GetByregisterInfo(model);
                bool isOTPok = tempUser.Otp.Equals(model.Otp);
                if (isOTPok)
                {
                    byte[] Bytekey = Convert.FromBase64String(tempUser.OtpKey);
                    var totp = new Totp(Bytekey, step: _otpConfig.Time, totpSize: _otpConfig.Length, timeCorrection: null);
                    long timeStepMatched;
                    bool verify = totp.VerifyTotp(DateTime.UtcNow, model.Otp, out timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);
                    if (verify)
                    {
                        var user = new ApplicationUser { UserName = model.MobileNo, Email = model.Email, PhoneNumber = model.MobileNo, StudentId = model.UserName, FullName = model.FullName, IsActive = true };
                        var result = await UserManager.CreateAsync(user, model.Password);


                        if (result.Succeeded)
                        {
                            var roleadded = await UserManager.AddToRoleAsync(user.Id, "Student");
                            await SignInAsync(user, isPersistent: true);
                            _tempUserService.Delete(tempUser.Id);
                            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                            // Send an email with this link
                            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                            return RedirectToAction("Index", "Home");
                        }
                        AddErrors(result);
                    }
                    else
                    {
                        ModelState.AddModelError("", "OTP timeout");
                        return View("RegisterOtp", model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "OTP is incorrect");
                    return View("RegisterOtp", model);
                }
            }
            // If we got this far, something failed, redisplay form
            return View("RegisterOtp", model);
        }

        public ActionResult Students(string currentFilter, string searchString, int? page = 1, int? NoOfRows = 10)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.page = page;
            ViewBag.CurrentFilter = searchString;
            ViewBag.NoOfRows = NoOfRows;


            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            // Look up the role
            string roleName = "Student";
            var role = roleManager.Roles.SingleOrDefault(r => r.Name == roleName);

            // Find the users in that role
            var roleUsers = UserManager.Users.Where(u => u.Roles.Any(r => r.RoleId == role.Id)).ToList();
            if (String.IsNullOrEmpty(searchString))
            {
                int totalRows = roleUsers.Count();
                var data = roleUsers.OrderByDescending(a => a.Id).Skip((page.Value - 1) * NoOfRows.Value).Take(NoOfRows.Value).ToList();
                var res = new StaticPagedList<ApplicationUser>(data.OrderBy(o => o.UserName), page.Value, NoOfRows.Value, totalRows);
                return View(res);
            }
            else
            {
                int totalRows = roleUsers.Where(a => a.StudentId.ToLower().Contains(searchString.ToLower())).Count();
                var data = roleUsers.Where(a => a.StudentId.ToLower().Contains(searchString.ToLower())).OrderByDescending(a => a.Id).Skip((page.Value - 1) * NoOfRows.Value).Take(NoOfRows.Value).ToList();
                var res = new StaticPagedList<ApplicationUser>(data.OrderBy(o => o.UserName), page.Value, NoOfRows.Value, totalRows);
                return View(res);
            }



        }

        public ActionResult StudentWiseBatch(string currentFilter, string searchString, int? page = 1, int? NoOfRows = 10)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.page = page;
            ViewBag.CurrentFilter = searchString;
            ViewBag.NoOfRows = NoOfRows;


            return View(_studentWiseBatchService.GetPageListWithStudentInfo(searchString, page.Value, NoOfRows.Value));
        }
        public ActionResult DeactivateStudent(string id, string currentFilter, string searchString, int? page = 1, int? NoOfRows = 10)
        {
            var student = UserManager.FindById(id);
            student.IsActive = false;
            var res = UserManager.Update(student);
            if (res.Succeeded)
            {
                _message.success(this, "Student deactivated");
                return RedirectToAction("Students", new { currentFilter = currentFilter, searchString = searchString, page = page, NoOfRows = NoOfRows });
            }
            _message.custom(this, "Student deactivation failed");
            return RedirectToAction("Students", new { currentFilter = currentFilter, searchString = searchString, page = page, NoOfRows = NoOfRows });
        }
        public ActionResult ActivateStudent(string id, string currentFilter, string searchString, int? page = 1, int? NoOfRows = 10)
        {
            var student = UserManager.FindById(id);
            student.IsActive = true;
            var res = UserManager.Update(student);
            if (res.Succeeded)
            {
                _message.success(this, "Student activated");
                return RedirectToAction("Students", new { currentFilter = currentFilter, searchString = searchString, page = page, NoOfRows = NoOfRows });
            }
            _message.custom(this, "Student activation failed");
            return RedirectToAction("Students", new { currentFilter = currentFilter, searchString = searchString, page = page, NoOfRows = NoOfRows });
        }

        public ActionResult DeactivateStudentBatch(int id, string currentFilter, string searchString, int? page = 1, int? NoOfRows = 10)
        {
            var studentWiseBatch = _studentWiseBatchService.GetDetails(id);
            studentWiseBatch.IsActive = false;
            var res = _studentWiseBatchService.Update(studentWiseBatch);
            if (res)
            {
                _message.success(this, "Deactivated successfully");
                return RedirectToAction("StudentWiseBatch", new { currentFilter = currentFilter, searchString = searchString, page = page, NoOfRows = NoOfRows });
            }
            _message.custom(this, "Deactivation failed");
            return RedirectToAction("StudentWiseBatch", new { currentFilter = currentFilter, searchString = searchString, page = page, NoOfRows = NoOfRows });
        }
        public ActionResult ActivateStudentBatch(int id, string currentFilter, string searchString, int? page = 1, int? NoOfRows = 10)
        {
            var studentWiseBatch = _studentWiseBatchService.GetDetails(id);
            studentWiseBatch.IsActive = true;
            var res = _studentWiseBatchService.Update(studentWiseBatch);
            if (res)
            {
                _message.success(this, "Activated successfully");
                return RedirectToAction("StudentWiseBatch", new { currentFilter = currentFilter, searchString = searchString, page = page, NoOfRows = NoOfRows });
            }
            _message.custom(this, "Activation failed");
            return RedirectToAction("StudentWiseBatch", new { currentFilter = currentFilter, searchString = searchString, page = page, NoOfRows = NoOfRows });
        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                //if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                EmailGateway.SendEmail(user.Email, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
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

        private async Task SignInAsync(ApplicationUser identityUser, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            var identity = await UserManager.CreateIdentityAsync(identityUser, DefaultAuthenticationTypes.ApplicationCookie);
            var studentId = string.IsNullOrWhiteSpace(identityUser.StudentId) ? "" : identityUser.StudentId.ToString();
            var studentWiseBatch=_studentWiseBatchService.GetAllByStudentId(studentId);
            bool isActive = false;
            if(studentWiseBatch!=null && studentWiseBatch.Count() > 0)
            {
                var count=studentWiseBatch.Count(m => m.IsActive);
                isActive = count > 0 ? true : false;
            }

            identity.AddClaim(new Claim("userid", identityUser.Id.ToString()));
            identity.AddClaim(new Claim("username", identityUser.UserName.ToString()));
            identity.AddClaim(new Claim("useremail", identityUser.Email.ToString()));
            identity.AddClaim(new Claim("userphone", identityUser.PhoneNumber.ToString()));
            identity.AddClaim(new Claim("studentId", string.IsNullOrWhiteSpace(identityUser.StudentId) ? "" : identityUser.StudentId.ToString()));
            string roleName = UserManager.GetRoles(identityUser.Id).FirstOrDefault();
            identity.AddClaim(new Claim("rolename", roleName));
            identity.AddClaim(new Claim("fullname", string.IsNullOrWhiteSpace(identityUser.FullName) ? "" : identityUser.FullName.ToString()));
            identity.AddClaim(new Claim("isactive", isActive.ToString()));


            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        public async Task<ActionResult> UserProfile()
        {
            var user = await UserManager.FindByIdAsync(User.GETUSERID());
            VMUserProfile vm = new VMUserProfile();
            vm.Id = user.Id;
            vm.Email = user.Email;
            vm.FullName = user.FullName;
            vm.PhoneNo = user.UserName;
            vm.UserName = user.StudentId;
            return View(vm);
        }
        [HttpPost]
        public async Task<ActionResult> UserProfile(VMUserProfile vm)
        {
            var user = await UserManager.FindByIdAsync(vm.Id);
            user.FullName = vm.FullName;
            var res = await UserManager.UpdateAsync(user);
            if (User.GETROLENAME() == "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("StudentIndex", "Home");
        }

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