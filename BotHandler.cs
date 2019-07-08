using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PandeBot
{
    public class BotHandler
    {
        ITelegramBotClient botClient;
        ILogger _log;
        public Database database { get; set; }


        public BotHandler(string token, Database db, ILogger log)
        {
            botClient = new TelegramBotClient(token);

            database = db;
            

            database.LoadDB();

            _log = log;
        }


        public async void Bot_OnMessage(Update update)
        {
            var message = update.Message;

            _log.LogInformation($"Recibió un mensaje de {message.From.FirstName} {message.From.LastName} - {message.From.Id}");
            if (message == null || (message.Type != MessageType.Text && message.Type != MessageType.Audio && message.Type != MessageType.Photo)) return;

            switch (message.Type)
            {
                case MessageType.Audio:
                    await HandleAudioMessage(message);
                    break;
                case MessageType.Text:
                    await HandleTextMessage(message);
                    break;
                case MessageType.Photo:
                    await HandlePhotoMessage(message);
                    break;
            }



        }

        async Task HandlePhotoMessage(Message message)
        {
            Chat chat = message.Chat;

            //foreach (var photo in message.Photo)
            //{
            var photo = message.Photo.First();
            if (!database.Listas.photos.Exists(a => a.Equals(photo.FileId)))
            {
                database.Listas.photos.Add(photo.FileId);

                await botClient.SendTextMessageAsync(
                    chatId: chat,
                    text: $"Tu foto se agregó a la DB! {char.ConvertFromUtf32(0x1F389)} "
                );

                database.SaveDB();
            }
            //}
        }

        async Task HandleAudioMessage(Message message)
        {
            Chat chat = message.Chat;

            if (!database.Listas.audios.Exists(a => a.Equals(message.Audio.FileId)))
            {
                database.Listas.audios.Add(message.Audio.FileId);

                await botClient.SendTextMessageAsync(
                    chatId: chat,
                    text: $"Tu audio se agregó a la DB! {char.ConvertFromUtf32(0x1F389)} "
                );

                database.SaveDB();
            }

        }


        async Task HandleTextMessage(Message message)
        {
            try
            {
                Chat chat = message.Chat;

                string originalMsg = message.Text.Replace("@PanderetaBot", "");
                string[] args = originalMsg.Split(" ");
                string command = args[0];

                switch (command)
                {
                    case "/addapodo":
                        if (args.Length == 1)
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: chat,
                                text: $"Indicá bien el apodo papá"
                            );

                            return;
                        }

                        if (!database.Listas.apodos.Exists(a => a.Equals(args[1])))
                        {
                            database.Listas.apodos.Add(args[1]);
                            await botClient.SendTextMessageAsync(
                                chatId: chat,
                                text: $"Agregaste el apodo **{args[1]}**"
                            );

                            database.SaveDB();
                        }

                        break;
                    case "/addfrase":
                        if (args.Length == 1)
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: chat,
                                text: $"Indicá bien la frase papá"
                            );

                            return;
                        }
                        string frase = originalMsg.Replace(command, "");

                        if (!database.Listas.frases.Exists(a => a.Equals(frase)))
                        {
                            database.Listas.frases.Add(frase);
                            await botClient.SendTextMessageAsync(
                                chatId: chat,
                                text: $"Agregaste la frase **{frase}**"
                            );

                            database.SaveDB();
                        }

                        break;
                    case "/apodo":

                        await botClient.SendTextMessageAsync(
                          chatId: chat,
                          text: database.Listas.apodos.RandomElement()
                        );
                        break;

                    case "/lopa":
                        await botClient.SendTextMessageAsync(
                          chatId: chat,
                          text: database.Listas.frases.RandomElement()
                        );
                        break;

                    case "/audio":
                        if (database.Listas.audios.Count() == 0)
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: chat,
                                text: $"No hay audios cargados :(. Te dejo una banana {char.ConvertFromUtf32(0x1F34C)}"
                            );

                            return;
                        }

                        await botClient.SendAudioAsync(
                            chatId: chat,
                            audio: database.Listas.audios.RandomElement()
                            );
                        break;
                    case "/foto":
                        if (database.Listas.photos.Count() == 0)
                        {
                            await botClient.SendTextMessageAsync(
                                chatId: chat,
                                text: $"No hay fotos cargadas :(. Te dejo un camello {char.ConvertFromUtf32(0x1F42A)}"
                            );

                            return;
                        }
                        await botClient.SendPhotoAsync(
                          chatId: chat,
                          photo: database.Listas.photos.RandomElement()
                        );
                        break;
                }

            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
            }
        }
    }
}
