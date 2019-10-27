using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GeoRedundant.FunctionApp.Configs;
using GeoRedundant.FunctionApp.Models;

using Microsoft.Azure.ServiceBus;

using Newtonsoft.Json;

namespace GeoRedundant.FunctionApp.Services
{
    /// <summary>
    /// This represents the service entity for messages.
    /// </summary>
    public class MessageService : IMessageService
    {
        private readonly AzureServiceBusSettings _settings;
        private readonly List<ITopicClient> _topicClients;
        private readonly List<ISubscriptionClient> _subscriptionClients;

        /// <summary>
        /// Initializes a new instance of the <see cref="IMessageService"/> class.
        /// </summary>
        /// <param name="settings"><see cref="AppSettings"/> instance.</param>
        public MessageService(AppSettings settings)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this._topicClients = new List<ITopicClient>();
            this._subscriptionClients = new List<ISubscriptionClient>();
        }

        /// <inheritdoc />
        public IMessageService WithTopicClients()
        {
            this._topicClients.Clear();

            foreach (var kvp in this._settings.ConnectionStrings)
            {
                var client = new TopicClient(kvp.Value, this._settings.Topic.Name);
                this._topicClients.Add(client);
            }

            return this;
        }

        /// <inheritdoc />
        public IMessageService WithSubscriptionClients()
        {
            this._subscriptionClients.Clear();

            foreach (var kvp in this._settings.ConnectionStrings)
            {
                var client = new SubscriptionClient(kvp.Value, this._settings.Topic.Name, this._settings.Topic.Subscription.Name, ReceiveMode.PeekLock);
                this._subscriptionClients.Add(client);
            }

            return this;
        }

        /// <inheritdoc />
        public async Task<string> SendAsync(string value)
        {
            var body = Encoding.UTF8.GetBytes(value);
            var message = new Message(body) { MessageId = Guid.NewGuid().ToString() };

            var exceptions = new ConcurrentQueue<Exception>();

            if (!this._topicClients.Any())
            {
                throw new InvalidOperationException("No TopicClient exist");
            }

            foreach (var client in this._topicClients)
            {
                try
                {
                    await client.SendAsync(message.Clone());
                }
                catch (Exception ex)
                {
                    exceptions.Enqueue(ex);
                }
            }

            if (exceptions.Count == this._topicClients.Count)
            {
                throw new AggregateException(exceptions);
            }

            return message.MessageId;
        }

        /// <inheritdoc />
        public async Task<string> SendAsync<T>(T value) where T : MessagePayload
        {
            var serialised = JsonConvert.SerializeObject(value, Formatting.Indented);

            return await this.SendAsync(serialised).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task ReceiveAsync(Func<Message, Task> callbackToProcess)
        {
            var messageIds = new List<string>();
            var msglock = new object();

            // Handles messages.
            async Task onMessageReceived(ISubscriptionClient client, int maxCount, Message message)
            {
                var duplicated = false;
                lock (msglock)
                {
                    duplicated = messageIds.Remove(message.MessageId);
                    if (!duplicated)
                    {
                        messageIds.Add(message.MessageId);
                        if (messageIds.Count > maxCount)
                        {
                            messageIds.RemoveAt(0);
                        }
                    }
                }

                if (!duplicated)
                {
                    await callbackToProcess(message).ConfigureAwait(false);
                }
            }

            var exceptions = new ConcurrentQueue<Exception>();

            // Handles exceptions.
            async Task onExceptionReceived(ExceptionReceivedEventArgs args)
            {
                exceptions.Enqueue(args.Exception);

                await Task.CompletedTask.ConfigureAwait(false);
            }

            if (!this._subscriptionClients.Any())
            {
                throw new InvalidOperationException("No SubscriptionClient exist");
            }

            foreach (var client in this._subscriptionClients)
            {
                client.RegisterMessageHandler(
                    (msg, token) => onMessageReceived(client, 1, msg),
                    new MessageHandlerOptions(onExceptionReceived) { AutoComplete = true, MaxConcurrentCalls = 1 });
            }

            if (exceptions.Count == this._subscriptionClients.Count)
            {
                throw new AggregateException(exceptions);
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}