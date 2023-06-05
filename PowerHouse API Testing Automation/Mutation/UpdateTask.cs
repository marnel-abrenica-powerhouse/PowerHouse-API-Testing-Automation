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

    public class UpdateTask
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
        public static string UpdatedName;
        public static string UpdatedDescription;
        public static int UpdatedPayout;
        public static int UpdatedMaxTime;
        public static int UpdatedTimeEstimate;

        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            Name = b.StringGenerator("alphanumeric" ,10);
            Description = b.StringGenerator("alphanumeric", 50);
            ProjectName = b.StringGenerator("alphanumeric", 10);
            ProjectOverview = b.StringGenerator("alphanumeric", 50);
            Payout = int.Parse(b.StringGenerator("allnumbers", 4));
            MaxTime = int.Parse(b.StringGenerator("allnumbers", 2));
            TimeEstimate = int.Parse(b.StringGenerator("allnumbers", 2));
            NewBidPrice = new CalculateBid().GetMaxBid(Payout);
            UpdatedName = b.StringGenerator("alphanumeric", 10);
            UpdatedDescription = b.StringGenerator("alphanumeric", 50);
            UpdatedPayout = int.Parse(b.StringGenerator("allnumbers", 4));
            UpdatedMaxTime = int.Parse(b.StringGenerator("allnumbers", 2));
            UpdatedTimeEstimate = int.Parse(b.StringGenerator("allnumbers", 2));

            string returnString = new CreateProject_Reusable().Invoke(ProjectName, ProjectOverview);
            JObject obj = JObject.Parse(returnString);
            ProjectId = obj["createProject"]["id"].Value<int>();

            string returnCreateTask = new CreateTask_Reusable().Invoke(Name, Description, ProjectId, Payout, MaxTime, TimeEstimate);
            JObject objCreateTask = JObject.Parse(returnCreateTask);
            TaskId = objCreateTask["createTask"]["task_id"].Value<int>();
        }




        [Test]
        public async Task MainTest()
        {

            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
mutation UpdateTask($data: IUpdateProjectTaskDTO!, $taskId: Float!) {
  updateTask(data: $data, task_id: $taskId) {
    name
    description
    payout
    max_time
    time_estimate
    task_id
    project_id
  }
}
    ",
                Variables = new
                {
                    data = new 
                        {
                        name = UpdatedName,
                        description = UpdatedDescription,
                        payout = UpdatedPayout,
                        max_time = UpdatedMaxTime,
                        time_estimate = UpdatedTimeEstimate
                        },
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
            string updatedName = obj["updateTask"]["name"].ToString();
            string updatedDescription = obj["updateTask"]["description"].ToString();
            int responseProjectId = obj["updateTask"]["project_id"].Value<int>();
            int updatedMaxTime = obj["updateTask"]["max_time"].Value<int>();
            int udpdatedTimeEstimate = obj["updateTask"]["time_estimate"].Value<int>();
            int taskId = obj["updateTask"]["task_id"].Value<int>();



            if (    UpdatedName != updatedName 
                ||  UpdatedDescription != updatedDescription
                ||  ProjectId != responseProjectId  
                ||  UpdatedMaxTime != updatedMaxTime
                ||  UpdatedTimeEstimate != udpdatedTimeEstimate
                ||  TaskId != taskId) 
            {
                throw new Exception("Api return does not match");
            }

            new DeleteTask_Reusable().Invoke(TaskId);
            new DeleteProject_Reusable().Invoke(ProjectId);
        }


    }

}