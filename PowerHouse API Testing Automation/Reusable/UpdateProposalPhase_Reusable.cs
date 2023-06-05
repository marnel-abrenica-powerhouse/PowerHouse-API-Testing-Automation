using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;


namespace PowerHouse_Api
{


    public class UpdateProposalPhase_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static int ProposalPhaseId;
        public static int UpdatedDurationSpan;
        public static int UpdatedMargin;
        public static string UpdatedPhaseTitle;



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
mutation UpdateProposalPhase($data: UpdateProposalPhaseInput!, $updateProposalPhaseId: Float!) {
  updateProposalPhase(data: $data, id: $updateProposalPhaseId) {
    amount_paid
    created_at
    duration_span
    duration_type
    hard_cost
    id
    margin
    markup_cost
    payment_status
    phase_tasks {
      id
    }
    proposal_id
    status
    title
    total_cost
    updated_at
  }
}
    ",
                Variables = new
                {
                    data = new {
                    duration_span = UpdatedDurationSpan,
                    duration_type = "DAY",
                    margin = UpdatedMargin,
                    status = "INPROGRESS",
                    title = UpdatedPhaseTitle
                    },
                    updateProposalPhaseId = ProposalPhaseId
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

        public string Invoke(int updatedDurationSpan, int updatedMargin, string updatedPhaseTitle, int updatedProposalPhaseId)
        {
            UpdatedDurationSpan = updatedDurationSpan;
            UpdatedMargin = updatedMargin;
            UpdatedPhaseTitle = updatedPhaseTitle;
            ProposalPhaseId = updatedProposalPhaseId;
            MainTest().Wait();
            return ReturnString;
        }

    }

}