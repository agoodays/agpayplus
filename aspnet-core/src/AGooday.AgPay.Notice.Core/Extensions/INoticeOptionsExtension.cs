namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// options extension.
    /// </summary>
    public interface INoticeOptionsExtension
    {
        /// <summary>
        /// Adds the services.
        /// </summary>
        /// <param name="services">Services.</param>
        void AddServices(IServiceCollection services);
    }
}
