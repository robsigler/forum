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

namespace Forum.Controllers
{
    [Route("posts")]
    [Controller]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PostController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostController(ILogger<PostController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            var count = _context.Posts.Count();
            var items = _context.Posts
                .OrderBy(s => s.CreatedDate)
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
            [Bind("Subject,Body")] NewPostViewModel newPostViewModel)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var post = new Post();
            try
            {
                if (ModelState.IsValid)
                {
                    post.Subject = newPostViewModel.Subject;
                    post.CreatedDate = DateTime.Now;
                    post.AuthorId = user.Id;
                    post.Body = newPostViewModel.Body;
                    post.CreatedDate = DateTime.Now;
                    post.AuthorId = user.Id;
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
            return Redirect("/posts/" + post.ID);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        [HttpGet]
        [Route("{id:int}", Name = "GetPost")]
        public async Task<IActionResult> GetPost(int id)
        {
            var post = (await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(t => t.ID == id));
            return View("Get", post);
        }

        [Authorize]
        [HttpGet("{id:int}/reply")]
        public IActionResult GetNewReplyPage(int id)
        {
            var viewModel = new NewReplyViewModel();
            return View("Reply/Create", viewModel);
        }

        [Authorize]
        [HttpPost("{id:int}/reply")]
        [Route("{id:int}/reply", Name = "PostReply")]
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
                    _context.Posts.Add(post);
                    await _context.SaveChangesAsync();
                    return RedirectToRoute("GetThread", new { id = id });
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.ToString());
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists " +
                                             "see your system administrator.");
            }
            return Redirect("/posts");
        }
    }
}