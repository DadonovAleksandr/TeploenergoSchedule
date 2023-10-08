using Microsoft.Extensions.DependencyInjection;
using TeploenergoSchedule.ViewModels.MainWindowVm;

namespace TeploenergoSchedule.ViewModels
{
    internal class ViewModelLocator
    {
        public MainWindowViewModel MainWindowViewModel => App.Host.Services.GetRequiredService<MainWindowViewModel>();
    }
}