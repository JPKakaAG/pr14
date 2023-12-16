using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace prac13
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int[,] matrix;
        private ContextMenu inputDataContextMenu;
        private ContextMenu resultContextMenu;
        public MainWindow()
        {
            InitializeComponent();
            InitializeContextMenu();
            Loaded += MainWindow_Loaded;

            // Добавление обработчика события к событию SelectedCellsChanged
            DGarray.SelectedCellsChanged += dataGrid_SelectedCellsChanged;
        }
        
        private void Window_Activated(object sender, EventArgs e)
        {
            tbColumn.Text = Data.ColumnCount.ToString();
            tbRows.Text = Data.RowCount.ToString();
            int[,] matrix = new int[Data.RowCount, Data.ColumnCount];
            visulaarray.Fill2Array(matrix);
            DGarray.ItemsSource = visulaarray.ToDataTable(matrix).DefaultView;
        }
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            // Создание и отображение окна настроек
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
       
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Загрузка размера таблицы из файла конфигурации
                (int rows, int columns) = ConfigurationSettings.LoadTableSize();
                Data.ColumnCount = columns;
                Data.RowCount = rows;
                tbColumn.Text = columns.ToString();
                tbRows.Text = rows.ToString();
                // Применение загруженного размера таблицы
                int[,] matrix = new int[rows, columns];
                visulaarray.Fill2Array(matrix);
                DGarray.ItemsSource = visulaarray.ToDataTable(matrix).DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке настроек: {ex.Message}");
            }
        }
        private void InitializeContextMenu()
        {
            // Создаем контекстное меню для блока "Исходные данные"
            inputDataContextMenu = new ContextMenu();

            // Создаем пункт меню "Очистить"
            MenuItem clearMenuItem = new MenuItem();
            clearMenuItem.Header = "Очистить";
            clearMenuItem.Click += ClearInputs_Click;
            inputDataContextMenu.Items.Add(clearMenuItem);
            // Назначаем контекстное меню блоку "Исходные данные"
            GIntial.ContextMenu = inputDataContextMenu;

            // Создаем контекстное меню для блока "Результат"
            resultContextMenu = new ContextMenu();

            MenuItem clearRezItem = new MenuItem();
            clearRezItem.Header = "Очистить";
            clearRezItem.Click += ClearRezult_Click;
            // Назначаем контекстное меню блоку "Результат"
            GRezult.ContextMenu = resultContextMenu;           
        }

        private void ClearInputs_Click(object sender, RoutedEventArgs e)
        {
            // Очищаем значения текстовых полей блока "Исходные данные"
            tbRows.Text = "";
            tbColumn.Text = "";
            
        }
        private void ClearRezult_Click(object sender, RoutedEventArgs e)
        {
            DGarray.ItemsSource = null;
            tblStatus.Text = "";
            tbResult.Text = "";
        }
        private void btnSaveMatrix_Click(object sender, RoutedEventArgs e)
        {
            if (DGarray.ItemsSource != null)
            {
                MatrixManager manager = new MatrixManager();

                // Сохранение матрицы в файл
                manager.SaveMatrix(matrix);
                MessageBox.Show("Матрица успешно сохранена.");
            }
            else
            {
                MessageBox.Show("Создай матрицу");
                return;
            }
        }
        private void btnOpenMatrix_CLick(object sender, RoutedEventArgs e)
        {
            MatrixManager manager = new MatrixManager();

            int[,] matrix = manager.LoadMatrix();

            if (matrix != null)
            {
                MessageBox.Show("Матрица успешно загружена:");
                int rows = matrix.GetLength(0);
                int columns = matrix.GetLength(1);

                DGarray.ItemsSource = visulaarray.ToDataTable(matrix).DefaultView;
            }
            else
            {
                MessageBox.Show("Файл с матрицей не существует или матрица не была сохранена.");
                return;
            }
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (DGarray.ItemsSource != null)
            {
                // Получение размеров матрицы
                int rows = DGarray.Items.Count;
                int columns = DGarray.Columns.Count;

                // Создание двумерного массива для хранения матрицы
                int[,] matrix = new int[rows, columns];

                // Итерация по ячейкам DataGrid и заполнение массива матрицей
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        DataGridCellInfo cellInfo = new DataGridCellInfo(DGarray.Items[i], DGarray.Columns[j]);
                        if (cellInfo.Column.GetCellContent(cellInfo.Item) is TextBlock cellContent)
                        {
                            if (int.TryParse(cellContent.Text, out int cellValue))
                            {
                                matrix[i, j] = cellValue;
                            }
                            else
                            {
                                // Обработка некорректных значений ячейки (например, если в ячейке не число)
                            }
                        }
                    }
                }
                List<int> columnSums = new List<int>();
                for (int col = 2; col <= columns; col += 2)
                {
                    int sum = 0;

                    for (int row = 0; row < rows; row++)
                    {
                        sum += matrix[row, col - 1];
                    }
                    columnSums.Add(sum);
                }
                tbResult.Text = "Сумма чётных столбцов:\n" + string.Join("\n", columnSums);
            }
            else if (int.TryParse(tbRows.Text, out int rows) && int.TryParse(tbColumn.Text, out int columns))
            { 
                if (rows > 0 && columns > 0)
                {
                    matrix = new int[rows, columns];

                    visulaarray.Fill2Array(matrix);

                    List<int> columnSums = new List<int>();

                    for (int col = 2; col <= columns; col += 2)
                    {
                        int sum = 0;

                        for (int row = 0; row < rows; row++)
                        {
                            sum += matrix[row, col - 1];
                        }
                        columnSums.Add(sum);
                    }
                    DGarray.ItemsSource = visulaarray.ToDataTable(matrix).DefaultView;
                    tbResult.Text = "Сумма чётных столбцов:\n" + string.Join("\n", columnSums);
                }
                else
                {
                    MessageBox.Show("Введи положительные строки и столбцы");
                }
            }
            else
            {
                MessageBox.Show("Введи строки и столбцы");
            }

        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Выполнил лучший разработчик: Девяткин Вадим Евгеньевич\r\nПрактическая №13 \r\n\r\nДаны координаты двух противоположных вершин прямоугольника: (x1, y1), (x2, y2).\r\nСтороны прямоугольника параллельны осям координат. Найти периметр и площадь данного прямоугольника.\r\n\r\nДан размер файла в байтах. Используя операцию деления нацело, найти количество полных килобайтов, которые занимает данный файл (1 килобайт = 1024 байта)");
        }
        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            // Получение размеров таблицы
            int rows = DGarray.Items.Count - 1;
            int columns = DGarray.Columns.Count;

            // Получение номера выделенной ячейки
            int selectedRow = DGarray.SelectedIndex;
            int selectedColumn = DGarray.CurrentCell.Column.DisplayIndex;

            // Обновление строки статуса
            tblStatus.Text = $"Размер таблицы: {rows}x{columns} | Выделенная ячейка: [{selectedRow + 1}, {selectedColumn + 1}]";
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите выйти?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
