using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;

namespace PowerHouse_Api
{


    public class UpdateOrganization_Reusable
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
        }


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
        }

        public string Invoke(int orgId, string orgName, string orgDescription, string website)
        {
            OrgId = orgId;
            OrgName = orgName;
            Description = orgDescription;
            Website = website;
            MainTest().Wait();
            return ReturnString;
        }

    }

}