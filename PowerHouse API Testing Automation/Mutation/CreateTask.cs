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

    public class CreateTask
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

            string returnString = new CreateProject_Reusable().Invoke(ProjectName, ProjectOverview);
            JObject obj = JObject.Parse(returnString);
            int projectId = obj["createProject"]["id"].Value<int>();
            ProjectId = projectId;
        }




        [Test]
        public async Task MainTest()
        {

            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
mutation CreateTask($name: String!, $description: String!, $projectId: Float!, $payout: Float, $maxTime: Float, $timeEstimate: Float) {
  createTask(
    data: {name: $name, description: $description, project_id: $projectId, payout: $payout, max_time: $maxTime, time_estimate: $timeEstimate}
  ) {
    task_id
    name
    description
    project_id
    payout
    max_time
    time_estimate
  }
}
    ",
                Variables = new
                {
                    name = Name,
                    description = Description,
                    projectId = ProjectId,
                    payout = Payout,
                    maxTime = MaxTime,
                    timeEstimate = TimeEstimate
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
            VerifyResponse();
            PostTest();
        }



        public void VerifyResponse()
        {
           
            JObject obj = JObject.Parse(ReturnString);
            string responseName = obj["createTask"]["name"].ToString();
            string responseDescription = obj["createTask"]["description"].ToString();
            int responseProjectId = obj["createTask"]["project_id"].Value<int>();
            int responseMaxTime = obj["createTask"]["max_time"].Value<int>();
            int responseTimeEstimate = obj["createTask"]["time_estimate"].Value<int>();



            if (Name != responseName || 
                Description != responseDescription || 
                ProjectId != responseProjectId  ||
                MaxTime != responseMaxTime ||
                TimeEstimate != responseTimeEstimate) 
            {
                throw new Exception("Api return does not match");
            }
                      
        }



        public void PostTest()
        {
            JObject deleleObj = JObject.Parse(ReturnString);
            int taskId = deleleObj["createTask"]["task_id"].Value<int>();
            new DeleteTask_Reusable().Invoke(taskId);
        }
    }

}