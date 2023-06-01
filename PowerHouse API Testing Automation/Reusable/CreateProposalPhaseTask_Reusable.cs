using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;


namespace PowerHouse_Api
{

    public class CreateProposalPhaseTask_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static int PhaseId;
        public static string Task2Title;
        public static string Task2Description;
        public static int Task2HardCost;



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
mutation CreateProposalPhaseTask($data: ICreateProposalPhaseTaskDTO!, $phaseId: Float!) {
  createProposalPhaseTask(data: $data, phase_id: $phaseId) {
    created_at
    description
    hard_cost
    id
    markup_cost
    phase_id
    title
    total_cost
    updated_at
  }
}
    ",
                Variables = new
                {
                    data = new 
                        {
                        title = Task2Title,
                        hard_cost = Task2HardCost,
                        description = Task2Description
                        },
                    phaseId = PhaseId
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

        public string Invoke(string taskTitle, int taskHardCost, string taskDescription, int phaseId)
        {
            Task2Title = taskTitle;
            Task2HardCost = taskHardCost;
            Task2Description = taskDescription;
            PhaseId = phaseId;
            MainTest().Wait();
            return ReturnString;
        }

    }

}