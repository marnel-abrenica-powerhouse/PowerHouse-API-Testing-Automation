using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;


namespace PowerHouse_Api
{
    public class CreateProposal_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static int OrgId;
        public static string ClientEmail;
        public static string ClientName;
        public static int UserId;
        public static string ProposalName;
        public static string ProposalNotes;
        public static int DurationSpan;
        public static int Margin;
        public static string PhaseTitle;
        public static string TaskDescription;
        public static int HardCost;
        public static string TaskTitle;


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
mutation CreateProposal($data: ICreateProposalDTO!) {
  createProposal(data: $data) {
    amount_paid
    client_email
    client_name
    client_user_id
    created_by_user_id
    hard_cost
    id
    markup_cost
    name
    notes
    organization_id
    payment_status
    phases {
      id
      title
      duration_span
      duration_type
      margin
      phase_tasks {
        description
        hard_cost
        id
        phase_id
        markup_cost
        title
        total_cost
      }
    }
    status
    total_cost
  }
}
    ",
                Variables = new
                {
                    data = new
                    {
                        client_email = ClientEmail,
                        client_name = ClientName,
                        client_user_id = UserId,
                        name = ProposalName,
                        notes = ProposalNotes,
                        organization_id = OrgId,
                        phases = new[]
                        {

                                        new
                                        {
                                            duration_span = DurationSpan,
                                            duration_type = "DAY",
                                            margin = Margin,
                                            title = PhaseTitle,
                                            phaseTasks = new[]
                                            {
                                                new
                                                {
                                                description = TaskDescription,
                                                hard_cost = HardCost,
                                                title = TaskTitle
                                                }
                                            }
                                         }

                        }
                    }
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


        public string Invoke(
            string clientEmail,
            string clientName,
            int clientUserId,
            string proposalName,
            string proposalNotes,
            int orgId,
            int phaseDurationSpan,
            int margin,
            string phaseTitle,
            string taskDescription,
            int hardCost,
            string taskTitle  
            )
        {
            ClientEmail= clientEmail;
            ClientName= clientName;
            UserId= clientUserId;
            ProposalName= proposalName;
            ProposalNotes= proposalNotes;
            OrgId= orgId;
            DurationSpan= phaseDurationSpan;
            Margin= margin;
            PhaseTitle= phaseTitle;
            TaskDescription= taskDescription;
            HardCost= hardCost;
            TaskTitle= taskTitle;

            MainTest().Wait();
            return ReturnString;
        }
    }

}