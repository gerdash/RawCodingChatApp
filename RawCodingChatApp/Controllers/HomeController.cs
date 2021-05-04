using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RawCodingChatApp.Database;
using RawCodingChatApp.Infrastructure;
using RawCodingChatApp.Infrastructure.Repository;
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
        private readonly IChatRepository _repo;

        public HomeController(ILogger<HomeController> logger, IChatRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public IActionResult Index()
        {
            var chats = _repo.GetChats(GetUserId());
            return View(chats);
        }

        public IActionResult Find([FromServices] AppDbContext context)
        {
            var users = context.Users
                .Where(u => u.Id != User.GetUserId())
                .ToList();

                return View(users);
        }

        public IActionResult Private()
        {
            var chats = _repo.GetPrivateChats(GetUserId());

            return View(chats);
        }

        public async Task<IActionResult> CreatePrivateRoom(string userId)
        {
            var id = await _repo.CreatePrivateRoom(GetUserId(), userId);
            return RedirectToAction("Chat", new { id });
        }

        [HttpGet("{id}")]
       public IActionResult Chat(int id)
        {
            var chat = _repo.GetChat(id);
            return View(chat);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(string name)
        {
            await _repo.CreateRoom(name, GetUserId());
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> JoinRoom(int id)
        {

            await _repo.JoinRoom(id, GetUserId());
            return RedirectToAction("Chat", "Home", new { id = id});

        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
