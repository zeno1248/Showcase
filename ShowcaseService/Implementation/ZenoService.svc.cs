using System.Collections.Generic;
using ShowcaseService.DataAccess;
using Showcase.ShowcaseService.Contract;
using System.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using System.Web.Hosting;
using System.IO;
using System;
using System.Configuration;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2.Responses;


namespace ShowcaseService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class ZenoService : IZenoService
    {
        public CarResponse GetCars(CarRequest request)
        {
            DataTable dt = DatabaseUtil.GetCar();
            List<Car> cars = new List<Car>();
            foreach (DataRow row in dt.Rows)
            {
                cars.Add(new Car { Brand = row["brand"].ToString(), Model = row["model"].ToString(), Color = row["color"].ToString() });
            }
            CarResponse response = new CarResponse();
            response.Cars = cars;
            response.Count = 3;
            return response;
        }

        public CalendarResponse GetGoogleCalendarWithAPI(CalendarRequest request)
        {
            /*
            string jsonPath = HostingEnvironment.MapPath("~/App_Data/google_calendar_client_secret.json");
            string[] Scopes = {"openid", "email", CalendarService.Scope.CalendarReadonly };
            ClientSecrets secret;

            using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
            {
                secret = GoogleClientSecrets.Load(stream).Secrets;
            }

            var initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = secret.ClientId,
                    ClientSecret = secret.ClientSecret,
                },
                Scopes = Scopes,
            };
            
            
            var flow = new GoogleAuthorizationCodeFlow(initializer);
            var identity = await HttpContext.Current.GetOwinContext().Authentication.GetExternalIdentityAsync(DefaultAuthenticationTypes.ApplicationCookie);
            var userId = identity.FindFirstValue("GoogleUserId");
            */
            try
            {
                string jsonPath = HostingEnvironment.MapPath("~/App_Data/google_calendar_client_secret.json");
                string[] Scopes = { CalendarService.Scope.CalendarReadonly };
                GoogleCredential credential;
                using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream).CreateScoped(CalendarService.Scope.CalendarReadonly);
                }

                var service = new CalendarService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Test Calendar",
                    ApiKey = ConfigurationManager.AppSettings["GoogleServiceAccountAPIKey"],
                });

                /*
                CalendarListResource.ListRequest request0 = service.CalendarList.List();
                request0.MaxResults = 10;

                // List events.
                CalendarList calendars = request0.Execute();

                if (calendars.Items != null && calendars.Items.Count > 0)
                {
                    foreach (var calendarItem in calendars.Items)
                    {
                        string desc = calendarItem.Description;
                    }
                }
                */

                EventsResource.ListRequest request1 = service.Events.List(request.CalendarName);
                request1.TimeMin = request.StartTime;
                request1.ShowDeleted = false;
                request1.SingleEvents = true;
                request1.MaxResults = 10;
                request1.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
                Events events = request1.Execute();

                List<Showcase.ShowcaseService.Contract.Event> eventList = new List<Showcase.ShowcaseService.Contract.Event>();
                if (events.Items != null && events.Items.Count > 0)
                {
                    foreach (var eventItem in events.Items)
                    {
                        eventList.Add(new Showcase.ShowcaseService.Contract.Event { Title = eventItem.Summary, StartTime = eventItem.Start.DateTime.ToString(), EndTime = eventItem.End.DateTime.ToString() });
                    }
                }
                CalendarResponse response = new CalendarResponse();
                response.Events = eventList;
                return response;

            }
            catch (Exception e)
            {
                throw;
            }

        }

        public GoogleResponse InvokeGoogleService(GoogleRequest request)
        {

            GoogleResponse response = new GoogleResponse();
            response.Message = "This is a test";

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                        {
                            ClientSecrets = new ClientSecrets
                            {
                                ClientId = "929463394906-lkrivbdgl8k2j4r8e5sm9rk8rcejli21.apps.googleusercontent.com",
                                ClientSecret = "QgSCXEro5F1SdDXGODd0q5EZ"
                            },
                            Scopes = new[] { CalendarService.Scope.Calendar },
                            DataStore = new FileDataStore("Drive.Api.Auth.Store")
                        });

            var token = new TokenResponse
            {
                AccessToken = request.User.AccessToken,
                RefreshToken = request.User.RefreshToken
            };

            var credential = new UserCredential(flow, Environment.UserName, token);

            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "ASP.NET MVC Sample"
            });

            var newEvent = new Google.Apis.Calendar.v3.Data.Event()
            {
                Summary = "Google I/O 2015",
                Location = "800 Howard St., San Francisco, CA 94103",
                Description = "A chance to hear more about Google's developer products.",
                Start = new EventDateTime()
                {
                    DateTime = DateTime.Parse("2018-03-01T09:00:00-07:00"),
                    TimeZone = "America/Chicago",
                },
                End = new EventDateTime()
                {
                    DateTime = DateTime.Parse("2018-03-01T11:00:00-07:00"),
                    TimeZone = "America/Chicago",
                },
                //Recurrence = new string[] { "RRULE:FREQ=DAILY;COUNT=2" },
                Attendees = new EventAttendee[] {
                        new EventAttendee() { Email = "lpage@example.com" },
                        new EventAttendee() { Email = "sbrin@example.com" },
                    },
                Reminders = new Google.Apis.Calendar.v3.Data.Event.RemindersData()
                {
                    UseDefault = false,
                    Overrides = new EventReminder[] {
                            new EventReminder() { Method = "email", Minutes = 24 * 60 },
                            new EventReminder() { Method = "sms", Minutes = 10 },
                        }
                },
                Transparency = "transparent"
            };

            EventsResource.InsertRequest apiRequest = service.Events.Insert(newEvent, "primary");
            Google.Apis.Calendar.v3.Data.Event createdEvent = apiRequest.Execute();

            response.Message = "Google Calendar Event Added: URL = " + createdEvent.HtmlLink;

            return response;
        }
    }
}
