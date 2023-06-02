using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;


namespace PowerHouse_Api
{

    public class CreateSensitiveData_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static string Name;
        public static int ProjectID;
        public static string Username;
        public static string Password;
        public static string Key;
        public static string InfoDescription;



        public void Precondition()
        {
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
        }

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
        }


        public string Invoke(string name, int projectId, string username, string password, string key, string infoDescription)
        {
            Name = name;
            ProjectID= projectId;
            Username= username;
            Password= password;
            Key= key;
            InfoDescription= infoDescription;

            MainTest().Wait();
            return ReturnString;
        }

        /*sample response
         *     {"createSensitiveData":{"data":{"description":"B8n93qj75iom1oduyroyi0v9wvvla43usvagobjufdemrrov8c","key":"Hcszqzpkfdlnxk5","password":"Kjvy86izyt8kq7v","username":"5jd63stdzhsuhxf"},"createdAt":"2023-06-02T17:34:22.851Z","id":"2c6012d2-d47e-4e12-85da-e52b7733f1a6","name":"1esygcnnymf13i2","project_id":503,"type":"CREDENTIAL","updatedAt":"2023-06-02T17:34:22.851Z"}}
         */
    }

}