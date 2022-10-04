using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.Data;
using NatuurlikBase.Models;

namespace NatuurlikBase.Controllers;

[Authorize(Roles = SR.Role_Admin)]
public class VideoController : Controller
{
    private readonly DatabaseContext db;
    public VideoController(DatabaseContext _db)
    {
        db = _db;
    }
    public IActionResult Index()
    {
        var videos = db.Video.ToList();
        return View(videos);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("Id,Title,VideoUrl")] Video video)
    {
        if (ModelState.IsValid)

        {
            if (db.Video.Any(c => c.Title == video.Title && c.VideoUrl == video.VideoUrl))
            {
                ViewBag.Error = "This video already exists!";
            }

            else

            {
                db.Video.Add(video);
                db.SaveChanges();

                TempData["success"] = "Video successfully added.";

                return RedirectToAction("Index");
            }

        }

        else if (!ModelState.IsValid)

        {
            ViewBag.modal = "invalid.";

        }
        return View(video);
    }

    public IActionResult Edit(int? id)
    {

        if (id == null)
        {
            return NotFound();
        }

        Video video = db.Video.Find(id);
        if (video == null)
        {
            return NotFound();
        }

        return View(video);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit([Bind("Id,Title,VideoUrl")] Video video)
    {
        if (ModelState.IsValid)

        {

            if (db.Video.Any(c => c.Title == video.Title && c.VideoUrl == video.VideoUrl))
            {
                ViewBag.Error = "Video already exists!";

            }
            else
            {
                db.Entry(video).State = EntityState.Modified;
                TempData["success"] = "Video Successfully Updated.";
                ViewBag.Confirm = "Confirm Video Details";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        return View(video);
    }

    public IActionResult Delete(int? id)
    {

        if (id == null)
        {
            return NotFound();
        }

        Video video = db.Video.Find(id);
        if (video == null)
        {
            return NotFound();
        }

        return View(video);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        Video video = db.Video.Find(id);
        db.Video.Remove(video);
        ViewBag.WriteOffReasonConfirmation = "Are you sure you want to delete this video?";
        TempData["success"] = "Video deleted successfully!";
        db.SaveChanges();
        return RedirectToAction("Index");

    }
}