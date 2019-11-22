using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CreditReversal.Models;
using CreditReversal.DAL;
using CreditReversal.Utilities;

namespace CreditReversal.BLL
{
    public class AgentFunction
    {
        private AccountFunctions account = new AccountFunctions();
        private AgentData agentdata = new AgentData();
        private ClientData clientdata = new ClientData();
        public int AddAgent(Agent agent)
        {
            int status = 0;
           
            try
            {
                status = agentdata.AddAgent(agent);
            }
            catch (Exception ex) {  ex.insertTrace("");  }
            return status;
        }

        public bool AddUser(Users user)
        {
            bool status = false;

            try
            {
                status = agentdata.AddUser(user);
            }
            catch (Exception ex) {  ex.insertTrace("");  }
            return status;

        }

        public bool AddBilling(AgentBilling AgentBilling)
        {
            bool status = false;

            try
            {
                status = agentdata.AddBilling(AgentBilling);
            }
            catch (Exception ex) {  ex.insertTrace("");  }
            return status;

        }

        public List<Agent> GetAgent(int agentid=0)
        {
            List<Agent> agent = new List<Agent>();
            try
            {
                agent = agentdata.GetAgent(agentid);
            }
            catch (Exception ex) {  ex.insertTrace("");  }

            return agent;
        }
        public Agent EditAgent(string AgentID)
        {
            Agent agent = new Agent();
            try
            {
                agent = agentdata.EditAgent(AgentID);
            }
           catch (Exception ex) {  ex.insertTrace("");  }
            return agent;
        }

        public AgentBilling GetAgentBilling(int AgentBillingId)
        {
            AgentBilling agent = new AgentBilling();
            try
            {
                agent = agentdata.GetAgentBilling(AgentBillingId);
            }
            catch (Exception ex) {  ex.insertTrace("");  }

            return agent;
        }

        public List<AgentBilling> GetAgentBillings(int agentid)
        {
            List<AgentBilling> agentB = new List<AgentBilling>();
            try
            {
                agentB = agentdata.GetAgentBillings(agentid);
            }
            catch (Exception ex) {  ex.insertTrace("");  }

            return agentB;
        }



        public List<AgentStaff> GetStaff(string agentid=null,string staffid=null)
        {
            List<AgentStaff> staff = new List<AgentStaff>();
            try
            {
                staff = agentdata.GetStaff(agentid,staffid);
            }
            catch (Exception ex) {  ex.insertTrace("");  }

            return staff;
        }
        public int AddStaff(AgentStaff staff)
        {
            int status = 0;
            bool stat = false;
            string username = staff.Email;
            try
            {
                int Id = staff.AgentStaffId;
                if (Id == 0)
                {
                    stat = account.CheckUsernameexistorNot(username);
                }
              
                if (stat == false)
                {
                    status = agentdata.AddStaff(staff);
                }

            }
            catch (Exception ex) {  ex.insertTrace("");  }
            return status;
        }

        public AgentStaff EditStaff(string staffId)
        {
            AgentStaff staff = new AgentStaff();
            try
            {
                staff = agentdata.EditStaff(staffId);
            }
            catch (Exception ex) {  ex.insertTrace("");  }

            return staff;
        }

        public Users EditUser(string Id,string UserRole)
        {
            Users user = new Users();
            try
            {
                user = agentdata.EditUser(Id, UserRole);

            }
           catch (Exception ex) {  ex.insertTrace("");  }
            return user;
        }
        public bool DeleteAgentStaff(string Id)
        {
            bool status = false;
            try
            {
                status = agentdata.DeleteAgentStaff(Id);
            }
            catch (Exception ex) {  ex.insertTrace("");  }
            return status;
        }


        public bool DeleteAgentBilling(int AgentBillingId)
        {
            bool status = false;
            try
            {
                status = agentdata.DeleteAgentBilling(AgentBillingId);
            }
            catch (Exception ex) {  ex.insertTrace("");  }
            return status;
        }
        public bool DeleteAgent(string agentId, string from = "")        {            bool status = false;            try            {                status = agentdata.DeleteAgent(agentId, from);            }            catch (Exception ex) { ex.insertTrace(""); }            return status;        }

        public bool CheckUsernameexistorNot(string CurrentEmail = "", int ClientId = 0)
        {
            bool status = false;
            try
            {
                if (CurrentEmail != "")
                {
                    status = clientdata.CheckUsernameexistorNot(CurrentEmail, ClientId);
                }
            }
            catch (Exception ex) { ex.insertTrace(""); }
            return status;
        }
        public bool AddAssignStaff(List<NewClient> client)        {            bool status = false;            try            {
                status = agentdata.AddAssignStaff(client);            }            catch (Exception ex) { ex.insertTrace(""); }            return status;        }
        public List<NewClient> GetClient(int id)        {            List<NewClient> newClients = new List<NewClient>();            try            {                newClients = agentdata.GetClient(id);            }            catch (Exception ex) { ex.insertTrace(""); }            return newClients;        }
        public List<CreditReportFiles> GetChallenges(string ClientId)        {            List<CreditReportFiles> creditreport = new List<CreditReportFiles>();            try            {                creditreport = agentdata.GetChallenges(ClientId);            }            catch (Exception ex) { ex.insertTrace(""); }            return creditreport;        }
    }
}