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

    public class DeleteProposalPhase
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
        public static int ProposalId;
        public static int DurationSpan;
        public static int Margin;
        public static string PhaseTitle;
        public static string TaskDescription;
        public static int HardCost;
        public static string TaskTitle;
        public static int Phase2DurationSpan;
        public static int Phase2Margin;
        public static string Phase2Title;
        public static int Phase2Id;



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
            Phase2DurationSpan = int.Parse(new Commands().StringGenerator("allnumbers", 2));
            Phase2Margin = int.Parse(new Commands().StringGenerator("allnumbers", 2));
            Phase2Title = new Commands().StringGenerator("allletters", 15);

            string returnProposalId = new CreateProposal_Reusable().Invoke(ClientEmail, ClientName, UserId, ProposalName, ProposalNotes, OrgId, DurationSpan, Margin, PhaseTitle, TaskDescription, HardCost, TaskTitle);
            JObject objProposalId = JObject.Parse(returnProposalId);
            ProposalId = objProposalId["createProposal"]["id"].Value<int>();

            string returnProposalPhaseId = new CreateProposalPhase_Reusable().Invoke(Phase2DurationSpan, Phase2Margin, Phase2Title, ProposalId);
            JObject objProposalPhaseId = JObject.Parse(returnProposalPhaseId);
            Phase2Id = objProposalId["createProposal"]["phases"][0]["id"].Value<int>();
        }




            [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
mutation DeleteProposalPhase($deleteProposalPhaseId: Float!) {
  deleteProposalPhase(id: $deleteProposalPhaseId) {
    id
  }
}
    ",
                Variables = new
                {
                    deleteProposalPhaseId = Phase2Id
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
            int proposalPhasId = obj["deleteProposalPhase"]["id"].Value<int>();


            if (
                   proposalPhasId != Phase2Id
                )
            {
                throw new Exception("Api return does not match");
            }
            new DeleteProposal_Reusable().Invoke(ProposalId);
        }

    }

}