﻿using GraphQL;
using GraphQL.Client.Http;
using NUnit.Framework;
using GraphQL.Client.Serializer.Newtonsoft;
using PowerHouse_API_Testing_Automation.AppManager;

namespace PowerHouse_Api
{
    [TestFixture]
    [Parallelizable]
    public class ListUsers
    {
        public static string AuthToken;
        public static string BaseUrl;
        public static string ProjectId;

        public void Precondition()
        {
            Commands b = new();
            Get_Update_Config a = new();
            AuthToken = a.GetConfig_("authToken");
            BaseUrl = a.GetConfig_("baseUrl");
            ProjectId = b.StringGenerator();


        }

        [Test]
        public async Task MainTest()
        {
            Precondition();

            var query = new GraphQLRequest
            {
                Query = @"
query ListUsers {
  listUsers {
    email
    roles {
      role {
        name
        role_id
        created_at
        updated_at
      }
      role_id
    }
    user_id
    updated_at
    profile {
      first_name
      last_name
    }
    organizations_members {
      id
    }
    organizations {
      name
    }
    last_activity_at
    is_active
    google_id
    created_at
    bids {
      id
    }
  }
}
    ",
                Variables = new
                {

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

            Console.WriteLine(response.Data);

        }

    }

}