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
    public class UserRolesHelper
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        public UserRolesHelper(ApplicationDbContext context)
        {
            this.userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            this.roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            this.db = context;
        }
        public bool IsUserInRole(string userId, string roleName)
        {
            return userManager.IsInRole(userId, roleName);
        }
        public IList<string> ListUserRoles(string userId)
        {
            return userManager.GetRoles(userId);
        }
        public IList<string> ListAbsentUserRoles(string userId)
        {
            var roles = db.Roles.Select(r => r.Name).ToList();
            var AbsentUserRoles = new List<string>();
            foreach (var role in roles)
            {
                if (!IsUserInRole(userId, role))
                {
                    AbsentUserRoles.Add(role);
                }
            }

            return AbsentUserRoles;
        }
        public bool AddUserToRole(string userId, string roleName)
        {
            var result = userManager.AddToRole(userId, roleName);
            return result.Succeeded;
        }
        public bool RemoveUserFromRole(string userId, string roleName)
        {
            var result = userManager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }
        public IList<ApplicationUser> UsersInRole (string roleName)
        {
            var userIDs = roleManager.FindByName(roleName).Users.Select(r => r.UserId);
            return userManager.Users.Where(u => userIDs.Contains(u.Id)).ToList();
        }
        public IList<ApplicationUser> GetAllUsersInRole(string roleName)
        {
            var userIDs = roleManager.FindByName(roleName).Users.Select(r => r.UserId);
            return userManager.Users.Where(u => userIDs.Contains(u.Id)).ToList();
        }

        public IList<ApplicationUser> UsersNotInRole(string roleName)
        {
            var userIDs = System.Web.Security.Roles.GetUsersInRole(roleName);
            return userManager.Users.Where(u => !userIDs.Contains(u.Id)).ToList();
        }
    }

    public class ProjectAssignmentHelper
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        public ProjectAssignmentHelper(ApplicationDbContext context)
        {
            this.userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            this.roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            this.db = context;
        }
        public void AssignUser(string userId, int projectId)
        {
            //    if (!HasProject(userId, projectId))
            //    {
            //        var user = db.Users.Find(userId);
            //        var project = db.Project.Find(projectId);
            //        project.Users.Add(user);
            //    }
            //}
            //public bool HasProject(string userId, int projectId)
            //{
            //    var user = userId;
            //    var project = projectId;
            //    var result = db.ProjectUser.Find(user).where(project);
            //    return true;
            //}

        }
    }
}