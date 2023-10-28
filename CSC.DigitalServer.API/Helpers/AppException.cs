using System;
using System.Globalization;

namespace CSC.DigitalServer.API.Helpers
{
	public class AppException : Exception
	{
		public AppException() : base() { }

		/// <summary>
		/// AppException
		/// </summary>
		/// <param name="message"></param>
		public AppException(string message) : base(message) { }

		/// <summary>
		/// AppException
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		public AppException(string message, params object[] args)
			: base(String.Format(CultureInfo.CurrentCulture, message, args))
		{
		}
	}
}
