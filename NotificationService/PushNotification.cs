using Newtonsoft.Json.Linq;
using NotificationService.Models;
using PushSharp.Apple;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace NotificationService
{
    public class PushNotification
    {
        public enum MobileType
        {
            Android, 
            iOS 
        }

        const string _androidApiKey = "(adroid app api key goes here)";
        const string _iOSP12Filename = "(certificate filename goes here)"; // file must be physically present in root
        const string _iOSP12Password = "(certificate password goes here)";

        public static Result Send(MobileType mobileType, string deviceToken, string jsonPayload)
        {
            if (mobileType == MobileType.Android)
            {
                try
                {
                    HttpWebRequest req = WebRequest.CreateHttp("https://fcm.googleapis.com/fcm/send");
                    req.Method = "POST";
                    req.ContentType = "application/json";
                    req.ServerCertificateValidationCallback = delegate (object nothing, X509Certificate cert, X509Chain chain, SslPolicyErrors err) { return true; };
                    req.Headers.Add("Authorization", "key=" + _androidApiKey);
                    using (StreamWriter sw = new StreamWriter(req.GetRequestStream()))
                    {
                        sw.Write(jsonPayload);
                    }
                    try
                    {
                        WebResponse res = req.GetResponse();
                        string result = string.Empty;
                        using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                        {
                            result = sr.ReadToEnd();
                        }
                        return new() { HasErrors = false, Response = result };
                    }
                    catch (Exception err)
                    {
                        return new() { HasErrors = true, Response = err.Message };
                    }
                }
                catch (Exception err)
                {
                    return new() { HasErrors = true, Response = err.Message };
                }
            }
            else if (mobileType == MobileType.iOS)
            {
                try
                {
                    var res = new Result() { HasErrors = false, Response = string.Empty };
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    ApnsServiceBroker apnsBroker = new ApnsServiceBroker(
                        new ApnsConfiguration(
#if DEBUG 
                            ApnsConfiguration.ApnsServerEnvironment.Sandbox,
#else
                            ApnsConfiguration.ApnsServerEnvironment.Production,
#endif 
                            new X509Certificate(_iOSP12Filename, _iOSP12Password).GetRawCertData(), _iOSP12Password));

                    apnsBroker.OnNotificationFailed += (notification, exception) => { res = new() { HasErrors = true, Response = exception.Message }; };
                    apnsBroker.Start();
                    try
                    {
                        apnsBroker.QueueNotification(new ApnsNotification(deviceToken, JObject.Parse(jsonPayload)));
                    }
                    catch (Exception err)
                    {
                        res = new() { HasErrors = true, Response = err.Message };
                    }
                    finally
                    {
                        apnsBroker.Stop();
                    }
                    return res;
                }
                catch (Exception err)
                {
                    return new() { HasErrors = true, Response = err.Message };
                }
            }
            else
            {
                return new() { HasErrors = false, Response = "Device not supported." };
            }
        }
    }
}
