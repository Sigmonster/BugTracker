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
        //Previous Data Saved before changes
        [Required]
        [AllowHtml]
        public string BodyData { get; set; }
        //Foreign Keys(Un-Changeable)
        [Required]
        public int TicketId { get; set; }//FK
        [Required]
        public int UpdatedByUserId { get; set; }//FK

        //Foreign Key (Un-Changeable)Tables
        public virtual ApplicationUser UpdatedByUser { get; set; }//Holds Associated User that updated
        public virtual TicketPost Ticket { get; set; }//Holds Associated Ticket



    }
}