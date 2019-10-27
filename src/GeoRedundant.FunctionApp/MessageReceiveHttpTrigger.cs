using System;
using System.Threading.Tasks;

using GeoRedundant.FunctionApp.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace GeoRedundant.FunctionApp
{
    /// <summary>
    /// This represents the HTTP trigger entity to receive messages.
    /// </summary>
    public class MessageReceiveHttpTrigger
    {
        private readonly IMessageService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceiveHttpTrigger"/> class.
        /// </summary>
        /// <param name="service"><see cref="IMessageService"/> instance.</param>
        public MessageReceiveHttpTrigger(IMessageService service)
        {
            this._service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Invokes the HTTP trigger.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> instance.</param>
        /// <param name="log"><see cref="ILogger"/> instance.</param>
        /// <returns>Returns the <see cref="IActionResult"/> as a response.</returns>
        [FunctionName(nameof(MessageReceiveHttpTrigger))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "messages/receive")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            await this._service
                      .WithSubscriptionClients()
                      .ReceiveAsync(async p =>
                      {
                          log.LogInformation($"Processed: {p.MessageId}");

                          await Task.CompletedTask.ConfigureAwait(false);
                      })
                      .ConfigureAwait(false);

            return new OkResult();
        }
    }
}