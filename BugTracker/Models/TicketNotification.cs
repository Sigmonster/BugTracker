using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketNotification
    {
        public int Id { get; set; }

        //Foreign Keys
        public string UserID { get; set; }
        public int TicketID { get; set; }
        //Foreign Key Tables
        public virtual ApplicationUser User { get; set; }//Holds Associated User
        public virtual TicketPost Ticket { get; set; }//Holds Associated Ticket
    }
}