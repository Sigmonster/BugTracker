using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketStatuses
    {
        public TicketStatuses()//Constructor
        {//This HashSet gets all TicketIDs associated with the TicketStatus
            this.Tickets = new HashSet<TicketPost>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        //Holds Lists of Associated records
        public virtual ICollection<TicketPost> Tickets { get; set; }
    }
}