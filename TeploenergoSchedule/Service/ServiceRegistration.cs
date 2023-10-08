using Microsoft.Extensions.DependencyInjection;
using TeploenergoSchedule.Service.UserDialogService;

namespace TeploenergoSchedule.Service
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IUserDialogService, WindowsUserDialogService>();
            return services;
        }
    }
}