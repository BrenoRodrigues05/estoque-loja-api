
namespace APILojaEstoque.Logging
{
    public class CustomerLogger : ILogger
    {
        readonly string loggername;
        readonly CustomLogggerProviderConfiguration loggerconfig;

        public CustomerLogger(string name, CustomLogggerProviderConfiguration config)
        {
            loggername = name;
            loggerconfig = config;
        }
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == loggerconfig.LogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, 
            Func<TState, Exception?, string> formatter)
        {
            string horario = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string message = $"{horario}/// {logLevel.ToString()} : {eventId.Id} - {formatter (state, exception)}";

            EscreverTextoNoArquivo(message);
        }

        private void EscreverTextoNoArquivo(string message)
        {
            string Caminho = @"C:\Users\CSM\Desktop\CURSOS\ASP NET CORE\LOGGING\LojaEstoque.txt";

            using (StreamWriter streamwriter = new StreamWriter(Caminho, true))
            {
                try
                {
                    streamwriter.WriteLine(message);
                    streamwriter.Close();
                }
                catch (Exception)
                {
                    Console.WriteLine($"[ERRO NO LOG] Falha ao escrever no arquivo: {message}");
                }
            }
        }
    }
}
