using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSC.DigitalServer.Services
{
	/// <summary>
	/// ServiceException
	/// </summary>
	public class ServiceException : Exception
	{
		/// <summary>
		/// ServiceException
		/// </summary>
		public ServiceException() : base() { }

		/// <summary>
		/// ServiceException
		/// </summary>
		/// <param name="message"></param>
		public ServiceException(string message) : base(message) { }

		/// <summary>
		/// ServiceException
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		public ServiceException(string message, params object[] args)
			: base(String.Format(CultureInfo.CurrentCulture, message, args))
		{
		}
	}
}
