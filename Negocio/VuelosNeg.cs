using System;
using System.Collections.Generic;
using System.Data;
using Entidades;
using System.Text;
using Datos;

namespace Negocio
{
    public class VuelosNeg
    {
        public string GetVuelos(string url, string VueloEntidad, string method = "POST")
        {

            Datos.VuelosData New = new Datos.VuelosData();
            return New.GetVuelos(url, VueloEntidad, method);

        }
        public int InsertVuelo(RespuestaVuelos vuelos)
        {
            Datos.VuelosData New = new Datos.VuelosData();
            return New.InsertVuelos(vuelos);
        }
    }
}
