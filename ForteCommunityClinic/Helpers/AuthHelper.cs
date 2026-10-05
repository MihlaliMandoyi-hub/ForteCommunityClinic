using System.Web;
using System.Web.UI;

namespace ForteCommunityClinic.Helpers
{
    public static class AuthHelper
    {
        // Call at the top of Page_Load on any page that requires login
        public static void RequireLogin(Page page)
        {
            if (HttpContext.Current.Session["UserID"] == null)
            {
                page.Response.Redirect("~/Account/Login.aspx");
            }
        }

        // Call after RequireLogin on pages restricted to specific roles
        public static void RequireRole(Page page, params string[] allowedRoles)
        {
            RequireLogin(page);

            string role = HttpContext.Current.Session["Role"]?.ToString();
            bool allowed = false;

            foreach (string r in allowedRoles)
            {
                if (r == role)
                {
                    allowed = true;
                    break;
                }
            }

            if (!allowed)
            {
                page.Response.Redirect("~/AccessDenied.aspx");
            }
        }
    }
}