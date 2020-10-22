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
    public class ThreadController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ThreadController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ThreadController(ILogger<ThreadController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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
            [Bind("Subject,Body")] NewThreadViewModel newThreadViewModel)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var thread = new Thread();
            try
            {
                if (ModelState.IsValid)
                {
                    thread.Subject = newThreadViewModel.Subject;
                    thread.CreatedDate = DateTime.Now;
                    thread.AuthorId = user.Id;
                    _context.Threads.Add(thread);
                    await _context.SaveChangesAsync();
                    // TODO: handle situation where thread gets created but post does not
                    var post = new Post();
                    post.Body = newThreadViewModel.Body;
                    post.CreatedDate = DateTime.Now;
                    post.AuthorId = user.Id;
                    post.ThreadId = thread.ID;
                    _context.Posts.Add(post);
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
            return Redirect("/threads");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        [Authorize]
        public async Task<IActionResult> Get(int? id)
        {
            var thread = (await _context.Threads.Include(t => t.Posts).ThenInclude(p => p.Author).FirstOrDefaultAsync(t => t.ID == id));
            return View(thread);
        }
    }
}