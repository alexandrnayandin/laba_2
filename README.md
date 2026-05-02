# Лабораторная работа №2

## Дисциплина

Разработка кода информационных систем

## Тема

Интерфейсы. Репозиторий. JSON.


## Описание работы

В рамках лабораторной работы выполнены три части:

1. Работа с интерфейсами в C#.
2. Реализация паттерна «Репозиторий» с использованием SQLite и Entity Framework Core.
3. Создание REST API для работы с пользователями через JSON.


## Часть 1. InterfacesDemo

В проекте `InterfacesDemo` реализованы примеры работы с интерфейсами в C#.

Реализованы:

* интерфейс `IMovable` и класс `Point`;
* интерфейс `IDrawable` и классы `Circle`, `Rectangle`;
* интерфейс `IShape` для вычисления площади и периметра;
* интерфейс `I3DShape` и класс `Cube`;
* пример «толстого» интерфейса `IDevice`;
* разделение интерфейсов по принципу ISP;
* интерфейс `IPayable` и способы оплаты;
* интерфейс `ILogger` и разные способы логирования.

### Запуск

```bash
cd InterfacesDemo
dotnet run
```

## Часть 2. RepositoryDemo

В проекте `RepositoryDemo` реализован паттерн «Репозиторий» для работы с базой данных SQLite.

Предметная область: библиотека книг.

Реализованы:

* сущности `Author` и `Book`;
* контекст базы данных `AppDbContext`;
* обобщенный интерфейс `IRepository<T>`;
* специализированный интерфейс `IBookRepository`;
* универсальный репозиторий `GenericRepository<T>`;
* репозиторий `BookRepository`;
* паттерн `Unit of Work`;
* CRUD-операции;
* асинхронные методы.

База данных `library.db` создается автоматически при запуске программы.

### Запуск

```bash
cd RepositoryDemo
dotnet run
```

## Часть 3. UserApi

В проекте `UserApi` реализован REST API для работы с пользователями.

Используется:

* ASP.NET Core Web API;
* SQLite;
* Entity Framework Core;
* JSON-формат запросов и ответов.

Таблица `User` содержит поля:

| Поле       | Описание                              |
| ---------- | ------------------------------------- |
| `Id`       | уникальный идентификатор пользователя |
| `Login`    | логин пользователя                    |
| `PassHash` | хеш пароля пользователя               |

Реализованы HTTP-методы:

| Метод    | Маршрут      | Описание                     |
| -------- | ------------ | ---------------------------- |
| `POST`   | `/user`      | создание пользователя        |
| `GET`    | `/user/{id}` | получение пользователя по id |
| `PUT`    | `/user/{id}` | обновление пользователя      |
| `DELETE` | `/user/{id}` | удаление пользователя        |

База данных `users.db` создается автоматически при запуске приложения.

### Запуск сервера

```bash
cd UserApi
dotnet run
```

После запуска сервер будет доступен по адресу, который отобразится в терминале, например:

```text
http://localhost:5148
```

### Примеры запросов PowerShell

Создание пользователя:

```powershell
Invoke-RestMethod -Uri "http://localhost:5148/user" -Method POST -ContentType "application/json" -Body '{"Login":"example_user","PassHash":"hashed_password"}'
```

Получение пользователя:

```powershell
Invoke-RestMethod -Uri "http://localhost:5148/user/1" -Method GET
```

Обновление пользователя:

```powershell
Invoke-RestMethod -Uri "http://localhost:5148/user/1" -Method PUT -ContentType "application/json" -Body '{"Login":"updated_user","PassHash":"new_hashed_password"}'
```

Удаление пользователя:

```powershell
Invoke-RestMethod -Uri "http://localhost:5148/user/1" -Method DELETE
```

## Отчеты

Отчеты по трем частям лабораторной работы находятся в папке `reports`.

## Используемые технологии

* C#
* .NET 8
* ASP.NET Core
* Entity Framework Core
* SQLite
* JSON
* REST API
