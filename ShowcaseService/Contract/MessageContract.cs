using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Showcase.ShowcaseService.Contract
{
    public enum GoogleRequestType
    {
        Default = 0,
        CalendarRead = 1,
        CalendarPush = 2
    }

    [MessageContract]
    public class CarRequest
    {
        [MessageBodyMember]
        public string CarModel { get; set; }

        [MessageBodyMember]
        public string CarColor { get; set; }
    }

    [MessageContract]
    public class CarResponse
    {
        [MessageBodyMember]
        public List<Car> Cars { get; set; }

        [MessageBodyMember]
        public int Count { get; set; }
    }

    [MessageContract]
    public class CalendarRequest
    {
        [MessageBodyMember]
        public DateTime StartTime { get; set; }

        [MessageBodyMember]
        public string CalendarName { get; set; }

        [MessageBodyMember]
        public string SortBy { get; set; }
    }

    [MessageContract]
    public class CalendarResponse
    {
        [MessageBodyMember]
        public List<Event> Events { get; set; }
    }

    [MessageContract]
    public class GoogleRequest
    {
        [MessageBodyMember]
        public GoogleUser User { get; set; }

        [MessageBodyMember]
        public GoogleRequestType Type { get; set; }
    }

    [MessageContract]
    public class GoogleResponse
    {
        [MessageBodyMember]
        public string Status { get; set; }

        [MessageBodyMember]
        public string Message { get; set; }
    }
}