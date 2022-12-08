# Тестовое задание для Lifehack

## Задание

Написать чат-бот (пишем и сервер и клиент) с вариантами ответа на предустановленные фразы, например, “Как дела?” - ”хорошо”.
 
Сценарий:
1. при присоединении бот спрашивает имя клиента, при отказе клиент остается анонимным.
2. выводим перечень возможных вопросов.
3. по команде “пока” завершаем чат
4. команда для получения списка всех присоединенных пользователей в чате
 
Реализация сервера - C#, dotNet core
Реализация клиента - C# (console / gui)


## Решение

Для решения данного тестового задания было создано консольное приложение, которое совмещает в себе возможности простого 
мессенджера и чатбота. Мессенджер является peer-messenger, то есть не имеет выделенного сервера. Каждый клиент присоединяeтся 
к любому другому клиенту напрямую. Мессенджер работает по протоколу TCP/IP. В целях упрощения решения все клиенты должны быть
запущены локально. Для соединения с другим клиентом нужно указать порт, который слушает данный клиент. После соединения 
графический интерфейс, реализованный в консоли, позволит пользователю обмениваться сообщениями с другим клиентом. Мессенджер 
может быть запущен в режиме чат-бота. В таком случае при соединении к нему клиента, чатбот начнет вести диалог согласно 
заданию. Из-за ограничения консоли, а именно невозможности нормального параллельного отображения текста в одной обласи экрана
и ожидания ввода в другой области экрана, сообщения приходящие от другого клиента могут не отображаться пока не будет нажата 
клавиша Enter. В случае общения с чатботом эта проблема почти незаметна.

Для запуска приложения необходимо установить .Net Framework Core 2.2 и запустить команду dotnet run, находясь в директории 
PeerMessenger. Приложение принимает следующие аргументы:

- -port [port number] - номер порта, который будет слушать клиент. Если параметр не указан, то будет слушаться любой 
свободный порт
- -bot - если указан данный параметр, то приложение будет работать в режиме чат-бота.

Пример запуска прогаммы: dotnet run -port 20 -bot

После запуска программы будет отображен интерфейс мессенджера, который состоит из хедера, области отображения сообщений и поля 
ввода сообщений. В поле ввода сообщений можно вводить специальные команды, начинающиеся с символа '/' :

- /connect [port number] - команда для присоединения к клиенту на указанном порту. После выполнения команды, сообщения 
введнные в поле ввода, будут отправлены данному клиенту
- /help - показывает список команд
- /quit - выход из приложения

Для проверки работы чатбота, запускаем в одном консольном окне команду: dotnet run -port 10

Затем в другом консольном окне: dotnet run -port 20 -bot

Далее в первом окне вводим: /connect 20. 

После ввода этой команды бот поздоровается и можно начать диалог с ним.


Пример диалога с чатботом:

20: Привет!

20: Как тебя зовут?

\>привет! 

\>меня зовут иван

20: Иван, ты можешь спросить меня:
 - Как меня зовут
 - Как дела
 - Сколько сейчас времени
 - Список пользователей в чате

\>как тебя зовут?

20: Меня зовут Бот Р2-Д2.

\>как дела?

20: Хорошо!

\>сколько времени?

20: Сейчас 18:33.

\>список пользователей

20: 
 - Jhon
 - Иван
 - 1 анонимный пользователь
 
\>2+2

20: Я тебя не понимаю.

\>пока

20: Пока, Иван!


Чтобы остаться анонимным, когда бот спрашивает имя, нужно ввести пробел и нажать Enter.