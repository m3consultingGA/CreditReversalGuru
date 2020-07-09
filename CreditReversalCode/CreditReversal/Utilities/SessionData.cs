using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Utilities
{
    public class SessionData
    {
        public bool GetLoginStatus()        {            bool status = false;            string userID = null;
            string Userrole = null;            try            {                if (HttpContext.Current.Session["UserId"] != null&& HttpContext.Current.Session["UserRole"]!=null) {
                    userID = HttpContext.Current.Session["UserId"].ToString();
                    Userrole = HttpContext.Current.Session["UserRole"].ToString();
                }                if (userID != null && Userrole != null)                {                    status = true;                }            }            catch (Exception ex)            {            }            return status;        }
        public string GetUserID()
        {
            string userID = null;
            try
            {

                if (HttpContext.Current.Session["UserId"] != null)
                {
                    userID = HttpContext.Current.Session["UserId"].ToString();
                }
            }
            catch (Exception ex)
            {

            }

            return userID;
        }
        public string GetUserName()
        {
            string username = null;

            try
            {
                if (HttpContext.Current.Session["UserName"] != null)
                {
                    username = HttpContext.Current.Session["UserName"].ToString();
                }
            }
            catch (Exception ex)
            {

            }
            return username;
        }
        public string GetUserRole()
        {
            string UserRole = null;

            try
            {
                if (HttpContext.Current.Session["UserRole"] != null)
                {
                    UserRole = HttpContext.Current.Session["UserRole"].ToString();
                }
            }
            catch (Exception ex)
            {

            }

            return UserRole;
        }
        public string GetEmail()
        {
            string Email = null;

            try
            {
                if (HttpContext.Current.Session["EmailAddress"] != null)
                {
                    Email = HttpContext.Current.Session["EmailAddress"].ToString();
                }
            }
            catch (Exception ex)
            {

            }

            return Email;
        }
        public string GetStatus()
        {
            string FirstName = null;

            try
            {
                if (HttpContext.Current.Session["Status"] != null)
                {
                    FirstName = HttpContext.Current.Session["Status"].ToString();
                }
            }
            catch (Exception ex)
            {

            }

            return FirstName;
        }

        public string GetCreatedBy()
        {
            string FirstName = null;

            try
            {
                if (HttpContext.Current.Session["CreatedBy"] != null)
                {
                    FirstName = HttpContext.Current.Session["CreatedBy"].ToString();
                }
            }
            catch (Exception ex)
            {

            }

            return FirstName;
        }

        public string GetCreatedDate()
        {
            string FirstName = null;

            try
            {
                if (HttpContext.Current.Session["CreatedDate"] != null)
                {
                    FirstName = HttpContext.Current.Session["CreatedDate"].ToString();
                }
            }
            catch (Exception ex)
            {

            }

            return FirstName;
        }

        public string GetAgentId()
        {
            string AgentId = null;
            try
            {
                if (HttpContext.Current.Session["AgentId"] != null)
                {
                    AgentId = HttpContext.Current.Session["AgentId"].ToString();
                }
            }
            catch (Exception ex)
            { }
            return AgentId;
        }
        public string GetStaffId()
        {
            string FirstName = null;

            try
            {
                if (HttpContext.Current.Session["StaffId"] != null)
                {
                    FirstName = HttpContext.Current.Session["StaffId"].ToString();
                }
            }
            catch (Exception ex)
            {

            }

            return FirstName;
        }
        public string GetClientId()
        {
            string FirstName = null;

            try
            {
                if (HttpContext.Current.Session["ClientId"] != null)
                {
                    FirstName = HttpContext.Current.Session["ClientId"].ToString();
                }
            }
            catch (Exception ex)
            {

            }

            return FirstName;
        }
        public string getDasboard()
        {
            string dashboard = "#";
            try
            {
                string role = null;
                role = GetUserRole();
                if (role == "admin")
                {
                    dashboard = "/dashboard/admin";
                }
                else if (role == "agentadmin")
                {
                    dashboard = "/dashboard/agent";
                }
                else if (role == "agentstaff")
                {
                    dashboard = "/dashboard/staff";
                }
                else if (role == "client")
                {
                    dashboard = "/dashboard/client";
                }

            }
            catch (Exception ex)
            { }
            return dashboard;
        }
        public int GetAgentClientId()
        {
            int id = 0;

            try
            {
                if (HttpContext.Current.Session["AgentClientId"] != null)
                {
                    id = Convert.ToInt32(HttpContext.Current.Session["AgentClientId"].ToString());
                }
            }
            catch (Exception ex)
            {

            }

            return id;
        }
    }
}