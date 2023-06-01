using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;

namespace PowerHouse_Api
{

    public class AssignProjectOwner_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static int ProjectId;
        public static int UserId;


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
          
        }

        public string Invoke(int projectId, int userId)
        {
            ProjectId= projectId;
            UserId= userId;
            MainTest().Wait();
            return ReturnString;
        }
    }

}