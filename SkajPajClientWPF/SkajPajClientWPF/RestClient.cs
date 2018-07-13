using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SkajPajClientWPF
{
    public enum httpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class RestClient
    {
        public string endPoint { get; set; }
        public httpVerb httpMethod { get; set; }

        public RestClient()
        {
            endPoint = string.Empty;
            httpMethod = httpVerb.GET;
        }

        public string makeRequest()
        {
            string strResponseValue = string.Empty;

            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endPoint);

                request.Method = httpMethod.ToString();
            
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new ApplicationException("ERROR CODE: " + response.StatusCode.ToString());
                    }

                    //Process the response stream... (could be JSON, XML or HTTL etc....)

                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            using (StreamReader reader = new StreamReader(responseStream))
                            {
                                strResponseValue = reader.ReadToEnd();
                            }//End of StreamReader
                        }
                    }//End of using ResponseStream

                }//End of using response
            } catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            

            return strResponseValue;
        }
    }
}
