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

    public class ActiveInviteMember
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static int OrgId;
        public static string Email = "marnel.abrenica.powerhouse@gmail.com";
        public static string MemberType = "PO";
        public static int MemberId;
        public static string InviteId;


        public void Precondition()
        {
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            OrgId = int.Parse(a.GetConfig_("orgId"));

            string returnCreateInviteMember = new CreateInviteMember_Reusable().Invoke(Email, MemberType, OrgId);
            JObject objInviteId = JObject.Parse(returnCreateInviteMember);
            InviteId = objInviteId["createInviteMember"]["id"].ToString();
        }




        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
mutation ActiveInviteMember($organizationId: Float!, $inviteId: String!) {
  activeInviteMember(organization_id: $organizationId, invite_id: $inviteId)
}
    ",
                Variables = new
                {
                    organizationId = OrgId,
                    inviteId = InviteId
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
            PostTest();
        }


        public void PostTest()
        {
            string returnOrgMember = new OrganizationMembers_Reusable().Invoke(OrgId);
            JObject obj = JObject.Parse(returnOrgMember);
            JArray organizationMembers = (JArray)obj["organizationMembers"];

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