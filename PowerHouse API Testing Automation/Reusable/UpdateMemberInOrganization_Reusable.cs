using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;


namespace PowerHouse_Api
{


    public class UpdateMemberInOrganization_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static int OrgId;
        public static string Email = "marnel.powerhouse@gmail.com";
        public static string MemberType = "PO";
        public static int MemberId;
        public static string InviteId;


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

mutation UpdateMemberInOrganization($memberId: Float!, $organizationId: Float!, $data: IAddMemberInOrganizationDTO!) {
  updateMemberInOrganization(member_id: $memberId, organization_id: $organizationId, data: $data) {
    activated_at
    active
    created_at
    id
    member_type
    organization_id
    updated_at
    user {
      user_id
    }
    organization {
      organization_id
    }
    user_id
  }
}

    ",
                Variables = new
                {
                    memberId = MemberId,
                    organizationId = OrgId,
                    data = new
                        {
                        organization_id = OrgId,
                        member_type = "OWNER",
                        member_id = MemberId
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

        public string Invoke(int memberId, int orgId)
        {
            MemberId = memberId;
            OrgId = orgId;
            MainTest().Wait();
            return ReturnString;
        }
    }

}