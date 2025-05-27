using Microsoft.AspNetCore.Mvc.Filters;

namespace WS_2_0.Filters
{
    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var response = context.HttpContext.Response;

            response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            response.Headers["Pragma"] = "no-cache";
            response.Headers["Expires"] = "-1";
            response.Headers["X-Content-Type-Options"] = "nosniff";

            base.OnResultExecuting(context);
        }
    }
}
