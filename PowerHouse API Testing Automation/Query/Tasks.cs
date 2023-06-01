using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;

namespace PowerHouse_Api
{
    [TestFixture]
    [Parallelizable]
    public class Tasks
    {
        public static String AuthToken;
        public static String BaseUrl;
        public static String ProjectId;

        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            ProjectId = b.StringGenerator();


        }

        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
query Tasks {
  tasks {
    name
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
    }
    updated_at
  }
}
    ",
                Variables = new
                {
                    
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

            Console.WriteLine(response.Data);

        }

    }

}