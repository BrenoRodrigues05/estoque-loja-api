namespace APILojaEstoque.Logging
{
    public class CustomLogggerProviderConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;

        public int EventId { get; set; } = 0;
    }
}
