﻿using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

namespace PowerHouse_Api
{
    [TestFixture]

    public class DeleteProject
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static int ProjectId;
        public static string Name;
        public static string Overview;


        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            Name = b.StringGenerator("alphanumeric" ,10);
            Overview = b.StringGenerator("alphanumeric", 50);

            string returnProjectId = new CreateProject_Reusable().Invoke(Name, Overview);

            JObject obj = JObject.Parse(returnProjectId);
            Console.WriteLine(returnProjectId);
            ProjectId = obj["createProject"]["id"].Value<int>();
        }




        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
mutation DeleteProject($projectId: Float!) {
  deleteProject(project_id: $projectId) {
    name
    project_id
  }
}
    ",
                Variables = new
                {
                    projectId = ProjectId
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

        }


    }

}