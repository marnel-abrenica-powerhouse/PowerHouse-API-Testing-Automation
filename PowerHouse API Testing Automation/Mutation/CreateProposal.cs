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

    public class CreateProposal
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
            OrgId = int.Parse(a.GetConfig_("orgId"));

            string returnOrgMember = new OrganizationMembers_Reusable().Invoke(OrgId);
            JObject objOrgMember = JObject.Parse(returnOrgMember);
            ClientEmail = objOrgMember["organizationMembers"][0]["user"]["email"].ToString();
            UserId = objOrgMember["organizationMembers"][0]["user_id"].Value<int>();
            string firstName = objOrgMember["organizationMembers"][0]["user"]["profile"]["first_name"].ToString();
            string lastName = objOrgMember["organizationMembers"][0]["user"]["profile"]["last_name"].ToString();
            ClientName = firstName + " " + lastName;

            ProposalName = new Commands().StringGenerator("allletters", 15);
            ProposalNotes = new Commands().StringGenerator("alphanumeric", 100);
            DurationSpan = int.Parse(new Commands().StringGenerator("allnumbers", 2));
            Margin = int.Parse(new Commands().StringGenerator("allnumbers", 2));
            PhaseTitle = new Commands().StringGenerator("allletters", 15);
            TaskDescription = new Commands().StringGenerator("alphanumeric", 100);
            HardCost = int.Parse(new Commands().StringGenerator("allnumbers", 2));
            TaskTitle = new Commands().StringGenerator("allletters", 15);
        }




            [Test]
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
            VerifyResponse();
        }

        public void VerifyResponse()
        {
            JObject obj = JObject.Parse(ReturnString);
            string clientEmail = obj["createProposal"]["client_email"].ToString();
            string clientName = obj["createProposal"]["client_name"].ToString();
            int clientUserId = obj["createProposal"]["client_user_id"].Value<int>();
            string proposalName = obj["createProposal"]["name"].ToString();
            string proposalNotes = obj["createProposal"]["notes"].ToString();
            int orgId = obj["createProposal"]["organization_id"].Value<int>();
            string phaseTitle = obj["createProposal"]["phases"][0]["title"].ToString();
            int phaseDurationSpan = obj["createProposal"]["phases"][0]["duration_span"].Value<int>();
            int margin = obj["createProposal"]["phases"][0]["margin"].Value<int>();
            string phaseTaskDescription = obj["createProposal"]["phases"][0]["phase_tasks"][0]["description"].ToString();
            int phaseTaskHardCost = obj["createProposal"]["phases"][0]["phase_tasks"][0]["hard_cost"].Value<int>();
            string phaseTaskTitle = obj["createProposal"]["phases"][0]["phase_tasks"][0]["title"].ToString();
            int proposalId = obj["createProposal"]["id"].Value<int>();

            if (
                   clientEmail != ClientEmail
                || clientName!= ClientName
                || clientUserId != UserId
                || proposalName != ProposalName
                || proposalNotes != ProposalNotes
                || orgId != OrgId
                || phaseTitle != PhaseTitle
                || phaseDurationSpan != DurationSpan
                || margin != Margin
                || phaseTaskDescription != TaskDescription
                || phaseTaskHardCost != HardCost
                || phaseTaskTitle != TaskTitle
                )
            {
                throw new Exception("Api return does not match");
            }
            new DeleteProposal_Reusable().Invoke(proposalId);
        }

    }

}