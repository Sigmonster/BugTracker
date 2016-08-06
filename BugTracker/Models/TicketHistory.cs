using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMM dd}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? UpdatedTime { get; set; }
        public virtual ApplicationUser UpdatedByUserID { get; set; }//Holds Associated User who updated


        //Previous Data Saved before changes
        [Required]
        public string PreviousTitle { get; set; }
        [Required]
        [AllowHtml]
        public string PreviousDescription { get; set; }
        //Foreign Keys(Changeable)
        [Required]
        public int PreviousTicketStatusID { get; set; }//FK
        [Required]
        public int PreviousTicketTypeID { get; set; }//FK
        [Required]
        public int PreviousTicketPriorityID { get; set; }//FK
        //Foreign Keys(Un-Changeable)
        [Required]
        public int TicketId { get; set; }//FK
        [Required]
        public int ProjectId { get; set; }//FK
        [Required]
        public int OwnerUserId { get; set; }//FK
        //Foreign Key Tables
        public virtual TicketStatuses TicketStatus { get; set; }//Holds Associated Ticket Status 
        public virtual TicketTypes TicketType { get; set; }//Holds Associated Ticket Type
        public virtual TicketPriorities TicketPriority { get; set; }//Holds Associated Ticket Priority
        //Foreign Key (Un-Changeable)Tables
        public virtual TicketPost Post { get; set; }//Holds Associated Ticket
        public virtual Projects Project { get; set; }//Holds Associated Project
        public virtual ApplicationUser OwnerUser { get; set; }//Holds Associated User Owner
        public virtual ApplicationUser UpdatedByUser { get; set; }//Holds Associated User who updated



    }
}