using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web.Mvc;
using BugTracker.Models;

namespace BugTracker.Models
{
    //################Begin Projects################
    public class EPBigVM
    {
        public List<ApplicationUser> UsersAll { get; set; }
        public List<ApplicationUser> UsersInProject { get; set; }
        public List<ApplicationUser> UsersNotInProject { get; set; }
        public List<ApplicationUser> SelectedRemoveUsers { get; set; }
        public Projects Project { get; set; }
        public List<EPSelectedVM> EPSelectedVM { get; set; }
        public EPSelectedListVM EPSelectedListVM { get; set; }
        public List<EPRMSelectedVM> EPRMSelectedVM { get; set; }
        public EPRMSelectedListVM EPRMSelectedListVM { get; set; }
    }
    //Users Not in Project
    public class EPSelectedVM
    {
        public string UserDisplayName { get; set; }
        public string UserEmail { get; set; }
        public string UserId { get; set; }
        public bool IsChecked { get; set; }
    }
    public class EPSelectedListVM
    {
        public List<EPSelectedVM> Users { get; set; }
    }
    //Users In Project
    public class EPRMSelectedVM
    {
        public string UserDisplayName { get; set; }
        public string UserEmail { get; set; }
        public string UserId { get; set; }
        public bool IsChecked { get; set; }
    }
    public class EPRMSelectedListVM{
        public List<EPRMSelectedVM> Users { get; set; }
    }

    public class ProjectDetailsVM
    {
        public Projects Project { get; set; }
        public List<ApplicationUser> ProjectManagers { get; set; }
        public int DeveloperCount { get; set; }
        public int UnassignedCount { get; set; }
        public int TotalTickets { get; set; }
    }
    //
    //################End Projects################
    //


    //################Begin Tickets###############
    //
    //ProjectDetailsVM (contains tickets)
    public class PDetailsVM
    {

    }
    //
    //DispTicketsVM
    public class DispTicketsVM
    {
        public List<TicketPost> TicketList { get; set; }
        public string TitleDesc { get; set; }
    }

    //################End Tickets###############

    //################Begin Comments###############
    //
    //CommentPOSTVM
    public class CommentPOSTVM
    {

    }
}