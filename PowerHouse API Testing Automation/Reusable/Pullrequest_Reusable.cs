using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;

namespace PowerHouse_Api
{ 
    public class Pullrequest_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static string Pr;
        public static string Comment;
        public static int TransactionId;


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
mutation Pullrequest($transactionId: Float!, $pr: String, $comments: String) {
  pullrequest(transaction_id: $transactionId, pr: $pr, comments: $comments) {
    pr
    comments
    transaction_id
    id
    acceptance_comments
    acceptance_status
    code_review_comment
    code_review_status
    created_at
    qa_comments
    qa_status
    updated_at
  }
}
    ",
                Variables = new
                {
                    transactionId = TransactionId,
                    pr = Pr,
                    comments= Comment
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


        public string Invoke(int transactionId, string pr, string comment)
        {
            TransactionId = transactionId;
            Pr= pr;
            Comment = comment;
            MainTest().Wait();
            return ReturnString;
        }
    }

}