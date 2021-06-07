using System;
using System.Collections.Generic;
using System.Text;

namespace _9.TelegramBot
{
    class User
    {
        public long Id { get; set; }
        public string LostMessage { get; set; }

       public User(long id,string lostMessage)
        {
            Id = id;
            LostMessage = lostMessage;
        }
    }
}
