using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class TicketPost
    {
        public TicketPost()//Constructor
        {//These HashSets get all TicketHistories, TicketAttachments, TicketComments, and TicketNotifications associated with this Ticket
            this.TicketHistories        = new HashSet<TicketHistory>();
            this.TicketAttachments      = new HashSet<TicketAttachment>();
            this.TicketComments         = new HashSet<TicketComment>();
            this.TicketNotifications    = new HashSet<TicketNotification>();
        }
        public int Id { get; set; }
        //Check format
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)]
        public DateTimeOffset Created { get; set; }
        //Check format
        [DisplayFormat(DataFormatString = "{0:g}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? Updated { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [AllowHtml]
        public string Description { get; set; }
        //Foreign Keys
        [Required]
        public int ProjectID { get; set; }//FK
        public int TicketStatusID { get; set; }//FK
        [Required]
        public int TicketTypeID { get; set; }//FK
        [Required]
        public int TicketPriorityID { get; set; }//FK

        public string OwnerUserID { get; set; }//FK
        public string AssignedToUserID { get; set; }//FK
        public string UpdatedByUserID { get; set; }//FK

        //Foreign Key Tables
        public virtual Projects Project { get; set; }//Holds Associated Project
        public virtual TicketStatuses TicketStatus { get; set; }//Holds Associated Ticket Status 
        public virtual TicketTypes TicketType { get; set; }//Holds Associated Ticket Type
        public virtual TicketPriorities TicketPriority { get; set; }//Holds Associated Ticket Priority
        public virtual ApplicationUser OwnerUser { get; set; }//Holds Associated User
        public virtual ApplicationUser AssignedToUser { get; set; }//Holds Associated User
        public virtual ApplicationUser UpdatedByUser { get; set; }//Holds Associated User


        //Holds Lists of Associated records
        public virtual ICollection<TicketHistory> TicketHistories {get; set;}
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketNotification> TicketNotifications { get; set; }

    }
}