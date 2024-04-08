using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Project_MusicRadio.Permissions
{
    public class ValidateSessionAtrib : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var httpContext = filterContext.HttpContext;

            var userJson = httpContext.Session.GetString("user");

            if (userJson == null)
            {
                filterContext.Result = new RedirectResult("~/Acces/Login");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
