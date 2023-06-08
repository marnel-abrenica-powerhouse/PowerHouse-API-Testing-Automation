using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace PowerHouse_Api
{
    [TestFixture]
    [Parallelizable]
    public class GetTaskById
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

        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();

            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            TaskName = b.StringGenerator("allletters", 10);
            TaskDescription = b.StringGenerator("alphanumeric", 50);
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
        }

        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
query GetTaskById($getTaskByIdId: Float!) {
  getTaskById(id: $getTaskByIdId) {
    ETA
    base_cost
    bids {
      id
    }
    complexity_level
    created_at
    description
    end_date
    end_date_to_bid
    lowest_bid
    markup_cost
    max_time
    name
    notion_link
    parent_id
    payout
    project {
      name
    }
    project_id
    remaining_time_to_start
    spent_time
    start_date
    status
    task_id
    task_sales_price
    task_type
    time_estimate
    total_bids
    transaction {
      id
        submissions {
        id
      }
    }
    updated_at
  }
}
    ",
                Variables = new
                {
                    getTaskByIdId = TaskId
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
            string taskName = obj["getTaskById"]["name"].ToString();
            string taskDescription = obj["getTaskById"]["description"].ToString();
            int projectId = obj["getTaskById"]["project_id"].Value<int>();
            int maxTime = obj["getTaskById"]["max_time"].Value<int>();
            int timeEstimate = obj["getTaskById"]["time_estimate"].Value<int>();


            if (TaskName != taskName ||
                TaskDescription != taskDescription ||
                ProjectId != projectId ||
                MaxTime != maxTime ||
                TimeEstimate != timeEstimate)
            {
                throw new Exception("Api return does not match");
            }

            new DeleteProject_Reusable().Invoke(ProjectId);
        }
    }

}