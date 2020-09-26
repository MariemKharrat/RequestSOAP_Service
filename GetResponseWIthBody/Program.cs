using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GetResponseWIthBody
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpStatusCode code;
            string body = "<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:tem=\"http://tempuri.org/\">\r\n   <soapenv:Header/>\r\n   <soapenv:Body>\r\n      <tem:EmployeeAsync></tem:EmployeeAsync>\r\n   </soapenv:Body>\r\n</soapenv:Envelope>";
            String resp = GetResponseWithBody("https://localhost:44376/Service.asmx?wsdl", HttpMethod.Post, body, out code);
            Console.WriteLine(resp);
            Console.ReadLine();
        }

        public static string GetResponseWithBody(string url, HttpMethod method, String Body, out HttpStatusCode status)
        {
            String resp = String.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method.Method;
            request.ContentType = "application/xml";
            request.Accept = "application/xml";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            if (Body.Equals(String.Empty))
            {
                request.ContentLength = 0;
            }
            else
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(Body);
                }
            }
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    status = response.StatusCode;
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            resp = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    if (httpResponse.StatusCode.Equals(null))
                    {
                        throw new Exception("Could not read HTTP Status code, Endpoint is unreachable.");
                    }
                    else
                    {
                        status = httpResponse.StatusCode;
                        using (Stream data = response.GetResponseStream())
                        using (var reader = new StreamReader(data))
                        {
                            resp = reader.ReadToEnd();
                            }
                    }
                }
            }
            catch (Exception e)
            {
               status = HttpStatusCode.BadGateway;
                resp = String.Empty;
            }
           return resp;
        }
    }
}
