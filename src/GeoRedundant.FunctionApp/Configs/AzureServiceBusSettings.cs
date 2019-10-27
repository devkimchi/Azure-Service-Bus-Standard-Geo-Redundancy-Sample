using System;
using System.Collections.Generic;

namespace GeoRedundant.FunctionApp.Configs
{
    /// <summary>
    /// This represents the app settings entity for Azure Service Bus.
    /// </summary>
    public class AzureServiceBusSettings
    {
        /// <summary>
        /// Gets or sets the list of connection strings for Azure Service Bus.
        /// </summary>
        public virtual Dictionary<string, string> ConnectionStrings { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AzureServiceBusTopicSettings"/> instance.
        /// </summary>
        public virtual AzureServiceBusTopicSettings Topic { get; set; }

        /// <summary>
        /// Converts the <see cref="AppSettings"/> instance into <see cref="AzureServiceBusSettings"/> instance implicitly.
        /// </summary>
        /// <param name="instance"><see cref="AppSettings"/> instance.</param>
        public static implicit operator AzureServiceBusSettings(AppSettings instance)
        {
            var settings = instance ?? throw new ArgumentNullException(nameof(instance));

            return settings.ServiceBus;
        }
    }
}