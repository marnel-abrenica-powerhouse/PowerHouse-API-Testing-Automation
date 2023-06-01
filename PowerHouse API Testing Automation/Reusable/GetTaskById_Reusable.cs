using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;

namespace PowerHouse_Api
{

    public class GetTaskById_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static int TaskId;
        public static string ReturnString;

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
query GetTaskById($getTaskByIdId: Float!) {
  getTaskById(id: $getTaskByIdId) {
    ETA
    base_cost
    bids {
      id
    }
    complexity_level
    created_at
    description
    end_date
    end_date_to_bid
    lowest_bid
    markup_cost
    max_time
    name
    notion_link
    parent_id
    payout
    project {
      name
    }
    project_id
    remaining_time_to_start
    spent_time
    start_date
    status
    task_id
    task_sales_price
    task_type
    time_estimate
    total_bids
    transaction {
      id
        submissions {
        id
      }
    }
    updated_at
  }
}
    ",
                Variables = new
                {
                    getTaskByIdId = TaskId
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

        public string Invoke(int taskId)
        {
            TaskId = taskId;
            MainTest().Wait();
            return ReturnString;    
        }

    }

}