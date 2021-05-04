using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace RawCodingChatApp.Models
{
    public class User : IdentityUser
    {
        public ICollection<ChatUser> Chats { get; set; }
    }
}
