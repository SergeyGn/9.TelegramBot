using System;
using System.Collections.Generic;
using System.Text;

namespace _9.TelegramBot
{
    class User
    {
        /// <summary>
        /// id пользователя
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// последнее сообщение пользователя
        /// </summary>
        public string LostMessage { get; set; }
        /// <summary>
        /// сохранненый путь пользователя
        /// </summary>
        public string PathSave { get; set; }

       public User(long Id,string LostMessage)
        {
            this.Id = Id;
            this.LostMessage = LostMessage;
        }
    }
}
