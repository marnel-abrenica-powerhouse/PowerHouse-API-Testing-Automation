using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;

namespace PowerHouse_Api
{
    [TestFixture]
    [Parallelizable]
    public class GetLowestBidByTaskId
    {
        public static String AuthToken;
        public static String BaseUrl;
        public static String ProjectId;

        public void Preconditino()
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
            Preconditino();

            var query = new GraphQLRequest
            {
                Query = @"
query GetLowestBidByTaskId($taskId: Float!) {
  getLowestBidByTaskId(task_id: $taskId) {
    amount
    created_at
    id
    status
    task {
      name
    }
    task_id
    updated_at
    user {
      email
    }
    user_id
  }
}
    ",
                Variables = new
                {
                    taskId = 101
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