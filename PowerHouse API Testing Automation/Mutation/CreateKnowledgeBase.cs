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

    public class CreateKnowledgeBase
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static string ProjectName;
        public static string ProjectOverview;
        public static int ProjectId;
        public static string Information;



        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();

            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            ProjectName = b.StringGenerator("alphanumeric", 10);
            ProjectOverview = b.StringGenerator("alphanumeric", 50);
            Information = b.StringGenerator("alphanumeric", 100);


            string returnString = new CreateProject_Reusable().Invoke(ProjectName, ProjectOverview);
            JObject obj = JObject.Parse(returnString);
            ProjectId = obj["createProject"]["id"].Value<int>();
        }




        [Test]
        public async Task MainTest()
        {

            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
mutation CreateKnowledgeBase($data: CreateKnowledgeBaseInput!) {
  createKnowledgeBase(data: $data) {
    information
    knowledge_base_id
    project_id
  }
}
    ",  
                Variables = new
                {
                    data = new {
                    information = Information,
                    project_id = ProjectId
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

            ReturnString = JsonConvert.SerializeObject(response.Data); ;
            VerifyResponse();
        }



        public void VerifyResponse()
        {
            JObject obj = JObject.Parse(ReturnString);
            string information = obj["createKnowledgeBase"]["information"].ToString();
            int projectId = obj["createKnowledgeBase"]["project_id"].Value<int>();


            if (  Information != information 
                ||ProjectId != projectId 
                ) 
            {
                throw new Exception("Api return does not match");
            }

            new DeleteProject_Reusable().Invoke(ProjectId);
        }

    }

}