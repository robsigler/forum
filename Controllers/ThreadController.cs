using System;
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
    [Route("threads")]
    [Controller]
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

        [Authorize]
        [HttpGet]
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
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost("create")]
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
            return Redirect("/threads/" + thread.ID);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        [Authorize]
        [HttpGet]
        [Route("{id:int}", Name="GetThread")]
        public async Task<IActionResult> GetThread(int id)
        {
            var thread = (await _context.Threads.Include(t => t.Posts).ThenInclude(p => p.Author).FirstOrDefaultAsync(t => t.ID == id));
            return View("Get", thread);
        }

        [Authorize]
        [HttpGet("{id:int}/reply")]
        public IActionResult GetNewReplyPage(int id)
        {
            var viewModel = new NewReplyViewModel();
            viewModel.ThreadId = id;
            return View("Reply/Create", viewModel);
        }

        [Authorize]
        [HttpPost("{id:int}/reply")]
        [Route("{id:int}/reply", Name="PostReply")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostReply(
            int id,
            [Bind("Body")] NewReplyViewModel newReplyViewModel)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            try
            {
                if (ModelState.IsValid)
                {
                    var post = new Post();
                    post.Body = newReplyViewModel.Body;
                    post.CreatedDate = DateTime.Now;
                    post.AuthorId = user.Id;
                    post.ThreadId = id;
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
    }
}