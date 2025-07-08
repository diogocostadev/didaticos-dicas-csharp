using Dica07.LoggingCorreto;

// Configurar o host com logging
var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Debug);
    })
    .ConfigureServices(services =>
    {
        services.AddTransient<LoggingDemo>();
    })
    .Build();

Console.WriteLine("🎯 Demonstração: Logging Correto no .NET");
Console.WriteLine("=" + new string('=', 50));
Console.WriteLine();

// Obter o serviço LoggingDemo do container DI
var loggingDemo = host.Services.GetRequiredService<LoggingDemo>();

// Executar demonstrações
Console.WriteLine("🚀 Iniciando demonstração de logging...");
Console.WriteLine();

Console.WriteLine(new string('-', 60));
loggingDemo.DemonstrarLoggingIncorreto();

Console.WriteLine(new string('-', 60));
loggingDemo.DemonstrarLoggingCorreto();

Console.WriteLine(new string('-', 60));
loggingDemo.DemonstrarFiltragePorParametros();

Console.WriteLine(new string('-', 60));
loggingDemo.DemonstrarLogScopes();

Console.WriteLine(new string('-', 60));
loggingDemo.DemonstrarLogEventIds();

Console.WriteLine(new string('-', 60));
loggingDemo.DemonstrarLoggerFactoryCustom();

Console.WriteLine(new string('-', 60));
loggingDemo.CompararPerformance();

Console.WriteLine("🎉 Demonstração concluída!");
Console.WriteLine();
Console.WriteLine("💡 Resumo da Dica:");
Console.WriteLine("   • Use message templates, não string interpolation");
Console.WriteLine("   • Nomeie parâmetros de forma descritiva");
Console.WriteLine("   • Preserve structured logging para filtragem");
Console.WriteLine("   • Use scopes para agrupar logs relacionados");
Console.WriteLine("   • Configure níveis de log apropriados");
Console.WriteLine("   • EventIds ajudam na categorização");

// Aguardar um pouco para garantir que todos os logs sejam processados
await Task.Delay(100);
