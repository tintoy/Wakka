using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wakka
{
	/// <summary>
	///		Configuration logic for Akka.NET.
	/// </summary>
	public static class AkkaConfiguration
	{
		/// <summary>
		///		The basic actor system configuration for Wakka.
		/// </summary>
		/// <returns>
		///		The Akka configuration.
		/// </returns>
		public static Config Basic()
		{
			// You know what? It was a shitty decision to use HOCON in Akka.NET. Especially since there's no way to configure the system without a HOCON string of some sort.
			// Why can't the Akka.NET guys EVER use something that already exists in the .NET platform instead of pulling over yet another AKKA / Scala idiom? Talk about NIH >:-(
			return ConfigurationFactory.ParseString(@"
				akka {
					loglevel = INFO
					loggers = [
						""Akka.Logger.Serilog.SerilogLogger, Akka.Logger.Serilog""
					]
				}
			");
		}

		/// <summary>
		///		Props for well-known actors.
		/// </summary>
		public static class PropsFor
		{
		}
	}
}
