namespace SHA3Visualization
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private CubeHandler cubeHandler;

        public MainWindow()
        {
            this.InitializeComponent();
            this.cubeHandler = new CubeHandler();
            this.DataContext = this.cubeHandler;
            this.ComboBoxDynamicFill();
        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (presentationMenuComboBox.SelectedValue)
            {
                case "State":
                    this.cubeHandler.StateCubePresetation();
                    break;
                case "Slice":
                    this.cubeHandler.SlicePresetation();
                    break;
                case "Lane":
                    this.cubeHandler.LanePresetation();
                    break;
                case "Row":
                    this.cubeHandler.RowPresetation();
                    break;
                case "Column":
                    this.cubeHandler.ColumnPresentation();
                    break;
                case "Hash Example":
                    this.cubeHandler.PerformHashing();

                    break;
                default: break;
            }
            OnContentChanged(sender,e);
        }

        private void ComboBoxDynamicFill()
        {
            var listOfContent = new List<string>
                                    {
                                        "---VISUALISATIONS---",
                                        "State",
                                        "Slice",
                                        "Lane",
                                        "Row",
                                        "Column",
                                        "---ALGHORITMS---",
                                        "---EXAMPLE---",
                                        "Hash Example"
                                    };

            foreach (var content in listOfContent) this.presentationMenuComboBox.Items.Add(content);
        }

        private void PlusZ_Click(object sender, RoutedEventArgs e)
        {
            this.cubeHandler.PlusZ_Click(sender,e);
        }

        private void PlusY_Click(object sender, RoutedEventArgs e)
        {
            this.cubeHandler.PlusY_Click(sender, e);
        }
        private void PlusX_Click(object sender, RoutedEventArgs e)
        {
            this.cubeHandler.PlusX_Click(sender, e);
        }

        private void MinusZ_Click(object sender, RoutedEventArgs e)
        {
            this.cubeHandler.MinusZ_Click(sender,e);
        }

        private void MinusY_Click(object sender, RoutedEventArgs e)
        {
            this.cubeHandler.MinusY_Click(sender, e);
        }

        private void MinusX_Click(object sender, RoutedEventArgs e)
        {
            this.cubeHandler.MinusX_Click(sender, e);
        }

        private void OnContentChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}