using System.Security.Cryptography;
using System.Text;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using TTS.DBService;

namespace TelegramTicketSystem;

public class Program
{
    private static Dictionary<string, BotAction> BotActions => new() {
        { "/start", new StartAction() },
        { "/register", new RegisterAction() }
    }; 

    private static async Task Main(string[] args)
    {
        var bot = new Telegram.BotAPI.TelegramBotClient(@"7029960925:AAHCLVfi0M0YNiapV4c2rdCUfqDYlXBJPwg");
        using var cts = new CancellationTokenSource();
        await LongPolling(bot, cts.Token);
        await Task.Delay(-1, cts.Token);
    }

    private static async Task LongPolling(TelegramBotClient bot, CancellationToken ct)
    {
        var updates = bot.GetUpdates();
        while (true)
        {
            if (ct.IsCancellationRequested) break;
            if (updates.Any())
            {
                foreach (Update update in updates)
                {
                    Message? msg = update.Message;
                    if (msg is null) continue;
                    if (BotActions.TryGetValue(msg.Text, out BotAction action))
                    {
                        await action.Execute(bot, update, ct).WaitAsync(ct);
                    }
                }
                var offset = updates.Last().UpdateId + 1;
                updates = bot.GetUpdates(offset);
            }
            else
            {
                updates = bot.GetUpdates();
            }
        }
    }
}

//class Program
//{
//    // Это клиент для работы с Telegram Bot API, который позволяет отправлять сообщения, управлять ботом, подписываться на обновления и многое другое.
//    private static ITelegramBotClient _botClient;

//    // Это объект с настройками работы бота. Здесь мы будем указывать, какие типы Update мы будем получать, Timeout бота и так далее.
//    private static ReceiverOptions _receiverOptions;

//    static async Task Main()
//    {
//        _botClient = new TelegramBotClient(@"7029960925:AAHCLVfi0M0YNiapV4c2rdCUfqDYlXBJPwg"); // Присваиваем нашей переменной значение, в параметре передаем Token, полученный от BotFather
//        _receiverOptions = new ReceiverOptions // Также присваем значение настройкам бота
//        {
//            AllowedUpdates = new[] // Тут указываем типы получаемых Update`ов, о них подробнее расказано тут https://core.telegram.org/bots/api#update
//            {
//                UpdateType.Message, // Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
//            },
//            // Параметр, отвечающий за обработку сообщений, пришедших за то время, когда ваш бот был оффлайн
//            // True - не обрабатывать, False (стоит по умолчанию) - обрабаывать
//            ThrowPendingUpdates = true,
//        };

//        using var cts = new CancellationTokenSource();

//        // UpdateHander - обработчик приходящих Update`ов
//        // ErrorHandler - обработчик ошибок, связанных с Bot API
//        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token); // Запускаем бота
//        var me = await _botClient.GetMeAsync(); // Создаем переменную, в которую помещаем информацию о нашем боте.
//        Console.WriteLine($"{me.FirstName} запущен!");

//        await Task.Delay(-1); // Устанавливаем бесконечную задержку, чтобы наш бот работал постоянно
//    }

//    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
//    {
//        // Обязательно ставим блок try-catch, чтобы наш бот не "падал" в случае каких-либо ошибок
//        try
//        {
//            // Сразу же ставим конструкцию switch, чтобы обрабатывать приходящие Update
//            switch (update.Type)
//            {
//                case UpdateType.Message:
//                    {
//                        // Эта переменная будет содержать в себе все связанное с сообщениями
//                        Message? message = update.Message;

//                        // From - это от кого пришло сообщение (или любой другой Update)
//                        User? user = message.From;

//                        // Выводим на экран то, что пишут нашему боту, а также небольшую информацию об отправителе
//                        Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

//                        // Chat - содержит всю информацию о чате
//                        Chat chat = message.Chat;

