using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;


namespace PowerHouse_Api
{
    [TestFixture]

    public class UpdateOrganization
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string Description;
        public static string OrgName;
        public static string Website;
        public static string ReturnString;
        public static int OrgId;

        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            OrgId = int.Parse(a.GetConfig_("orgId"));
            Description = b.StringGenerator("alphanumeric" ,50);
            OrgName = b.StringGenerator("alphanumeric", 10);
            Website = "https://www."+b.StringGenerator("alphanumeric", 10)+".com";
        }

        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
mutation UpdateOrganization($data: IUpdateOrganizationDTO!, $organizationId: Float!) {
  updateOrganization(data: $data, organization_id: $organizationId) {
    created_at
    description
    name
    organization_id
    updated_at
    website
  }
}
    ",
                Variables = new
                {
                    organizationId = OrgId,
                    data =  new {
                        description = Description,
                        name = OrgName,
                        website = Website
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
            VerifyResponse();
        }

        public void VerifyResponse()
        {
            JObject obj = JObject.Parse(ReturnString);
            string responseName = obj["updateOrganization"]["name"].ToString();
            string responseDescription = obj["updateOrganization"]["description"].ToString();
            string responseWebsite = obj["updateOrganization"]["website"].ToString();

            
            if (responseName != OrgName || responseDescription != Description || responseWebsite != Website)
            {
                throw new Exception("Api return does not match");
            }

            new UpdateOrganization_Reusable().Invoke(OrgId, "PowerHouse Automation Testing", Description, Website);
        }

    }

}