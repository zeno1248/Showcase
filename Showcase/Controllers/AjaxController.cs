using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace Showcase.Controllers
{
    public class AjaxController : Controller
    {

        public ActionResult GetCars()
        {
            List<Car> cars = new List<Car>();
            try
            {
                string connString = ConfigurationManager.ConnectionStrings["Azure"].ConnectionString;
                using (var conn = new SqlConnection(connString))
                {
                    using (var adapter = new SqlDataAdapter("SELECT * FROM dbo.CARS", conn))
                    {
                        using (var dataTable = new DataTable())
                        {
                            adapter.Fill(dataTable);
                            foreach (DataRow row in dataTable.Rows)
                            {
                                cars.Add(new Car { Brand = row["brand"].ToString(), Model = row["model"].ToString(), Color = row["color"].ToString() });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.ToString();
            }

            //var person = new Person { FirstName = fname, LastName = lname };
            return Json(cars);
        }

        /*
        public ActionResult GetChineseCalendar(int year, int month)
        {
            MonthData monthData = ChineseCalendar.GetMonthData(year, month);

            return Json(monthData);
        }
        */
    }

    public class Car
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
    }

}