using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using static _9.TelegramBot.MessageFile;

namespace _9.TelegramBot
{
    class Program
    {
        static TelegramBotClient bot;
        static string pathContent = @"Content\";
        static string pathDocuments = @"Content\Documents\";
        static string pathImages = @"Content\Photo\";
        static string pathSounds = @"Content\Sounds\";
        static string pathVideo = @"Content\Video\";
        static Dictionary<long, User> listUsers=new Dictionary<long, User>();
        static void Main(string[] args)
        {
            string token = "1887811869:AAHxziYTVufs3VTuox-pX735Z6SbDIvkP08";

            bot = new TelegramBotClient(token);
   
            bot.OnMessage += MessageListener;
            bot.StartReceiving();
            Console.ReadKey();
        }

        private static void MessageListener(object sender, MessageEventArgs e)
        {
            if(!listUsers.ContainsKey(e.Message.Chat.Id))
            { 
                listUsers.Add(e.Message.Chat.Id, new User(e.Message.Chat.Id, ""));
            }
            User user = listUsers[e.Message.Chat.Id];
            string currentMessage = e.Message.Text;
            string backMessage = user.LostMessage;
            switch (user.LostMessage)
            {
                case "/show":

                    string answer = CheckNumber(e.Message.Text, pathContent, e);
                    if(answer=="hueta")
                    {
                        return;
                    }
                    else
                    {
                        user.LostMessage = currentMessage;
                        GetList(answer, e);
                        return;
                    }
                    
            }

            user.LostMessage = currentMessage;

            MessageFile file;
            string text = $"{DateTime.Now.ToLongTimeString()}:{e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";
            Console.WriteLine($"{text} TypeMessage: {e.Message.Type.ToString()}");

            switch (e.Message.Type)
            {
                case Telegram.Bot.Types.Enums.MessageType.Document:
                    Console.WriteLine($"{e.Message.Document.FileName} {e.Message.Document.MimeType}");
                    file = new MessageFile(e.Message.Document.FileId, e.Message.Document.FileName, $"{pathDocuments}{e.Message.Document.FileName}");
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Video:
                    Console.WriteLine($"{e.Message.Video.FileName} {e.Message.Video.MimeType}");
                    file = new MessageFile(e.Message.Video.FileId,$"{e.Message.Video.FileId}{e.Message.Video.FileName}[{e.Message.Chat.FirstName}][{DateTime.Now.ToLongDateString()}].mp4", pathVideo);
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Photo:
                    var result = e.Message.Photo[0];
                    foreach (var sizeFile in e.Message.Photo)
                    {
                        if (result.FileSize < sizeFile.FileSize)
                        {
                            result = sizeFile;
                        }
                    }
                    file = new MessageFile(result.FileId, $"[{e.Message.Chat.FirstName}][{DateTime.Now.ToLongDateString()}]{result.FileUniqueId}.jpg", pathImages);
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Audio:
                    Console.WriteLine($"{e.Message.Audio.FileName} {e.Message.Audio.MimeType}");
                    file = new MessageFile(e.Message.Audio.FileId,
                        $"{e.Message.Audio.FileName}[{e.Message.Chat.FirstName}][{DateTime.Now.ToLongDateString()}].mp3",
                        pathSounds);
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Text:
             
                        switch (e.Message.Text)
                    {
                        case "/help":
                            bot.SendTextMessageAsync(e.Message.Chat.Id, $"Это бот файлообменник. Загрузи что-нибудь от сюда или сохрани сюда");
                            break;
                        case "/start":
                            bot.SendTextMessageAsync(e.Message.Chat.Id, $"Это бот файлообменник. Загрузи что-нибудь от сюда или сохрани сюда");
                            break;
                        case "/show":
                            GetList(pathContent, e);
                            break;
                        default:
                            bot.SendTextMessageAsync(e.Message.Chat.Id, "я не понимаю твою команду введи /help и я подскажу тебе");
                            break;
                    }
                    return;
                default:
                    bot.SendTextMessageAsync(e.Message.Chat.Id, $"Это что шутка? Я тут для загрузки и показа что загрузили");
                    return;
            }

                Download(file.FileId, $"{file.FilePath}{file.FileName}");

                bot.SendTextMessageAsync(e.Message.Chat.Id, $"файл загружен");
            if (e.Message.Text == null) return;
        }

        static async void Download(string fileId, string path)  //скачать на компьютер
        {
            var file = await bot.GetFileAsync(fileId);
            FileStream fs = new FileStream(path, FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();
            fs.Dispose();

        }

        static void GetList(string path, MessageEventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            List<string> ListItem = new List<string>();
            string answer;
            //string pathReturn;
            foreach (var item in di.GetDirectories())
            {
                ListItem.Add(item.Name);
            }
            answer = $"[1]{ListItem[0]}";
            for (int i = 1; i < ListItem.Count; i++)
            {
                answer += $"\n[{i + 1}]{ListItem[i]}";
            }
            bot.SendTextMessageAsync(e.Message.Chat.Id, answer);
            return;
            //int result = CheckNumber(e.Message.Text, 0, ListItem.Count, e);
            //pathReturn = $"{path}/{ListItem[result]}";
            //return pathReturn;     
        }
        static public string CheckNumber(string text, string path, MessageEventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            List<string> ListItem = new List<string>();
            foreach (var item in di.GetDirectories())
            {
                ListItem.Add(item.Name);
            }
            //int result = CheckNumber(e.Message.Text, 0, ListItem.Count, e);
            //pathReturn = $"{path}/{ListItem[result]}";
            if (int.TryParse(text,out int result))
            {
                result -= 1;
                if (result >= 0 && result < ListItem.Count)
                {
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "правильный ввод");
                    string returnPath = $"{path}{ListItem[result]}";
                    returnPath=returnPath.Replace(@"\\",@"\");
                    return returnPath;
                }
                //else
                //{
                //    CheckNumber(e.Message.Text, min, max, e);
                //    return 0;
                //}
            }
            bot.SendTextMessageAsync(e.Message.Chat.Id, "неправильный ввод");
            return "hueta";
        }
        static async void Loading(string fileId, string path)

        {
            FileStream fs = new FileStream(path, FileMode.Open);
            await bot.DownloadFileAsync(path, fs);
            fs.Close();
            fs.Dispose();
        }
    }
}
