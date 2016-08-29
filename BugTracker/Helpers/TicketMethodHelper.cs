using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Helpers
{
    public class TicketCustomHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //
        //Notifications ######################################################
        //

        //Assignment Notification
        public void AssignmentNotification(TicketPost oldTicket, TicketPost editedTicket, DateTimeOffset ticketUpdatedTimeStamp, ApplicationUser ticketEditor)
        {
            if (oldTicket.AssignedToUserID != null)
            {
                var newTicketNotification = new TicketNotification();
                newTicketNotification.Notification =  
                    " Assigned Ticket # " + oldTicket.Id + " to " + GetDisplayName(editedTicket.AssignedToUserID);
                newTicketNotification.TriggeredByUserId = ticketEditor.Id;
                newTicketNotification.TicketID = oldTicket.Id;
                newTicketNotification.Created = ticketUpdatedTimeStamp;
                newTicketNotification.UserID = oldTicket.AssignedToUserID;
                db.TicketNotifications.Add(newTicketNotification);
            }

            //Notification for new Assignee
            var newTicketNotification2 = new TicketNotification();
            newTicketNotification2.Notification = 
                " Assigned Ticket # " + oldTicket.Id + " to you.";
            newTicketNotification2.TriggeredByUserId = ticketEditor.Id;
            newTicketNotification2.TicketID = oldTicket.Id;
            newTicketNotification2.Created = ticketUpdatedTimeStamp;
            newTicketNotification2.UserID = editedTicket.AssignedToUserID;
            db.TicketNotifications.Add(newTicketNotification2);

            //Notify Owner
            var newTicketNotification3 = new TicketNotification();
            newTicketNotification3.Notification =
                " The Ticket # " + oldTicket.Id + " that you own, has been assigned to " + GetDisplayName(editedTicket.AssignedToUserID);
            newTicketNotification3.TriggeredByUserId = ticketEditor.Id;
            newTicketNotification3.TicketID = oldTicket.Id;
            newTicketNotification3.Created = ticketUpdatedTimeStamp;
            newTicketNotification3.UserID = db.TicketPosts.Find(oldTicket.Id).OwnerUserID;
            db.TicketNotifications.Add(newTicketNotification3);
            //Save Ticket Notification(s)
            db.SaveChanges();

        }
        public void GenericTicketChangeNotification(string ticketAsigneeId , ApplicationUser ticketEditor, int ticketId, DateTimeOffset ticketUpdatedTimeStamp)
        {
            var assignedUserId = (db.TicketPosts.Find(ticketId).AssignedToUser != null) ? db.TicketPosts.Find(ticketId).AssignedToUser.Id : "Unassigned";

            if (assignedUserId != "Unassigned")
            {
                //Notify Asignee
                var newTicketNotification = new TicketNotification();
                newTicketNotification.Notification =
                    " Has made changes to Ticket # " + ticketId + ".";
                newTicketNotification.TriggeredByUserId = ticketEditor.Id;
                newTicketNotification.TicketID = ticketId;
                newTicketNotification.Created = ticketUpdatedTimeStamp;
                newTicketNotification.UserID = ticketAsigneeId;
                db.TicketNotifications.Add(newTicketNotification);
            }
            //Notify Owner
            var newTicketNotification2 = new TicketNotification();
            newTicketNotification2.Notification =
                " Has made changes to Ticket # " + ticketId + ".";
            newTicketNotification2.TriggeredByUserId = ticketEditor.Id;
            newTicketNotification2.TicketID = ticketId;
            newTicketNotification2.Created = ticketUpdatedTimeStamp;
            newTicketNotification2.UserID = db.TicketPosts.Find(ticketId).OwnerUserID;
            db.TicketNotifications.Add(newTicketNotification2);
            //Save Ticket Notification
            db.SaveChanges();

        }
        public void TicketCommentNotification(ApplicationUser commentor, TicketComment tComment, DateTimeOffset commentTimeStamp)
        {
            var assignedUserId = (db.TicketPosts.Find(tComment.TicketID).AssignedToUser != null) ? db.TicketPosts.Find(tComment.TicketID).AssignedToUser.Id : "Unassigned";

            if (assignedUserId != "Unassigned")
            {
             //Notify Assignee
            var newTicketNotification = new TicketNotification();
            newTicketNotification.Notification = 
                " Has commented on Ticket # " + tComment.TicketID + ".";
            newTicketNotification.TriggeredByUserId = commentor.Id;
            newTicketNotification.TicketID = tComment.TicketID;
            newTicketNotification.Created = commentTimeStamp;
            newTicketNotification.UserID = assignedUserId;
            db.TicketNotifications.Add(newTicketNotification);
            }
            
            //Notify Owner
            var newTicketNotification2 = new TicketNotification();
            newTicketNotification2.Notification =
                " Has commented on Ticket # " + tComment.TicketID + ".";
            newTicketNotification2.TriggeredByUserId = commentor.Id;
            newTicketNotification2.TicketID = tComment.TicketID;
            newTicketNotification2.Created = commentTimeStamp;
            newTicketNotification2.UserID = db.TicketPosts.Find(tComment.TicketID).OwnerUserID;
            db.TicketNotifications.Add(newTicketNotification2);
            //Save Ticket Notifications
            db.SaveChanges();
        }
        public void CommentEditNotification(ApplicationUser commentor, TicketComment tComment, DateTimeOffset commentTimeStamp)
        {
            var assignedUserId = (db.TicketPosts.Find(tComment.TicketID).AssignedToUser != null) ? db.TicketPosts.Find(tComment.TicketID).AssignedToUser.Id : "Unassigned";

            if (assignedUserId != "Unassigned")
            {
                var newTicketNotification = new TicketNotification();
                newTicketNotification.Notification =
                    " Has edited a comment on Ticket # " + tComment.TicketID + ".";
                newTicketNotification.TriggeredByUserId = commentor.Id;
                newTicketNotification.TicketID = tComment.TicketID;
                newTicketNotification.Created = commentTimeStamp;
                newTicketNotification.UserID = assignedUserId;
                db.TicketNotifications.Add(newTicketNotification);
                //Save Ticket Notification
                db.SaveChanges();
            }
        }
        public void TicketAttachmentNotification(ApplicationUser formSubmitter, TicketAttachment ticketAttachment, DateTimeOffset commentTimeStamp)
        {
            var assignedUserId = (db.TicketPosts.Find(ticketAttachment.TicketID).AssignedToUser != null) ? db.TicketPosts.Find(ticketAttachment.TicketID).AssignedToUser.Id : "Unassigned";

            if (assignedUserId != "Unassigned")
            {
                //Notify Assignee
                var newTicketNotification = new TicketNotification();
                newTicketNotification.Notification =
                    " Has added an Attachment on Ticket # " + ticketAttachment.TicketID + ".";
                newTicketNotification.TriggeredByUserId = formSubmitter.Id;
                newTicketNotification.TicketID = ticketAttachment.TicketID;
                newTicketNotification.Created = commentTimeStamp;
                newTicketNotification.UserID = assignedUserId;
                db.TicketNotifications.Add(newTicketNotification);
            }

            //Notify Owner
            var newTicketNotification2 = new TicketNotification();
            newTicketNotification2.Notification =
                " Has added an Attachment on Ticket # " + ticketAttachment.TicketID + ".";
            newTicketNotification2.TriggeredByUserId = formSubmitter.Id;
            newTicketNotification2.TicketID = ticketAttachment.TicketID;
            newTicketNotification2.Created = commentTimeStamp;
            newTicketNotification2.UserID = db.TicketPosts.Find(ticketAttachment.TicketID).OwnerUserID;
            db.TicketNotifications.Add(newTicketNotification2);
            //Save Ticket Notifications
            db.SaveChanges();
        }
        public void NewTicketNotification(Projects currentProject, ApplicationUser formSubmitter, DateTimeOffset timeStamp, TicketPost newticket)
        {
            //UserRoles Helper
            var userRolesHelper = new UserRolesHelper(db);
            var project = db.Projects.Find(currentProject.Id);
            var allProjectManagers = userRolesHelper.GetAllUsersInRole("Project Manager").ToList();
            var projectManagersInProject = allProjectManagers.Where(u => u.Projects.Contains(project)).ToList();
            //Notify all PMs that a new ticket has been created on their Project
            foreach (var item in projectManagersInProject)
            {
            var newTicketNotification = new TicketNotification();
            newTicketNotification.Notification =
                " Has added a New Ticket(#"+ newticket.Id+ ") on Project(#"+ currentProject.Id + "): " + currentProject.Name +
                ", Ticket Priority: " + GetPriorityName(newticket.TicketPriorityID);
            newTicketNotification.TriggeredByUserId = formSubmitter.Id;
            newTicketNotification.TicketID = newticket.Id;
            newTicketNotification.Created = timeStamp;
            newTicketNotification.UserID = item.Id;
            db.TicketNotifications.Add(newTicketNotification);
            //Save Ticket Notification
            db.SaveChanges();
            }
        }

        //
        //History ############################################################
        //

        //Assignment History
        public void AssignmentHistory(TicketPost oldTicket, TicketPost newTicket, DateTimeOffset ticketUpdatedTimeStamp, ApplicationUser ticketEditor, string propertyChanged)
        {
            var newHistory = new TicketHistory();
            newHistory.TicketId = oldTicket.Id;
            newHistory.UpdatedByUserId = ticketEditor.Id;
            newHistory.PropertyChanged = propertyChanged;
            newHistory.UpdatedTime = ticketUpdatedTimeStamp;
            newHistory.OldAndNewValues = propertyChanged +" was changed From: " + 
                ((oldTicket.AssignedToUserID != null) ? GetDisplayName(oldTicket.AssignedToUserID) : "Unassigned") 
                + "  To: " + GetDisplayName(newTicket.AssignedToUserID);
            db.TicketHistories.Add(newHistory);
            db.SaveChanges();

        }
        public void GenericHistory(TicketPost oldTicket, string oldPropertyName, string newPropertyName, DateTimeOffset ticketUpdatedTimeStamp, ApplicationUser ticketEditor, string propertyChanged)
        {
            var newHistory = new TicketHistory();
            newHistory.TicketId = oldTicket.Id;
            newHistory.UpdatedByUserId = ticketEditor.Id;
            newHistory.PropertyChanged = propertyChanged;
            newHistory.UpdatedTime = ticketUpdatedTimeStamp;
            newHistory.OldAndNewValues = propertyChanged + " was changed From: " +
                oldPropertyName
                + "  To: " + newPropertyName;
            db.TicketHistories.Add(newHistory);
            db.SaveChanges();

        }

        //Get Ticket Property Names
        public string GetPriorityName(int Id)
        {
            return db.TicketPriorities.Find(Id).Name;
        }
        public string GetStatusName(int Id)
        {
            return db.TicketStatuses.Find(Id).Name;
        }
        public string GetTypeName(int Id)
        {
            return db.TicketTypes.Find(Id).Name;
        }

        //Get User Property Data
        private string GetDisplayName(string userId)
        {
            var DisplayName = db.Users.Where(u => u.Id == userId).FirstOrDefault().DisplayName;
            return DisplayName;
        }
        public string GetTicketAssignedUserId(int ticketId)
        {
            var user = (db.TicketPosts.Find(ticketId).AssignedToUser != null) ? db.TicketPosts.Find(ticketId).AssignedToUser.Id : "Unassigned";
            return user;
        }


    }

}