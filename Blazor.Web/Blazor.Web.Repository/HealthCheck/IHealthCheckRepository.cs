namespace Blazor.Web.Repository.HealthCheck
{
    public interface IHealthCheckRepository
    {
        Task<Blazor.Web.Models.Models.HealthCheck> ApiHealthCheck();
    }
}
