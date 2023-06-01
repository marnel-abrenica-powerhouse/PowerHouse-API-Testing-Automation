using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;

namespace PowerHouse_Api
{

    public class AcceptSuggestion_Reusable
    {
        public static String AuthToken;
        public static String BaseUrl;
        public static String ReturnString;
        public static int SuggestionId;

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
mutation AcceptSuggestion($suggestionId: Float!) {
  acceptSuggestion(suggestion_id: $suggestionId) {
    association
    project_id
    status
    suggestion
    suggestion_id
    task {
      transaction {
        id
      }
    }
    task_id
    title
  }
}
    ",
                Variables = new
                {
                    suggestionId = SuggestionId
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

        public string Invoke(int suggestionId)
        {
            SuggestionId= suggestionId;
            MainTest().Wait();
            return ReturnString;
        }
    }

}