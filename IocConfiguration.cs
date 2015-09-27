using Akka.Actor;
using Autofac;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wakka
{
	/// <summary>
	///		IOC configuration logic.
	/// </summary>
	public static class IocConfiguration
	{
		/// <summary>
		///		This assembly.
		/// </summary>
		static readonly Assembly ThisAssembly = typeof(IocConfiguration).Assembly;

		/// <summary>
		///		Build a container with required components and services.
		/// </summary>
		/// <param name="actorSystemName">
		///		The name of the actor system to create.
		/// </param>
		/// <returns>
		///		The container.
		/// </returns>
		public static IContainer BuildContainer(string actorSystemName)
		{
			if (String.IsNullOrWhiteSpace(actorSystemName))
				throw new ArgumentException("Argument cannot be null, empty, or entirely componsed of whitespace: 'actorSystemName'.", "actorSystemName");

			ContainerBuilder containerBuilder = new ContainerBuilder();

			containerBuilder
				.Register(
					context => ActorSystem.Create(
						actorSystemName,
						AkkaConfiguration.Basic()
					)
				)
				.As<ActorSystem>()
				.SingleInstance();

			containerBuilder
				.RegisterApiControllers(ThisAssembly);

			return containerBuilder.Build();
		}
	}
}
