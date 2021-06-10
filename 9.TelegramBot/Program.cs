using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using static _9.TelegramBot.MessageFile;

namespace _9.TelegramBot
{
    class Program
    {
        private static TelegramBotClient _bot;
        private static string _pathContent = @"Content\";
        private static string _pathDocuments = @"Content\Documents\";
        private static string _pathImages = @"Content\Photo\";
        private static string _pathSounds = @"Content\Sounds\";
        private static string _pathVideo = @"Content\Video\";
        private static Dictionary<long, User> _listUsers = new Dictionary<long, User>();
        static void Main(string[] args)
        {
            string token = "1887811869:AAHxziYTVufs3VTuox-pX735Z6SbDIvkP08";
            if (!Directory.Exists(_pathContent))
            {
                Directory.CreateDirectory(_pathContent);
                Directory.CreateDirectory(_pathDocuments);
                Directory.CreateDirectory(_pathImages);
                Directory.CreateDirectory(_pathSounds);
                Directory.CreateDirectory(_pathVideo);
            }
            _bot = new TelegramBotClient(token);

            _bot.OnMessage += MessageListener;
            _bot.StartReceiving();
            Console.ReadKey();
        }

        private static void MessageListener(object sender, MessageEventArgs e) //обработчик сообщений
        {
            if (!_listUsers.ContainsKey(e.Message.Chat.Id))
            {
                _listUsers.Add(e.Message.Chat.Id, new User(e.Message.Chat.Id, ""));
            }
            User user = _listUsers[e.Message.Chat.Id];
            string СurrentMessage = e.Message.Text;
            string Path = user.PathSave;
            switch (user.LostMessage)
            {
                case "/show":
                    string answer = CheckNumber(e.Message.Text, _pathContent, e);
                    if (answer == "null")
                    {
                        return;
                    }
                    else
                    {
                        user.LostMessage = СurrentMessage;
                        GetDirectoryFolder(answer, e);
                        user.PathSave = answer;
                        return;
                    }
                default:
                    if (user.PathSave != null)
                    {
                        answer = CheckNumber(e.Message.Text, Path, e);
                        Load(e.Message.Chat.Id, answer);
                        user.PathSave = null;
                        return;
                    }
                    break;
            }

            user.LostMessage = СurrentMessage;

            MessageFile File;
            string Text = $"{DateTime.Now.ToLongTimeString()}:{e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";
            Console.WriteLine($"{Text} TypeMessage: {e.Message.Type.ToString()}");

            switch (e.Message.Type)
            {
                case Telegram.Bot.Types.Enums.MessageType.Document:
                    Console.WriteLine($"{e.Message.Document.FileName} {e.Message.Document.MimeType}");
                    File = new MessageFile(e.Message.Document.FileId, e.Message.Document.FileName, $"{_pathDocuments}{e.Message.Document.FileName}");
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Video:
                    Console.WriteLine($"{e.Message.Video.FileName} {e.Message.Video.MimeType}");
                    File = new MessageFile(e.Message.Video.FileId, $"{e.Message.Video.FileId}{e.Message.Video.FileName}[{e.Message.Chat.FirstName}][{DateTime.Now.ToLongDateString()}].mp4", _pathVideo);
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
                    File = new MessageFile(result.FileId, $"[{e.Message.Chat.FirstName}][{DateTime.Now.ToLongDateString()}]{result.FileUniqueId}.jpg", _pathImages);
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Audio:
                    Console.WriteLine($"{e.Message.Audio.FileName} {e.Message.Audio.MimeType}");
                    File = new MessageFile(e.Message.Audio.FileId,
                        $"{e.Message.Audio.FileName}[{e.Message.Chat.FirstName}][{DateTime.Now.ToLongDateString()}].mp3",
                        _pathSounds);
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Text:

                    switch (e.Message.Text)
                    {
                        case "/help":
                            _bot.SendTextMessageAsync(e.Message.Chat.Id, $"Это бот файлообменник. Загрузи что-нибудь от сюда или сохрани сюда" +
                                $"\n/show - показывает контент который можно загрузить с сервера");
                            break;
                        case "/start":
                            _bot.SendTextMessageAsync(e.Message.Chat.Id, $"Это бот файлообменник. Загрузи что-нибудь от сюда или сохрани сюда." +
                                $"Для дополнительной информации воспользуйся командой /help");
                            break;
                        case "/show":
                            GetDirectoryFolder(_pathContent, e);
                            break;
                        default:
                            _bot.SendTextMessageAsync(e.Message.Chat.Id, "я не понимаю твою команду введи /help и я подскажу тебе");
                            break;
                    }
                    return;
                default:
                    _bot.SendTextMessageAsync(e.Message.Chat.Id, $"Это что шутка? Я тут для загрузки и показа что загрузили");
                    return;
            }

            Download(File.FileId, $"{File.FilePath}{File.FileName}");

            _bot.SendTextMessageAsync(e.Message.Chat.Id, $"файл загружен");
            if (e.Message.Text == null) return;
        }
               
