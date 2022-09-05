using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using ReduxHelper.Actions;
using ReduxHelper.Options;
using Serilog;

CreateLogger();

ParserResultExtensions.WithNotParsed(
    ParserResultExtensions.WithParsed(
        Parser.Default.ParseArguments<ArgumentOptions>(
            (IEnumerable<string>)args),
        (o =>
        {
            int action = (int)o.Action;
            (new CreateActionBoilerplateAction() ?? throw new NullReferenceException("No action found")).ExecuteAsync(o).Wait(60000);
        })),
    (errors => {
        foreach(var error in errors)
        {
            Log.Error($"{error.Tag.ToString()}: {error.ToString()}");
        }
    })
    );

static void CreateLogger()
{
    Log.Logger = ConfigurationLoggerConfigurationExtensions.Configuration(new LoggerConfiguration().ReadFrom, (IConfiguration)JsonConfigurationExtensions.AddJsonFile(new ConfigurationBuilder(), "appsettings.json").Build(), null).CreateLogger();
    Log.Logger.Debug("Logger created");
}