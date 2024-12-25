using System;
using System.Collections.Generic;

class SimpleInterpreter
{
    private Dictionary<string, double> numericVariables = new Dictionary<string, double>();
    private Dictionary<string, string> stringVariables = new Dictionary<string, string>();

    public void Execute(string command)
    {
        command = command.Trim();
        if (string.IsNullOrEmpty(command))
            return;

        // Простейший парсер команд
        if (command.StartsWith("print "))
        {
            string varName = command.Substring(6).Trim();
            if (numericVariables.ContainsKey(varName))
            {
                Console.WriteLine(numericVariables[varName]);
            }
            else if (stringVariables.ContainsKey(varName))
            {
                Console.WriteLine(stringVariables[varName]);
            }
            else
            {
                Console.WriteLine("Ошибка: Переменная не найдена.");
            }
        }
        else if (command.StartsWith("input "))
        {
            string varName = command.Substring(6).Trim();
            Console.Write($"Введите значение для {varName}: ");
            string userInput = Console.ReadLine();

            // Проверяем, является ли ввод числом
            if (double.TryParse(userInput, out double numericValue))
            {
                numericVariables[varName] = numericValue;
            }
            else
            {
                // Если не число, сохраняем как строку
                stringVariables[varName] = userInput;
            }
        }
        else if (command.Contains("="))
        {
            var parts = command.Split('=');
            if (parts.Length == 2)
            {
                string varName = parts[0].Trim();
                string expression = parts[1].Trim();

                // Проверяем, является ли выражение числовым или строковым
                if (expression.StartsWith(""") && expression.EndsWith("""))
                {
                    // Строка
                    string value = expression.Trim('"');
                    stringVariables[varName] = value;
                }
                else
                {
                    // Число
                    double value = EvaluateExpression(expression);
                    numericVariables[varName] = value;
                }
            }
            else
            {
                Console.WriteLine("Ошибка: Неправильный синтаксис присваивания.");
            }
        }
        else
        {
            Console.WriteLine("Ошибка: Неизвестная команда.");
        }
    }

    private double EvaluateExpression(string expression)
    {
        // Упрощённый парсер арифметических выражений
        var parts = expression.Split(new char[] { '+', '-', '*', '/' }, StringSplitOptions.RemoveEmptyEntries);
        double result = 0;
        char operation = '+';
        foreach (var part in parts)
        {
            double value;
            if (double.TryParse(part.Trim(), out value))
            {
                result = PerformOperation(result, value, operation);
            }
            else if (numericVariables.ContainsKey(part.Trim()))
            {
                value = numericVariables[part.Trim()];
                result = PerformOperation(result, value, operation);
            }

            // Определяем операцию для следующего значения
            if (expression.Contains(part))
            {
                int index = expression.IndexOf(part) + part.Length;
                if (index < expression.Length)
                {
                    operation = expression[index];
                }
            }
        }

        return result;
    }

    private double PerformOperation(double currentResult, double value, char operation)
    {
        switch (operation)
        {
            case '+':
                return currentResult + value;
            case '-':
                return currentResult - value;
            case '*':
                return currentResult * value;
            case '/':
                if (value != 0)
                    return currentResult / value;
                else
                    throw new DivideByZeroException("Ошибка: Деление на ноль.");
            default:
                throw new InvalidOperationException($"Ошибка: Неизвестная операция '{operation}'.");
        }
    }
}
