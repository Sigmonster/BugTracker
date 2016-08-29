using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

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
    public class EPRMSelectedListVM {
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

    //
    //################ Start Project Manager Views ################
    //PM/Dashboards
    public class PMDashboardVM
    {
        public List<Projects> MyProjects { get; set; }
        public List<PMTicketProjectsSelectVM> PMVMListForPartials { get; set; }
    }

    public class PMTicketProjectsSelectVM
    {
        public string ProjectName { get; set; }
        public int ProjectId { get; set; }
        public List<PMUsersInProjectVM> PMUsersInProjectVMList { get; set; }
        public List<PMAssignUsersTicketVM> PMAssignUsersTicketList { get; set; }
    }
    //Users in Project
    public class PMUsersInProjectVM
    {
        public int ProjectId { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
    }
    //Tickets In Project
    public class PMAssignUsersTicketVM
    {
        public int ProjectId { get; set; }
        public int TicketId { get; set; }
        public TicketPost Ticket { get; set; }
        public bool IsChecked { get; set; }
    }

    //
    //################ End Project Manager Views ################
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

    public class TopDispHist
    {
        public DateTimeOffset? Created { get; set; }
        public string DisplayName { get; set; }
        public List<TicketHistory> HistEntriesList {get;set;}
    }



    //################End Tickets###############
    //

    //
    //################ Begin Notifications ###############
    //
    public class MyNotifications
    {
        public bool MarkAsRead { get; set; }
        public int  NotificationId { get; set; }
        public TicketNotification Notification { get; set; }
    }

    //
    //################ End Notifications ###############
    //

    //################Begin Comments###############
    //
    //CommentPOSTVM
    public class CommentPOSTVM
    {

    }

    //################End Comments###############

    //################Begin Home###############
    public class MyHomeVM
    {
        public ApplicationUser CurrentUser  { get; set; }
        public int myOwnedTicketsCount      { get; set; }
        public int myAssignedTicketsCount   { get; set; }
        public int myNotificationsCount     { get; set; }
        public int myNotificationsLast24Hrs { get; set; }
        public int myNotificationsLast72Hrs { get; set; }
        public int myNotificationsLast7Days { get; set; }
        public List<Projects> myProjects    { get; set; }
        public List<string> myRoles { get; set; }
        public MyHomeProjects MyHomeProjects { get; set; }

    }
    public class MyHomeProjects
    {
        public List<Projects> Projects { get; set; }
        public List<string> allProjectManagers { get; set; }
    }
    //################End Home###############

    //################ Begin Admin ###############
    public class AdminUsersVM
    {
        public string Title { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
    public class AdminTicketsVM
    {
        public string Title { get; set; }
        public List<TicketPost> Tickets { get; set; }
    }


    //################ End Admin ###############
}