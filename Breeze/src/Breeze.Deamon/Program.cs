using System.Threading;
using Microsoft.Extensions.Logging;
using Stratis.Bitcoin;
using Stratis.Bitcoin.Builder;
using Stratis.Bitcoin.Configuration;
using Stratis.Bitcoin.Logging;

namespace Breeze.Deamon
{
	public class Program
    {
        public static void Main(string[] args)
        {
			// configure Full Node
			Logs.Configure(new LoggerFactory().AddConsole(LogLevel.Trace, false));
			NodeSettings nodeSettings = NodeSettings.FromArguments(args);

			var node = (FullNode)new FullNodeBuilder()
				.UseNodeSettings(nodeSettings)	
				//.UseWallet()
				//.UseApi()
				//.UseBlockNotification()			
				.Build();

			System.Console.WriteLine();

			// == shut down thread ==
			new Thread(() =>
			{

				System.Console.WriteLine("Press one key to stop");
				System.Console.ReadLine();
				node.Dispose();
			})
			{
				IsBackground = true //so the process terminates
			}.Start();

			// start Full Node - this will also start the API
			node.Start();
			node.WaitDisposed();
			node.Dispose();
			
		}
    }
}
