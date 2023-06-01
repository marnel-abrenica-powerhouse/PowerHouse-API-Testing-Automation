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

    public class AddCardToOrganization
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
            Token = new StripePayment().Invoke();
        }




        [Test]
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
            VerifyResponse();
        }

        public void VerifyResponse()
        {
            JObject obj = JObject.Parse(ReturnString);
            string card_id = obj["addCardToOrganization"]["card_id"].ToString();
            int exp_month = obj["addCardToOrganization"]["exp_month"].Value<int>();
            int exp_year = obj["addCardToOrganization"]["exp_year"].Value<int>();
            int last4 = obj["addCardToOrganization"]["last4"].Value<int>();
            int organization_id = obj["addCardToOrganization"]["organization_id"].Value<int>();


            if (

                   card_id != Token 
                || exp_month != 12 
                || exp_year != 2035 
                || last4 != 1111 
                || organization_id != OrgId

                )
            {
                throw new Exception("Api return does not match");
            }

            new DeleteCard_Reusable().Invoke(Token);

        }

    }

}