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
    public class GetUserBidByTaskId
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
        public static int MarkUpPrice;
        public static decimal NewBidPrice;
        public static int BidId;

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
            NewBidPrice = new CalculateBid().GetMaxBid(Payout - 1);

            string returnOrg = new CreateProject_Reusable().Invoke(ProjectName, ProjectOverview);
            JObject orgObj = JObject.Parse(returnOrg);
            int projectId = orgObj["createProject"]["id"].Value<int>();
            ProjectId = projectId;

            string returnTask = new CreateTask_Reusable().Invoke(TaskName, TaskDescription, ProjectId, Payout, MaxTime, TimeEstimate);
            JObject obj = JObject.Parse(returnTask);
            int taskId = obj["createTask"]["task_id"].Value<int>();
            TaskId = taskId;
            new PublishTasksToMarketplace_Reusable().Invoke(TaskId);

            string returnBid = new CreateBid_Reusable().Invoke(Convert.ToInt32(NewBidPrice), TaskId);
            JObject objBid = JObject.Parse(returnBid);
            BidId = objBid["createBid"]["id"].Value<int>();
        }

        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
query GetUserBidByTaskId($taskId: Float!) {
  getUserBidByTaskId(task_id: $taskId) {
    amount
    created_at
    id
    status
    task {
      name
    }
    task_id
    updated_at
    user {
      email
    }
    user_id
  }
}
    ",
                Variables = new
                {
                    taskId = TaskId
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
            int returnTaskId = obj["getUserBidByTaskId"]["task_id"].Value<int>();
            int returnBidId = obj["getUserBidByTaskId"]["id"].Value<int>();


            if (returnTaskId != TaskId || returnBidId != BidId)
            {
                throw new Exception("Api return does not match");
            }
        }
    }

}