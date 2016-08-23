using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketAttachment
    {
        public int Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)]
        public DateTimeOffset Created { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public string FileURL { get; set; }
        //Foreign Keys
        public string UserID { get; set; }
        [Required]
        public int TicketID { get; set; }
        //Foreign Key Tables
        public virtual ApplicationUser User { get; set; }//Holds Associated User
        public virtual TicketPost Ticket { get; set; }//Holds Associated Ticket
    }
}