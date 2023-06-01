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

    public class CreateSensitiveData
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

        }




        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
mutation CreateSensitiveData($data: ICreateSensitiveDataDTO!) {
  createSensitiveData(data: $data) {
    data {
      description
      key
      password
      username
    }
    createdAt
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
                    data = new 
                        {
                        name = Name,
                        project_id = ProjectID,
                        sensitive_data = new 
                            {
                            username = Username,
                            password = Password,
                            key = Key,
                            description = InfoDescription
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
            VerifyResponse();
        }

        public void VerifyResponse()
        {
            JObject obj = JObject.Parse(ReturnString);
            string dataDescription = obj["createSensitiveData"]["data"]["description"].ToString();
            string dataKey = obj["createSensitiveData"]["data"]["key"].ToString();
            string dataUsername = obj["createSensitiveData"]["data"]["username"].ToString();
            string dataPassword = obj["createSensitiveData"]["data"]["password"].ToString();
            string name = obj["createSensitiveData"]["name"].ToString();
            int projectId = obj["createSensitiveData"]["project_id"].Value<int>();
            string sensitiveDataId = obj["createSensitiveData"]["id"].ToString();



            if (    Name != name
                ||  InfoDescription != dataDescription
                ||  Key != dataKey
                ||  Username != dataUsername
                ||  Password != dataPassword
                ||  ProjectID != projectId)
            {
                throw new Exception("Api return does not match");
            }

            new DeleteSensitiveData_Reusable().Invoke(sensitiveDataId);
            new DeleteProject_Reusable().Invoke(ProjectID);
        }

    }

}