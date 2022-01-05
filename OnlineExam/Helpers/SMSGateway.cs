using Newtonsoft.Json;
using RestSharp;
using System;
using System.Configuration;

namespace OnlineExam.Helpers
{
    public static class SMSGateway
    {
        private readonly static string baseUrl = ConfigurationManager.AppSettings["Sms_ServiceProviderBaseUrl"];
        private readonly static string api_token = ConfigurationManager.AppSettings["Sms_Api_Token"];
        private readonly static string sid = ConfigurationManager.AppSettings["Sms_Api_SID"];

        public static Response SendSMS(string message, string receiverNumber)
        {
            var request = new RestRequest();
            request.AddHeader("Accept", "application/json");
            request.RequestFormat = DataFormat.Json; // Or DataFormat.Xml, if you prefer

            RestClient client = new RestClient();
            client = new RestClient(baseUrl);

            request.AddParameter("api_token", api_token);
            request.AddParameter("sid", sid);
            request.AddParameter("sms", message);
            request.AddParameter("msisdn", receiverNumber);
            request.AddParameter("csms_id", DateTime.Now.Date.ToString("yyyymmdd"));

            var response = client.Execute(request);
            var content = response.Content;
            var deserializeResponseResult = JsonConvert.DeserializeObject<Response>(content);
            return deserializeResponseResult;
            //return new Response { status_code = 200 };
        }
    }

    public class Response
    {
        public string status { get; set; }
        public int status_code { get; set; }
        public string error_message { get; set; }
    }
}