using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;


namespace PowerHouse_Api
{ 

    public class UpdateCodeReviewStatus_Reusable
    {
        public static String AuthToken;
        public static String BaseUrl;
        public static String ReturnString;
        public static string CodeReviewComment;
        public static int SubmissionId;
        public static string Status;

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
mutation UpdateCodeReviewStatus($submissionId: Float!, $status: StatusSubmission, $comments: String) {
  updateCodeReviewStatus(
    id: $submissionId
    data: {code_review_status: $status, code_review_comment: $comments}
  ) {
    id
    acceptance_comments
    acceptance_status
    code_review_comment
    code_review_status
    comments
    created_at
    pr
    qa_comments
    qa_status
    transaction_id
    updated_at
  }
}
    ",
                Variables = new
                {
                    submissionId = SubmissionId,
                    comments = CodeReviewComment,
                    status = Status
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

        public string Invoke(int submissionId, string comment, string status)
        {
            SubmissionId= submissionId;
            CodeReviewComment= comment;
            Status= status;
            MainTest().Wait();
            return ReturnString;

        }

    }

}