using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string TriggeredByUserId { get; set; }
        public string Notification { get; set; }
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? Created { get; set; }
        public bool Read { get; set; }
        public bool Star { get; set; }
        //Foreign Key Tables
        public virtual ApplicationUser TriggeredByUser { get; set; }//Holds Associated User Id.
        public virtual ApplicationUser User { get; set; }//Holds Associated User
        public virtual TicketPost Ticket { get; set; }//Holds Associated Ticket
    }
}