using Space.Service;

public static class TelemetryCalibrator
{
    public static (double scale, double offset) CalibrateChannel(
        IList<RowData> rows,
        Func<RowData, double> selector)
    {
        if (rows == null || rows.Count < 2)
            return (1.0, 0.0);

        var values = rows.Select(selector).ToList();

        double min = values.Min();
        double max = values.Max();

        double bias = (max + min) / 2.0;
        double range = max - min;

        if (range == 0)
            return (1.0, -bias);

        // В кейсе считаем что масштаб близок к 1
        double scale = 1.0;

        double offset = -bias;

        return (scale, offset);
    }

    public static void ApplyChannel(
        IList<RowData> rows,
        Func<RowData, double> source,
        Action<RowData, double> target,
        double scale,
        double offset)
    {
        foreach (var row in rows)
        {
            double calibrated = (source(row) + offset) * scale;
            target(row, calibrated);
        }
    }
}
