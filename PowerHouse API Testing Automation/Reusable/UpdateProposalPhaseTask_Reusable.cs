using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;


namespace PowerHouse_Api
{

    public class UpdateProposalPhaseTask_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static int ProposalPhaseTaskId;
        public static string UpdatedTaskTitle;
        public static int UpdatedTaskHardCost;
        public static string UpdatedTaskDescription;


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
mutation UpdateProposalPhaseTask($data: UpdateProposalPhaseTaskInput!, $updateProposalPhaseTaskId: Float!) {
  updateProposalPhaseTask(data: $data, id: $updateProposalPhaseTaskId) {
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
                        title = UpdatedTaskTitle,
                        hard_cost = UpdatedTaskHardCost,
                        description = UpdatedTaskDescription
                        },
                    updateProposalPhaseTaskId = ProposalPhaseTaskId
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

        public string Invoke(string updatedTaskTitle, int updatedTaskHardCost, string updatedTaskDescription, int proposalPhaseTaskId)
        {
            UpdatedTaskTitle = updatedTaskTitle;
            UpdatedTaskHardCost = updatedTaskHardCost;
            UpdatedTaskDescription = updatedTaskDescription;
            ProposalPhaseTaskId = proposalPhaseTaskId;
            MainTest().Wait();
            return ReturnString;
        }
    }

}