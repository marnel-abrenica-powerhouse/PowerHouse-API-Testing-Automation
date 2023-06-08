using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;

namespace PowerHouse_Api
{
    public class UpsertOrganizationIntegrations_Reusable
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
                        jira_token = "ATATT3xFfGF0GlPrzpFrsVqv8zabWWe8tZ-xQl8h2_2OTSqXa56nBcdn9XIdUxlOEJH6kkY3FynZmk22I1-r-ChQ3BTKoVecDKqg5CMTIAm9jpvG9fPBQW7Eg0tnhtVXEsKz88BOj1f10Ezp5UDbq1GVeobt_C8pfC0CgC5pRaVHfaCs43Xl3hA=C23E108C"
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

        }

        public string Invoke(int orgId)
        {
            OrgId = orgId;
            MainTest().Wait();
            return ReturnString;
        }

    }

}