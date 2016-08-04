using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models.BugTracker
{
    public class zMyCustomCLass
    {
        public zMyCustomCLass()
        {
            this.Detailsz = new HashSet<zDetail>();
            this.Historiesz = new HashSet<zHistory>();
        }

        public string Property1 { get; set; }
        public string Name { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMM dd}", ApplyFormatInEditMode = true)]
        public DateTimeOffset Created { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMM dd}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? Updated { get; set; }

        public virtual ICollection<zDetail> Detailsz { get; set; }
        public virtual ICollection<zHistory> Historiesz { get; set; }



        //public BlogPost()
        //{
        //    this.Comments = new HashSet<Comment>();
        //}
        //public int Id { get; set; }
        //[Require]
        //[DisplayFormat(DataFormatString = "{0:MMM dd}", ApplyFormatInEditMode = true)]
        //public DateTimeOffset Created { get; set; }
        //[DisplayFormat(DataFormatString = "{0:MMM dd}", ApplyFormatInEditMode = true)]
        //public DateTimeOffset? Updated { get; set; }
        //[Require]
        //public string Title { get; set; }
        //public string Slug { get; set; }
        //[Require]
        //[AllowHtml]
        //public string Body { get; set; }
        //public string MediaURL { get; set; }
        //public bool Published { get; set; }
        //public virtual ICollection<Comment> Comments { get; set; }
    }
}