using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace prac13
{
    static class visulaarray
    {
        /// <summary>
        /// Метод для двухмерного массива
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this T[,] matrix)
        {
            var res = new DataTable();
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                res.Columns.Add("col" + (i + 1), typeof(T));
            }

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                var row = res.NewRow();

                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    row[j] = matrix[i, j];
                }

                res.Rows.Add(row);
            }

            return res;
        }
        /// <summary>
        /// Функция заполянет массив  числами от 0 до 10
        /// </summary>
        /// <param name="array"> имя массива для заполнения</param>
        public static void Fill2Array(int[,] array)
        {
            // Получаем размеры массива
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            Random rnd = new Random();

            // Заполняем массив заданным значением
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    array[i, j] = rnd.Next(0, 10);
                }
            }
        }      
    }
    public class MatrixManager
    {
        private const string MatrixFilePath = "matrix.txt";

        public void SaveMatrix(int[,] matrixData)
        {
            int rows = matrixData.GetLength(0);
            int columns = matrixData.GetLength(1);

            using (StreamWriter writer = new StreamWriter(MatrixFilePath))
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        writer.Write(matrixData[i, j] + " ");
                    }
                    writer.WriteLine();
                }
            }
        }

        public int[,] LoadMatrix()
        {
            if (File.Exists(MatrixFilePath))
            {
                using (StreamReader reader = new StreamReader(MatrixFilePath))
                {
                    string[] firstLineValues = reader.ReadLine().Split(' ');
                    int rows = 1;
                    int columns = firstLineValues.Length - 1;

                    while (!reader.EndOfStream)
                    {
                        reader.ReadLine();
                        rows++;
                    }

                    int[,] matrixData = new int[rows, columns];
                    int currentRow = 0;

                    using (StreamReader newReader = new StreamReader(MatrixFilePath))
                    {
                        while (!newReader.EndOfStream)
                        {
                            string[] values = newReader.ReadLine().Split(' ');

                            for (int j = 0; j < columns; j++)
                            {
                                matrixData[currentRow, j] = int.Parse(values[j]);
                            }

                            currentRow++;
                        }
                    }

                    return matrixData;
                }
            }

            return null; // Если файл не существует или матрица не была сохранена, возвращаем null.
        }
    }
    public static class ConfigurationSettings
    {
        private static string configFilePath = "config.ini";

        public static void SaveTableSize(int[,] matrixData)
        {
            int rows = matrixData.GetLength(0);
            int columns = matrixData.GetLength(1);
            try
            {
                // Запись размера таблицы в файл конфигурации
                string configData = $"{rows},{columns}";
                File.WriteAllText(configFilePath, configData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при сохранении настроек: {ex.Message}");
            }
        }

        public static (int rows, int columns) LoadTableSize()
        {
            try
            {
                if (File.Exists(configFilePath))
                {
                    // Чтение размера таблицы из файла конфигурации
                    string configData = File.ReadAllText(configFilePath);
                    string[] sizes = configData.Split(',');

                    if (sizes.Length == 2 && int.TryParse(sizes[0], out int rows) && int.TryParse(sizes[1], out int columns))
                    {
                        return (rows, columns);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при чтении настроек: {ex.Message}");
            }

            // Если файл конфигурации не существует или настройки не удалось прочитать, возвращаем значения по умолчанию
            return (5, 5); // Пример значений по умолчанию
        }
    }
}
