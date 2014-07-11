using System;
using System.Reflection;
using System.Windows;

namespace TwitterKiller
{
    public class ConventionalDialogService : IDialogService
    {
        public void ShowDialog<TViewModel>(TViewModel viewModel, WindowMode windowMode)
        {
            var viewName = typeof(TViewModel).Name.Replace("Model", "");
            var viewFullName = typeof(TViewModel).Namespace + "." + viewName;
            var viewType = Assembly.GetExecutingAssembly().GetType(viewFullName);

            if (viewType == null)
                throw new ArgumentException("There is no item for " + typeof(TViewModel).FullName, "viewModel");

            var view = Activator.CreateInstance(viewType) as Window;

            if (view == null)
                throw new ArgumentException("There is no item for" + typeof(TViewModel).FullName, "viewModel");

            view.DataContext = viewModel;
            
            if (windowMode == WindowMode.Modal)
                view.ShowDialog();
            else
                view.Show();    
        }
    }
}
