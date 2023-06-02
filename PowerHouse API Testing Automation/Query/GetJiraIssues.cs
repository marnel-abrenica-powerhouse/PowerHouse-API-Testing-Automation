using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;

namespace PowerHouse_Api
{
    [TestFixture]
    [Parallelizable]
    public class GetJiraIssues
    {
        public static String AuthToken;
        public static String BaseUrl;
        public static String ProjectId;
        public static int OrgId;

        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            OrgId = int.Parse(a.GetConfig_("orgId"));
            ProjectId = b.StringGenerator();


        }

        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
query GetJiraIssues($filters: GetJiraIssuesInput!) {
  getJiraIssues(filters: $filters) {
    items {
      id
    }
    totalItems
  }
}
    ",
                Variables = new
                {
                    filters = new {
                        organization_id = OrgId,
                        skip = 0,
                        take = 30,
                        search = "test"
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

            Console.WriteLine(response.Data);

        }

    }

}