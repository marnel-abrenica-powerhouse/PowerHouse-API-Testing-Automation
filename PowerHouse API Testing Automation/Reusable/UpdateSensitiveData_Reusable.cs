using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;


namespace PowerHouse_Api
{


    public class UpdateSensitiveData_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static string SensitiveDataId;
        public static string UpdatedName;
        public static string UpdatedUsername;
        public static string UpdatedPassword;
        public static string UpdatedInfoDescription;
        public static string UpdatedKey;


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
        }

        public string Invoke(string sensitiveDataId, string updatedUsername, string updatedPassword, string updatedKey, string updatedDescription)
        {
            SensitiveDataId = sensitiveDataId;
            UpdatedUsername = updatedUsername;
            UpdatedPassword = updatedPassword;
            UpdatedKey = updatedKey;
            UpdatedInfoDescription = updatedDescription;
            MainTest().Wait();
            return ReturnString;
        }
    }

}