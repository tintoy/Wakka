using Akka.Actor;
using System;
using System.Collections.Generic;
using Wakka.Messages;

namespace Wakka.Actors
{
	/// <summary>
	///		A (hackneyed) example of an actor that greets callers to the web API.
	/// </summary>
	public sealed class Greeter
		: TypedActor, IHandle<Introduce>
	{
		/// <summary>
		///		The span of time for which any given caller is remembered.
		/// </summary>
		public static readonly TimeSpan MemoryWindowPeriod = TimeSpan.FromSeconds(5);

		/// <summary>
		///		The date / time each caller was last greeted.
		/// </summary>
		readonly Dictionary<string, DateTimeOffset> _lastGreeted = new Dictionary<string, DateTimeOffset>();

		/// <summary>
		///		Create a new <see cref="Greeter"/>.
		/// </summary>
		public Greeter()
		{
		}

		/// <summary>
		///		Handle an introduction.
		/// </summary>
		/// <param name="introduction">
		///		The introduction message.
		/// </param>
		public void Handle(Introduce introduction)
		{
			if (introduction == null)
				throw new ArgumentNullException("introduction");

			string fullName = $"{introduction.Salutation}. {introduction.FirstName} {introduction.LastName}";

			DateTimeOffset now = DateTimeOffset.Now;
			DateTimeOffset lastGreeted;
			_lastGreeted.TryGetValue(fullName, out lastGreeted);

			if (now - lastGreeted > MemoryWindowPeriod)
			{
				// Either we've never met them, or we forgot.
				Sender.Tell(
					$"Hello, {introduction.Salutation}. {introduction.LastName}, nice to meet you."
				);
			}
			else
			{
				Sender.Tell(
					$"Hello again, {introduction.FirstName} :-)"
				);
			}

			_lastGreeted[fullName] = now;
		}
	}
}
