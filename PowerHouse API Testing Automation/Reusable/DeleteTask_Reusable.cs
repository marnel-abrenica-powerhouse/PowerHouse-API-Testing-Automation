using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;


namespace PowerHouse_Api
{

    public class DeleteTask_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static int TaskId;

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
mutation DeleteTask($deleteTaskId: Float!) {
  deleteTask(id: $deleteTaskId) {
    name
  }
}
    ",
                Variables = new
                {
                    deleteTaskId = TaskId
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


        }

        public void Invoke(int taskId)
        {
            TaskId= taskId;
            MainTest().Wait();
        }
    }

}