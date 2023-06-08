using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json.Linq;

namespace PowerHouse_Api
{
    [TestFixture]
    [Parallelizable]
    public class GenerateProjectMissingInfo
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ProjectName;
        public static string ProjectOverview;
        public static int ProjectId;

        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            ProjectName = b.StringGenerator("alphanumeric", 10);
            ProjectOverview = b.StringGenerator("alphanumeric", 50);

            string returnString = new CreateProject_Reusable().Invoke(ProjectName, ProjectOverview);
            JObject obj = JObject.Parse(returnString);
            int projectId = obj["createProject"]["id"].Value<int>();
            ProjectId = projectId;
        }

        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
query GenerateProjectMissingInfo($projectId: Float!) {
  generateProjectMissingInfo(project_id: $projectId) {
    infos
    text
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

            Console.WriteLine(response.Data);

        }

        public void PostTest()
        {
            new DeleteProject_Reusable().Invoke(ProjectId);
        }
    }

}