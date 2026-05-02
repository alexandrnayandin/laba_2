using System;
using System.Collections.Generic;
using System.IO;

// Задание 1. Базовые интерфейсы

interface IMovable
{
    void Move(int x, int y);
}

class Point : IMovable
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Move(int x, int y)
    {
        X += x;
        Y += y;
        Console.WriteLine($"Точка перемещена. Новые координаты: X = {X}, Y = {Y}");
    }
}

interface IDrawable
{
    void Draw();
}

// Задание 2. Интерфейсы и наследование

interface IShape
{
    double GetArea();
    double GetPerimeter();
}

class Circle : IDrawable, IShape
{
    public double Radius { get; set; }

    public Circle(double radius)
    {
        Radius = radius;
    }

    public void Draw()
    {
        Console.WriteLine($"Рисуется круг с радиусом {Radius}");
    }

    public double GetArea()
    {
        return Math.PI * Radius * Radius;
    }

    public double GetPerimeter()
    {
        return 2 * Math.PI * Radius;
    }
}

class Rectangle : IDrawable, IShape
{
    public double Width { get; set; }
    public double Height { get; set; }

    public Rectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public void Draw()
    {
        Console.WriteLine($"Рисуется прямоугольник {Width} x {Height}");
    }

    public double GetArea()
    {
        return Width * Height;
    }

    public double GetPerimeter()
    {
        return 2 * (Width + Height);
    }
}

interface I3DShape : IShape
{
    double GetVolume();
}

class Cube : I3DShape
{
    public double Side { get; set; }

    public Cube(double side)
    {
        Side = side;
    }

    public double GetArea()
    {
        return 6 * Side * Side;
    }

    public double GetPerimeter()
    {
        return 12 * Side;
    }

    public double GetVolume()
    {
        return Side * Side * Side;
    }
}

// Задание 3. Сегрегация интерфейсов ISP

interface IDevice
{
    void Print();
    void Scan();
    void Fax();
}

class OldPrinter : IDevice
{
    public void Print()
    {
        Console.WriteLine("Старый пример: принтер печатает.");
    }

    public void Scan()
    {
        // Пустой метод, потому что принтер не умеет сканировать
    }

    public void Fax()
    {
        // Пустой метод, потому что принтер не умеет отправлять факс
    }
}

class OldScanner : IDevice
{
    public void Print()
    {
        // Пустой метод, потому что сканер не умеет печатать
    }

    public void Scan()
    {
        Console.WriteLine("Старый пример: сканер сканирует.");
    }

    public void Fax()
    {
        // Пустой метод, потому что сканер не умеет отправлять факс
    }
}

interface IPrinter
{
    void Print();
}

interface IScanner
{
    void Scan();
}

interface IFax
{
    void Fax();
}

class Printer : IPrinter
{
    public void Print()
    {
        Console.WriteLine("Принтер печатает документ.");
    }
}

class Scanner : IScanner
{
    public void Scan()
    {
        Console.WriteLine("Сканер сканирует документ.");
    }
}

class MultifunctionDevice : IPrinter, IScanner, IFax
{
    public void Print()
    {
        Console.WriteLine("МФУ печатает документ.");
    }

    public void Scan()
    {
        Console.WriteLine("МФУ сканирует документ.");
    }

    public void Fax()
    {
        Console.WriteLine("МФУ отправляет факс.");
    }
}

// Задание 4. Интерфейсы и полиморфизм

interface IPayable
{
    void Pay(decimal amount);
}

class CreditCard : IPayable
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Оплата банковской картой на сумму {amount} руб.");
    }
}

class Cash : IPayable
{
    public void Pay(decimal amount)
    {
        Console.WriteLine($"Оплата наличными на сумму {amount} руб.");
    }
}

interface ILogger
{
    void Log(string message);
}

class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"Лог в консоль: {message}");
    }
}

class FileLogger : ILogger
{
    public void Log(string message)
    {
        File.AppendAllText("log.txt", message + Environment.NewLine);
        Console.WriteLine("Сообщение записано в файл log.txt");
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Лабораторная работа №2. Часть 1. Интерфейсы");
        Console.WriteLine();

        Console.WriteLine("Задание 1. IMovable");
        Point point = new Point(2, 3);
        point.Move(5, 7);
        Console.WriteLine();

        Console.WriteLine("Задание 1. IDrawable");
        List<IDrawable> figures = new List<IDrawable>
        {
            new Circle(4),
            new Rectangle(5, 6)
        };

        DrawAll(figures);
        Console.WriteLine();

        Console.WriteLine("Задание 2. IShape");
        IShape circle = new Circle(3);
        IShape rectangle = new Rectangle(4, 8);

        PrintShapeInfo(circle);
        PrintShapeInfo(rectangle);
        Console.WriteLine();

        Console.WriteLine("Задание 2. I3DShape");
        Cube cube = new Cube(4);
        PrintShapeInfo(cube);
        Console.WriteLine($"Объем куба: {cube.GetVolume():F2}");
        Console.WriteLine();

        Console.WriteLine("Задание 3. Толстый интерфейс IDevice");
        OldPrinter oldPrinter = new OldPrinter();
        OldScanner oldScanner = new OldScanner();

        oldPrinter.Print();
        oldScanner.Scan();
        Console.WriteLine();

        Console.WriteLine("Задание 3. Разделенные интерфейсы ISP");
        Printer printer = new Printer();
        Scanner scanner = new Scanner();
        MultifunctionDevice mfu = new MultifunctionDevice();

        printer.Print();
        scanner.Scan();
        mfu.Print();
        mfu.Scan();
        mfu.Fax();
        Console.WriteLine();

        Console.WriteLine("Задание 4. IPayable");
        ProcessPayment(new CreditCard(), 1500);
        ProcessPayment(new Cash(), 700);
        Console.WriteLine();

        Console.WriteLine("Задание 4. ILogger");
        DoWork(new ConsoleLogger());
        DoWork(new FileLogger());

        Console.WriteLine();
        Console.WriteLine("Программа завершена.");
    }

    static void DrawAll(List<IDrawable> figures)
    {
        foreach (IDrawable figure in figures)
        {
            figure.Draw();
        }
    }

    static void PrintShapeInfo(IShape shape)
    {
        Console.WriteLine($"Площадь: {shape.GetArea():F2}");
        Console.WriteLine($"Периметр: {shape.GetPerimeter():F2}");
    }

    static void ProcessPayment(IPayable method, decimal amount)
    {
        method.Pay(amount);
    }

    static void DoWork(ILogger logger)
    {
        logger.Log("Выполняется рабочая операция.");
    }
}
