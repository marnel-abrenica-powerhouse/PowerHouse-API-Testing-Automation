using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;


namespace PowerHouse_Api
{

    public class DeleteProject_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static int ProjectId;

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
mutation DeleteProject($projectId: Float!) {
  deleteProject(project_id: $projectId) {
    name
    project_id
  }
}
    ",
                Variables = new
                {
                    projectId = ProjectId
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

        public void Invoke(int projectId)
        {
            ProjectId= projectId;
            MainTest().Wait();
        }
    }

}