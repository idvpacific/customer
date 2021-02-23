using System.Web.Mvc;

namespace iCore_Customer.Areas.SPF
{
    public class SPFAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SPF";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SPF_default",
                "SPF/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}