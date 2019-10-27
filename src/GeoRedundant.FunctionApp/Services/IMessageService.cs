using System;
using System.Threading.Tasks;

using GeoRedundant.FunctionApp.Models;

using Microsoft.Azure.ServiceBus;

namespace GeoRedundant.FunctionApp.Services
{
    /// <summary>
    /// This provides interfaces to <see cref="MessageService"/> class.
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Adds the list of <see cref="TopicClient"/> instances.
        /// </summary>
        /// <returns>Returns the <see cref="IMessageService"/> instance.</returns>
        IMessageService WithTopicClients();

        /// <summary>
        /// Adds the list of <see cref="SubscriptionClient"/> instances.
        /// </summary>
        /// <returns>Returns the <see cref="IMessageService"/> instance.</returns>
        IMessageService WithSubscriptionClients();

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="value">Message payload.</param>
        /// <returns>Returns the message ID.</returns>
        Task<string> SendAsync(string value);

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <typeparam name="T">Type of message payload.</typeparam>
        /// <param name="value">Message payload.</param>
        /// <returns>Returns the message ID.</returns>
        Task<string> SendAsync<T>(T value) where T : MessagePayload;

        /// <summary>
        /// Receives a list of messages and processes them.
        /// </summary>
        /// <param name="callbackToProcess"><see cref="Func{Message, Task}"/> expression.</param>
        /// <returns>Returns the <see cref="Task"/>.</returns>
        Task ReceiveAsync(Func<Message, Task> callbackToProcess);
    }
}