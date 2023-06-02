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
    public class KnowMoreAboutProject
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
            ProjectName = b.StringGenerator("alphanumeric", 15);
            ProjectOverview = b.StringGenerator("alphanumeric", 50);

            string returnProjectId = new CreateProject_Reusable().Invoke(ProjectName, ProjectOverview);
            JObject objProjectId = JObject.Parse(returnProjectId);
            ProjectId = objProjectId["createProject"]["id"].Value<int>();
        }

        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
query Query($projectId: Float!, $question: String!) {
  knowMoreAboutProject(project_id: $projectId, question: $question)
}
    ",
                Variables = new
                {
                    projectId = ProjectId,
                    question = "explain the project"
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
            PostTest();
        }

        public void PostTest()
        {
            new DeleteProject_Reusable().Invoke(ProjectId);
        }
    }

}