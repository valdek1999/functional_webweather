using System;

namespace WebWeather.Models
{
    public record ErrorViewModel
    {
        public string RequestId { get; init; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
