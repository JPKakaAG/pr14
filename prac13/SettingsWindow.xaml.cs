using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace prac13
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            tbColumns.Text = Convert.ToString(Data.ColumnCount);
            tbRows.Text =  Convert.ToString(Data.RowCount);
        }      
        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            
            // Чтение введенных значений из TextBox
            if (int.TryParse(tbRows.Text, out int rows) && int.TryParse(tbColumns.Text, out int columns))
            {
                try
                {
                    Data.ColumnCount = columns;
                    Data.RowCount = rows;
                    // Сохранение размера таблицы в файле конфигурации
                    ConfigurationSettings.SaveTableSize(rows, columns);
                    MessageBox.Show("Настройки сохранены.");                  
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении настроек: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Некорректные значения для размера таблицы!");
            }
        }
    }
}
