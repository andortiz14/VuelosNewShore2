using System;
using System.Collections.Generic;
using System.Text;

namespace Entidades
{
    public class RespuestaVuelos
    {
        public string DepartureDate { get; set; }
        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
        public string FlightNumber { get; set; }
        public string Price { get; set; }
        public string Currency { get; set; }
    }
}
