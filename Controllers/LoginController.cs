using EmployeMasterCrud.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EmployeMasterCrud.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("loginViewModel")))
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                if (loginViewModel != null)
                {
                    if (loginViewModel.email == "superadmin@companyname.com" && loginViewModel.password == "123456")
                    {
                        HttpContext.Session.SetString("username", "Super Admin");

                        loginViewModel.username = "Super Admin";
                        //request.userType = 1;

                        HttpContext.Session.SetString("loginViewModel", JsonConvert.SerializeObject(loginViewModel));

                        //TempData["Success"] = "Login Successfull!";
                        //TempData["TostData"] = "<script>$(function(){ var Toast = Swal.mixin({  toast: true,  position: 'top-end', showConfirmButton: false,  timer: 3000}); Toast.fire({  icon: 'success',  title: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.'})} </script>";
                        TempData["TostData"] = "showTost('success',' Login Successfull!');";

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.Clear();

                        ModelState.AddModelError("", "Incorrect Credentials.");
                    }
                }

            }

            return View("Index");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index");
        }
    }
}
