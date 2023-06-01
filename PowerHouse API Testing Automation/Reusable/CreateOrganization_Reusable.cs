using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace PowerHouse_Api
{


    public class CreateOrganization_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string Description;
        public static string OrgName;
        public static string Website;
        public static string ReturnString;


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
mutation CreateOrganization($data: ICreateOrganizationDTO!) {
  createOrganization(data: $data) {
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
            VerifyResponse();
        }

        public void VerifyResponse()
        {
            JObject obj = JObject.Parse(ReturnString);
            string responseName = obj["createOrganization"]["name"].ToString();
            string responseDescription = obj["createOrganization"]["description"].ToString();
            string responseWebsite = obj["createOrganization"]["website"].ToString();

            
            if (responseName != OrgName || responseDescription != Description || responseWebsite != Website)
            {
                throw new Exception("Api return does not match");
            } 
        }

        public string Invoke(string description, string orgName, string website)
        {
            Description= description;
            OrgName= orgName;
            Website= website;
            MainTest().Wait();
            return ReturnString;            
        }
    }

}