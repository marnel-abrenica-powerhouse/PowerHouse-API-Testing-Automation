using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;

namespace PowerHouse_Api
{
    public class UpdateProposal_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static string ClientEmail;
        public static string ClientName;
        public static int UserId;
        public static int ProposalId;
        public static string UpdatedProposalName;
        public static string UpdatedProposalNotes;


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
mutation UpdateProposal($data: IUpdateProposalDTO!, $updateProposalId: Float!) {
  updateProposal(data: $data, id: $updateProposalId) {
    client_email
    client_name
    client_user_id
    name
    notes
    status
    id
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
                        name = UpdatedProposalName,
                        notes = UpdatedProposalNotes,
                        status = "INPROGRESS"
                    },
                    updateProposalId = ProposalId
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

        public string Invoke(string clientEmail, string clientName, int userId, string updatedProposalName, string updatedProposalNotes)
        {
            ClientEmail = clientEmail;
            ClientName = clientName;
            UserId = userId;
            UpdatedProposalName = updatedProposalName;
            UpdatedProposalNotes = updatedProposalNotes;
            MainTest().Wait();
            return ReturnString;
        }
    }

}