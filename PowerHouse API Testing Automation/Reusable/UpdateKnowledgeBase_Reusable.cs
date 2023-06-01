using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;

namespace PowerHouse_Api
{ 

    public class UpdateKnowledgeBase_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static int KnowledgeBaseId;
        public static string UpdateInformation;


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
mutation UpdateKnowledgeBase($knowledgeBaseId: Float!, $data: UpdateKnowledgeBaseInput!) {
  updateKnowledgeBase(knowledge_base_id: $knowledgeBaseId, data: $data) {
    information
    knowledge_base_id
    project_id
  }
}
    ",  
                Variables = new
                {
                    knowledgeBaseId = KnowledgeBaseId,
                    data = new 
                        {
                        information = UpdateInformation
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

            ReturnString = JsonConvert.SerializeObject(response.Data);
        }

        public string Invoke(int knowledgeBaseId, string updateInformation)
        {
            KnowledgeBaseId= knowledgeBaseId;
            UpdateInformation = updateInformation;
            MainTest().Wait();
            return ReturnString;
        }
    }

}