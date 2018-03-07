using Showcase.ShowcaseService.Contract;
using System.ServiceModel;

namespace ShowcaseService
{
    [ServiceContract]
    public interface IZenoService
    { 
        [OperationContract]
        CarResponse GetCars(CarRequest request);

        [OperationContract]
        CalendarResponse GetGoogleCalendarWithAPI(CalendarRequest request);

        [OperationContract]
        GoogleResponse InvokeGoogleService(GoogleRequest request);
    }

}
