using System.Runtime.Serialization;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Auth.OAuth2;

namespace Showcase.ShowcaseService.Contract
{
    [DataContract]
    public class Car
    {
        [DataMember]
        public string Brand { get; set; }
        [DataMember]       
        public string Model { get; set; }
        [DataMember]
        public string Color { get; set; }
    }

    [DataContract]
    public class Event
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string StartTime { get; set; }
        [DataMember]
        public string EndTime { get; set; }
    }

    [DataContract]
    public class GoogleUser
    {
        [DataMember]
        public string AccessToken { get; set; }

        [DataMember]
        public string RefreshToken { get; set; }

    }
}