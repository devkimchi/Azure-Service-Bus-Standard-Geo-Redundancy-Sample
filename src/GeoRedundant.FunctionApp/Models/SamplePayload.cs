namespace GeoRedundant.FunctionApp.Models
{
    /// <summary>
    /// This represents the sample model entity.
    /// </summary>
    public class SamplePayload : MessagePayload
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public virtual string Message { get; set; }
    }
}