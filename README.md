# DaJet MCP Server <a href="https://hub.docker.com/r/zhichkin/dajet-mcp-server"><img width="32" height="32" alt="docker-logo" src="https://github.com/user-attachments/assets/e41122f3-8aae-4ea0-9bb3-289b874b5c4c" /></a>

MCP-сервер для получения и анализа метаданных и струкутуры баз данных 1С:Предприятие 8.

Функционал сервера основан на использовании библиотеки [DaJet Metadata](https://github.com/zhichkin/dajet-metadata) и таким образом аналогичен функционалу [DaJet HTTP Server](https://github.com/zhichkin/dajet-http-server).

> DaJet MCP Server получает объекты метаданных 1С:Предприятие 8 вместе с описанием структуры хранения этих объектов в базе данных, то есть с именами таблиц и полей.

### Установка и запуск на Windows или Linux

1. Установить [Microsoft .NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
2. Скачать дистрибутив [DaJet MCP Server](https://github.com/zhichkin/dajet-mcp-server/releases/latest)
3. Создать рабочий каталог и распаковать в него дистрибутив, например: ```C:\dajet-mcp-server```
4. Перейти в каталог установки и запустить исполняемый файл ```DaJet.Mcp.Server.exe```

### Установка и запуск в Docker

1. Получить образ из Docker Hub

```
docker pull zhichkin/dajet-mcp-server
```

2. Запустить контейнер в Docker

```
docker run --name dajet-mcp-server --user=root -it -p 5000:5000 zhichkin/dajet-mcp-server
```

### Настройка сервера (корневой каталог установки)

1. Адрес и порт MCP-сервера для работы по протоколу HTTP указываются в файле ```appsetting.json```. По умолчанию: ```http://localhost:3000```.
2. Настроить подключения к базам данных 1С:Предприятие 8 в файле ```datasources.json```. Сервер может работать сразу с несколькими базами данных. Обращение к базам данных выполняется AI-агентом по её имени.

**Файл datasources.json**

```JSON
{
  "DataSources": [
    {
      "Name": "MS_TEST",
      "Type": "SqlServer",
      "ConnectionString": "Data Source=server;Initial Catalog=database;Integrated Security=True;Encrypt=False;"
    }
  ]
}
```

### Подключение MCP-сервера в IDE Cursor

**Файл mcp.json**

```JSON
{
  "mcpServers": {
    "dajet-mcp-server": {
      "url": "http://localhost:3000"
    }
  }
}
```

### Доступные инструменты MCP-сервера

|Инструмент|Описание|Параметры|
|---|---|---|
|get_metadata_type_names|Получает список поддерживаемых типов объектов метаданных|Нет|
|get_database_names|Получает список имён доступных баз данных|Нет|
|get_database_description|Получает описание базы данных (конфигурации) по её имени|Имя базы данных (конфигурации)|
|get_database_metadata|Получает описание структуры метаданных базы данных (конфигурации) по её имени|Имя базы данных (конфигурации)|
|get_metadata_object|Получает описание структуры объекта метаданных базы данных (конфигурации) по его типу и имени|Имя базы данных (конфигурации)<br>Тип объекта метаданных<br>Имя объекта метаданных|
