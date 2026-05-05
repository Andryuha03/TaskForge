using System.Windows;
using TaskForge.ViewModels;

namespace TaskForge.Views.Dialogs
{
    public partial class EditProfileWindow : Window
    {
        public EditProfileWindow(EditProfileViewModel viewModel)
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
