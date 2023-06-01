using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;

namespace PowerHouse_Api
{

    public class User_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;

        public void Precondition()
        {
            Commands b = new();
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
query User {
  user {
    id: user_id
    googleId: google_id
    email
    organizations {
      id: organization_id
      name
    }
    roles {
      role {
        name
        id: role_id
      }
    }
    profile {
      firstName: first_name
      lastName: last_name
      avatar
      country
      timezone
      linkedinUrl: linkedin_url
      expertise {
        id
        name: stack_name
      }
    }
  }
}
    ",
                Variables = new
                {
                    
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

        public string Invoke()
        {
            MainTest().Wait();
            return ReturnString;
        }

    }

}