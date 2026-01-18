namespace Liv.Lck.Telemetry
{
    public interface ILckTelemetryClient
    {
        /// <summary>
        /// Send a <see cref="LckTelemetryEvent"/>
        /// </summary>
        /// <param name="lckTelemetryEvent">The <see cref="LckTelemetryEvent"/> to send</param>
        void SendTelemetry(LckTelemetryEvent lckTelemetryEvent);
        
        /// <summary>
        /// Send some error telemetry for the given <see cref="ILckResult"/>
        /// </summary>
        /// <param name="lckResult">The <see cref="ILckResult"/> to send error telemetry for</param>
        void SendErrorTelemetry(ILckResult lckResult);
    }
}
