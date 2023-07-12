// 5850666179:AAFbwzTQOdrLOgJGNZOh2wa_59c71lk56f4

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using HtmlAgilityPack;

var botClient = new TelegramBotClient("5850666179:AAFbwzTQOdrLOgJGNZOh2wa_59c71lk56f4");

using CancellationTokenSource cts = new();

ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() 
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

cts.Cancel();


async Task<string> GetCurrencyRate()
{
    var url = "https://myfin.by/currency/minsk";
    var web = new HtmlWeb();
    var doc = await web.LoadFromWebAsync(url);

    var rateElement = doc.DocumentNode.SelectSingleNode("//span[contains(@class, 'accent')]");
    var usdValue = doc.DocumentNode.SelectSingleNode("//a[contains(@class , 'currency ')]");

    if (rateElement != null)
    {
        var rate = rateElement.InnerText.Trim();
        var value = usdValue.InnerText.Trim();

        return $"Текущий курс: {value} = {rate} BYN \n ";
    }

    return "Не удалось получить данные курса";
}



async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message is not { } message)
        return;
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{messageText}' message from {message.From.FirstName}");

    BotCommand[] commands = await botClient.GetMyCommandsAsync();

    if (message.Text == "/start")
    {

        var commandList = string.Join("\n", commands.Select(c => $"/{c.Command} - {c.Description}"));
        
            Message mainMessege = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Добро пожаловать!\nСписок команд:\n " +
                    $"{commandList}",
                    cancellationToken: cancellationToken
                    );
            
        
       
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
    audio: InputFile.FromUri("https://github.com/pastilkaxo/USD-Course-Bot/blob/main/Sounds/%D0%92%D0%B0%D0%B6%D0%BD%D0%BE.mp3"),
    cancellationToken: cancellationToken
            );
    }


    if (message.Text == "/currency")
    {
        string currencyRate = await GetCurrencyRate();
        await botClient.SendTextMessageAsync(chatId: chatId, text: currencyRate);
        return;
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