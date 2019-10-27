namespace GeoRedundant.FunctionApp.Configs
{
    /// <summary>
    /// This represents the app settings entity for Azure Service Bus Topic.
    /// </summary>
    public class AzureServiceBusTopicSettings
    {
        /// <summary>
        /// Gets or sets the topic name.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AzureServiceBusTopicSubscriptionSettings"/> instance.
        /// </summary>
        public virtual AzureServiceBusTopicSubscriptionSettings Subscription { get; set; }
    }
}