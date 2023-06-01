using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;


namespace PowerHouse_Api
{ 


    public class CreateKnowledgeBase_Reusable
    {
        public static String AuthToken;
        public static String BaseUrl;
        public static String ReturnString;
        public static int ProjectId;
        public static string Information;



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

        }

        public string Invoke(string information, int projectId)
        {
            ProjectId= projectId;
            Information= information;
            MainTest().Wait();
            return ReturnString;
        }

    }

}