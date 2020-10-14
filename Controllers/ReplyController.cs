using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Forum.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Forum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace Forum.Controllers
{
    public class ReplyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ThreadController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReplyController(ILogger<ThreadController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var count = _context.Threads.Count();
            var items = _context.Threads
                .Take(5)
                .Include(t => t.Author)
                .ToList();
            return View(items);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Body,ThreadId")] Reply reply)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            try
            {
                if (ModelState.IsValid)
                {
                    reply.CreatedDate = DateTime.Now;
                    reply.AuthorId = user.Id;
                    _context.Replies.Add(reply);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.ToString());
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists " +
                                             "see your system administrator.");
            }

            return View(thread);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}