using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;

namespace PowerHouse_Api
{
    [TestFixture]
    [Parallelizable]
    public class FindProject
    {
        public static String AuthToken;
        public static String BaseUrl;

        public void Precondition()
        {
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");


        }

        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
                    query Query($projectId: Float!) {
  findProject(project_id: $projectId) {
    code_base_url
    codebase
    created_at
    db_connection_string
    db_type
    hosting
    name
    organization {
      name
    }
    organization_id
    overview
    percentage_completed
    project {
      name
    }
    project_id
    project_manager {
      email
    }
    project_owner {
      email
    }
    project_owner_id
    project_tasks {
      name
    }
    project_type
    stacks {
      id
    }
    sub_projects {
      name
    }
    tech_lead {
      email
    }
    tech_lead_id
    tech_requirements
    total_prds
    total_tasks
    updated_at
  }
}
    ",
                Variables = new
                {
                    projectId = 62
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