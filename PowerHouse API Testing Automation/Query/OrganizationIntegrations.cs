﻿using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PowerHouse_Api
{ 
    [Parallelizable]
    public class OrganizationIntegrations
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static int OrgId;
        public static string ReturnString;

        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            OrgId = int.Parse(a.GetConfig_("orgId"));

            new UpsertOrganizationIntegrations_Reusable().Invoke(OrgId);
        }

        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
query OrganizationIntegrations($organizationId: Float!) {
  organizationIntegrations(organization_id: $organizationId) {
    id
    created_at
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
            JObject obj = JObject.Parse(ReturnString);
            string domain = obj["organizationIntegrations"]["jira_domain"].ToString();
            string email = obj["organizationIntegrations"]["jira_email"].ToString();
            string token = obj["organizationIntegrations"]["jira_token"].ToString();


            if (
                    domain != "powerhouse01"
                || email != "mabrenica.stormblue@gmail.com"
                || !token.Contains("108C")
                )
            {
                throw new Exception("Api return does not match");
            }
      
        }

    }

}