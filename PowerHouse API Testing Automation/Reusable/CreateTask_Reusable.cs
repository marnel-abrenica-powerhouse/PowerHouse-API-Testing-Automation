using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;


namespace PowerHouse_Api
{

    public class CreateTask_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static string Name;
        public static string Description;
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
        }



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
        }


        public string Invoke(string name, string description, int projectId, int payout, int maxTime, int timeEstimate)
        {
            Name = name;
            Description = description;
            ProjectId = projectId;
            Payout = payout;
            MaxTime = maxTime;
            TimeEstimate = timeEstimate;

            MainTest().Wait();
            return ReturnString;            
        }
    }

}