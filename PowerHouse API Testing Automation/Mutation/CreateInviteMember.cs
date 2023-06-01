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

    public class CreateInviteMember
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static int OrgMemberId;
        public static int OrgId;
        public static string Email = "marnel.abrenica.powerhouse@gmail.com";
        public static string MemberType = "PO";
        public static int MemberId;

        public void Precondition()
        {
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            OrgId = int.Parse(a.GetConfig_("orgId"));
        }




        [Test]
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
            VerifyResponse();
        }

        public void VerifyResponse()
        {
            JObject obj = JObject.Parse(ReturnString);
            string responseEmail = obj["createInviteMember"]["email"].ToString();
            string responseMemberType = obj["createInviteMember"]["member_type"].ToString();


            if (responseEmail != "marnel.abrenica.powerhouse@gmail.com" || responseMemberType != "PO")
            {
                throw new Exception("Api return does not match");
            }

            string returnOrgMember = new OrganizationMembers_Reusable().Invoke(OrgId);
            JObject objMemberId = JObject.Parse(returnOrgMember);
            JArray organizationMembers = (JArray)objMemberId["organizationMembers"];

            foreach (JObject member in organizationMembers)
            {
                string memberEmail = (string)member["user"]["email"];
                if (memberEmail == Email)
                {
                    MemberId = (int)member["user_id"];
                }
            }
            new RemoveMemberInOrganization_Reusable().Invoke(MemberId, OrgId);
        }

    }

}