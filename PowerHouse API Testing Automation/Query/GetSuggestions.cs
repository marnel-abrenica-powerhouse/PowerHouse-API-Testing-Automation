using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PowerHouse_Api
{
    [TestFixture]
    [Parallelizable]
    public class GetSuggestions
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
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
        public static int UserId;


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
            Payout = int.Parse(b.StringGenerator("allnumbers", 3));
            MaxTime = int.Parse(b.StringGenerator("allnumbers", 2));
            TimeEstimate = int.Parse(b.StringGenerator("allnumbers", 2));

            string returnOrg = new CreateProject_Reusable().Invoke(ProjectName, ProjectOverview);
            JObject orgObj = JObject.Parse(returnOrg);
            int projectId = orgObj["createProject"]["id"].Value<int>();
            ProjectId = projectId;

            string returnTask = new CreateTask_Reusable().Invoke(TaskName, TaskDescription, ProjectId, Payout, MaxTime, TimeEstimate);
            JObject obj = JObject.Parse(returnTask);
            int taskId = obj["createTask"]["task_id"].Value<int>();
            TaskId = taskId;

            string returnUser = new User_Reusable().Invoke();
            JObject objUser = JObject.Parse(returnUser);
            UserId = objUser["user"]["id"].Value<int>();
        }

        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
query GetSuggestions($filters: SuggestionFilter!) {
  getSuggestions(filters: $filters) {
    association
    project {
      name
    }
    project_id
    status
    suggestion
    suggestion_id
    task {
      name
    }
    task_id
    title
    user {
      email
    }
    user_id
  }
}
    ",
                Variables = new
                {
                    projectId = ProjectId,
                    filters = new {
                        project_id = ProjectId,
                        status = "PENDING",
                        task_id = TaskId,
                        user_id = UserId

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
            Console.WriteLine(ReturnString);
            PostTest();
        }

        public void PostTest()
        {
            new DeleteProject_Reusable().Invoke(ProjectId);
        }
    }

}