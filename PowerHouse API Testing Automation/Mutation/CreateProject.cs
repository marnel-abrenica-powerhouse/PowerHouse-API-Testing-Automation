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

    public class CreateProject
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static int OrgMemberId;
        public static int OrgId;
        public static string Name;
        public static string Overview;

        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            OrgId = int.Parse(a.GetConfig_("orgId"));
            Name = b.StringGenerator("alphanumeric" ,10);
            Overview = b.StringGenerator("alphanumeric", 50);

            string returnString = new OrganizationMembers_Reusable().Invoke(OrgId);
            JObject obj = JObject.Parse(returnString);
            OrgMemberId = obj["organizationMembers"][0]["id"].Value<int>();
        }




        [Test]
        public async Task CreateProject_()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
mutation CreateProject($name: String!, $overview: String!, $stackId: Float, $projectManagerId: Float, $techLeadId: Float, $organizationId: Float!) {
  createProject(
    data: {name: $name, overview: $overview, stack_id: $stackId, organization_id: $organizationId, project_manager_id: $projectManagerId, tech_lead_id: $techLeadId}
  ) {
    id: project_id
    name
    tech_lead {
      user_id
    }
    overview
    stacks {
      id
    }
    project_manager {
      user_id
    }
    project_owner {
      user_id
    }
  }
}
    ",
                Variables = new
                {
                    name = Name,
                    overview = Overview,
                    stackId = 1,
                    techLeadId = OrgMemberId,
                    projectManagerId = OrgMemberId,
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
            int responseProjectId = obj["createProject"]["id"].Value<int> ();
            string responseName = obj["createProject"]["name"].ToString();
            string responseOverview = obj["createProject"]["overview"].ToString();
            int responseStackId = obj["createProject"]["stacks"]["id"].Value<int>();

            if (Name != responseName || Overview != responseOverview || responseStackId != 1)
            {
                throw new Exception("Api return does not match");
            }
            new DeleteProject_Reusable().Invoke(responseProjectId);
        }

    }

}