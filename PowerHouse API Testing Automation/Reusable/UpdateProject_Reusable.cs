using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;

namespace PowerHouse_Api
{


    public class UpdateProject_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static string UpdatedName;
        public static string UpdatedOverview;
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
        }

        public string Invoke(string updatedName, string updatedOverview, int projectId)
        {
            UpdatedName = updatedName;
            UpdatedOverview = updatedOverview;
            ProjectId = projectId;
            MainTest().Wait();
            return ReturnString;
        }

    }

}