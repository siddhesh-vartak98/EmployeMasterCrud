using EmployeMasterCrud.ViewModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EmployeMasterCrud.ActionFilters
{
    public class CheckSession : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (string.IsNullOrEmpty(filterContext.HttpContext.Session.GetString("loginViewModel")))
            {
                filterContext.Result = new RedirectResult("~/Login");
            }
            else
            {
                LoginViewModel loginViewModel = JsonConvert.DeserializeObject<LoginViewModel>(filterContext.HttpContext.Session.GetString("loginViewModel")!)!;
            }
        }
    }
}
