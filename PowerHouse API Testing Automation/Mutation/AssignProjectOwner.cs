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

    public class AssignProjectOwner
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static string ProjectName;
        public static string ProjectOverview;
        public static int ProjectId;
        public static int UserId;


        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();

            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            ProjectName = b.StringGenerator("alphanumeric", 10);
            ProjectOverview = b.StringGenerator("alphanumeric", 50);


            string returnString = new CreateProject_Reusable().Invoke(ProjectName, ProjectOverview);
            JObject objProjectId = JObject.Parse(returnString);
            int projectId = objProjectId["createProject"]["id"].Value<int>();
            ProjectId = projectId;

            string returnUser = new User_Reusable().Invoke();
            JObject objUser = JObject.Parse(returnUser);
            int userId = objUser["user"]["id"].Value<int>();
            UserId  = userId;
        }


        [Test]
        public async Task MainTest()
        {

            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
mutation AssignProjectOwner($projectId: Float!, $userId: Float!) {
  assignProjectOwner(project_id: $projectId, user_id: $userId) {
    project_id
    organization_id
    project_owner_id
    project_manager {
      profile {
        user_id
      }
    }
    project_type
    tech_lead_id
  }
}
    ",
                Variables = new
                {
                    projectId = ProjectId,
                    userId = UserId
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
            VerifyResponse();
          
        }


        public void VerifyResponse()
        {
           
            JObject obj = JObject.Parse(ReturnString);
            int responseProjectId = obj["assignProjectOwner"]["project_id"].Value<int>();
            int responseUserId = obj["assignProjectOwner"]["project_manager"]["profile"]["user_id"].Value<int>();

            if (ProjectId != responseProjectId  ||
                UserId != responseUserId) 
            {
                throw new Exception("Api return does not match");
            }
            new DeleteProject_Reusable().Invoke(ProjectId);
           
        }

    }

}