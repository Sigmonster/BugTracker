using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Projects
    {
        public Projects()//Constructor
        {//These HashSets get all TicketIDs and Users associated with the Project
            this.TicketIDs = new HashSet<TicketPost>();
            this.Users = new HashSet<ProjectUsers>();

        }
        public int Id { get; set; }
        public string Name { get; set; }
        //Holds Lists of Associated records
        public virtual ICollection<TicketPost> TicketIDs { get; set; }
        public virtual ICollection<ProjectUsers> Users { get; set; }

    }
}