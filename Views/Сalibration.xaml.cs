using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Space.Views
{
    public partial class Calibration : Page
    {
        public Calibration()
        {
            InitializeComponent();
        }

        private void PerformCalibration_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            var rows = mainWindow?.Rows;

            if (rows == null || rows.Count == 0)
            {
                MessageBox.Show(
                    "Сначала загрузите данные телеметрии",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            // 1. Смещение нуля (B)
            double meanX = rows.Average(r => r.Sun1X);
            double meanY = rows.Average(r => r.Sun1Y);

            // Общее смещение (упрощённая модель)
            double offsetB = (meanX + meanY) / 2.0;

            // 2. Масштаб (A)
            var centered = rows.Select(r =>
            {
                double cx = r.Sun1X - offsetB;
                double cy = r.Sun1Y - offsetB;
                return Math.Sqrt(cx * cx + cy * cy);
            });

            double avgMagnitude = centered.Average();

            double scaleA = avgMagnitude > 0
                ? 1.0 / avgMagnitude
                : 1.0;

            // 3. Применение к данным
            foreach (var row in rows)
            {
                row.Sun1X_C = scaleA * row.Sun1X + offsetB;
                row.Sun1Y_C = scaleA * row.Sun1Y + offsetB;
            }

            // 4. Отображение в UI
            TbOffsetX.Text = offsetB.ToString("F4");
            TbOffsetY.Text = offsetB.ToString("F4");
            TbScaleX.Text = scaleA.ToString("F4");
            TbScaleY.Text = scaleA.ToString("F4");

            // 5. Сохранение коэффициентов
            mainWindow.SetCalibrationCoefficients(scaleA, offsetB, scaleA, offsetB);


            MessageBox.Show(
                "Калибровка успешно выполнена",
                "Успех",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow)?.ReturnToMainPage();
        }
    }
}
