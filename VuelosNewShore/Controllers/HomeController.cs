using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Entidades;
using Negocio;
using System.Web.Mvc;
using Newtonsoft.Json;
using OpenQA.Selenium.Remote;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using VuelosNewShore.Models;

namespace VuelosNewShore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(EntidadVuelo model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string url = "http://testapi.vivaair.com/otatest/api/values";
                    string method = "POST";
                    string fecha = model.From.ToString("yyyy-MM-dd");
                                        VueloEnt vueloEnt = new VueloEnt();
                    VuelosNeg vuelosNeg = new VuelosNeg();

                    vueloEnt.Origin = model.Origin;
                    vueloEnt.Destination = model.Destination;
                    vueloEnt.From = fecha;

                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(vueloEnt);

                    string rest = vuelosNeg.GetVuelos(url, json, method);

                    rest = rest.TrimStart('\"');
                    rest = rest.TrimEnd('\"');
                    rest = rest.Replace("\\", "");

                    List<RespuestaVuelos> vuelosList = JsonConvert.DeserializeObject<List<RespuestaVuelos>>(rest);

                    return View(vuelosList);
                }
                return Redirect("Index");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}