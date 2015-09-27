using Microsoft.Owin.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Wakka
{
	/// <summary>
	///		Quick-and-dirty demo of an Akka actor, driven by ASP.NET WebAPI.
	/// </summary>
	static class Program
	{
		/// <summary>
		///		The main program entry-point.
		/// </summary>
		static void Main()
		{
			ConfigureLogging();

			SynchronizationContext.SetSynchronizationContext(
				new SynchronizationContext()
			);
			try
			{
				// Save the set of existing trace listeners so we can trim OWIN's listeners(s) out after startup.
				HashSet<TraceListener> existingTraceListeners = GetCurrentTraceListeners();

				Log.Information("Starting up...");
				using (WebApp.Start("http://+:19123", OwinConfiguration.Configure))
				{
					RemoveNewListeners(existingTraceListeners); // Goodbye, thanks for nothing :-)

					Log.Information("Running (press enter to shut down.");
					Console.ReadLine();

					Log.Information("Shutting down...");
				}
				Log.Information("Terminated.");
			}
			catch (Exception eUnexpected)
			{
				AggregateException aggregateException = eUnexpected as AggregateException;
				if (aggregateException != null)
				{
					aggregateException.Flatten().Handle(innerException =>
					{
						Log.Error(
							innerException,
							"Unexpected error: {ErrorMessage}",
							innerException.Message
						);

						return true;
					});
				}
				else
				{
					Log.Error(
						eUnexpected,
						"Unexpected error: {ErrorMessage}",
						eUnexpected.Message
					);
				}
			}
		}

		/// <summary>
		///		Get all currently-configured trace listeners.
		/// </summary>
		/// <returns>
		///		The set of trace listeners currently configured in <see cref="Trace.Listeners"/>.
		/// </returns>
		static HashSet<TraceListener> GetCurrentTraceListeners()
		{
			return new HashSet<TraceListener>(
				Trace.Listeners.Cast<TraceListener>()
			);
		}

		/// <summary>
		///		Remove all newly-configured trace listeners (i.e. any not present in the supplied set of previous trace listeners).
		/// </summary>
		/// <param name="previousTraceListeners">
		///		The set of previously-configured trace listeners.
		/// </param>
		static void RemoveNewListeners(HashSet<TraceListener> previousTraceListeners)
		{
			if (previousTraceListeners == null)
				throw new ArgumentNullException("previousTraceListeners");

			HashSet<TraceListener> currentTraceListeners = GetCurrentTraceListeners();
			currentTraceListeners.ExceptWith(previousTraceListeners);
			foreach (TraceListener owinListener in currentTraceListeners)
				Trace.Listeners.Remove(owinListener);
		}

		/// <summary>
		///		Configure the global logger.
		/// </summary>
		static void ConfigureLogging()
		{
			Log.Logger =
				new LoggerConfiguration()
					.MinimumLevel.Verbose()
					.Enrich.FromLogContext()
					.Enrich.WithThreadId()
					.WriteTo.Trace()
					.WriteTo.LiterateConsole()
					.CreateLogger();
		}
	}
}
