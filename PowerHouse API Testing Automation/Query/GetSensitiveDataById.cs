﻿using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Xml.Linq;

namespace PowerHouse_Api
{
    [TestFixture]
    [Parallelizable]
    public class GetSensitiveDataById
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static string ProjectName;
        public static string ProjectOverview;
        public static string SensitiveDataName;
        public static string SensitiveDataId;
        public static string Username;
        public static string Password;
        public static string Key;
        public static string InfoDescription;
        public static int ProjectId;


        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            ProjectName = b.StringGenerator("alphanumeric", 15);
            ProjectOverview = b.StringGenerator("alphanumeric", 50);
            SensitiveDataName = b.StringGenerator("alphanumeric", 15);
            Username = b.StringGenerator("alphanumeric", 15);
            Password = b.StringGenerator("alphanumeric", 15);
            Key = b.StringGenerator("alphanumeric", 15);
            InfoDescription = b.StringGenerator("alphanumeric", 50);

            string returnProjectId = new CreateProject_Reusable().Invoke(ProjectName, ProjectOverview);
            JObject objProjectId = JObject.Parse(returnProjectId);
            ProjectId = objProjectId["createProject"]["id"].Value<int>();

            string returnSensitiveDataId = new CreateSensitiveData_Reusable().Invoke(SensitiveDataName, ProjectId, Username, Password, Key, InfoDescription);
            JObject objSensitiveDataId = JObject.Parse(returnSensitiveDataId);
            SensitiveDataId = objSensitiveDataId["createSensitiveData"]["id"].ToString();
        }

        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
query GetSensitiveDataById($getSensitiveDataByIdId: String!) {
  getSensitiveDataById(id: $getSensitiveDataByIdId) {
    createdAt
    data {
      description
      key
      password
      username
    }
    id
    name
    project_id
    type
    updatedAt
  }
}
    ",
                Variables = new
                {
                    getSensitiveDataByIdId = SensitiveDataId

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
            JObject obj = JObject.Parse(ReturnString);
            string name = obj["getSensitiveDataById"]["name"].ToString();
            string description = obj["getSensitiveDataById"]["data"]["description"].ToString();
            string key = obj["getSensitiveDataById"]["data"]["key"].ToString();
            string username = obj["getSensitiveDataById"]["data"]["username"].ToString();
            string password = obj["getSensitiveDataById"]["data"]["password"].ToString();
            int projectId = obj["getSensitiveDataById"]["project_id"].Value<int>();

            if (SensitiveDataName != name
                || InfoDescription != description
                || Key != key
                || Username != username
                || Password != password
                || ProjectId != projectId)
            {
                throw new Exception("Api return does not match");
            }
            new DeleteSensitiveData_Reusable().Invoke(SensitiveDataId);
            new DeleteProject_Reusable().Invoke(ProjectId);
        }
    }

}