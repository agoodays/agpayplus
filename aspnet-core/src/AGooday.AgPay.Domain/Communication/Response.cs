using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Communication
{
    public class Response<T>
    {
        public bool Success { get; protected set; }
        public string Message { get; protected set; }
        public T Data { get; protected set; }

        public Response(bool success, string message, T data)
        {
            this.Success = success;
            this.Message = message ?? string.Empty;
            this.Data = data;
        }

        /// <summary>
        /// Produces a failure response.
        /// </summary>
        /// <param name="message">Error message.</param>
        public Response(string message) : this(false, message, default(T))
        {
        }

        /// <summary>
        /// Producess a successful response.
        /// </summary>
        /// <param name="data">Returned data.</param>
        public Response(T data) : this(true, string.Empty, data)
        {
        }
    }
}
