using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System.Collections;
using BugTracker.Helpers;

namespace BugTracker.Controllers
{
    public class BTController : Controller
    {


        private ApplicationDbContext db = new ApplicationDbContext();

        //##########################################################################
        //BT Projects Start Section
        //List the logged in User's Assigned Projects
        // GET: BT/MyProjects
        public ActionResult MyProjects()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var myProjects = user.Projects.ToList().OrderBy(m => m.Name);
            if (myProjects.Count() < 1 )//Checks if ther are any projects are assigned.
            {
                string errcode = "Access Denied, MyProjects, No Projects. User:" + user.UserName.ToString();
                return RedirectToAction("Err403", "BT", new { errcode = errcode });
            }
            
            return View(myProjects);
        }
        //
        //Includes Projects Info & Tickets
        // GET: BT/ProjectDetails/5
        public async Task<ActionResult> ProjectDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = await db.Projects.FindAsync(id);
            if (projects == null)
            {
                return HttpNotFound();
            }

            var pTickets= new DispTicketsVM();
            //pTickets.TLTitleDesc = "Showing Tickets from " + projects.Name.ToString();
            pTickets.TicketList = projects.Tickets.ToList();
            ViewData["TicketsCollection"] = projects.Tickets.ToList();
            
            return View(projects);
        }
        //
        //BT Projects End Section
        //##########################################################################

        //##########################################################################
        //BT Tickets Start Section
        //GET: BT/MyTickets
        public ActionResult MyTickets()
        {
            var user = db.Users.Find(User.Identity.GetUserId());//Get User's Identity

            //Passing Model to Partial for owner tickets
            var ticketsOwner = db.TicketPosts.Where(x => x.OwnerUserID == user.Id).ToList();
            var DispTicketsVM = new DispTicketsVM();
            DispTicketsVM.TitleDesc = "Tickets which " + user.DisplayName + " is the Owner/Submitter.";
            DispTicketsVM.TicketList = ticketsOwner;
            ViewData["DispTicketsVM_Owner"] = DispTicketsVM;

            //Passing list of tickets to the partialview with ViewData
            var ticketsAssigned = db.TicketPosts.Where(x => x.AssignedToUserID == user.Id).ToList();
            ViewBag.TicketDisplayDescription = "Tickets Assigned to " + user.DisplayName;
            ViewData["TicketsCollection"] = ticketsAssigned;

            return View();
        }

        //
        //Ticket Details
        //GET: BT/Ticket
        public ActionResult Ticket(int Id)
        {
            //UserRoles Helper
            var userRolesHelper = new UserRolesHelper(db);
            //Get Ticket & Instantiate Model
            var ticket = db.TicketPosts.Find(Id);
            var TicketPost = new TicketPost();
            TicketPost = ticket;
            //Set Dropdowns for edit tab.
            ViewBag.TicketPriorityID = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityID);
            ViewBag.TicketStatusID = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusID);
            ViewBag.TicketTypeID = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeID);
            var allDevelopers = userRolesHelper.GetAllUsersInRole("Developer").OrderBy(u => u.DisplayName);
            var developersInProject = allDevelopers.Where(u => u.Projects.Contains(ticket.Project));
            string assignedUser;
            if (ticket.AssignedToUserID == null)
            {
                assignedUser = "Unassigned";
            }
            else
            {
                assignedUser = ticket.AssignedToUserID;
            }
            ViewBag.AssignedToUserID = new SelectList(developersInProject, "Id", "DisplayName", assignedUser);


            return View(TicketPost);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTicketForm([Bind(Include = "Id,Created,Updated,Title,Description,ProjectID,TicketStatusID,TicketTypeID,TicketPriorityID,OwnerUserID,AssignedToUserID")] TicketPost ticketPost)
        {
            if (ModelState.IsValid)
            {
                var ticketCustomHelper = new TicketCustomHelper();

                var currentTicket   = db.TicketPosts.Find(ticketPost.Id);//Current Ticket from the database.
                var editedTicket    = ticketPost;//Changes that were submitted through Form Post
                var allUsers        = db.Users;
                var ticketEditor    = db.Users.Find(User.Identity.GetUserId());//User that edited the ticket
                var ticketUpdatedTimeStamp = DateTimeOffset.UtcNow;
                var changesMade = false;//is used to control one final saved of all ticket edits
                var updateNoftication = false;//is used to control & send one generic edit message
                db.TicketPosts.Attach(currentTicket);//Sets currentTicket ready for changes.

                //All Editable Items
                //  AssignedToUserID
                //  TicketPriorityID
                //  TicketTypeID
                //  TicketStatusID       
                //  Description
                //  Title


                if (currentTicket.AssignedToUserID != editedTicket.AssignedToUserID && User.IsInRole("Project Manager"))
                {
                    //Create Ticket Assignment History, Create Ticket Assignment Notifications, Set ticket property change
                    ticketCustomHelper.AssignmentHistory(currentTicket, editedTicket, ticketUpdatedTimeStamp, ticketEditor, "Assignment");//History
                    ticketCustomHelper.AssignmentNotification(currentTicket, editedTicket, ticketUpdatedTimeStamp, ticketEditor);//Notification
                    currentTicket.AssignedToUserID = editedTicket.AssignedToUserID;//set ticket edit change
                    changesMade = true;
                }

                if (currentTicket.TicketPriorityID != editedTicket.TicketPriorityID)
                {
                    //Create Ticket History, Set Ticket Edit Change
                    var editedPropertyName = ticketCustomHelper.GetPriorityName(editedTicket.TicketPriorityID);//Get ticket priority name
                    ticketCustomHelper.GenericHistory(currentTicket, currentTicket.TicketPriority.Name, editedPropertyName, ticketUpdatedTimeStamp, ticketEditor, "Ticket Priority");
                    currentTicket.TicketPriorityID = editedTicket.TicketPriorityID;//Set Ticket Edit Change
                    changesMade = true;
                    updateNoftication = true;
                }
                if (currentTicket.TicketTypeID != editedTicket.TicketTypeID)
                {
                    //Create Ticket History, Set Ticket Edit Change
                    var editedPropertyName = ticketCustomHelper.GetTypeName(editedTicket.TicketTypeID);//Get ticket type name
                    ticketCustomHelper.GenericHistory(currentTicket,currentTicket.TicketType.Name, editedPropertyName, ticketUpdatedTimeStamp, ticketEditor, "Ticket Type");//History
                    currentTicket.TicketTypeID = editedTicket.TicketTypeID;//Set Ticket Edit Change
                    changesMade = true;
                    updateNoftication = true;

                }
                if (currentTicket.TicketStatusID != editedTicket.TicketStatusID)
                {
                    //Create Ticket History, Set Ticket Edit Change
                    var editedPropertyName = ticketCustomHelper.GetStatusName(editedTicket.TicketStatusID);//Get ticket priority name
                    ticketCustomHelper.GenericHistory(currentTicket,currentTicket.TicketStatus.Name, editedPropertyName, ticketUpdatedTimeStamp, ticketEditor, "Ticket Status");//History
                    currentTicket.TicketStatusID = editedTicket.TicketStatusID;//Set Ticket Edit Change
                    changesMade = true;
                    updateNoftication = true;
                }
                if (currentTicket.Description != editedTicket.Description)
                {
                    //Create Ticket History, Set Ticket Edit Change
                    ticketCustomHelper.GenericHistory(currentTicket,currentTicket.Description, editedTicket.Description, ticketUpdatedTimeStamp, ticketEditor, "Ticket Description");//History
                    currentTicket.Description = editedTicket.Description;//Set Ticket Edit Change
                    changesMade = true;
                    updateNoftication = true;
                }
                if (currentTicket.Title != editedTicket.Title)
                {
                    //Create Ticket History, Set Ticket Edit Change
                    ticketCustomHelper.GenericHistory(currentTicket,currentTicket.Title, editedTicket.Title, ticketUpdatedTimeStamp, ticketEditor, "Ticket Title");//History
                    currentTicket.Title = editedTicket.Title;//Set Ticket Edit Change
                    changesMade = true;
                    updateNoftication = true;
                }

                if (changesMade)
                {
                    //Sends Nofitication to Ticket Asignee. (Not when Assignee is changed)
                    //(Nofication for Asignee change is sent in the Assignment section above)
                    if (updateNoftication)
                    {
                        ticketCustomHelper.GenericTicketChangeNotification(currentTicket.AssignedToUserID, ticketEditor, currentTicket.Id, ticketUpdatedTimeStamp);
                    }
                    //Set time/editor, and save changes.
                    currentTicket.UpdatedByUserID = ticketEditor.Id;
                    currentTicket.Updated = ticketUpdatedTimeStamp;
                    db.SaveChanges();
                }
                return RedirectToAction("Ticket", "BT", new { id = currentTicket.Id });
            }

            return RedirectToAction("Ticket", "BT", new { id = ticketPost.Id });
        }

        //Gets Display Name
        private string GetDisplayName(string userId)
        {
            string DisplayName = db.Users.FirstOrDefault(u => u.Id == userId).DisplayName;
            return DisplayName;
        }

        //
        //BT Tickets End Section
        //#########################################################################

        //#########################################################################
        //START PROJECT MANAGER SECTION
        // GET: BT/GlobalTickets
        public ActionResult GlobalTickets()
        {
            if(User.IsInRole("Registered User"))
            {
                return RedirectToAction("FP403Error", "Error");
            }
            //Get All Tickets, Sorted by Created Date
                var allTicketsList = db.TicketPosts.ToList().OrderByDescending(m => m.Created).ToList();
                var DispTicketsVM_All = new DispTicketsVM();
                DispTicketsVM_All.TitleDesc = "All Tickets, Sorted by Created Date/Time";
                DispTicketsVM_All.TicketList = allTicketsList;
                ViewData["DispTicketsVM_AllTickets"] = DispTicketsVM_All;


                return View();
        }
        //
        //
        //GET : BT/EditProjects()
        public ActionResult EditProjectUsers(int id)
        {
            var EPBigVM = new EPBigVM();
            EPBigVM.Project = db.Projects.Find(id);
            EPBigVM.UsersAll = db.Users.ToList();
            EPBigVM.UsersInProject = EPBigVM.Project.Users.ToList();
            EPBigVM.UsersNotInProject = db.Users.ToList().Except(EPBigVM.UsersInProject).ToList();
            EPBigVM.SelectedRemoveUsers = EPBigVM.Project.Users.ToList();
            List<EPSelectedVM> list = new List<EPSelectedVM>();
            var notInProject = db.Users.ToList().Except(EPBigVM.UsersInProject).ToArray();

            for (var i = 0; i < notInProject.Length; i++)
            {
                list.Add(new EPSelectedVM() { UserDisplayName = notInProject[i].DisplayName, UserEmail = notInProject[i].Email, UserId = notInProject[i].Id, IsChecked = false });
            }
            List<EPRMSelectedVM> listRM = new List<EPRMSelectedVM>();
            var inProject = db.Projects.Find(id).Users.ToArray();
            for (var i = 0; i < inProject.Length; i++)
            {
                listRM.Add(new EPRMSelectedVM() { UserDisplayName = inProject[i].DisplayName, UserEmail = inProject[i].Email, UserId = inProject[i].Id, IsChecked = false });
            }
            //Not in Project
            EPBigVM.EPSelectedListVM = new EPSelectedListVM();
            EPBigVM.EPSelectedListVM.Users = list;
            //In Project
            EPBigVM.EPRMSelectedListVM = new EPRMSelectedListVM();
            EPBigVM.EPRMSelectedListVM.Users = listRM;

            //EPBigVM.EPSelectedVM = not needed?
            ViewBag.Title = ViewData["ProjectName"] = EPBigVM.Project.Name;
            ViewData["CurrentProject"] = id;
            return View(EPBigVM);
        }
        //Add users to project
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProjectAddUser(EPSelectedListVM model, int Id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditProjectUsers", "BT", new { id = Id });
            }

            var currentProject = db.Projects.Find(Id);
            var selectedUsers = model.Users.Where(u => u.IsChecked.Equals(true)).ToList();//get's only users that were checked.
            var allUsers = db.Users;
            for (var i = 0; i < selectedUsers.Count(); i++)
            {
                var user = allUsers.Find(selectedUsers[i].UserId);//get current user.
                currentProject.Users.Add(user);
            }
            db.Entry(currentProject).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("EditProjectUsers", "BT", new { id = Id });
        }

        //
        //
        //Remove users from Project
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProjectRMUser(EPRMSelectedListVM model, int Id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("EditProjectUsers", "BT", new { id = Id });
            }

            var currentProject = db.Projects.Find(Id);
            var selectedUsers = model.Users.Where(u => u.IsChecked.Equals(true)).ToList();//get's only users that were checked.
            var allUsers = db.Users;
            for (var i = 0; i < selectedUsers.Count(); i++)
            {
                var user = allUsers.Find(selectedUsers[i].UserId);//get current user model.
                currentProject.Users.Remove(user);
            }
            db.Entry(currentProject).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("EditProjectUsers", "BT", new { id = Id });
        }
        //
        //
        //END PROJECT MANAGER SECTION
        //#########################################################################

        //#########################################################################
        //Error Pages Section
        //403 Forbidden
        public ActionResult Err403(string errcode)
        {
            ViewData["ErrorCode"] = errcode;
            return View();
        }


    }
}
