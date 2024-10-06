using System;

namespace Project
{
    /// <summary>
    /// Класс, содержащий методы для работы с файловым вводом-выводом в папке WorkingFiles
    /// </summary>
    internal static class IOMethods
    {
        static readonly char sep = Path.DirectorySeparatorChar; // Знак разделителя для пути к файлу
        // Путь к папке с файлами для работы
        static readonly String path = Environment.CurrentDirectory + $"{sep}..{sep}..{sep}..{sep}..{sep}WorkingFiles{sep}";

        /// <summary>
        /// Получение всех строк из файла input.txt методами класса File
        /// </summary>
        /// <returns>Возвращает массив строк исходного файла</returns>
        /// <exception cref="Exception">Выбрасывает исключения при ошибках чтения и открытия файла</exception>
        public static String[] GetText()
        {
            String fullPath = $"{path}input.txt"; // Путь к файлу для чтения

            try // Попытка чтения файла
            {
                String[] text = File.ReadAllLines(fullPath, System.Text.Encoding.UTF8); // Массив строк файла
                return text;
            } 
            catch (FileNotFoundException) { throw new Exception("Входной Файл на диске отсутствует"); }
            catch (DirectoryNotFoundException) { throw new Exception("Входной Файл на диске отсутствует"); }
            catch (FileLoadException) { throw new Exception("Проблемы с открытием файла"); }
            catch (AccessViolationException) { throw new Exception("Проблемы с открытием файла"); }
            catch (UnauthorizedAccessException) { throw new Exception("Проблемы с открытием файла"); }
            catch (InvalidDataException) { throw new Exception("Проблемы с чтением данных из файла"); }
            catch (OperationCanceledException) { throw new Exception("Проблемы с чтением данных из файла"); }
        }

        /// <summary>
        /// Генерация строки для записи в файл
        /// </summary>
        /// <param name="originalArray">Массив значений, которые необходимо записать в файл</param>
        /// <returns>Возвращает строку для записи в файл</returns>
        private static String GenerateOutputString(in double[] originalArray)
        {
            String outputString = ""; // Переменная для выходной строки
            foreach (double elem in originalArray)
            {
                outputString += $"{elem:f4}:"; // Записываем элемент с точностью до 4-х знаков
            }

            return outputString[..^1]; // Обрезаем двоеточие в конце и возвращаем строку
        }

        /// <summary>
        /// Запись результата в файл output.txt методами класса File
        /// </summary>
        /// <param name="result">Данные, которые необходимо записать</param>
        /// <exception cref="Exception">Выбрасывает исключения, связанные с проблемами записи данных и сохранения файла</exception>
        public static void WriteResult(in double[] result)
        {
            String fullPath = $"{path}output.txt"; // Путь к выходному файлу
            String outputString = GenerateOutputString(result); // Генерация выходной строки

            try // Попытка записи в файл
            {
                File.WriteAllText(fullPath, outputString);
            }
            catch (FileLoadException) { throw new Exception("Проблемы с записью данных в файл"); }
            catch (AccessViolationException) { throw new Exception("Проблемы с записью данных в файл"); }
            catch (UnauthorizedAccessException) { throw new Exception("Проблемы с записью данных в файл"); }
            catch (InvalidDataException) { throw new Exception("Проблемы с записью данных в файл"); }
            catch (OperationCanceledException) { throw new Exception("Проблемы с записью данных в файл"); }
        }
    }

    /// <summary>
    /// Класс, реализующий методы решения основной задачи
    /// </summary>
    internal class Solution
    {
        /// <summary>
        /// Конверитрует строку в массив значений типа long
        /// </summary>
        /// <param name="line">Строка для обработки</param>
        /// <param name="values">Массив, в который записываются значения</param>
        /// <exception cref="Exception">При отсутствии корректных данных в строке выбрасывает исключение</exception>
        private static void ConvertLineToValues(in String line, out long[] values)
        {
            bool hasCorrectValues = false; // Флаг, указывающий на наличие корректных значений
            String[] strValues = line.Split(' '); // Значения в строковом представлении

            values = new long[strValues.Length]; // Массив для хранения корректных значений
            for(int current = 0; current < strValues.Length; current++) 
            {
                // Выполняется, если значение приводится к типу long и имеет значение, большее 0
                if (long.TryParse(strValues[current], out values[current]) && values[current] > 0)
                {
                    hasCorrectValues = true;
                }
            }

            if (!hasCorrectValues) // Проброс исключения при отсутствии корректных данных
            {
                throw new Exception("Корректных данных в файле нет");
            }
        }

        /// <summary>
        /// Обрабатывает исходные значения по правилу, описанному в варианте 12
        /// </summary>
        /// <param name="result">Массив double, в который записывается результат</param>
        /// <param name="values">Массив long исходных значений</param>
        /// <exception cref="Exception">Выбрасывает исключение при отсутствии корректных данных</exception>
        private static void ProcessValues(out double[] result, ref long[] values)
        {
            result = new double[values.Length + 1]; // Массив для записи результатов (размера +1 для записи дефолтного значения)
            result[0] = 1; // Дефолтное значение
            int position = 1; // Индекс, по которому запишется следующий результат

            foreach (long current in values) 
            {
                if (current > 0)
                {
                    result[position] = Math.Sqrt(current) / result[position - 1];
                    position++;
                }
            }

            // Если не удалось посчитать ни одно значение, выбрасываем исключение
            if (position == 1)
            {
                throw new Exception("«Корректных данных в файле нет");
            }

            Array.Resize(ref result, position); // Ужимаем массив, так как некоторые данные могли не подойти
        }

        /// <summary>
        /// Точка входа в программу. Реализует решение основной задачи
        /// </summary>
        public static void Main()
        {
            bool wasException; // Флаг, указывающий на наличие выброшенного исключения
            // Блок, исполняющийся, пока пользователь не нажмёт ESC или программа не завершится без ошибок
            do 
            {
                wasException = false; // Исключения ещё не произошло в этой итерации
                try // Попытка выполнить задачу
                {
                    String[] text = IOMethods.GetText(); // Получение строк из файла

                    ConvertLineToValues(text[0], out long[] values); // Получение значений из строки
                    ProcessValues(out double[] result, ref values); // Подсчёт результата

                    IOMethods.WriteResult(result); // Запись результата
                }
                catch (Exception ex) // не получилось :(
                {
                    wasException = true; // Выброшено исключение

                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Программа приостановила работу. Нажмите ESC, чтобы выйти, или ENTER, чтобы попробовать ещё раз");

                    if (Console.ReadKey().Key == ConsoleKey.Escape) { break; }
                }
            } while (wasException);
        }
    }
}
