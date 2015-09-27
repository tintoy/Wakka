using System;

namespace Wakka.Messages
{
	/// <summary>
	///		Message representing the introduction of an API caller to the greeter.
	/// </summary>
	public sealed class Introduce
	{
		/// <summary>
		///		Create a new <see cref="Introduce"/> message.
		/// </summary>
		/// <param name="firstName">
		///		The caller's first name.
		/// </param>
		/// <param name="lastName">
		///		The caller's last name.
		/// </param>
		/// <param name="salutation">
		///		The caller's salutation (e.g. Mr, Ms, Mrs, Dr, etc).
		/// </param>
		public Introduce(string firstName, string lastName, string salutation)
		{
			if (String.IsNullOrWhiteSpace(firstName))
				throw new ArgumentException("Argument cannot be null, empty, or entirely componsed of whitespace: 'firstName'.", "firstName");

			if (String.IsNullOrWhiteSpace(lastName))
				throw new ArgumentException("Argument cannot be null, empty, or entirely componsed of whitespace: 'lastName'.", "lastName");

			if (String.IsNullOrWhiteSpace(salutation))
				throw new ArgumentException("Argument cannot be null, empty, or entirely componsed of whitespace: 'salutation'.", "salutation");

			FirstName = firstName;
			LastName = lastName;
			Salutation = salutation;
		}

		/// <summary>
		///		The caller's first name.
		/// </summary>
		public string FirstName
		{
			get;
		}

		/// <summary>
		///		The caller's last name.
		/// </summary>
		public string LastName
		{
			get;
		}

		/// <summary>
		///		The caller's salutation (e.g. Mr, Ms, Mrs, Dr, etc).
		/// </summary>
		public string Salutation
		{
			get;
		}
	}
}
