// 5850666179:AAFbwzTQOdrLOgJGNZOh2wa_59c71lk56f4

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botClient = new TelegramBotClient("5850666179:AAFbwzTQOdrLOgJGNZOh2wa_59c71lk56f4");

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{messageText}' message from {message.From.FirstName}");



    if(message.Text == "/start")
    {
        Message mainMessege = await botClient.SendTextMessageAsync(
            chatId:chatId,
            text: "Добро пожаловать!\n/help - Список команд.",
            cancellationToken: cancellationToken
            );
        return;
    }



    if(message.Text == "/photo")
    {
        Message sentPhoto = await botClient.SendPhotoAsync(
       chatId: chatId,
  photo: InputFile.FromUri("https://github.com/TelegramBots/book/raw/master/src/docs/photo-ara.jpg"),
  caption: "<b>Ara bird</b>. <i>Source</i>: <a href=\"https://pixabay.com\">Pixabay</a>",
  parseMode: ParseMode.Html,
  cancellationToken: cancellationToken);

        return;
    }

    if (message.Text == "/sound")
    {
        Message sentAudio = await botClient.SendAudioAsync(
             chatId: chatId,
    audio: InputFile.FromUri("https://github.com/TelegramBots/book/raw/master/src/docs/audio-guitar.mp3"),
    /*
    performer: "Joel Thomas Hunger",
    title: "Fun Guitar and Ukulele",
    duration: 91, // in seconds
    */
    cancellationToken: cancellationToken
            );
    }


      


   



 

}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}