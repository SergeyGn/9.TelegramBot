using System;
using System.Collections.Generic;
using System.Text;

namespace _9.TelegramBot
{
    class MessageFile
    {
        /// <summary>
        /// файл id
        /// </summary>
        public string FileId { get; }
        /// <summary>
        /// имя файла
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// прошедший путь пользоватателя
        /// </summary>
        public string FilePath { get; set; }

        public MessageFile(string FileId, string FileName, string FilePath)
        {
            this.FileId = FileId;
            this.FileName = FileName;
            this.FilePath = FilePath;
        }
    }
}
