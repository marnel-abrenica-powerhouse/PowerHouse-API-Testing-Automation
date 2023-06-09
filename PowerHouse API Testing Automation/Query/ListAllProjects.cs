﻿using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PowerHouse_Api
{
    [TestFixture]
    [Parallelizable]
    public class ListAllProjects
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ProjectName;
        public static string ProjectOverview;
        public static string ReturnString;
        public static int ProjectId;
        public static int OrgId;

        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            OrgId = int.Parse(a.GetConfig_("orgId"));
            ProjectName = b.StringGenerator("allletters", 10);
            ProjectOverview = b.StringGenerator("alphanumeric", 50);

            string returnOrg = new CreateProject_Reusable().Invoke(ProjectName, ProjectOverview);
            JObject orgObj = JObject.Parse(returnOrg);
            int projectId = orgObj["createProject"]["id"].Value<int>();
            ProjectId = projectId;
        }

        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
query ListAllProjects($organizationId: Float, $name: String, $orderBy: OrderByInput) {
  listAllProjects(organization_id: $organizationId, name: $name, order_by: $orderBy) {
    name
    code_base_url
    codebase
    created_at
    db_connection_string
    db_type
    hosting
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
      sub_projects {
        name
      }
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
                    organizationId = OrgId,
                    orderBy = new {
                    order = "asc",
                    field = "name"
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

            string jsonString = JsonConvert.SerializeObject(response.Data);
            ReturnString = jsonString;
            Console.WriteLine(ReturnString);
            PostTest();

        }

        public void PostTest()
        {
            new DeleteProject_Reusable().Invoke(ProjectId);
        }
    }

}