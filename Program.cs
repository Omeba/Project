using System;
using System.Net.Http.Headers;

namespace Project
{
    internal static class IOMethods
    {
        static String path = Environment.CurrentDirectory + "\\..\\..\\..\\..\\WorkingFiles\\";

        public static String[] GetText()
        {
            String fullPath = $"{path}input.txt";

            try
            {
                String[] text = File.ReadAllLines(fullPath, System.Text.Encoding.UTF8);
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

        private static String generateOutputString(in double[] originalArray)
        {
            String outputString = "";
            foreach (double elem in originalArray)
            {
                outputString += $"{elem:f4}:";
            }

            return outputString[..^1];
        }

        public static void WriteResult(in double[] result)
        {
            String fullPath = $"{path}output.txt";
            String outputString = generateOutputString(result);

            try
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

    internal static class Solution
    {
        private static void ConvertLineToValues(in String line, out long[] values)
        {
            bool hasCorrectValues = false;
            String[] strValues = line.Split(' ');

            values = new long[strValues.Length];
            for(int current = 0; current < strValues.Length; current++)
            {
                if (long.TryParse(strValues[current], out values[current]) && values[current] > 0)
                {
                    hasCorrectValues = true;
                }
            }

            if (!hasCorrectValues)
            {
                throw new Exception("Корректных данных в файле нет");
            }
        }

        private static void ProcessValues(out double[] result, ref long[] values)
        {
            double previousValue = 1;
            result = new double[values.Length];
            int position = 0;

            foreach (long current in values) 
            {
                if (current > 0)
                {
                    previousValue = Math.Sqrt(current) / previousValue;
                    result[position++] = previousValue;
                }
            }

            if (position == 0)
            {
                throw new Exception("«Корректных данных в файле нет");
            }

            Array.Resize(ref result, position);
        }

        public static void Main()
        {
            ConsoleKeyInfo keyToExit;
            do 
            {
                try
                {
                    String[] text = IOMethods.GetText();

                    long[] values;
                    ConvertLineToValues(text[0], out values);

                    double[] result;
                    ProcessValues(out result, ref values);

                    IOMethods.WriteResult(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Программа приостановила работу. Нажмите ESC, чтобы выйти, или ENTER, чтобы попробовать ещё раз");
                }

                keyToExit = Console.ReadKey();
            } while (keyToExit.Key != ConsoleKey.Escape);
        }
    }
}