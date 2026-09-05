using System.Windows;
using NUSHPOS.ViewModels;

namespace NUSHPOS.Views;

public partial class DesignEditorWindow : Window
{
    public DesignEditorWindow(DesignEditorViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.RequestClose += () => Close();
    }
}
