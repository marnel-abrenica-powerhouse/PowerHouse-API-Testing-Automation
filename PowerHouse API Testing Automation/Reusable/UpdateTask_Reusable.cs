﻿using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;
using Newtonsoft.Json;


namespace PowerHouse_Api
{ 

    public class UpdateTask_Reusable
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ReturnString;
        public static int TaskId;
        public static string UpdatedName;
        public static string UpdatedDescription;
        public static int UpdatedPayout;
        public static int UpdatedMaxTime;
        public static int UpdatedTimeEstimate;


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
mutation UpdateTask($data: IUpdateProjectTaskDTO!, $taskId: Float!) {
  updateTask(data: $data, task_id: $taskId) {
    name
    description
    payout
    max_time
    time_estimate
    task_id
    project_id
  }
}
    ",
                Variables = new
                {
                    data = new 
                        {
                        name = UpdatedName,
                        description = UpdatedDescription,
                        payout = UpdatedPayout,
                        max_time = UpdatedMaxTime,
                        time_estimate = UpdatedTimeEstimate
                        },
                    taskId = TaskId
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

        public string Invoke(string updatedName, string updatedDescription, int updatedPayout, int updatedMaxTime, int updatedTimeEstimate, int taskId)
        {
            UpdatedName = updatedName;
            UpdatedDescription = updatedDescription;
            UpdatedPayout = updatedPayout;
            UpdatedMaxTime = updatedMaxTime;
            UpdatedTimeEstimate = updatedTimeEstimate;
            TaskId = taskId;
            MainTest().Wait();
            return ReturnString;
        }
    }

}