using DbComparator.App.Services;
using DbComparator.App.ViewModels;
using DbComparator.App.Views.Windows;
using DbConectionInfoRepository.Repositories;
using Ninject;
using System.Windows;

namespace DbComparator.App
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            IKernel kernel = new StandardKernel();
            kernel.Bind<IInfoDbRepository>().To<InfoDbConnection>();

            kernel.Bind<IMessagerVM>().To<MessagerViewModel>();
            kernel.Bind<IDbInfoCreatorVM>().To<DbInfoCreatorViewModel>();
            kernel.Bind<IGeneralDbInfoVM>().To<GeneralDbInfoViewModel>();
            kernel.Bind<ICollectionEqualizer>().To<CollectionEqualizer>();
            kernel.Bind<IFieldsEqualizer>().To<FieldsEqualizer>();
            kernel.Bind<IAutoComparator>().To<AutoComparator>();

            var mainVM = kernel.Get<MainWindowViewModel>();

            MainWindow = new MainWindow();
            MainWindow.DataContext = mainVM;
            MainWindow.Show();

        }
    }
}
