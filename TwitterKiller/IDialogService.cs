namespace TwitterKiller
{
    public interface IDialogService
    {
        void ShowDialog<TViewModel>(TViewModel viewModel, WindowMode windowMode);
    }
}
