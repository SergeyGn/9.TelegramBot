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
        public string FileId { get => _fileId;}
        /// <summary>
        /// имя файла
        /// </summary>
        public string FileName { get => _fileName; set => _fileName = value; }
        public string FilePath { get => _filePath; set => _filePath = value; }

        private string _fileId;
        private string _fileName;
        private string _filePath;

       public MessageFile(string FileId, string FileName, string FilePath)
        {
            _fileId = FileId;
            _fileName = FileName;
            _filePath = FilePath;
        }
    }
}
