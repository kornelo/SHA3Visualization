namespace SHA3Visualization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // The change in CameraPhi when you press the up and down arrows.
        private const double CameraDPhi = 0.1;

        // The change in CameraR when you press + or -.
        private const double CameraDr = 0.1;

        // The change in CameraTheta when you press the left and right arrows.
        private const double CameraDTheta = 0.1;

        // public cube
        public Cube Cube;

        public static Cube Cube1;

        public bool HashedCube;

        // The main object model group.
        internal Model3DGroup MainModel3Dgroup = new Model3DGroup();

        // The camera's current location.
        private double _cameraPhi = 14.5 * Math.PI / 18.0; // 30 degrees

        private double _cameraR = 30.0;

        private double _cameraTheta = 8.0 * Math.PI / 18.0; // 30 degrees

        // Materials used for normal and selected models.
        private Material _normalMaterial;

        // The list of selectable models.
        private Dictionary<GeometryModel3D, string> _selectableModels;

        private Material _selectedMaterial;

        // The currently selected model.
        private GeometryModel3D _selectedModel;

        // The camera.
        private PerspectiveCamera _theCamera;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        public void PerformHashing()
        {
            var alghorithm = new SHA3Managed(224);
            byte[] data = { };
            alghorithm.ComputeHash(data);
            this.RefreshModelView();
        }

        public void RefreshModelView()
        {
            // clear out the existing geometry XAML
            this.MainModel3Dgroup.Children.Clear();
            this.MainTabView.Children.Clear();

            // Collection of Models
            this._selectableModels = this.Cube?.ReturnListOfModels();

            // Add the group of models to a ModelVisual3D.
            var model_visual = new ModelVisual3D { Content = this.Cube?.ReturnMainModel() };

            // Display the main visual to the viewportt.
            this.MainTabView.Children.Add(model_visual);
        }

        private static Brush SelectedBrush(string value, bool selected = false)
        {
            TextBlock textBlock;
            if (selected)
                textBlock = new TextBlock { Text = value, Background = new SolidColorBrush(Colors.GreenYellow) };
            else textBlock = new TextBlock { Text = value, Background = Brushes.Transparent };
            var size = new Size(40, 40);
            var viewBox = new Viewbox { Child = textBlock };
            viewBox.Measure(size);
            viewBox.Arrange(new Rect(size));
            var bitmap = new RenderTargetBitmap(35, 40, 80, 80, PixelFormats.Pbgra32);

            bitmap.Render(viewBox);

            return new ImageBrush(bitmap);
        }

        private void ColumnPresentation()
        {
            const string sth = "abc";

            // Preparing Cube
            this.Cube = new Cube(1, 5, 1, Encoding.ASCII.GetBytes(sth));

            this.RefreshModelView();
            this.HashedCube = true ? !this.HashedCube : this.HashedCube;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (this.presentationMenuComboBox.SelectedValue)
            {
                case "State":
                    this.StateCubePresetation();
                    break;
                case "Slice":
                    this.SlicePresetation();
                    break;
                case "Lane":
                    this.LanePresetation();
                    break;
                case "Row":
                    this.RowPresetation();
                    break;
                case "Column":
                    this.ColumnPresentation();
                    break;
                case "Hash Example":
                    this.PerformHashing();
                    break;
                default: break;
            }
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

        private void DefineLights(Model3DGroup modelGroup)
        {
            modelGroup.Children.Add(new AmbientLight(Colors.DarkSlateGray));
            modelGroup.Children.Add(new DirectionalLight(Colors.Gray, new Vector3D(3.0, -2.0, 1.0)));
            modelGroup.Children.Add(new DirectionalLight(Colors.DarkGray, new Vector3D(-3.0, -2.0, -1.0)));
        }

        private void LanePresetation()
        {
            const string sth = "abc";

            // Preparing Cube
            this.Cube = new Cube(1, 1, 8, Encoding.ASCII.GetBytes(sth));

            this.RefreshModelView();
            this.HashedCube = true ? !this.HashedCube : this.HashedCube;
        }

        private void MinusX_Click(object sender, RoutedEventArgs e)
        {
            this._cameraPhi -= CameraDPhi;
            if (this._cameraPhi < Math.PI / 2.0) this._cameraPhi = Math.PI / 2.0;
            this.PositionCamera();
        }

        private void MinusY_Click(object sender, RoutedEventArgs e)
        {
            this._cameraTheta -= CameraDTheta;
            this.PositionCamera();
        }

        private void MinusZ_Click(object sender, RoutedEventArgs e)
        {
            this._cameraR -= 10 * CameraDr;
            this.PositionCamera();
        }

        private void PlusX_Click(object sender, RoutedEventArgs e)
        {
            this._cameraPhi += CameraDPhi;
            if (this._cameraPhi < Math.PI / 2.0) this._cameraPhi = Math.PI / 2.0;
            this.PositionCamera();
        }

        private void PlusY_Click(object sender, RoutedEventArgs e)
        {
            this._cameraTheta += CameraDTheta;
            this.PositionCamera();
        }

        private void PlusZ_Click(object sender, RoutedEventArgs e)
        {
            this._cameraR += 10 * CameraDr;
            this.PositionCamera();
        }

        // Position the camera.
        private void PositionCamera()
        {
            // Calculate the camera's position in Cartesian coordinates.
            var y = this._cameraR * Math.Sin(this._cameraPhi);
            var hyp = this._cameraR * Math.Cos(this._cameraPhi);
            var x = hyp * Math.Cos(this._cameraTheta);
            var z = hyp * Math.Sin(this._cameraTheta);

            // Moving camera
            const double xMove = 6;
            const double yMove = 7;
            const double zMove = 0;

            this._theCamera.Position = new Point3D(x + xMove, y + yMove, z + zMove);

            // Look toward the origin.
            this._theCamera.LookDirection = new Vector3D(-x, -y, -z);

            // Set the Up direction.
            this.xValue.Text = this._theCamera.Position.X.ToString(CultureInfo.InvariantCulture);
            this.yValue.Text = this._theCamera.Position.Y.ToString(CultureInfo.InvariantCulture);
            this.zValue.Text = this._theCamera.Position.Z.ToString(CultureInfo.InvariantCulture);
        }

        private void RowPresetation()
        {
            const string sth = "abc";

            // Preparing Cube
            this.Cube = new Cube(5, 1, 1, Encoding.ASCII.GetBytes(sth));

            this.RefreshModelView();
            this.HashedCube = true ? !this.HashedCube : this.HashedCube;
        }

        private void SHA3Visualizer_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    this._cameraPhi += CameraDPhi;
                    if (this._cameraPhi < Math.PI / 2.0) this._cameraPhi = Math.PI / 2.0;
                    break;
                case Key.Down:
                    this._cameraPhi -= CameraDPhi;
                    if (this._cameraPhi < -Math.PI / 2.0) this._cameraPhi = -Math.PI / 2.0;
                    break;
                case Key.Left:
                    this._cameraTheta += CameraDTheta;
                    break;
                case Key.Right:
                    this._cameraTheta -= CameraDTheta;
                    break;
                case Key.Add:
                case Key.OemPlus:
                    this._cameraR -= CameraDr;
                    if (this._cameraR < CameraDr) this._cameraR = CameraDr;
                    break;
                case Key.Subtract:
                case Key.OemMinus:
                    this._cameraR += CameraDr;
                    break;
            }

            // Update the camera's position.
            this.PositionCamera();
        }

        private void SHA3Visualizer_Loaded(object sender, RoutedEventArgs e)
        {
            // Give the camera its initial position.
            this._theCamera = new PerspectiveCamera { FieldOfView = 60 };
            this.MainTabView.Camera = this._theCamera;
            this.PositionCamera();

            // Define lights.
            this.DefineLights(this.MainModel3Dgroup);

            // Fill up for menu ComboBox
            this.ComboBoxDynamicFill();

            // Create the model.
            // PerformHashing();
            // StateCubePresetation();

            // Loading View
            this.RefreshModelView();
        }

        private void SHA3Visualizer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Deselect the prevously selected model.
            if (this._selectedModel != null)
            {
                var value = this._selectableModels[this._selectedModel];

                // Make the normal and selected materials.
                this._normalMaterial = new DiffuseMaterial(SelectedBrush(value));

                this._selectedModel.Material = this._normalMaterial;

                // back site of number
                var index = this._selectableModels[this._selectedModel].IndexOf(value, StringComparison.Ordinal);
                if (Math.Abs(this._selectedModel.Bounds.Z % 2) < 1)
                    this._selectedModel = this._selectableModels.ElementAt(index + 1).Key;
                else this._selectedModel = this._selectableModels.ElementAt(index - 1).Key;

                this._selectedModel.Material = this._normalMaterial;
                this._selectedModel = null;
            }

            // Get the mouse's position relative to the viewport.
            var mousePos = e.GetPosition(this.MainTabView);

            // Perform the hit test.
            var result = VisualTreeHelper.HitTest(this.MainTabView, mousePos);

            // See if we hit a model.
            var meshResult = result as RayMeshGeometry3DHitTestResult;
            if (meshResult != null)
            {
                var model = (GeometryModel3D)meshResult.ModelHit;
                if (this._selectableModels.ContainsKey(model))
                {
                    this._selectedModel = model;
                    this._selectedModel.Material = this._selectedMaterial;

                    var value = this._selectableModels[this._selectedModel];

                    // Make the selected materials.
                    this._selectedMaterial = new DiffuseMaterial(SelectedBrush(value, true));

                    // back site of number selection
                    var index = this._selectableModels[this._selectedModel].IndexOf(value, StringComparison.Ordinal);
                    if (Math.Abs(this._selectedModel.Bounds.Z % 2) < 1)
                        this._selectedModel = this._selectableModels.ElementAt(index + 1).Key;
                    else this._selectedModel = this._selectableModels.ElementAt(index - 1).Key;

                    this._selectedModel.Material = this._selectedMaterial;
                }
            }
        }

        private void SlicePresetation()
        {
            const string sth = "abc";

            // Preparing Cube
            this.Cube = new Cube(1, Encoding.ASCII.GetBytes(sth));

            this.RefreshModelView();
            this.HashedCube = true ? !this.HashedCube : this.HashedCube;
        }

        private void StateCubePresetation()
        {
            // Preparing Cube
            // cube = new Cube(8,new byte[]{});
            const string sth = "abc";

            // Preparing Cube
            this.Cube = new Cube(8, Encoding.ASCII.GetBytes(sth));

            this.RefreshModelView();

            this.HashedCube = true ? !this.HashedCube : this.HashedCube;
        }

        // See what was clicked.
    }
}