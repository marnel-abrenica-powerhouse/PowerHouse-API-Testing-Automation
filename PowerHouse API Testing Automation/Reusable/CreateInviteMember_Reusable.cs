using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;


namespace PowerHouse_Api
{

    public class CreateInviteMember_Reusable
    {
        public static String AuthToken;
        public static String BaseUrl;
        public static String ReturnString;
        public static int OrgId;
        public static String Email;
        public static String MemberType;

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
mutation CreateInviteMember($data: CreateInviteMemberInput!) {
  createInviteMember(data: $data) {
    id
    member_type
    activated_at
    active
    created_at
    email
    organization {
      name
    }
  }
}
    ",
                Variables = new
                {
                    data = new {
                    email = Email,
                    member_type = MemberType,
                    organization_id = OrgId
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

        public string Invoke(string email, string memberType, int orgId)
        {
            Email= email;
            MemberType= memberType; 
            OrgId= orgId;
            MainTest().Wait();
            return ReturnString;
        }

    }

}