using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;


namespace PowerHouse_Api
{
    [TestFixture]
    [Parallelizable]
    public class RetrieveCard
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string StripeToken;
        public static int OrgId;


        public void Precondition()
        {
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            OrgId = int.Parse(a.GetConfig_("orgId"));

            StripeToken = new StripePayment().Invoke();
            new AddCardToOrganization_Reusable().Invoke(StripeToken, OrgId);
        }

        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
query RetrieveCard($retrieveCardId: String!) {
  retrieveCard(id: $retrieveCardId) {
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
                    retrieveCardId = StripeToken
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

            Console.WriteLine(response.Data);
            PostTest();
        }

        public void PostTest()
        {
            new DeleteCard_Reusable().Invoke(StripeToken);
        }
    }

}