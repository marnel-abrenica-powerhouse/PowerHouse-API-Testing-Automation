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
    public class Tasks
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static string TaskName;
        public static string TaskDescription;
        public static string ProjectName;
        public static string ProjectOverview;
        public static int TaskId;
        public static int ProjectId;
        public static int Payout;
        public static int MaxTime;
        public static int TimeEstimate;

        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            TaskName = b.StringGenerator("allletters", 10);
            TaskDescription = b.StringGenerator("alphanumeric", 50);
            Payout = int.Parse(b.StringGenerator("allnumbers", 4));
            MaxTime = int.Parse(b.StringGenerator("allnumbers", 3));
            TimeEstimate = int.Parse(b.StringGenerator("allnumbers", 3));

            string returnOrg = new CreateProject_Reusable().Invoke(ProjectName, ProjectOverview);
            JObject orgObj = JObject.Parse(returnOrg);
            int projectId = orgObj["createProject"]["id"].Value<int>();
            ProjectId = projectId;

            string returnString = new CreateTask_Reusable().Invoke(TaskName, TaskDescription, ProjectId, Payout, MaxTime, TimeEstimate);
            JObject obj = JObject.Parse(returnString);
            int taskId = obj["createTask"]["task_id"].Value<int>();
            TaskId = taskId;

            new PublishTasksToMarketplace_Reusable().Invoke(TaskId);

        }

        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
query Tasks($assignee: FilterTask, $taskStatus: TaskStatus) {
  tasks(
    filters: {assignee: $assignee, bring_expired: true, task_status: $taskStatus}
  ) {
    id: task_id
    name
    lowestBid: lowest_bid
    maxTime: max_time
    timeEstimate: time_estimate
    createdAt: created_at
    project {
      stacks {
        name: stack_name
      }
    }
  }
}
    ",
                Variables = new
                {
                    
                    assignee = "UNASSIGNED",
                    taskStatus = "PUBLISHED"

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
            JObject obj = JObject.Parse(ReturnString);
            JArray taskList = (JArray)obj["tasks"];
            int taskFound = 0;

            foreach (JObject tasks in taskList)
            {
                string raw = (string)tasks["id"];
                int resutlProjectId = Convert.ToInt32(raw);

                if (resutlProjectId == TaskId)
                {
                    taskFound++;
                }

            }

            if (taskFound == 0)
            {
                throw new Exception("Task not found");
            }

            new DeleteTask_Reusable().Invoke(TaskId);
            new DeleteProject_Reusable().Invoke(ProjectId);
        }
    }

}