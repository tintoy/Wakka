using Akka.Actor;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Wakka.Actors;
using Wakka.Messages;

namespace Wakka.Controllers
{
	/// <summary>
	///		The greeting API controller.
	/// </summary>
	[RoutePrefix("greet")]
	public sealed class GreetController
		: ApiController
	{
		/// <summary>
		///		The Wakka actor system
		/// </summary>
		readonly ActorSystem _actorSystem;

		/// <summary>
		///		Create a new greeting API controller.
		/// </summary>
		/// <param name="actorSystem">
		///		The Wakka actor system.
		/// </param>
		public GreetController(ActorSystem actorSystem)
		{
			if (actorSystem == null)
				throw new ArgumentNullException("actorSystem");

			_actorSystem = actorSystem;
		}

		/// <summary>
		///		Greet the caller.
		/// </summary>
		/// <param name="firstName">
		///		The caller's first name.
		/// </param>
		/// <param name="lastName">
		///		The caller's last name.
		/// </param>
		/// <param name="salutation">
		///		The caller's salutation.
		/// </param>
		/// <returns>
		///		A greeting (HTTP 200) or an error response (HTTP 400, 500, or 504).
		/// </returns>
		[HttpGet, Route]
		public async Task<IHttpActionResult> GreetAsync(
			[Required(ErrorMessage = "What's your first name?")]
			string firstName = null,
			[Required(ErrorMessage = "What's your last name?")]
			string lastName = null,
			[Required(ErrorMessage = "What's your salutation (i.e. Mr, Ms, Dr)?")]
			string salutation = null
		)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			IActorRef greeter = GetGreeter();

			string greeting;
			try
			{
                greeting = await greeter.Ask<string>(
					message: new Introduce(firstName, lastName, salutation),
					timeout: TimeSpan.FromSeconds(5)
				);
			}
			catch (TimeoutException)
			{
				return Content(HttpStatusCode.GatewayTimeout, "Timed out waiting for the greeter to respond.");
			}

			return Ok(greeting);
		}

		/// <summary>
		///		Get a reference to the <see cref="Greeter"/> actor.
		/// </summary>
		/// <returns>
		///		An <see cref="IActorRef"/> for the greeter.
		/// </returns>
		IActorRef GetGreeter()
		{
			// This actor would become a bottleneck. Consider creating a shortest-queue router / pool of greeters to handle requests.
			// But then how do they share state? Depends how up-to-date their information needs to be.
			return _actorSystem.ActorOf<Greeter>("Greeter");
		}
	}
}
