using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RawCodingChatApp.Database;
using RawCodingChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RawCodingChatApp.Views.ViewComponents
{
    public class RoomViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public RoomViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
        
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var chats = _context.ChatUsers
                .Include(x => x.Chat)
                .Where(cu => cu.UserId == userId 
                && cu.Chat.Type == ChatType.Room)
                .Select(x => x.Chat)
                .ToList();

            return View(chats);

        }
    }
}
