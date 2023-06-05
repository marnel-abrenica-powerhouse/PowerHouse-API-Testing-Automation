using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;


namespace PowerHouse_Api
{

    public class UpdateProfile_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static string Bio;
        public static string FirstName;
        public static string LastName;
        public static string LinkedInUrl;



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
mutation UpdateProfile($data: IUpdateProfileDTO, $avatarFile: Upload) {
  updateProfile(data: $data, avatar_file: $avatarFile) {
    avatar
    bio
    country
    created_at
    expertise {
      id
    }
    first_name
    last_name
    linkedin_url
    profile_id
    timezone
    updated_at
    user {
      user_id
    }
  }
}
    ",
                Variables = new
                {
                    data = new 
                        {
                        bio = Bio,
                        country = "Philippines (the)",
                        expertisies = new[] { "Angular", "Nest" },
                        first_name = FirstName,
                        last_name = LastName,
                        linkedin_url = LinkedInUrl,
                        timezone = "Asia/Manila"
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

        public string Invoke(string bio, string firstName, string lastName, string linkedInUrl)
        {
            Bio = bio;
            FirstName = firstName;
            LastName = lastName;
            LinkedInUrl = linkedInUrl;
            MainTest().Wait();
            return ReturnString; 
        }
    }

}