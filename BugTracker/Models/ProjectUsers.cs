using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class ProjectUsers
    {
        public int Id { get; set; }

        //Foreign Keys
        public string UserID { get; set; }
        public int ProjectID { get; set; }
        //Foreign Key Tables
        public virtual ApplicationUser User { get; set; }//Holds Associated User
        public virtual Projects Project { get; set; }//Holds Associated Project
    }
}