using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    public class TicketController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Ticket
        public ActionResult Index()
        {
            var post = db.TicketPosts.Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.UpdatedByUser);
            return View(post.ToList());
        }

        // GET: Ticket/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketPost ticketPost = db.TicketPosts.Find(id);
            if (ticketPost == null)
            {
                return HttpNotFound();
            }
            return View(ticketPost);
        }

        // GET: Ticket/Create
        public ActionResult Create(int? projectId)
        {
            //ViewBag.OwnerUserID = new SelectList(db.Users, "Id", "FirstName");
            //ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name");
            ViewBag.TicketPriorityID = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusID = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeID = new SelectList(db.TicketTypes, "Id", "Name");
            //ViewBag.UpdatedByUserID = new SelectList(db.Users, "Id", "FirstName");
            if (projectId != null)
            {
                TicketPost model = new TicketPost();
                model.ProjectID = (int)projectId;
                return View(model);
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            string errcode = "Access Denied, TicketCreate, No ProjectId was present. User:" + user.UserName.ToString();
            return RedirectToAction("Err403", "BT", new { errcode = errcode });

        }

        // POST: Ticket/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Status,Created,Updated,Title,Description,ProjectID,TicketStatusID,TicketTypeID,TicketPriorityID,OwnerUserID,AssignedToUserID,UpdatedByUserID")] TicketPost ticketPost)
        {
            if (!ModelState.IsValid)
            {

                return View(ticketPost);
            }
            ticketPost.Created = DateTimeOffset.Now;
            ticketPost.OwnerUserID = User.Identity.GetUserId();
            ticketPost.TicketStatusID = 3;
            db.TicketPosts.Add(ticketPost);
            db.SaveChanges();
                
            return RedirectToAction("ProjectDetails", "BT", new { id = ticketPost.ProjectID } );
        }

        // GET: Ticket/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketPost ticketPost = db.TicketPosts.Find(id);
            if (ticketPost == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerUserID = new SelectList(db.ApplicationUsers, "Id", "FirstName", ticketPost.OwnerUserID);
            ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", ticketPost.ProjectID);
            ViewBag.TicketPriorityID = new SelectList(db.TicketPriorities, "Id", "Name", ticketPost.TicketPriorityID);
            ViewBag.TicketStatusID = new SelectList(db.TicketStatuses, "Id", "Name", ticketPost.TicketStatusID);
            ViewBag.TicketTypeID = new SelectList(db.TicketTypes, "Id", "Name", ticketPost.TicketTypeID);
            ViewBag.UpdatedByUserID = new SelectList(db.ApplicationUsers, "Id", "FirstName", ticketPost.UpdatedByUserID);
            return View(ticketPost);
        }

        // POST: Ticket/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Status,Created,Updated,Title,Description,ProjectID,TicketStatusID,TicketTypeID,TicketPriorityID,OwnerUserID,AssignedToUserID,UpdatedByUserID")] TicketPost ticketPost)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerUserID = new SelectList(db.ApplicationUsers, "Id", "FirstName", ticketPost.OwnerUserID);
            ViewBag.ProjectID = new SelectList(db.Projects, "Id", "Name", ticketPost.ProjectID);
            ViewBag.TicketPriorityID = new SelectList(db.TicketPriorities, "Id", "Name", ticketPost.TicketPriorityID);
            ViewBag.TicketStatusID = new SelectList(db.TicketStatuses, "Id", "Name", ticketPost.TicketStatusID);
            ViewBag.TicketTypeID = new SelectList(db.TicketTypes, "Id", "Name", ticketPost.TicketTypeID);
            ViewBag.UpdatedByUserID = new SelectList(db.ApplicationUsers, "Id", "FirstName", ticketPost.UpdatedByUserID);
            return View(ticketPost);
        }

        // GET: Ticket/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketPost ticketPost = db.TicketPosts.Find(id);
            if (ticketPost == null)
            {
                return HttpNotFound();
            }
            return View(ticketPost);
        }

        // POST: Ticket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketPost ticketPost = db.TicketPosts.Find(id);
            db.TicketPosts.Remove(ticketPost);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
