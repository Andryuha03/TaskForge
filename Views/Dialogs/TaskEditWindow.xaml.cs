using System.Windows;
using TaskForge.ViewModels;

namespace TaskForge.Views.Dialogs
{
    public partial class TaskEditWindow : Window
    {
        public TaskEditWindow(TaskEditViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.RequestClose += (s, saved) =>
            {
                DialogResult = saved;
                Close();
            };
        }
    }
}