using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PowerHouse_Api
{
    [TestFixture]

    public class UpdateProject
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static string Name;
        public static string Overview;
        public static string UpdatedName;
        public static string UpdatedOverview;
        public static int OrgId;
        public static int ProjectId;

        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            OrgId = int.Parse(a.GetConfig_("orgId"));
            Name = b.StringGenerator("alphanumeric" ,10);
            Overview = b.StringGenerator("alphanumeric", 50);
            UpdatedName = b.StringGenerator("alphanumeric", 10);
            UpdatedOverview = b.StringGenerator("alphanumeric", 50);

            string returnProjectId = new CreateProject_Reusable().Invoke(Name, Overview);
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
mutation UpdateProject($data: IUpdateProjectDTO!, $projectId: Float!) {
  updateProject(data: $data, project_id: $projectId) {
    name
    overview
    organization_id
    project_id
  }
}
    ",
                Variables = new
                {
                    data = new 
                        {
                        name= UpdatedName,
                        overview= UpdatedOverview
                        },
                    projectId= ProjectId
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
            VerifyResponse();
        }

        public void VerifyResponse()
        {
            JObject obj = JObject.Parse(ReturnString);
            int responseProjectId = obj["updateProject"]["project_id"].Value<int> ();
            string responseName = obj["updateProject"]["name"].ToString();
            string responseOverview = obj["updateProject"]["overview"].ToString();

            if (UpdatedName != responseName || UpdatedOverview != responseOverview || ProjectId != responseProjectId)
            {
                throw new Exception("Api return does not match");
            }
            new DeleteProject_Reusable().Invoke(ProjectId);
        }

    }

}