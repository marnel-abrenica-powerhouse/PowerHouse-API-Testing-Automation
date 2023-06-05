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

    public class UpdateSensitiveData
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
        public static string UpdatedName;
        public static string UpdatedUsername;
        public static string UpdatedPassword;
        public static string UpdatedInfoDescription;
        public static string UpdatedKey;


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
            UpdatedName = b.StringGenerator("alphanumeric", 15);
            UpdatedUsername = b.StringGenerator("allletters", 10);
            UpdatedPassword = b.StringGenerator("alphanumeric", 10);
            UpdatedInfoDescription = b.StringGenerator("alphanumeric", 50);
            UpdatedKey = b.StringGenerator("alphanumeric", 20);

            string returnProjectId = new CreateProject_Reusable().Invoke(ProjectName, ProjectOverview);
            JObject objProjectId = JObject.Parse(returnProjectId);
            ProjectID = objProjectId["createProject"]["id"].Value<int>();

            string returnCreateSensitiveData = new CreateSensitiveData_Reusable().Invoke(Name, ProjectID, Username, Password, Key, InfoDescription);
            JObject objCreateSensitiveData = JObject.Parse(returnCreateSensitiveData);
            SensitiveDataId = objCreateSensitiveData["createSensitiveData"]["id"].ToString();
        }




        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
mutation UpdateSensitiveData($data: IUpdateSensitiveDataDTO!, $updateSensitiveDataId: String!) {
  updateSensitiveData(data: $data, id: $updateSensitiveDataId) {
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
                    updateSensitiveDataId = SensitiveDataId,
                    data = new 
                        {
                        name = UpdatedName,
                        sensitive_data = new 
                            {
                            username = UpdatedUsername,
                            password = UpdatedPassword,
                            key = UpdatedKey,
                            description = UpdatedInfoDescription
                        },
                        type = "CREDENTIAL"
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
            JObject obj = JObject.Parse(ReturnString);
            string updatedDescription = obj["updateSensitiveData"]["data"]["description"].ToString();
            string updatedKey = obj["updateSensitiveData"]["data"]["key"].ToString();
            string updatedUsername = obj["updateSensitiveData"]["data"]["username"].ToString();
            string updatedPassword = obj["updateSensitiveData"]["data"]["password"].ToString();
            string updatedName = obj["updateSensitiveData"]["name"].ToString();
            int projectId = obj["updateSensitiveData"]["project_id"].Value<int>();
            string sensitiveDataId = obj["updateSensitiveData"]["id"].ToString();



            if (    UpdatedInfoDescription != updatedDescription
                ||  UpdatedKey != updatedKey
                ||  UpdatedUsername != updatedUsername
                ||  UpdatedPassword != updatedPassword
                ||  ProjectID != projectId
                ||  UpdatedName != updatedName
                ||  SensitiveDataId != sensitiveDataId)
            {
                throw new Exception("Api return does not match");
            }

            new DeleteSensitiveData_Reusable().Invoke(sensitiveDataId);
            new DeleteProject_Reusable().Invoke(ProjectID);
        }

    }

}