//                        // Добавляем проверку на тип Message
//                        switch (message.Type)
//                        {
//                            // Тут понятно, текстовый тип
//                            case MessageType.Text:
//                                {
//                                    switch (message.Text)
//                                    {
//                                        case "/start":
//                                            UserService userService = new();
//                                            UserDTO? user_db = await userService.Get(user.Id);
//                                            if (user_db is null)
//                                            {
//                                                ReplyKeyboardMarkup registerKeyboard = new(
//                                                    new List<KeyboardButton[]>() {
//                                                        new KeyboardButton[]
//                                                        {
//                                                            new("Регистрация"),
//                                                        },
//                                                    });
//                                                registerKeyboard.ResizeKeyboard = true;
//                                                await botClient.SendTextMessageAsync(
//                                                    chat.Id,
//                                                    $"Приветствуем вас \"{user.Username}\"\n" +
//                                                    $"Вы не зарегистрированы в системе!\n" +
//                                                    $"Для того, чтобы зарегистрироваться введите команду /register",
//                                                    replyMarkup: registerKeyboard,
//                                                    cancellationToken: cancellationToken);
//                                            }
//                                            return;
//                                        case "Регистрация":
//                                            user_db = new();
//                                            await botClient.SendTextMessageAsync(
//                                                    chat.Id,
//                                                    $"Введите вашу фамилию",
//                                                    cancellationToken: cancellationToken);
//                                            await botClient.SendTextMessageAsync(
//                                                    chat.Id,
//                                                    $"Введите ваше имя",
//                                                    cancellationToken: cancellationToken);
//                                            return;
//                                    }
//                                    return;
//                                }
//                        }

//                        return;
//                    }

//                case UpdateType.CallbackQuery:
//                    {
//                        // Переменная, которая будет содержать в себе всю информацию о кнопке, которую нажали
//                        var callbackQuery = update.CallbackQuery;

//                        // Аналогично и с Message мы можем получить информацию о чате, о пользователе и т.д.
//                        var user = callbackQuery.From;

//                        // Выводим на экран нажатие кнопки
//                        Console.WriteLine($"{user.FirstName} ({user.Id}) нажал на кнопку: {callbackQuery.Data}");

//                        // Вот тут нужно уже быть немножко внимательным и не путаться!
//                        // Мы пишем не callbackQuery.Chat , а callbackQuery.Message.Chat , так как
//                        // кнопка привязана к сообщению, то мы берем информацию от сообщения.
//                        var chat = callbackQuery.Message.Chat;

//                        // Добавляем блок switch для проверки кнопок
//                        switch (callbackQuery.Data)
//                        {
//                            // Data - это придуманный нами id кнопки, мы его указывали в параметре
//                            // callbackData при создании кнопок. У меня это button1, button2 и button3

//                            case "button1":
//                                {
//                                    // В этом типе клавиатуры обязательно нужно использовать следующий метод
//                                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
//                                    // Для того, чтобы отправить телеграмму запрос, что мы нажали на кнопку

//                                    await botClient.SendTextMessageAsync(
//                                        chat.Id,
//                                        $"Вы нажали на {callbackQuery.Data}");
//                                    return;
//                                }

//                            case "button2":
//                                {
//                                    // А здесь мы добавляем наш сообственный текст, который заменит слово "загрузка", когда мы нажмем на кнопку
//                                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "Тут может быть ваш текст!");

//                                    await botClient.SendTextMessageAsync(
//                                        chat.Id,
//                                        $"Вы нажали на {callbackQuery.Data}");
//                                    return;
//                                }

//                            case "button3":
//                                {
//                                    // А тут мы добавили еще showAlert, чтобы отобразить пользователю полноценное окно
//                                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "А это полноэкранный текст!", showAlert: true);

//                                    await botClient.SendTextMessageAsync(
//                                        chat.Id,
//                                        $"Вы нажали на {callbackQuery.Data}");
//                                    return;
//                                }
//                        }

//                        return;
//                    }
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine(ex.ToString());
//        }
//    }

//    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
//    {
//        // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
//        var ErrorMessage = error switch
//        {
//            ApiRequestException apiRequestException
//                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
//            _ => error.ToString()
//        };

//        Console.WriteLine(ErrorMessage);
//        return Task.CompletedTask;
//    }
//}

//public class TelegramBot
//{
//    public async void Start()
//    {
//        TelegramBotClient bot = new(@"7029960925:AAHCLVfi0M0YNiapV4c2rdCUfqDYlXBJPwg");

//        User user = await bot.GetMeAsync();

//        await Console.Out.WriteLineAsync($"{user.Id} {user.Username}");
//    }
//}

public abstract class BotAction
{
    public abstract string Name { get; }

    public abstract bool InCommandQueue { get; }

    public abstract Task Execute(ITelegramBotClient botClient, Update update, CancellationToken ct);

    public bool Compare(string name) => Name.Equals(name);
}

