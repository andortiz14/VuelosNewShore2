using System;
using System.Net.Http;
using Entidades;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;
using System.Linq;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;


namespace Datos
{
    public class VuelosData
    {
        public string GetVuelos (string url, string VueloEntidad, string method)
        {

            string result = "";


            try
            {

                JavaScriptSerializer js = new JavaScriptSerializer();

                ////serializamos el objeto
                //string json =  Newtonsoft.Json.JsonConvert.SerializeObject(objectRequest);

                //peticion
                WebRequest request = WebRequest.Create(url);
                //headers
                request.Method = method;
                request.PreAuthenticate = true;
                request.ContentType = "application/json;charset=utf-8'";
                request.Timeout = 10000; 

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(VueloEntidad);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

            }
            catch (Exception e)
            {

                result = e.Message;

            }

            return result;
        }
    }
}

