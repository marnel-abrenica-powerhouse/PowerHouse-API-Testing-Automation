﻿using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PowerHouse_Api
{
    [TestFixture]

    public class SetDefaultCard
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
            OrgId = int.Parse(a.GetConfig_("orgId"));

            string returnStripeToken = new StripePayment().Invoke();
            Token = returnStripeToken;
            new AddCardToOrganization_Reusable().Invoke(Token, OrgId);
        }




        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
mutation SetDefaultCard($setDefaultCardId: String!) {
  setDefaultCard(id: $setDefaultCardId) {
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
                    setDefaultCardId = Token
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
            string card_id = obj["setDefaultCard"]["card_id"].ToString();
            bool isDefault = obj["setDefaultCard"]["is_default"].Value<bool>(); 

            if (
                   card_id != Token
                   || isDefault != true
                )
            {
                throw new Exception("Api return does not match");
            }

            new DeleteCard_Reusable().Invoke(card_id);
        }

    }

}