// <copyright file="AsyncRequestServer.cs" company="kdehaan">
// Copyright (c) kdehaan. All rights reserved.
// </copyright>

namespace CloudDesignPatterns.AsyncRequestReply
{
    using System.Net;
    using CloudDesignPatterns.BaseComponents;

    /// <summary>
    /// Server that handles the Async Request Reply pattern. See: <seealso href="https://learn.microsoft.com/en-us/azure/architecture/patterns/async-request-reply">Async Request Reply</seealso>.
    /// </summary>
    /// <param name="port">listening port.</param>
    internal class AsyncRequestServer : BaseServerApp
    {
        private Dictionary<string, DateTime> processing;
        private Dictionary<string, DateTime> completed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRequestServer"/> class.
        /// </summary>
        /// <param name="port">listening port.</param>
        public AsyncRequestServer(int port)
            : base(port)
        {
            this.processing = new Dictionary<string, DateTime>();
            this.completed = new Dictionary<string, DateTime>();

            this.Endpoints.Add("asyncPayload", payload =>
            {
                var requestInProgress = this.processing.TryGetValue(payload, out DateTime startTime);
                if (requestInProgress)
                {
                    this.ProcessRequest(payload, startTime);
                }

                var requestCompleted = this.completed.TryGetValue(payload, out DateTime completedTime);
                if (!requestInProgress)
                {
                    if (requestCompleted)
                    {
                        // this implementation will maintain the result after the expiration up until it is accessed for the first time.
                        return CreateResponse(HttpStatusCode.Accepted, $"Request available at endpoint asyncResult/{payload}. Result expires after: {completedTime.AddSeconds(300)}");
                    }
                    else
                    {
                        this.processing[payload] = DateTime.UtcNow;
                        return CreateResponse(HttpStatusCode.Accepted, $"Request is being processed. Estimated completion: {DateTime.UtcNow.AddSeconds(30)}");
                    }
                }
                else
                {
                    return CreateResponse(HttpStatusCode.Conflict, "Request is already being processed.");
                }
            });

            this.Endpoints.Add("asyncStatus", payload =>
            {
                var requestInProgress = this.processing.TryGetValue(payload, out DateTime startTime);
                if (requestInProgress)
                {
                    this.ProcessRequest(payload, startTime);
                }

                var requestCompleted = this.completed.TryGetValue(payload, out DateTime completedTime);
                if (!requestInProgress && !requestCompleted)
                {
                    return CreateResponse(HttpStatusCode.NotFound, "Requested resource not found.");
                }
                else
                {
                    if (requestCompleted)
                    {
                        return CreateResponse(HttpStatusCode.OK, $"Request has been processed. Completed at: {completedTime}");
                    }
                    else
                    {
                        return CreateResponse(HttpStatusCode.Found, "Request is in progess.");
                    }
                }
            });

            this.Endpoints.Add("asyncResult", payload =>
            {
                var requestInProgress = this.processing.TryGetValue(payload, out DateTime startTime);
                if (requestInProgress)
                {
                    this.ProcessRequest(payload, startTime);
                }

                var requestCompleted = this.completed.TryGetValue(payload, out DateTime completedTime);
                if (!requestCompleted)
                {
                    return CreateResponse(HttpStatusCode.NotFound, "Requested resource not found.");
                }
                else
                {
                    string responseMessage = $"Result for payload '{payload}': Processed successfully at {completedTime}.";
                    if (completedTime.AddSeconds(300) < DateTime.UtcNow)
                    {
                        this.processing.Remove(payload);
                        responseMessage += " (This result will no longer be available)";
                    }

                    return CreateResponse(HttpStatusCode.OK, responseMessage);
                }
            });
        }

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <param name="payload">payload key.</param>
        /// <param name="startTime">process start time.</param>
        /// <returns>If the request is complete.</returns>
        private bool ProcessRequest(string payload, DateTime startTime)
        {
            if (startTime.AddSeconds(30) < DateTime.UtcNow)
            {
                this.processing.Remove(payload);
                this.completed[payload] = startTime.AddSeconds(30);
                return true;
            }

            return false;
        }
    }
}
