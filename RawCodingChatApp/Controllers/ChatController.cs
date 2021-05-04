using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RawCodingChatApp.Database;
using RawCodingChatApp.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RawCodingChatApp.Models;
using RawCodingChatApp.Infrastructure.Repository;
using RawCodingChatApp.Infrastructure;

namespace RawCodingChatApp.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ChatController : BaseController
    {
        private IHubContext<ChatHub> _chat;
        public ChatController(IHubContext<ChatHub> chat)
        {
            _chat = chat;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessage(
            string message, 
            int roomId,
            [FromServices] IChatRepository repo)
        {
            var Message = await repo.CreateMessage(roomId, message, User.Identity.Name);

            await _chat.Clients.Group(roomId.ToString())
                .SendAsync("ReceiveMessage", new { 
                Text = Message.Text,
                Name = Message.Name,
                TimeStamp = Message.TimeStamp.ToString("dd/MM/yyyy hh:mm:ss")
                });

            return Ok();
        }
    }
}
