using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Space.Service;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System;

namespace Space.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<RowData> _rows;
        private bool _isDataLoaded = false;
        private bool _isDataCalibrated = false;
        private Schedules _schedulesPage;
        public event PropertyChangedEventHandler PropertyChanged;
        private double _coefAX = 1.0, _coefBX = 0.0;
        private double _coefAY = 1.0, _coefBY = 0.0;

        public void SetCalibrationCoefficients(
    double coefAX, double coefBX,
    double coefAY, double coefBY)
        {
            _coefAX = coefAX;
            _coefBX = coefBX;
            _coefAY = coefAY;
            _coefBY = coefBY;

            // Применяем к данным
            foreach (var row in Rows)
            {
                row.Sun1X_C = _coefAX * row.Sun1X + _coefBX;
                row.Sun1Y_C = _coefAY * row.Sun1Y + _coefBY;
            }

            IsDataCalibrated = true;
            UpdateButtonStates();
        }

        // Методы получения коэффициентов
        public double GetCoefAX() => _coefAX;
        public double GetCoefBX() => _coefBX;
        public double GetCoefAY() => _coefAY;
        public double GetCoefBY() => _coefBY;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsDataLoaded
        {
            get => _isDataLoaded;
            set
            {
                if (_isDataLoaded != value)
                {
                    _isDataLoaded = value;
                    OnPropertyChanged();
                    UpdateButtonStates();
                }
            }
        }

        public bool IsDataCalibrated
        {
            get => _isDataCalibrated;
            set
            {
                if (_isDataCalibrated != value)
                {
                    _isDataCalibrated = value;
                    OnPropertyChanged();

                    UpdateButtonStates();
                }
            }
        }

        public ObservableCollection<RowData> Rows
        {
            get => _rows;
            set
            {
                _rows = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _rows = new ObservableCollection<RowData>();
            dataGrid.ItemsSource = _rows;
            DataContext = this;

            UpdateButtonStates();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Выберите файл с данными"
            };

            if (dlg.ShowDialog() != true) return;

            LoadTxtToGrid(dlg.FileName);
        }

        private void LoadTxtToGrid(string path)
        {
            Rows.Clear();
            IsDataLoaded = false;
            IsDataCalibrated = false;

            try
            {
                int rowCount = 0;
                foreach (var line in File.ReadLines(path))
                {
                    var s = line.Trim();
                    if (string.IsNullOrEmpty(s)) continue;

                    if (s.StartsWith("Время", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var parts = s.Split(
                        new[] { ' ', '\t', ';', ',' },
                        StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length < 3)
                        continue;

                    if (TryParseDouble(parts[0], out var time) &&
                        TryParseDouble(parts[1], out var sun1x) &&
                        TryParseDouble(parts[2], out var sun1y))
                    {
                        var rowData = new Space.Service.RowData
                        {
                            Time = time,
                            Sun1X = sun1x,
                            Sun1Y = sun1y
                        };



                        Rows.Add(rowData);
                        rowCount++;
                    }
                }

                if (rowCount > 0)
                {
                    IsDataLoaded = true;
                    UpdateButtonStates();
                }
                else
                {
                    MessageBox.Show("Не удалось загрузить данные из файла.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке файла:\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static bool TryParseDouble(string input, out double value)
        {
            return double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                || double.TryParse(input, NumberStyles.Float, new CultureInfo("ru-RU"), out value);
        }

        private void btnCallibr_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDataLoaded)
            {
                MessageBox.Show("Сначала загрузите данные!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var calibrationPage = new Views.Calibration();
            NavigateToPage(calibrationPage);
        }

        private void btnGraph_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDataLoaded || !IsDataCalibrated)
            {
                MessageBox.Show("Сначала загрузите и откалибруйте данные!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _schedulesPage = new Views.Schedules(this);
            NavigateToPage(_schedulesPage);
        }



        private void btnResult_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsDataLoaded || !IsDataCalibrated)
                {
                    MessageBox.Show("Сначала загрузите и откалибруйте данные!",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                double coefAX = GetCoefAX();
                double coefBX = GetCoefBX();
                double coefAY = GetCoefAY();
                double coefBY = GetCoefBY();

                // Диалог сохранения файла
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                    FileName = $"Отчет_калибровки_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    DefaultExt = ".xlsx",
                    Title = "Сохранить отчет Excel",
                    InitialDirectory = @"D:\C#\Space\Excel" // папка по умолчанию
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName; // полный путь выбранного файла

                    // Создаём отчёт
                    ExcelReportService.CreateReport(
                        Rows.ToList(),
                        coefAX, coefBX,
                        coefAY, coefBY,
                        signalPlot: null,        // без графиков
                        comparisonPlot: null,    // без графиков
                        filePath: filePath       // полный путь
                    );

                    var result = MessageBox.Show(
                        $"Отчет успешно сохранен:\n{filePath}\n\nХотите открыть файл?",
                        "Успешно", MessageBoxButton.YesNo, MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчета:\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void NavigateToPage(Page page)
        {
            MainContent.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(page);
        }

        public void ReturnToMainPage()
        {
            MainFrame.Visibility = Visibility.Collapsed;
            MainContent.Visibility = Visibility.Visible;
            MainFrame.Navigate(null);

            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            btnCallibr.IsEnabled = IsDataLoaded;
            btnGraph.IsEnabled = IsDataLoaded && IsDataCalibrated;
            btnResult.IsEnabled = IsDataLoaded && IsDataCalibrated;

            if (!IsDataLoaded)
            {
                tbName.Text = "Главное меню";
            }
            else if (IsDataLoaded && !IsDataCalibrated)
            {
                tbName.Text = $"Данные загружены требуется калибровка";
            }
            else if (IsDataLoaded && IsDataCalibrated)
            {
                tbName.Text = $"Данные загружены и откалиброваны ";
            }
        }

        public void MarkDataAsCalibrated()
        {
            IsDataCalibrated = true;
            UpdateButtonStates();
        }

        public ObservableCollection<RowData> GetCalibratedData() => Rows;
    }
}