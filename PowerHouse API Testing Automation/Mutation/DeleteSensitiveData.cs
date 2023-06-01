using GraphQL;
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

    public class DeleteSensitiveData
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static string Name;
        public static string ProjectName;
        public static string ProjectOverview;
        public static int ProjectID;
        public static string Username;
        public static string Password;
        public static string Key;
        public static string InfoDescription;
        public static string SensitiveDataId;




        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            Name = b.StringGenerator("alphanumeric" ,15);
            ProjectName = b.StringGenerator("alphanumeric", 15);
            ProjectOverview = b.StringGenerator("alphanumeric", 50);
            Username = b.StringGenerator("allletters", 10);
            Password = b.StringGenerator("alphanumeric", 10);
            Key = b.StringGenerator("alphanumeric", 20);
            InfoDescription = b.StringGenerator("alphanumeric", 50);

            string returnProjectId = new CreateProject_Reusable().Invoke(ProjectName, ProjectOverview);
            JObject objProjectId = JObject.Parse(returnProjectId);
            ProjectID = objProjectId["createProject"]["id"].Value<int>();

            string returnSensitiveDataId = new CreateSensitiveData_Reusable().Invoke(Name, ProjectID, Username, Password, Key, InfoDescription);
            JObject objSensitiveData = JObject.Parse(returnSensitiveDataId);
            SensitiveDataId = objSensitiveData["createSensitiveData"]["id"].ToString();
        }


        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
mutation DeleteSensitiveData($deleteSensitiveDataId: String!) {
  deleteSensitiveData(id: $deleteSensitiveDataId) {
    id
  }
}
    ",
                Variables = new
                {
                    deleteSensitiveDataId = SensitiveDataId
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
        }

    }

}