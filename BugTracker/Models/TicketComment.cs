using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketComment
    {
        public int Id { get; set; }
        //check format
        [Required]
        [DisplayFormat(DataFormatString = "{0:MMM dd}", ApplyFormatInEditMode = true)]
        public DateTimeOffset Created { get; set; }
        public string CommentBody { get; set; }
        //Foreign Keys
        public string UserID { get; set; }
        public int TicketID { get; set; }
        //Foreign Key Tables
        public virtual ApplicationUser User { get; set; }//Holds Associated User
        public virtual TicketPost Ticket { get; set; }//Holds Associated Ticket
    }
}