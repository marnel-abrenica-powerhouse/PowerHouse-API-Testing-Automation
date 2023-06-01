using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Newtonsoft.Json;

namespace PowerHouse_API_Testing_Automation.AppManager
{
    public class Get_Update_Config
    {
        //Get value of JSON file
        public string GetConfig_(string configType)
        {
            string? returnString = null;

            IConfigurationRoot config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();


            if (configType == "authToken")
            {
                returnString = config["authToken"];
            }
            else if (configType == "baseUrl")
            {
                returnString = config["baseUrl"];
            }
            else if (configType == "orgId")
            {
                returnString = config["orgId"];
            }

            return returnString;
        }


        /*modify value of JSON file
        public void ModifyConfigValue()
        {


            var configPath = "appsettings.json";
            var json = File.ReadAllText(configPath);

            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            jsonObj["authToken"] = "test";

            string updatedJson = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(configPath, updatedJson);
        }
        */
    }
}
