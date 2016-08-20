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
                newTicketNotification.Notification = GetDisplayName(ticketEditor.Id) 
                    + " reassigned Ticket # " + oldTicket.Id + " to " + GetDisplayName(editedTicket.AssignedToUserID);
                newTicketNotification.TicketID = oldTicket.Id;
                newTicketNotification.Created = ticketUpdatedTimeStamp;
                newTicketNotification.UserID = oldTicket.AssignedToUserID;
                db.TicketNotifications.Add(newTicketNotification);
            }

            //Notification for new Assignee
            var newTicketNotification2 = new TicketNotification();
            newTicketNotification2.Notification = GetDisplayName(ticketEditor.Id)
                + " assigned Ticket # " + oldTicket.Id + " to you.";
            newTicketNotification2.TicketID = oldTicket.Id;
            newTicketNotification2.Created = ticketUpdatedTimeStamp;
            newTicketNotification2.UserID = editedTicket.AssignedToUserID;
            db.TicketNotifications.Add(newTicketNotification2);
            //Save Ticket Notification(s)
            db.SaveChanges();
        }
        public void GenericTicketChangeNotification(string ticketAsigneeId , ApplicationUser ticketEditor, int ticketId, DateTimeOffset ticketUpdatedTimeStamp)
        {
            var newTicketNotification = new TicketNotification();
            newTicketNotification.Notification = ticketEditor.DisplayName +
                " has made changes to Ticket # " + ticketId + ".";
            newTicketNotification.TicketID = ticketId;
            newTicketNotification.Created = ticketUpdatedTimeStamp;
            newTicketNotification.UserID = ticketAsigneeId;
            db.TicketNotifications.Add(newTicketNotification);
            //Save Ticket Notification
            db.SaveChanges();

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
        private string GetDisplayName(string userId)
        {
            var DisplayName = db.Users.Where(u => u.Id == userId).FirstOrDefault().DisplayName;
            return DisplayName;
        }

    }

}