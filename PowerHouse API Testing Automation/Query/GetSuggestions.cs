using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;

namespace PowerHouse_Api
{
    [TestFixture]
    [Parallelizable]
    public class GetSuggestions
    {
        public static String AuthToken;
        public static String BaseUrl;
        public static String ProjectId;

        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            ProjectId = b.StringGenerator();


        }

        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
query GetSuggestions($filters: SuggestionFilter!) {
  getSuggestions(filters: $filters) {
    association
    project {
      name
    }
    project_id
    status
    suggestion
    suggestion_id
    task {
      name
    }
    task_id
    title
    user {
      email
    }
    user_id
  }
}
    ",
                Variables = new
                {
                    projectId =  69,
                    filters = new {
                        project_id = 69,
                        status = "PENDING",
                        task_id = 101,
                        user_id = 42

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