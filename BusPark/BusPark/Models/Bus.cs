using System;

namespace BusPark.Models
{
    public class Bus
    {
        public string BusNumber { get; set; } // Номер автобуса
        public string DriverName { get; set; }  // Фамилия и инициалы водителя
        public string RouteNumber { get; set; } // Номер маршрута
        public Bus(string busNumber, string driverName, string routeNumber) // Чтобы создавать объект одной строчкой
        {
            BusNumber = busNumber;
            DriverName = driverName;
            RouteNumber = routeNumber;
        }
    }
}
