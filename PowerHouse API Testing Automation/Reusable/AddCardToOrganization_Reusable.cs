﻿using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;

namespace PowerHouse_Api
{

    public class AddCardToOrganization_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static int OrgId;
        public static string Token;


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
mutation AddCardToOrganization($token: String!, $organizationId: Float!) {
  addCardToOrganization(token: $token, organization_id: $organizationId) {
    brand
    card_id
    created_at
    exp_month
    exp_year
    is_default
    last4
    organization_id
    provider
    updated_at
  }
}
    ",
                Variables = new
                {
                    token = Token,
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

        public string Invoke(string token, int orgId)
        {
            Token = token;
            OrgId = orgId;
            MainTest().Wait();
            return ReturnString;
        }

    }

}