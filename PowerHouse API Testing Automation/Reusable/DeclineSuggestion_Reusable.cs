using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace PowerHouse_Api
{

    public class DeclineSuggestion_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
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
mutation DeclineSuggestion($suggestionId: Float!) {
  declineSuggestion(suggestion_id: $suggestionId) {
    association
    project {
      project_id
    }
    project_id
    status
    suggestion
    suggestion_id
    task {
      task_id
    }
    task_id
    title
    user {
      user_id
    }
    user_id
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
            SuggestionId = suggestionId;
            MainTest().Wait();
            return ReturnString;
        }
    }

}