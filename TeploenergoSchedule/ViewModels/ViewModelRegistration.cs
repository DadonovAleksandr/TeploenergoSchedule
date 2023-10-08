using Microsoft.Extensions.DependencyInjection;
using TeploenergoSchedule.ViewModels.MainWindowVm;

namespace TeploenergoSchedule.ViewModels
{
    public static class ViewModelRegistration
    {
        public static IServiceCollection RegisterViewModels(this IServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();
            return services;
        }
    }
}