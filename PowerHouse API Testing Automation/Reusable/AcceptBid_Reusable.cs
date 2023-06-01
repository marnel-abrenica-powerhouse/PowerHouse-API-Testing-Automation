using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

namespace PowerHouse_Api
{
    [TestFixture]

    public class AcceptBid_Reusable
    {
        public static String AuthToken;
        public static String BaseUrl;
        public static String ReturnString;
        public static int BidId;

        public void Precondition()
        {

            Get_Update_Config a = new();
 

            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");

        }


        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
mutation AcceptBid($bidId: Float!) {
  acceptBid(bid_id: $bidId) {
    amount
    created_at
    id
    status
    task_id
    updated_at
    user_id
    task {
      transaction {
        id
      }
    }
  }
}
    ",
                Variables = new
                {
                    bidId = BidId
                }
    };

            var client = new GraphQLHttpClient(BaseUrl, new NewtonsoftJsonSerializer());
            client.HttpClient.DefaultRequestHeaders.Add("Authorization", AuthToken);
            var response = await client.SendQueryAsync<dynamic>(query);
            if (response.Errors != null && response.Errors.Any())
            {
                // Handle the GraphQL response errors
                foreach (var error in response.Errors)
                {
                    Console.WriteLine($"GraphQL error: {error.Message}");
                }
                throw new Exception("GraphQL request failed.");
            }

            string jsonString = JsonConvert.SerializeObject(response.Data);
            ReturnString = jsonString;
        }

        public string Invoke(int bidId)
        {
            BidId= bidId;
            MainTest().Wait();
            return ReturnString;
        }
    }

}