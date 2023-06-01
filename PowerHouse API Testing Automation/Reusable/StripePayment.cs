using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json.Linq;
using PowerHouse_API_Testing_Automation.AppManager;

namespace PowerHouse_Api
{

    public class StripePayment
    {
        public static string ReturnString;

        public async Task StripePayment_()
        {
            Commands a = new Commands();
            string guid = "6d58fd4d-f353-4db2-b4bc-" + a.StringGenerator("alphanumeric", 18);
            string muid = "06053366-cc2c-4909-8864-" + a.StringGenerator("alphanumeric", 17);
            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = ~DecompressionMethods.None;
            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.stripe.com/v1/payment_methods"))
                {
                    request.Headers.TryAddWithoutValidation("authority", "api.stripe.com");
                    request.Headers.TryAddWithoutValidation("accept", "application/json");
                    request.Headers.TryAddWithoutValidation("accept-language", "en-US,en;q=0.9");
                    request.Headers.TryAddWithoutValidation("origin", "https://js.stripe.com");
                    request.Headers.TryAddWithoutValidation("referer", "https://js.stripe.com/");
                    request.Headers.TryAddWithoutValidation("sec-ch-ua", "\"Google Chrome\";v=\"113\", \"Chromium\";v=\"113\", \"Not-A.Brand\";v=\"24\"");
                    request.Headers.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
                    request.Headers.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
                    request.Headers.TryAddWithoutValidation("sec-fetch-dest", "empty");
                    request.Headers.TryAddWithoutValidation("sec-fetch-mode", "cors");
                    request.Headers.TryAddWithoutValidation("sec-fetch-site", "same-site");
                    request.Headers.TryAddWithoutValidation("sec-gpc", "1");
                    request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36");

                    var contentList = new List<string>();
                    contentList.Add($"type={Uri.EscapeDataString("card")}");
                    contentList.Add($"billing_details%5Bname%5D={Uri.EscapeDataString("Marnel")}");
                    contentList.Add($"card%5Bnumber%5D={Uri.EscapeDataString("4111111111111111")}");
                    contentList.Add($"card%5Bcvc%5D={Uri.EscapeDataString("999")}");
                    contentList.Add($"card%5Bexp_month%5D={Uri.EscapeDataString("12")}");
                    contentList.Add($"card%5Bexp_year%5D={Uri.EscapeDataString("35")}");
                    contentList.Add($"guid={Uri.EscapeDataString(guid)}");
                    contentList.Add($"muid={Uri.EscapeDataString(muid)}");
                    contentList.Add($"sid={Uri.EscapeDataString("5270c40a-fabf-41e1-87c5-495116ec11d096d9w6")}");
                    contentList.Add($"payment_user_agent={Uri.EscapeDataString("stripe.js/0c2dc5a68b;+stripe-js-v3/0c2dc5a68b")}");
                    contentList.Add($"time_on_page={Uri.EscapeDataString("22887")}");
                    contentList.Add($"key={Uri.EscapeDataString("pk_test_51MovxzJuhc1hdAaVySzQuwNAbCxX4mhv4zG8OvgiLoYq2Vazj27h2cSxaSqDbozI2HrqW5lFiWcmbHFpPIqsDYp900T86P0npc")}");
                    request.Content = new StringContent(string.Join("&", contentList));
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                    var response = await httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        JObject objId = JObject.Parse(responseContent);
                        string id = objId["id"].ToString();
                        ReturnString = id;
                    }
                    else
                    {
                        throw new Exception($"Stripe payment request failed: {response.StatusCode}");
                    }
                }
            }
        }

        public string Invoke()
        {
            StripePayment_().Wait();
            return ReturnString;
        }
    }
}
