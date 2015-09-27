using Akka.Actor;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin.BuilderProperties;
using Owin;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Http;

namespace Wakka
{
	/// <summary>
	///		OWIN configuration logic.
	/// </summary>
	public static class OwinConfiguration
	{
		/// <summary>
		///		Configure the application.
		/// </summary>
		/// <param name="app">
		///		The application to configure.
		/// </param>
		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "HttpConfiguration's lifetime is managed by OWIN")]
		public static void Configure(IAppBuilder app)
		{
			if (app == null)
				throw new ArgumentNullException("app");

			IContainer container = IocConfiguration.BuildContainer(actorSystemName: "Wakka");
			app.UseAutofacMiddleware(container);

			HttpConfiguration webApiConfiguration = new HttpConfiguration
			{
				// Only used when mapping attribute routes.
				DependencyResolver = new AutofacWebApiDependencyResolver(container)
			};
			app.UseAutofacWebApi(webApiConfiguration); // Share OWIN lifetime scope.

			webApiConfiguration.MapHttpAttributeRoutes();
			webApiConfiguration.EnsureInitialized();

			app.UseWebApi(webApiConfiguration);

			// Explicitly start the actor system when the OWIN app is started.
			ActorSystem actorSystem = container.ResolveNamed<ActorSystem>("Wakka");

			// Try to gracefully shut down the actor system when the host is shutting down.
			AppProperties appProperties = new AppProperties(app.Properties);
			appProperties.OnAppDisposing.Register(() =>
			{
				actorSystem.Shutdown();
				actorSystem.AwaitTermination(
					timeout: TimeSpan.FromSeconds(5)
				);
            });
        }
	}
}
