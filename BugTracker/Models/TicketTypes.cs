using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketTypes
    {
        public TicketTypes()//Constructor
        {//This HashSet gets all TicketIDs associated with the TicketType
            this.TicketIDs = new HashSet<TicketPost>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        //Holds Lists of Associated records
        public virtual ICollection<TicketPost> TicketIDs { get; set; }
    }
}