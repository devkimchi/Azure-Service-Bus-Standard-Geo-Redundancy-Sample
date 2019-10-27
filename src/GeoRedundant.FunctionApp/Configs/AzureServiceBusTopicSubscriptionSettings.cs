namespace GeoRedundant.FunctionApp.Configs
{
    /// <summary>
    /// This represents the app settings entity for Azure Service Bus Topic Subscription.
    /// </summary>
    public class AzureServiceBusTopicSubscriptionSettings
    {
        /// <summary>
        /// Gets or sets the subscription name.
        /// </summary>
        public virtual string Name { get; set; }
    }
}