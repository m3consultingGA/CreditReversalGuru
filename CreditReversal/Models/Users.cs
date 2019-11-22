using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreditReversal.Models
{
    public class Users
    {

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string UserRole { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int AgentClientId {get;set;}

    }
}