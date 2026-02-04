namespace AGooday.AgPay.Logging.Serilog
{
    public class SerilogOptions
    {
        public string SystemName { get; set; } = "Unknown";
        public string Version { get; set; } = "1.0.0";
        public string BaseLogPath { get; set; } = "logs";
        public bool EnableSeq { get; set; } = true;
        public string SeqUrl { get; set; }
        public string SeqApiKey { get; set; }

        // 可选：允许自定义 outputTemplate
        public string OutputTemplate { get; set; } =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{SystemName}:{Version}][{CRequestId}]-[{RequestId}]:{UserId} [{ThreadId}] {Level:u3} {SourceContext} - {Message:lj}{NewLine}{Exception}";
    }
}
