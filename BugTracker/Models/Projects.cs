using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Projects
    {
        public Projects()//Constructor
        {//These HashSets get all Tickets and Users associated with the Project
            this.Tickets = new HashSet<TicketPost>();
            this.Users = new HashSet<ApplicationUser>();

        }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        //Holds Lists of Associated records
        public virtual ICollection<TicketPost> Tickets { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

    }
}