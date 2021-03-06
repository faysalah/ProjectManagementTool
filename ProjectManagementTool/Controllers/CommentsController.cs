﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectManagementTool.Models;
using Microsoft.AspNet.Identity;

namespace ProjectManagementTool.Controllers
{
    public class CommentsController : Controller
    {
        private ProjectManagementToolEntities db = new ProjectManagementToolEntities();
        private ApplicationDbContext adb = new ApplicationDbContext();
        // GET: Comments
        public ActionResult Index(int? id)
        {
            //var comments = db.Comments.Include(c => c.Task);
            if (id == null)
                return RedirectToAction("Index","Tasks");
                
            var comments = from a in adb.Users.AsEnumerable()
                           join b in db.Comments.Where(x => x.TaskId == id)
                           on a.Id equals b.commenterUserId
                           select new CommentViewModel {
                              Id= b.Id ,
                              Comment = b.Comment1,
                              CommenterName = a.Name,
                              DateTime = b.DateTime
                           };
            Task task = db.Tasks.Where(x => x.Id == id).FirstOrDefault();
            ViewBag.Task = task;
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            ViewBag.TaskId = new SelectList(db.Tasks, "Id", "Name");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TaskId,Comment1,DateTime,commenterUserId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.commenterUserId = User.Identity.GetUserId();
                comment.DateTime = DateTime.Now;
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TaskId = new SelectList(db.Tasks, "Id", "UserId", comment.TaskId);
            return View(comment);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TaskId = new SelectList(db.Tasks, "Id", "UserId", comment.TaskId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TaskId,Comment1,DateTime,commenterUserId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TaskId = new SelectList(db.Tasks, "Id", "UserId", comment.TaskId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
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
