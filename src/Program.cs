
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace kms
{

	using System.CommandLine;
	using System.CommandLine.Parsing;

	using kms.View;
	using kms.Services.Impl;
	using kms.Util;
	using kms.Models;
    using log4net;
    using System.Threading.Tasks;

    /// <summary>
    /// Main entry point for Kennel Management System.
    /// </summary>
    class Program
	{

        private static  ILog LOG => LogManager.GetLogger(typeof(Program));

		private readonly Option<FileInfo> configOption = new("--config")
		{
			Description = "Path to configuration file."
		};

		static int Main(string[] args) =>
			new Program().Run(args);

		private int Run(string[] args)
		{
			// initialise commandline parser
			RootCommand cli = new(StringUtils.APPLICATION_NAME);
			cli.Options.Add(configOption);
			cli.SetAction(ParseCommandLine);

			// launch application based on CLI input
			return cli.Parse(args).Invoke();
		}

		private async Task<int> ParseCommandLine(ParseResult parseResult)
		{
			if (parseResult.Errors.Count > 0)
			{
				foreach (ParseError parseError in parseResult.Errors)
				{
					Console.Error.WriteLine(parseError.Message);
				}
				return 1;
			}
			ConfigurationService configurationService = new(parseResult.GetValue(configOption)?.FullName);
			return new BaseNavigation(configurationService).Launch();
		}

	}

}