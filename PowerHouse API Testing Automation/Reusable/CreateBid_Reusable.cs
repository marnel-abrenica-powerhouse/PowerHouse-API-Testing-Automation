﻿using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;


namespace PowerHouse_Api
{
    public class CreateBid_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static int TaskId;
        public static int BidPrice;


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
mutation CreateBid($data: CreateBidInput!) {
  createBid(data: $data) {
    task {
      transaction {
        id
      }
    }
    id
    task_id
    user_id
    amount
  }
}
    ",
                Variables = new
                {
                    data = new {
                    amount = BidPrice,
                    task_id = TaskId
                    }
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


        public string Invoke(int bidPrice, int taskId)
        {
            BidPrice = bidPrice;
            TaskId = taskId;
            MainTest().Wait();
            return ReturnString;            
        }
    }

}