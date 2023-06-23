using System.Windows;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Globalization;
using System;
using System.Windows.Data;
using Microsoft.Win32;
using System.Linq;

namespace Topaz1_json_redactor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private ObservableCollection<RowData> rows;
        private string path;
        private string jsonString;
        private JObject jObj;

        public MainWindow()
        {
            InitializeComponent();
            rows = new ObservableCollection<RowData>(); 
            jsonDataGrid.ItemsSource = rows;
        }

        /// <summary>
        /// Метод открытия файла через графический интерфейс
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFile(object sender, RoutedEventArgs e)
        {

            OpenFileDialog fileDialog = new OpenFileDialog();
            bool? response = fileDialog.ShowDialog();

            if (response == true)
            {
                //Считываем json файл в строку, преобразовываем строку в JObject
                path = fileDialog.FileName;
                jsonString = File.ReadAllText(path);
                jObj = JsonConvert.DeserializeObject<JObject>(jsonString);

                //Отбираем из первоначального JObject значение под ключом "parameters",
                //конвертируем в подготовленную структуру данных для удобства обращения с DataGrid
                string parametersJsonString = JsonConvert.SerializeObject(jObj["parameters"], Formatting.Indented);
                rows = JsonConvert.DeserializeObject<ObservableCollection<RowData>>(parametersJsonString);

                //Обновляем данные в графическом интерфейсе
                jsonDataGrid.ItemsSource = rows;
                jsonTextBox.Text = parametersJsonString;
            }
        }

        /// <summary>
        /// Метод сохранения файла через графический интерфейс
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            bool? response = saveFileDialog.ShowDialog();

            if (response == true)
            {
                //Собираем отредактированную часть json, с основным объёмом, сохраняем в файл.
                jObj["parameters"] = JArray.FromObject(rows);
                File.WriteAllText(saveFileDialog.FileName, jObj.ToString());
                MessageBox.Show("Файл сохранен");
            }
            
        }
        /// <summary>
        /// Метод обновления текстового поля в соответствии с таблицей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateJsonTextBox(object sender, SelectedCellsChangedEventArgs e)
        {
            jsonTextBox.Text = JsonConvert.SerializeObject(rows, Formatting.Indented);
        }

    }

    /// <summary>
    /// Конвертер необходимый для проверки содержания подстроки в строке при использовании DataTrigger
    /// </summary>
    public class SubstringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object RowData, CultureInfo culture)
        {
            if (value is string str && RowData is string substring)
            {
                return str.Contains(substring);
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object RowData, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Структура данных для работы с DataGrid
    /// </summary>
    public class RowData
    {
        public string Name { get; set; } 
        public dynamic Value { get; set; }
        public string Group { get; set; }
    }
}
