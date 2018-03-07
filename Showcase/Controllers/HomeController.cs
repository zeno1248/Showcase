using Resources;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Web.Mvc;
using System.Collections.Generic;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

using Showcase.Util;

namespace Showcase.Controllers
{
    public class HomeController : Controller
    {
        ILogger _log { get; } = ZenoLogging.CreateLogger<HomeController>();

        [NonAction]
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string culture = "en-US";
            if (filterContext.HttpContext.Session["culture"] != null)
                culture =  filterContext.HttpContext.Session["culture"].ToString();

            string[] languages, cultures;
            switch (culture)
            {
                /*
                case "fr-FR":
                    languages = new string[] { "French", "Chinese", "English" };
                    cultures = new string[] {"zh-CN", "en-US" };
                    break;
                    */
                case "zh-CN":
                    languages = new string[] { "Chinese", "English" };
                    cultures = new string[] {"en-US"};
                    break;
                default:
                    languages = new string[] { "English", "Chinese" };
                    cultures = new string[] { "zh-CN"};
                    break;
            }
            
            ResourceManager rm = new ResourceManager("Showcase.App_LocalResources.Resource", typeof(Resource).Assembly);
            for(int i=0; i<languages.Length; i++)
            {
                languages[i] = rm.GetString(languages[i]);
            }
            
            ViewBag.Cultures = cultures;
            ViewBag.Languages = languages;
            base.OnActionExecuted(filterContext);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Demo(string id)
        {
            return View("Demo_" + id);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult AccessDenied()
        {
            ViewBag.Message = "You choose not to allow us to access your Google Calendar.";

            return View();
        }

        public ActionResult SetCulture(string culture)
        {
            HttpContext.Session["culture"] = culture;
            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        [AsyncTimeout(60000)]
        public async Task<ActionResult> GoogleCalendarAsync(CancellationToken cancellationToken)
        {
            
            var result = await new AuthorizationCodeMvcApp(this, new Showcase.Util.ZenoFlowMetadata()).
                AuthorizeAsync(cancellationToken);

            if (result.Credential != null)
            {
                
                var service = new CalendarService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = result.Credential,
                    ApplicationName = "ASP.NET MVC Sample"
                });

                if (String.IsNullOrEmpty(Request.Form["CalendarID"]))
                {
                    CalendarListResource.ListRequest request0 = service.CalendarList.List();
                    request0.MaxResults = 10;

                    // List events.
                    CalendarList calendars = request0.Execute();
                    string desc = string.Empty;
                    Dictionary<string, string> calendarList = new Dictionary<string, string>();
                    if (calendars.Items != null && calendars.Items.Count > 0)
                    {
                        foreach (var calendarItem in calendars.Items)
                        {
                            calendarList.Add(calendarItem.Id, calendarItem.Summary);
                        }
                    }
                    ViewBag.Message = "Please select which calendar you want to add.";
                    return View(calendarList);
                }
                else
                {

                    DateTime tomorrow = DateTime.Today.AddDays(1);
                    string chineseDate = ChineseCalendar.GetChineseDate(tomorrow);

                    var newEvent = new Google.Apis.Calendar.v3.Data.Event()
                    {
                        Summary = chineseDate,

                        Start = new EventDateTime()
                        {
                            Date = tomorrow.ToString("yyyy-MM-dd"),
                        },
                        End = new EventDateTime()
                        {
                            Date = tomorrow.ToString("yyyy-MM-dd"),
                        },
                        Transparency = "transparent"
                    };

                    EventsResource.InsertRequest request = service.Events.Insert(newEvent, Request.Form["CalendarID"]);
                    Event createdEvent = request.Execute();

                    ViewBag.Message = "Google Calendar Event Added: URL = " + createdEvent.HtmlLink;
                    return View();
                }
                /*
                Showcase.ZenoService.ZenoServiceClient zeno = new Showcase.ZenoService.ZenoServiceClient();
                GoogleRequest request = new GoogleRequest();
                request.Type = ShowcaseService.Contract.GoogleRequestType.CalendarPush;
                request.User = new ShowcaseService.Contract.GoogleUser(){ AccessToken = result.Credential.Token.AccessToken, RefreshToken = result.Credential.Token.RefreshToken};
                GoogleResponse response = zeno.InvokeGoogleService(request);
                ViewBag.Message = response.Message;
                */

            }
            else
            {
                return new RedirectResult(result.RedirectUri);
            }
        }

    }
}