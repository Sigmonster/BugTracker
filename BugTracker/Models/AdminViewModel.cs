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

    public class AdminUserData
    {

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
    public class AdminEditUser
    {
        //[Required]
        //[StringLength(40, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        //[Display(Name = "Display Name")]
        //public string UserDisplayName { get; set; }
        //[Required]
        //[StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        //[Display(Name = "First Name")]
        //public string UserFirstName { get; set; }
        //[Required]
        //[StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        //[Display(Name = "Last Name")]
        //public string UserLastName { get; set; }
        
    }
        public class ProjectEditUsersViewModel
    {
        public Projects CurrentProject { get; set; }
        public List<ApplicationUser> ApplicationUsersList { get; set; }
    }

}