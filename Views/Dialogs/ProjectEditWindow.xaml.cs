using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TaskForge.ViewModels;

namespace TaskForge.Views.Dialogs
{
    public partial class ProjectEditWindow : Window
    {


        public ProjectEditWindow(ProjectEditViewModel viewModel)
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

    public class BoolToProjectEditTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool isNew && isNew) ? "Новый проект" : "Редактирование проекта";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
