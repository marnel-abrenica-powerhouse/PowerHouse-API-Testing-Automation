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
    public class GetLowestBidByTaskId
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static string Name;
        public static string Description;
        public static string ProjectName;
        public static string ProjectOverview;
        public static int ProjectId;
        public static int Payout;
        public static int MaxTime;
        public static int TimeEstimate;
        public static int NewBidPrice;
        public static int TaskId;
        public static int BidId;

        public void Preconditino()
        {
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            Name = b.StringGenerator("alphanumeric", 10);
            Description = b.StringGenerator("alphanumeric", 50);
            ProjectName = b.StringGenerator("alphanumeric", 10);
            ProjectOverview = b.StringGenerator("alphanumeric", 50);
            Payout = int.Parse(b.StringGenerator("allnumbers", 4));
            MaxTime = int.Parse(b.StringGenerator("allnumbers", 2));
            TimeEstimate = int.Parse(b.StringGenerator("allnumbers", 2));
            NewBidPrice = new CalculateBid().GetMaxBid(Payout-1);

            string returnString = new CreateProject_Reusable().Invoke(ProjectName, ProjectOverview);
            JObject obj = JObject.Parse(returnString);
            int projectId = obj["createProject"]["id"].Value<int>();
            ProjectId = projectId;

            string returnTask = new CreateTask_Reusable().Invoke(Name, Description, ProjectId, Payout, MaxTime, TimeEstimate);
            JObject objTaskId = JObject.Parse(returnTask);
            TaskId = objTaskId["createTask"]["task_id"].Value<int>();
            new PublishTasksToMarketplace_Reusable().Invoke(TaskId);

            string returnBid = new CreateBid_Reusable().Invoke(Convert.ToInt32(NewBidPrice), TaskId);
            JObject objBid = JObject.Parse(returnBid);
            BidId = objBid["createBid"]["id"].Value<int>();
        }

        [Test]
        public async Task MainTest()
        {
            Preconditino();

            var query = new GraphQLRequest
            {
                Query = @"
query GetLowestBidByTaskId($taskId: Float!) {
  getLowestBidByTaskId(task_id: $taskId) {
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
            int bidId = obj["getLowestBidByTaskId"]["id"].Value<int>();
            int taskIdReturn = obj["getLowestBidByTaskId"]["task_id"].Value<int>();

            if (bidId != BidId || taskIdReturn != TaskId)
            {
                throw new Exception("Api return does not match");
            }

            new DeleteTask_Reusable().Invoke(TaskId);
            new DeleteProject_Reusable().Invoke(ProjectId);
        }
    }

}