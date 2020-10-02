using System;
using System.Collections.Generic;
using System.Text;

namespace Entidades
{
    public class RespuestaVuelos
    {
        public DateTime DepartureDate { get; set; }
        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
        public string FlightNumber { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }

        //public string DepartureDate { get; set; }
        //public string DepartureStation { get; set; }
        //public string ArrivalStation { get; set; }
        //public string FlightNumber { get; set; }
        //public string Price { get; set; }
        //public string Currency { get; set; }
    }
}
