using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web.Mvc;

namespace BugTracker.Models
{

    public class AdminIndex2ViewModel
    {
        public List<TicketPost> AllTickets { get; set; }
        public List<Projects> AllProjects {get; set;}
        public List<ApplicationUser> AllUsers { get; set; }
    }

    public class AdminIndexViewModel
    {
        public List<ApplicationUser> AllUsers { get; set; }
    }
    public class AdminUserViewModel
    {
        public ApplicationUser User { get; set; }
        public MultiSelectList Roles { get; set; }
        public string[] SelectedRoles { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }

}