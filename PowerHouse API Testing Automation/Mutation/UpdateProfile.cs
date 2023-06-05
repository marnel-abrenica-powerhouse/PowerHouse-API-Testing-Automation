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
    public class UpdateProfile
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
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            Bio = b.StringGenerator();
            FirstName = b.StringGenerator();
            LastName = b.StringGenerator();
            LinkedInUrl = "https://www." + b.StringGenerator() + ".com";
        }

        [Test]
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
            Console.WriteLine(ReturnString);
            PostTest();
        }

        public void PostTest()
        {
            JObject obj = JObject.Parse(ReturnString);
            string bio = obj["updateProfile"]["bio"].ToString();
            string country = obj["updateProfile"]["country"].ToString();
            string firstName = obj["updateProfile"]["first_name"].ToString();
            string lastName = obj["updateProfile"]["last_name"].ToString();
            string linkedInUrl = obj["updateProfile"]["linkedin_url"].ToString();


            if (bio != Bio
                || country != "Philippines (the)"
                || firstName != FirstName
                || lastName != LastName
                || linkedInUrl != LinkedInUrl
                )
            {
                throw new Exception("Api return does not match");
            }

            new UpdateProfile_Reusable().Invoke(Bio, "Marnel", "PowerHouse", LinkedInUrl);

        }
    }
}