public class StartAction : BotAction
{
    public override string Name => @"/start";

    public override bool InCommandQueue => false;

    public override async Task Execute(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        Message? message = update.Message;
        User? user = message.From;
        Chat chat = message.Chat;
        UserService userService = new();
        UserDTO? user_db = await userService.Get(user.Id);
        if (user_db is null)
        {
            ReplyKeyboardMarkup registerKeyboard = new(
                new List<KeyboardButton[]>() {
                    new KeyboardButton[]
                    {
                        new("Регистрация"),
                    },
                });
            registerKeyboard.ResizeKeyboard = true;
            await bot.SendMessageAsync(
                chat.Id,
                $"Приветствуем вас \"{user.Username}\"\n" +
                $"Вы не зарегистрированы в системе!\n" +
                $"Для того, чтобы зарегистрироваться введите команду /register",
                replyMarkup: registerKeyboard,
                cancellationToken: ct);
        }
        else
        {
            await bot.SendMessageAsync(
                chat.Id,
                $"Приветствуем вас {user_db.FirstName} {user_db.LastName} {user_db.MiddleName}\n" +
                $"из подразделения {user_db.Department}\n" +
                $"Введите пароль от учетной записи",
                cancellationToken: ct);
            string password = (await new UserAnswer().Get(bot, ct).WaitAsync(ct)).Text;
            string passHash = Convert.ToBase64String(
                                MD5.Create().ComputeHash(
                                    Encoding.UTF8.GetBytes(
                                        password)));
            if (user_db.Password.Equals(passHash))
                await bot.SendMessageAsync(
                    chat.Id,
                    $"Вы в системе",
                    cancellationToken: ct);
            else
                await bot.SendMessageAsync(
                    chat.Id,
                    $"Идите нахуй",
                    cancellationToken: ct);
        }
    }
}

public class RegisterAction : BotAction
{
    public override string Name => @"/register";

    public override bool InCommandQueue => false;

    public override async Task Execute(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        Message? message = update.Message;
        User? user = message.From;
        Chat chat = message.Chat;
        UserService userService = new();
        UserDTO user_db = new() { 
            Id = user.Id,
            Name = user.Username,
        };
        await bot.SendMessageAsync(
                chat.Id,
                $"Введите фамилию",
                cancellationToken: ct);
        user_db.FirstName = (await new UserAnswer().Get(bot, ct).WaitAsync(ct)).Text;
        await bot.SendMessageAsync(
                chat.Id,
                $"Введите имя",
                cancellationToken: ct);
        user_db.LastName = (await new UserAnswer().Get(bot, ct).WaitAsync(ct)).Text;
        await bot.SendMessageAsync(
                chat.Id,
                $"Введите отчество",
                cancellationToken: ct);
        user_db.MiddleName = (await new UserAnswer().Get(bot, ct).WaitAsync(ct)).Text;
        await bot.SendMessageAsync(
                chat.Id,
                $"Введите должность",
                cancellationToken: ct);
        user_db.Post = (await new UserAnswer().Get(bot, ct).WaitAsync(ct)).Text;
        await bot.SendMessageAsync(
                chat.Id,
                $"Введите подразделение",
                cancellationToken: ct);
        user_db.Department = (await new UserAnswer().Get(bot, ct).WaitAsync(ct)).Text;
        await bot.SendMessageAsync(
                chat.Id,
                $"Введите пароль",
                cancellationToken: ct);
        user_db.Password = Convert.ToBase64String(
                                MD5.Create().ComputeHash(
                                    Encoding.UTF8.GetBytes(
                                        (await new UserAnswer().Get(bot, ct).WaitAsync(ct)).Text)));
        userService.Add(user_db);
    }
}

public class UserAnswer
{
    public async Task<Message> Get(ITelegramBotClient bot, CancellationToken ct)
    {
        Message msg = null;
        IEnumerable<Update?> updates = Enumerable.Empty<Update?>();
        updates = await bot.GetUpdatesAsync();
        bool isBreak = false;
        while (true)
        {
            if (ct.IsCancellationRequested) break;
            var offset = updates?.LastOrDefault()?.UpdateId + 1;
            updates = await bot.GetUpdatesAsync(offset);
            foreach (Update update in updates)
            {
                msg = update.Message;
                if (msg is null) continue;
                isBreak = true;
            }
            if (isBreak) break;
        }
        return msg;
    }
}