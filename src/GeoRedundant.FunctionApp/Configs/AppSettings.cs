using Aliencube.AzureFunctions.Extensions.Configuration.AppSettings;
using Aliencube.AzureFunctions.Extensions.Configuration.AppSettings.Extensions;

namespace GeoRedundant.FunctionApp.Configs
{
    /// <summary>
    /// This represents the app settings entity.
    /// </summary>
    public class AppSettings : AppSettingsBase
    {
        private const string ServiceBusSettingsKey = "AzureServiceBus";

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettings"/> class.
        /// </summary>
        public AppSettings()
        {
            this.ServiceBus = this.Config.Get<AzureServiceBusSettings>(ServiceBusSettingsKey);
        }

        /// <summary>
        /// Gets the <see cref="AzureServiceBusSettings"/> instance.
        /// </summary>
        public virtual AzureServiceBusSettings ServiceBus { get; }
    }
}