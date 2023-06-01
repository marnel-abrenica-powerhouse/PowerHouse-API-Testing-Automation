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

    public class RemoveMemberInOrganization
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
            new CreateInviteMember_Reusable().Invoke(Email, MemberType, OrgId);

            string returnOrgMember = new OrganizationMembers_Reusable().Invoke(OrgId);
            JObject obj = JObject.Parse(returnOrgMember);
            JArray organizationMembers = (JArray)obj["organizationMembers"];
            foreach (JObject member in organizationMembers)
            {
                string memberEmail = (string)member["user"]["email"];
                if(memberEmail == Email) {
                    MemberId = (int)member["user_id"];
                }
            }

        }




        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
mutation RemoveMemberInOrganization($memberId: Float!, $organizationId: Float!) {
  removeMemberInOrganization(member_id: $memberId, organization_id: $organizationId) {
    activated_at
    active
    created_at
    id
    member_type
    organization_id
    updated_at
  }
}
    ",
                Variables = new
                {
                    memberId = MemberId,
                    organizationId = OrgId
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

    }

}