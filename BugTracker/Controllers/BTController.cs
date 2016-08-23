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
using System.IO;

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
            //UserRoles Helper
            var userRolesHelper = new UserRolesHelper(db);

            var pTickets= new DispTicketsVM();
            pTickets.TicketList = projects.Tickets.ToList();
            ViewData["TicketsCollection"] = projects.Tickets.ToList();

            //Get users in roles for project details data
            var allDevelopers = userRolesHelper.GetAllUsersInRole("Developer").OrderBy(u => u.DisplayName);
            var developersInProject = allDevelopers.Where(u => u.Projects.Contains(projects));
            var allProjectManagers = userRolesHelper.GetAllUsersInRole("Project Manager").OrderBy(u => u.DisplayName);
            var projectManagersInProject = allProjectManagers.Where(u => u.Projects.Contains(projects));
   

            var ProjectDetailsVM = new ProjectDetailsVM();
            ProjectDetailsVM.Project = projects;
            ProjectDetailsVM.DeveloperCount = developersInProject.Count();
            ProjectDetailsVM.ProjectManagers = projectManagersInProject.ToList();
            ProjectDetailsVM.TotalTickets = projects.Tickets.Count();
            ProjectDetailsVM.UnassignedCount = projects.Tickets.Where(t => t.AssignedToUser == null).Count();

            //Data for create ticket partial
            ViewBag.TicketPriorityID = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketTypeID = new SelectList(db.TicketTypes, "Id", "Name");

            return View(ProjectDetailsVM);
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

            //Get Comments for ticket
            var commentList = ticket.TicketComments.OrderByDescending(c => c.Created).ToList();
            ViewData["CommentList"] = (List<TicketComment>)commentList;

            //For Attachment Partial - May not need.
            ViewData["currentTicket"] = (TicketPost)ticket;

            return View(TicketPost);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTicket(TicketPost ticket)
        {
            //HelperMethod for Histories/Notifications
            var ticketCustomHelper = new TicketCustomHelper();
            //Variables
            var currentProject = db.Projects.Find(ticket.ProjectID);//Current Project from the database.
            var formSubmitter = db.Users.Find(User.Identity.GetUserId());//User that submitted form
            var timeStamp = DateTimeOffset.UtcNow;
            var newticket = ticket;

            //####Start Access Control Section####
            var allowed = false;//Controls Access

            if (User.IsInRole("Submitter") && currentProject.Users.Contains(formSubmitter))
            {
                allowed = true;
            }
            //####End Access Control Section####

            if(ModelState.IsValid && allowed == true)
            {
                newticket.OwnerUserID = formSubmitter.Id;
                newticket.TicketStatusID = 3;
                newticket.Created = timeStamp;
                db.TicketPosts.Add(newticket);
                db.SaveChanges();


                ticketCustomHelper.NewTicketNotification(currentProject, formSubmitter, timeStamp, newticket);
            }




            if (allowed == false)
            {
                ViewData["ErrorCode"] = User.Identity.Name + " Permission not granted, CreateTicket, Project:" + currentProject.Id;
                return RedirectToAction("Err403", "BT");
            }

            return RedirectToAction("ProjectDetails", "BT", new { id = currentProject.Id });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTicketForm([Bind(Include = "Id,Created,Updated,Title,Description,ProjectID,TicketStatusID,TicketTypeID,TicketPriorityID,OwnerUserID,AssignedToUserID")] TicketPost ticketPost)
        {
            if (ModelState.IsValid)
            {
                //HelperMethod for Histories/Notifications
                var ticketCustomHelper = new TicketCustomHelper();

                var currentTicket   = db.TicketPosts.Find(ticketPost.Id);//Current Ticket from the database.
                var editedTicket    = ticketPost;//Changes that were submitted through Form Post
                var allUsers        = db.Users;
                var ticketEditor    = db.Users.Find(User.Identity.GetUserId());//User that edited the ticket
                var ticketUpdatedTimeStamp = DateTimeOffset.UtcNow;
                var changesMade = false;//is used to control one final saved of all ticket edits
                var updateNoftication = false;//is used to control & send one generic edit message
                db.TicketPosts.Attach(currentTicket);//Sets currentTicket ready for changes.

                //####Start Access Control Section####
                var allowed = false;//Controls Access
                if (User.IsInRole("Admin"))
                {
                    allowed = true;
                }
                else if (User.IsInRole("Project Manager") && currentTicket.Project.Users.Contains(ticketEditor))
                {
                    allowed = true;
                }
                else if (User.IsInRole("Developer") && currentTicket.AssignedToUser != null)
                {
                    if (currentTicket.AssignedToUser == ticketEditor)
                    {
                        allowed = true;
                    }

                }
                else if (User.IsInRole("Submitter") && currentTicket.OwnerUser == ticketEditor)
                {
                    allowed = true;
                }
                //####End Access Control Section####

                //ONLY Editable Items   - Details/Overview of each section.
                //  AssignedToUserID    - PM Only, Creates History Entry, Sends Notification to new asignee & old assignee.
                //  TicketPriorityID    -  Creates History Entry, Sends generic Notification
                //  TicketTypeID        -  Creates History Entry, Sends generic Notification
                //  TicketStatusID      -  Creates History Entry, Sends generic Notification
                //  Description         -  Creates History Entry, Sends generic Notification
                //  Title               -  Creates History Entry, Sends generic Notification

                if (allowed)
                {
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
                        ticketCustomHelper.GenericHistory(currentTicket, currentTicket.TicketType.Name, editedPropertyName, ticketUpdatedTimeStamp, ticketEditor, "Ticket Type");//History
                        currentTicket.TicketTypeID = editedTicket.TicketTypeID;//Set Ticket Edit Change
                        changesMade = true;
                        updateNoftication = true;

                    }
                    if (currentTicket.TicketStatusID != editedTicket.TicketStatusID)
                    {
                        //Create Ticket History, Set Ticket Edit Change
                        var editedPropertyName = ticketCustomHelper.GetStatusName(editedTicket.TicketStatusID);//Get ticket priority name
                        ticketCustomHelper.GenericHistory(currentTicket, currentTicket.TicketStatus.Name, editedPropertyName, ticketUpdatedTimeStamp, ticketEditor, "Ticket Status");//History
                        currentTicket.TicketStatusID = editedTicket.TicketStatusID;//Set Ticket Edit Change
                        changesMade = true;
                        updateNoftication = true;
                    }
                    if (currentTicket.Description != editedTicket.Description)
                    {
                        //Create Ticket History, Set Ticket Edit Change
                        ticketCustomHelper.GenericHistory(currentTicket, currentTicket.Description, editedTicket.Description, ticketUpdatedTimeStamp, ticketEditor, "Ticket Description");//History
                        currentTicket.Description = editedTicket.Description;//Set Ticket Edit Change
                        changesMade = true;
                        updateNoftication = true;
                    }
                    if (currentTicket.Title != editedTicket.Title)
                    {
                        //Create Ticket History, Set Ticket Edit Change
                        ticketCustomHelper.GenericHistory(currentTicket, currentTicket.Title, editedTicket.Title, ticketUpdatedTimeStamp, ticketEditor, "Ticket Title");//History
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
                if (allowed == false)
                {
                    ViewData["ErrorCode"] = User.Identity.Name + " Permission not granted, TicketEditForm, Ticket: " + currentTicket.Id;
                    return RedirectToAction("Err403", "BT");
                }
            }

            return RedirectToAction("Ticket", "BT", new { id = ticketPost.Id });
        }

        //Ticket Attachment
        // POST: BT/AddTicketAttachment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTicketAttachment([Bind(Include = "TicketId,Description")] TicketAttachment ticketAttachment, HttpPostedFileBase file)
        {
            //HelperMethod for Histories/Notifications
            var ticketCustomHelper = new TicketCustomHelper();
            //Variables
            var currentTicket = db.TicketPosts.Find(ticketAttachment.TicketID);//Current Ticket from the database.
            var formSubmitter = db.Users.Find(User.Identity.GetUserId());//User that submitted form
            var timeStamp = DateTimeOffset.UtcNow;

            //####Start Access Control Section####
            var allowed = false;//Controls Access
            if (User.IsInRole("Admin"))
            {
                allowed = true;
            }
            else if (User.IsInRole("Project Manager") && currentTicket.Project.Users.Contains(formSubmitter))
            {
                allowed = true;
            }
            else if (User.IsInRole("Developer") && currentTicket.AssignedToUser != null)
            {
                if(currentTicket.AssignedToUser == formSubmitter)
                {
                    allowed = true;
                }
                
            }
            else if (User.IsInRole("Submitter") && currentTicket.OwnerUser == formSubmitter)
            {
                allowed = true;
            }
            //####End Access Control Section####

            if (file != null && file.ContentLength > 0 && file.ContentLength < 3000)
            {
                //check the file name to make sure its an image
                var ext = Path.GetExtension(file.FileName).ToLower();

                //if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
                //    ModelState.AddModelError("image", "Invalid Format.");
            }

            if (ModelState.IsValid && allowed == true)
            {
                if (file != null)
                {
                    //relative server path
                    var filePath = "/Uploads/";
                    // path on physical drive on server
                    var absPath = Server.MapPath("~" + filePath);
                    // file url for relative path
                    ticketAttachment.FilePath = filePath + file.FileName;
                    //save image to Uploads
                    file.SaveAs(Path.Combine(absPath, file.FileName));
                    ticketAttachment.FileURL = filePath + file.FileName;
                }

                db.TicketAttachments.Add(ticketAttachment);
                ticketAttachment.UserID = formSubmitter.Id;
                ticketAttachment.Created = timeStamp;
                db.SaveChanges();
                return RedirectToAction("Ticket", "BT", new { Id = ticketAttachment.TicketID });

            }
            if(allowed == false)
            {
                ViewData["ErrorCode"] = User.Identity.Name + " Permission not granted, Attachment, Ticket: " + currentTicket.Id;
                return RedirectToAction("Err403", "BT");
            }

            return RedirectToAction("Ticket", "BT", new { Id = ticketAttachment.TicketID });

        }
           

        //helper
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
        //START COMMENT SECTION
        // POST: BT/AddComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(TicketComment comment)
        {
            if (ModelState.IsValid && comment.CommentBody != null)
            {
                var commentor = db.Users.Find(User.Identity.GetUserId());
                var timeStamp = DateTimeOffset.UtcNow;
                //HelperMethod for Histories/Notifications
                var ticketCustomHelper = new TicketCustomHelper();
                
                //add data & save comment
                comment.Created = timeStamp;
                comment.UserID = commentor.Id;
                db.TicketComments.Add(comment);
                db.SaveChanges();

                //Create Notification
                ticketCustomHelper.TicketCommentNotification(commentor, comment,timeStamp);

                return RedirectToAction("Ticket", "BT", new { id = comment.TicketID });
            }
            return RedirectToAction("Ticket", "BT", new { id = comment.TicketID });
        }

        //
        //END COMMENT SECTION
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
