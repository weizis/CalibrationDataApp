using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Linq;
using System.Windows.Controls;
using Space.Views;
using OxyPlot.Legends;
using System.Windows;
using OxyPlot.Wpf;

namespace Space.Views
{
    public partial class Schedules : Page
    {
        private MainWindow _main;

        public Schedules(MainWindow main)
        {
            InitializeComponent();
            _main = main;
            Loaded += Schedules_Loaded;
        }
        public PlotView SignalPlotView => chartSignal;        // chartSignal – твой PlotView для сигнала
        public PlotView ComparisonPlotView => chartComparison; // chartComparison – PlotView для сравнения

        private void btnSignal_Click(object sender, RoutedEventArgs e)
        {
            if (_main == null || _main.Rows.Count == 0) return;

            var calibrated = _main.Rows.Select(r => r.Sun1X_C).ToList();
            if (calibrated.All(v => v == 0))
                calibrated = _main.Rows.Select(r => r.Sun1X).ToList();

            var model = new PlotModel { Title = "Сигнал" };

            // Линия сигнала
            var series = new LineSeries { Title = "Сигнал", Color = OxyColors.Blue };
            for (int i = 0; i < calibrated.Count; i++)
                series.Points.Add(new DataPoint(i, calibrated[i]));

            model.Series.Add(series);

            // Легенда
            var legend = new Legend
            {
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.TopRight,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendTitle = "Легенда"
            };
            model.Legends.Add(legend);

            plotView.Model = model;
        }

        private void btnSimul_Click(object sender, RoutedEventArgs e)
        {
            if (_main == null || _main.Rows.Count == 0) return;

            var raw = _main.Rows.Select(r => r.Sun1X).ToList();
            var calib = _main.Rows.Select(r => r.Sun1X_C).ToList();
            if (calib.All(v => v == 0)) calib = raw.ToList();

            var model = new PlotModel { Title = "Сравнение" };

            // Две линии
            var s1 = new LineSeries { Title = "Исходный", Color = OxyColors.Red };
            var s2 = new LineSeries { Title = "Калиброванный", Color = OxyColors.Green };

            for (int i = 0; i < raw.Count; i++)
            {
                s1.Points.Add(new DataPoint(i, raw[i]));
                s2.Points.Add(new DataPoint(i, calib[i]));
            }

            model.Series.Add(s1);
            model.Series.Add(s2);

            // Легенда
            var legend = new Legend
            {
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.TopRight,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendTitle = "Легенда"
            };
            model.Legends.Add(legend);

            plotView.Model = model;
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.ReturnToMainPage();
        }
        private void Schedules_Loaded(object sender, RoutedEventArgs e)
        {
            btnSignal_Click(null, null);
        }
    }
}
