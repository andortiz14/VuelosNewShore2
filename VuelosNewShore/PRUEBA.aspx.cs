using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Entidades;
using Negocio;

namespace VuelosNewShore
{
    public partial class PRUEBA : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string url = "http://testapi.vivaair.com/otatest/api/values";
            string method = "POST";

            VueloEnt vueloEnt = new VueloEnt();
            VuelosNeg vuelosNeg = new VuelosNeg();

            vueloEnt.Origin = "BOG";
            vueloEnt.Destination = "CTG";
            vueloEnt.From = "2020-09-30";

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(vueloEnt);

           // var rest = vuelosNeg.GetVuelos(url, json, method);


        }
    }
}