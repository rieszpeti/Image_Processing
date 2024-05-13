using System.Diagnostics.Metrics;
using System.Reflection.Metadata.Ecma335;

namespace WebApi.Startup
{
    public class ImageProcessMetrics
    {
        public const string MeterName = "ImageProcess.Api";

        private readonly Counter<long> _requestCounter;
        private readonly Histogram<double> _requestDuration;

        public ImageProcessMetrics(IMeterFactory meterFactory)
        {
            var meter = meterFactory.Create(MeterName);

            _requestCounter = meter.CreateCounter<long>(
                "imageProcess.api.imageProcess_requests.count");

            _requestDuration = meter.CreateHistogram<double>(
                "imageProcess.api.imageProcess_requests.count");
        }

        public void IncreaseImageRequestCount() => _requestCounter.Add(1);

        public TrackedRequestDuration MeasureRequestDuration() => new TrackedRequestDuration(_requestDuration);
    }

    public class TrackedRequestDuration : IDisposable
    {
        private readonly long _requestStartTime = TimeProvider.System.GetTimestamp();
        private readonly Histogram<double> _histogram;

        public TrackedRequestDuration(Histogram<double> histogram)
        {
            _histogram = histogram;
        }

        public void Dispose()
        {
            var elapsed = TimeProvider.System.GetElapsedTime(_requestStartTime);
            _histogram.Record(elapsed.Microseconds);
        }
    }
}
