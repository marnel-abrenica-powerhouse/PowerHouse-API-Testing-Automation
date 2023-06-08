﻿using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PowerHouse_Api
{
    [TestFixture]
    [Parallelizable]
    public class GetUnderReviewTasks
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static string TaskName;
        public static string TaskDescription;
        public static string ProjectName;
        public static string ProjectOverview;
        public static string Pr;
        public static string Comment;
        public static int ProjectId;
        public static int Payout;
        public static int TaskId;
        public static int MaxTime;
        public static int TimeEstimate;
        public static decimal NewBidPrice;
        public static int BidId;
        public static int TransactionId;
        public static string CodeReviewComment;
        public static int SubmissionId;

        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            TaskName = b.StringGenerator("allletters", 10);
            TaskDescription = b.StringGenerator("alphanumeric", 50);
            Pr = "https://www." + b.StringGenerator("allletters", 10) + ".com";
            Comment = b.StringGenerator("alphanumeric", 50);
            ProjectName = b.StringGenerator("allletters", 10);
            ProjectOverview = b.StringGenerator("alphanumeric", 50);
            Payout = int.Parse(b.StringGenerator("allnumbers", 4));
            MaxTime = int.Parse(b.StringGenerator("allnumbers", 3));
            TimeEstimate = int.Parse(b.StringGenerator("allnumbers", 3));
            NewBidPrice = new CalculateBid().GetMaxBid(Payout - 1);


            string returnOrg = new CreateProject_Reusable().Invoke(ProjectName, ProjectOverview);
            JObject orgObj = JObject.Parse(returnOrg);
            int projectId = orgObj["createProject"]["id"].Value<int>();
            ProjectId = projectId;

            string returnTask = new CreateTask_Reusable().Invoke(TaskName, TaskDescription, ProjectId, Payout, MaxTime, TimeEstimate);
            JObject obj = JObject.Parse(returnTask);
            int taskId = obj["createTask"]["task_id"].Value<int>();
            TaskId = taskId;
            new PublishTasksToMarketplace_Reusable().Invoke(TaskId);


            string returnBid = new CreateBid_Reusable().Invoke(Convert.ToInt32(NewBidPrice), TaskId);
            JObject objBid = JObject.Parse(returnBid);
            BidId = objBid["createBid"]["id"].Value<int>();


            string returnAccpetBid = new AcceptBid_Reusable().Invoke(BidId);
            JObject objAcceptBid = JObject.Parse(returnAccpetBid);
            TransactionId = objAcceptBid["acceptBid"]["task"]["transaction"]["id"].Value<int>();

            new Pullrequest_Reusable().Invoke(TransactionId, Pr, Comment);
            string returnGetTaskById = new GetTaskById_Reusable().Invoke(TaskId);
            JObject objGetTaskById = JObject.Parse(returnGetTaskById);
            SubmissionId = objGetTaskById["getTaskById"]["transaction"]["submissions"]["id"].Value<int>();

        }

        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
query GetUnderReviewTasks($projectId: Float!) {
  getUnderReviewTasks(project_id: $projectId) {
    ETA
    base_cost
    bids {
      id
    }
    complexity_level
    created_at
    description
    end_date
    end_date_to_bid
    lowest_bid
    markup_cost
    max_time
    name
    notion_link
    parent_id
    payout
    project {
      name
    }
    project_id
    remaining_time_to_start
    spent_time
    start_date
    status
    task_id
    task_sales_price
    task_type
    time_estimate
    total_bids
    transaction {
      id
    }
    updated_at
  }
}
    ",
                Variables = new
                {
                    projectId = ProjectId
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
            PostTest();

        }

        public void PostTest()
        {

            JObject obj = JObject.Parse(ReturnString);
            string taskName = obj["getUnderReviewTasks"][0]["name"].ToString();
            string taskDescription = obj["getUnderReviewTasks"][0]["description"].ToString();
            int projectId = obj["getUnderReviewTasks"][0]["project_id"].Value<int>();
            int taskId = obj["getUnderReviewTasks"][0]["task_id"].Value<int>();

            if (taskName != TaskName || taskDescription != TaskDescription || projectId != ProjectId || taskId != TaskId)
            {
                throw new Exception("Api return does not match");
            }

        }
    }

}