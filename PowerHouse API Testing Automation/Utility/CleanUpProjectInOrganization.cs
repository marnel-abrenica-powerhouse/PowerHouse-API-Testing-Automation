using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PowerHouse_Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerHouse_API_Testing_Automation.Utility
{
    [TestFixture]
    public class CleanUpProjectInOrganization
    {
        public int OrgId = 3;

        List<int> ProjectIdsInORg = new();
        List <string> SensitiveDataIds = new();
        List<int> ProposalIds = new();



        [Test]
        public void Execute()
        {

            string returnProjecList = new ListAllProjects_Reusable().Invoke(OrgId);
            JObject obj = JObject.Parse(returnProjecList);
            JArray projectIds = (JArray)obj["listAllProjects"];
            DeleteProject_Reusable a = new();
            int projectCount =0;
            int deleteCount =0;

            foreach (JObject project in projectIds)
            {
                string raw = (string)project["project_id"];
                int resutlProjectId = Convert.ToInt32(raw);
                ProjectIdsInORg.Add(resutlProjectId);
            }

            DeleteProposals();
            DeleteSensitiveData();

            foreach (int id in ProjectIdsInORg)
            {
                projectCount++;
                try { 
                    a.Invoke(id);
                    deleteCount++;
                }
                catch { }
            }
            

            Console.WriteLine("Total Projects: " + projectCount.ToString());
            Console.WriteLine("Projects deleted: "+ deleteCount.ToString()) ;
        }


        public void DeleteSensitiveData()
        {
            GetSensitiveDataByProjectId_Reusable a = new();
            DeleteSensitiveData_Reusable b = new();


            foreach (int projectId in ProjectIdsInORg)
            {

                try
                {
                    string returnSensitiveData = a.Invoke(projectId);
                    JObject objSensitiveData = JObject.Parse(returnSensitiveData);
                    string sensitiveDataId = objSensitiveData["getSensitiveDataByProjectId"][0]["id"].ToString();
                    SensitiveDataIds.Add(sensitiveDataId);
                }
                catch
                {
                }
            }

            foreach (string id in SensitiveDataIds)
            {
                b.Invoke(id);
            }
        }

        public void DeleteProposals()
        {
            string proposalId = new GetAllProposals_Reusable().Invoke(OrgId);
            JObject obj = JObject.Parse(proposalId);
            JArray proposalsList = (JArray)obj["getAllProposals"];
            DeleteProposal_Reusable a = new();

            foreach (JObject proposal in proposalsList)
            {
                string raw = (string)proposal["id"];
                int resutlProjectId = Convert.ToInt32(raw);
                ProposalIds.Add(resutlProjectId);
            }

            foreach( int savedId in ProposalIds)
            {
                a.Invoke(savedId);
            }
        }


    }
}
