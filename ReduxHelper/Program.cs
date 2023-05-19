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
            args ),
        (o =>
        {
            (new CreateActionBoilerplateAction() ?? throw new NullReferenceException( "No action found" )).ExecuteAsync( o ).Wait( 60000 );
        }) ),
    (errors =>
    {
        foreach ( var error in errors ) {
            Log.Error( $"{error.Tag}: {error}" );
        }
    })
    );

static void CreateLogger()
{
    Log.Logger = new LoggerConfiguration().ReadFrom.Configuration( new ConfigurationBuilder().AddJsonFile( "appsettings.json" ).Build() ).CreateLogger();
    Log.Logger.Debug( "Logger created" );
}