        private static void GetDirectoryFolder(string path, MessageEventArgs e) //показать каталог по заданному пути
        {
            DirectoryInfo di = new DirectoryInfo(path);
            List<string> ListItem = new List<string>();
            string Answer;
            if (path == _pathContent)
            {
                foreach (var item in di.GetDirectories())
                {
                    ListItem.Add(item.Name);
                }
                Answer = $"[1]{ListItem[0]}";
                for (int i = 1; i < ListItem.Count; i++)
                {
                    Answer += $"\n[{i + 1}]{ListItem[i]}";
                }
            }
            else
            {
                foreach (var item in di.GetFiles())
                {
                    ListItem.Add(item.Name);
                }
                Answer = $"1){ListItem[0]}";
                for (int i = 1; i < ListItem.Count; i++)
                {
                    Answer += $"\n{i + 1}){ListItem[i]}";
                }
            }
            _bot.SendTextMessageAsync(e.Message.Chat.Id, Answer);
            return;
        }
        private static string CheckNumber(string text, string path, MessageEventArgs e) //проверить число
        {
            DirectoryInfo di = new DirectoryInfo(path);
            List<string> ListItem = new List<string>();
            if (path == _pathContent)
            {
                foreach (var item in di.GetDirectories())
                {
                    ListItem.Add(item.Name);
                }
            }
            else
            {
                foreach (var item in di.GetFiles())
                {
                    ListItem.Add(item.Name);
                }
            }

            if (int.TryParse(text, out int result))
            {
                result -= 1;
                if (result >= 0 && result < ListItem.Count)
                {
                    string ReturnPath = $"{path}{ListItem[result]}\\";
                    return ReturnPath;
                }
            }
            _bot.SendTextMessageAsync(e.Message.Chat.Id, "неправильный ввод, нужно ввести номер из списка," +
                " попробуйте заново через команду /show");
            return "null";
        }
        private static async Task Load(long Id, string Path) //загрузить файл пользователю из заданного каталога
        {
            Path = Path.Remove(Path.Length - 1);

            using (FileStream stream = System.IO.File.OpenRead(Path))
            {
                string FileName = System.IO.Path.GetFileName(Path);
                var InputFile = new InputOnlineFile(stream, FileName);
                if (Path.Contains(_pathDocuments))
                {
                    await _bot.SendDocumentAsync(Id, InputFile);
                }
                else if (Path.Contains(_pathImages))
                {
                   await _bot.SendPhotoAsync(Id, InputFile);
                }
                else if (Path.Contains(_pathSounds))
                {
                    await _bot.SendAudioAsync(Id, InputFile);
                }
                else if (Path.Contains(_pathVideo))
                {
                   await _bot.SendVideoAsync(Id, InputFile);
                }
            }
        }

        private static async Task Download(string FileId, string Path)  //скачать на компьютер
        {
            var File = await _bot.GetFileAsync(FileId);
            FileStream fs = new FileStream(Path, FileMode.Create);
            await _bot.DownloadFileAsync(File.FilePath, fs);
            fs.Close();
            fs.Dispose();
        }
    }
}
