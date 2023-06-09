using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;

namespace PowerHouse_Api
{
    [TestFixture]

    public class UpsertOrganizationIntegrations
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static int OrgId;


        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            OrgId = int.Parse(a.GetConfig_("orgId"));
        }




        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
mutation UpsertOrganizationIntegrations($data: CreateOrganizationIntegrationInput!, $organizationId: Float!) {
  upsertOrganizationIntegrations(data: $data, organization_id: $organizationId) {
    created_at
    id
    jira_domain
    jira_email
    jira_token
    organization_id
    trello_key
    trello_token
    updated_at
  }
}
    ",
                Variables = new
                {
                    data = new 
                        {
                        jira_domain = "powerhouse01",
                        jira_email = "mabrenica.stormblue@gmail.com",
                        jira_token = "ATATT3xFfGF0VdTIvrgHZ0KHNiC40TODwOOaokBcWNLk8XRjx2B9_RGMKTzYSRtdBHLjOARBvzn1mYJqLLlC8ZdqXxvPm3Vy_8qMaF71_Xn97pEgnrnkBJJRJ54MRhLPxce3s7gTk63wCfUc_mFIyYDu62MfOF2nvtbbcROImWSYu3KlBE4Y6J0=FDB07C1E"
                    },
                    organizationId = OrgId
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
            new GetJiraIssues_Reusable().Invoke(OrgId);
        }

    }

}