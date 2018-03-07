using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;


namespace Showcase
{
    public class CultureAwareControllerActivator : IControllerActivator
    {
        public IController Create(RequestContext requestContext, Type controllerType)
        {
            //Get the {language} parameter in the RouteData
            string culture = requestContext.HttpContext.Session["culture"] == null ?
                "en-US" : requestContext.HttpContext.Session["culture"].ToString();

            //Get the culture info of the language code
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            return DependencyResolver.Current.GetService(controllerType) as IController;
        }
    }
}