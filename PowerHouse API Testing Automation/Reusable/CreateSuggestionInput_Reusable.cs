using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;

namespace PowerHouse_Api
{

    public class CreateSuggestionInput_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static int ProjectId;
        public static int TaskId;
        public static string Suggestion;
        public static string SuggestionTitle;


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
mutation CreateSuggestion($data: CreateSuggestionInput!) {
  createSuggestion(data: $data) {
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
    user_id
  }
}
    ",
                Variables = new
                {
                    data = new {
                    project_id = ProjectId,
                    suggestion = Suggestion,
                    task_id = TaskId,
                    title = SuggestionTitle
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

        }

        public string Invoke(int projectId, int taskId, string suggestionTitle, string suggestion)
        {
            ProjectId= projectId;
            TaskId= taskId;
            SuggestionTitle= suggestionTitle;
            Suggestion= suggestion;
            MainTest().Wait();
            return ReturnString;
        }
    }

}