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

    public class AcceptSuggestion
    {
        public static String AuthToken;
        public static String BaseUrl;
        public static String ReturnString;
        public static string TaskName;
        public static string TaskDescription;
        public static string ProjectName;
        public static string ProjectOverview;
        public static int ProjectId;
        public static int Payout;
        public static int TaskId;
        public static int MaxTime;
        public static int TimeEstimate;
        public static string Suggestion;
        public static string SuggestionTitle;
        public static int SuggestionId;

        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();

            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            TaskName = b.StringGenerator("allletters", 10);
            TaskDescription = b.StringGenerator("alphanumeric", 50);
            SuggestionTitle = b.StringGenerator("allletters", 10);
            Suggestion = b.StringGenerator("alphanumeric", 50);
            ProjectName = b.StringGenerator("allletters", 10);
            ProjectOverview = b.StringGenerator("alphanumeric", 50);
            Payout = int.Parse( b.StringGenerator("allnumbers", 3));
            MaxTime = int.Parse(b.StringGenerator("allnumbers", 2));
            TimeEstimate = int.Parse(b.StringGenerator("allnumbers", 2));
  
            

            string returnOrg = new CreateProject_Reusable().Invoke(ProjectName, ProjectOverview);
            JObject orgObj = JObject.Parse(returnOrg);
            int projectId = orgObj["createProject"]["id"].Value<int>();
            ProjectId = projectId;

            string returnTask =  new CreateTask_Reusable().Invoke(TaskName, TaskDescription, ProjectId, Payout, MaxTime, TimeEstimate);
            JObject obj = JObject.Parse(returnTask);
            int taskId = obj["createTask"]["task_id"].Value<int>();
            TaskId = taskId;

            string returnSuggestionId = new CreateSuggestionInput_Reusable().Invoke(ProjectId, TaskId, SuggestionTitle, Suggestion);
            JObject objSuggestion = JObject.Parse(returnSuggestionId);
            int suggestionId = objSuggestion["createSuggestion"]["suggestion_id"].Value<int>();
            SuggestionId = suggestionId;
        }


        [Test]
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
            Console.WriteLine(ReturnString);
            VerifyResponse();
        }

        public void VerifyResponse()
        {
            JObject obj = JObject.Parse(ReturnString);
            int taskIdReturn = obj["acceptSuggestion"]["task_id"].Value<int>();
            int projectIdReturn = obj["acceptSuggestion"]["project_id"].Value<int>();
            string suggestionTitleReturn = obj["acceptSuggestion"]["title"].ToString();
            string suggestionReturn = obj["acceptSuggestion"]["suggestion"].ToString();
            int suggestionIdReturn = obj["acceptSuggestion"]["suggestion_id"].Value<int>();

            if (taskIdReturn!= TaskId || projectIdReturn != ProjectId || suggestionTitleReturn != SuggestionTitle || suggestionReturn != Suggestion || suggestionIdReturn != SuggestionId)
            { 
                throw new Exception("Api return does not match");
            }

            new DeleteTask_Reusable().Invoke(TaskId);
            new DeleteProject_Reusable().Invoke(ProjectId);
        }

    }

}