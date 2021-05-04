using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RawCodingChatApp.Database;
using RawCodingChatApp.Infrastructure;
using RawCodingChatApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RawCodingChatApp.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var chats = _context.Chats
                .Include(x => x.Users)
                .Where(x => !x.Users
                .Any(y => y.UserId == GetUserId()))
                .ToList();
            return View(chats);
        }

        public IActionResult Find()
        {
            var users = _context.Users
                .Where(u => u.Id != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                .ToList();

                return View(users);
        }

        public IActionResult Private()
        {
            var chats = _context.Chats
                .Include(x => x.Users)
                .ThenInclude(x => x.User)
                .Where(x => x.Type == ChatType.Private
                && x.Users.Any(y => y.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value))
                .ToList();

            return View(chats);
        }

        public async Task<IActionResult> CreatePrivateRoom(string userId)
        {
            var chat = new Chat
            {
                Type = ChatType.Private
            };

            chat.Users.Add(new ChatUser
                {
                    UserId = userId
                });

            _context.Chats.Add(chat);

            await _context.SaveChangesAsync();
            return RedirectToAction("Chat", new { id = chat.Id});
        }

        [HttpGet("{id}")]
       public IActionResult Chat(int id)
        {
            var chat = _context.Chats
                .Include(c => c.Messages)
                .FirstOrDefault(c => c.Id == id);
            return View(chat);
        }

      

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int chatId, string message)
        {
            var Message = new Message
            {
                ChatId = chatId,
                Text = message,
                Name = User.Identity.Name,
                TimeStamp=DateTime.Now
            };

            _context.Messages.Add(Message);
            await _context.SaveChangesAsync();
            return RedirectToAction("Chat", new { id = chatId});
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(string name)
        {
            var chat = new Chat
            {
                Name = name,
                Type = ChatType.Room
            };

            chat.Users.Add(new ChatUser {

                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Role = UserRole.Admin
            }); 

            _context.Chats.Add(chat);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> JoinRoom(int id)
        {
            var chatUser = new ChatUser
            {
                ChatId = id,

                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                Role = UserRole.Member
            };

            _context.ChatUsers.Add(chatUser);

            await _context.SaveChangesAsync();

            return RedirectToAction("Chat", "Home", new { id = id});

        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
