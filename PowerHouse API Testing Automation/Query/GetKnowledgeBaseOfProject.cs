using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PowerHouse_Api
{
    [TestFixture]
    [Parallelizable]
    public class GetKnowledgeBaseOfProject
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static int ProjectId;
        public static string ProjectName;
        public static string ProjectOverview;
        public static string Information;

        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            ProjectName = b.StringGenerator("allletters", 10);
            ProjectOverview = b.StringGenerator("alphanumeric", 50);
            Information = b.StringGenerator("alphanumeric", 50);

            string returnOrg = new CreateProject_Reusable().Invoke(ProjectName, ProjectOverview);
            JObject orgObj = JObject.Parse(returnOrg);
            int projectId = orgObj["createProject"]["id"].Value<int>();
            ProjectId = projectId;

            new CreateKnowledgeBase_Reusable().Invoke(Information, ProjectId);
        }

        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
query GetKnowledgeBaseOfProject($filters: KnowledgeBaseFilter!) {
  getKnowledgeBaseOfProject(filters: $filters) {
    information
    knowledge_base_id
    project {
      name
    }
    project_id
  }
}
    ",
                Variables = new
                {
                    filters = new {
                        project_id = ProjectId,

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
            Console.WriteLine(ReturnString);
            PostTest();

        }

        public void PostTest()
        {
            JObject obj = JObject.Parse(ReturnString);
            string information = obj["getKnowledgeBaseOfProject"]["information"].ToString();
            int projectId = obj["getKnowledgeBaseOfProject"]["project_id"].Value<int>();

            if (information != Information || projectId != ProjectId)
            {
                throw new Exception("Api return does not match");
            }
            new DeleteProject_Reusable().Invoke(ProjectId);
        }
    }

}