using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.Windows.Media.Media3D;
using SHA3Visualization.SHA3;

namespace SHA3Visualization
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }



        #region Initializations

        // The main object model group.
        internal Model3DGroup MainModel3Dgroup = new Model3DGroup();

        // The camera.
        private PerspectiveCamera _theCamera;

        // public cube
        public Cube Cube;

        public bool HashedCube;

        // The camera's current location.
        private double _cameraPhi = 14.5 * Math.PI / 18.0; // 30 degrees
        private double _cameraTheta =  8.0 * Math.PI / 18.0; // 30 degrees
        private double _cameraR = 30.0;

        // The change in CameraPhi when you press the up and down arrows.
        private const double CameraDPhi = 0.1;

        // The change in CameraTheta when you press the left and right arrows.
        private const double CameraDTheta = 0.1;

        // The change in CameraR when you press + or -.
        private const double CameraDr = 0.1;

        // The currently selected model.
        private GeometryModel3D _selectedModel;

        // Materials used for normal and selected models.
        private Material _normalMaterial, _selectedMaterial;

        // The list of selectable models.
        private Dictionary<GeometryModel3D, string> _selectableModels;

        private void SHA3Visualizer_Loaded(object sender, RoutedEventArgs e)
        {

            // Give the camera its initial position.
            _theCamera = new PerspectiveCamera
            {
                FieldOfView = 60
            };
            MainTabView.Camera = _theCamera;
            PositionCamera();

            // Define lights.
            DefineLights(MainModel3Dgroup);

            //Fill up for menu ComboBox
            ComboBoxDynamicFill();

            // Create the model.
            //PerformHashing();
            // StateCubePresetation();


            //Loading View
            RefreshModelView();
        }

        public void RefreshModelView()
        {
            // clear out the existing geometry XAML
            MainModel3Dgroup.Children.Clear();
            MainTabView.Children.Clear();

            //Collection of Models
            _selectableModels = Cube?.ReturnListOfModels();

            // Add the group of models to a ModelVisual3D.
            ModelVisual3D model_visual = new ModelVisual3D
            {
                Content = Cube?.ReturnMainModel()
            };

            // Display the main visual to the viewportt.
            MainTabView.Children.Add(model_visual);
        }

        #endregion


        // Position the camera.
        private void PositionCamera()
        {
            // Calculate the camera's position in Cartesian coordinates.
            double y = _cameraR * Math.Sin(_cameraPhi);
            double hyp = _cameraR * Math.Cos(_cameraPhi);
            double x = hyp * Math.Cos(_cameraTheta);
            double z = hyp * Math.Sin(_cameraTheta);

            // Moving camera
            const double xMove = 6;
            const double yMove = 7;
            const double zMove = 0;

            _theCamera.Position = new Point3D(x +xMove , y + yMove, z + zMove);

            // Look toward the origin.
            _theCamera.LookDirection = new Vector3D(-x , -y, -z);

            // Set the Up direction.

            xValue.Text = _theCamera.Position.X.ToString(CultureInfo.InvariantCulture);
            yValue.Text = _theCamera.Position.Y.ToString(CultureInfo.InvariantCulture);
            zValue.Text = _theCamera.Position.Z.ToString(CultureInfo.InvariantCulture);

        }

        #region Hit Testing Code

        // See what was clicked.

        #endregion Hit Testing Code

        private void SHA3Visualizer_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    _cameraPhi += CameraDPhi;
                    if (_cameraPhi < Math.PI / 2.0) _cameraPhi = Math.PI / 2.0;
                    break;
                case Key.Down:
                    _cameraPhi -= CameraDPhi;
                    if (_cameraPhi <  -Math.PI / 2.0) _cameraPhi = -Math.PI / 2.0;
                    break;
                case Key.Left:
                    _cameraTheta += CameraDTheta;
                    break;
                case Key.Right:
                    _cameraTheta -= CameraDTheta;
                    break;
                case Key.Add:
                case Key.OemPlus:
                    _cameraR -= CameraDr;
                    if (_cameraR < CameraDr) _cameraR = CameraDr;
                    break;
                case Key.Subtract:
                case Key.OemMinus:
                    _cameraR += CameraDr;
                    break;
            }

            // Update the camera's position.
            PositionCamera();
        }

        private void SHA3Visualizer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            

            // Deselect the prevously selected model.
            if (_selectedModel != null)
            {
                var value = _selectableModels[_selectedModel];

                // Make the normal and selected materials.
                _normalMaterial = new DiffuseMaterial(SelectedBrush(value));
                
                _selectedModel
                    .Material = _normalMaterial;

                //back site of number
                var index = _selectableModels[_selectedModel].IndexOf(value, StringComparison.Ordinal);
                if (Math.Abs(_selectedModel.Bounds.Z % 2) < 1)
                    _selectedModel = _selectableModels.ElementAt(index + 1).Key;
                else
                    _selectedModel = _selectableModels.ElementAt(index - 1).Key;

                _selectedModel.Material = _normalMaterial;
                _selectedModel = null;
            }

            // Get the mouse's position relative to the viewport.
            Point mousePos = e.GetPosition(MainTabView);

            // Perform the hit test.
            HitTestResult result =
                VisualTreeHelper.HitTest(MainTabView, mousePos);

            // See if we hit a model.
            RayMeshGeometry3DHitTestResult meshResult =
                result as RayMeshGeometry3DHitTestResult;
            if (meshResult != null)
            {
                GeometryModel3D model = (GeometryModel3D)meshResult.ModelHit;
                if (_selectableModels.ContainsKey(model))
                {
                    _selectedModel = model;
                    _selectedModel.Material = _selectedMaterial;

                    var value = _selectableModels[_selectedModel];

                    // Make the selected materials.
                    _selectedMaterial = new DiffuseMaterial(SelectedBrush(value, true));

                    //back site of number selection
                    var index = _selectableModels[_selectedModel].IndexOf(value, StringComparison.Ordinal);
                    if (Math.Abs(_selectedModel.Bounds.Z % 2) < 1)
                        _selectedModel = _selectableModels.ElementAt(index + 1).Key;
                    else
                    _selectedModel = _selectableModels.ElementAt(index - 1).Key;

                    _selectedModel.Material = _selectedMaterial;
                }
            }
        }

        private void DefineLights(Model3DGroup modelGroup)
        {
            modelGroup.Children.Add(new AmbientLight(Colors.DarkSlateGray));
            modelGroup.Children.Add(
                new DirectionalLight(Colors.Gray,
                    new Vector3D(3.0, -2.0, 1.0)));
            modelGroup.Children.Add(
                new DirectionalLight(Colors.DarkGray,
                    new Vector3D(-3.0, -2.0, -1.0)));
        }


        public void PerformHashing()
        {
            var alghorithm = new SHA3.SHA3(224);
            byte[] data = { };
            alghorithm.ComputeHash(data);
            RefreshModelView();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            switch (presentationMenuComboBox.SelectedValue)
            {
            case "State":
                StateCubePresetation();
                break;
            case "Slice":
                SlicePresetation();
                break;
            case "Lane":
                LanePresetation();
                break;
            case "Row":
                RowPresetation();
                break;
            case "Column":
                ColumnPresentation();
                break;
            case "Hash Example":
                PerformHashing();
                break;
            default:
                    break;
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

            foreach (var content in listOfContent)
            {
                presentationMenuComboBox.Items.Add(content);
            }

        }

        private static Brush SelectedBrush(string value, bool selected = false)
        {
            TextBlock textBlock; 
            if(selected) textBlock= new TextBlock() { Text = value, Background = new SolidColorBrush(Colors.GreenYellow) };
            else textBlock = new TextBlock() { Text = value, Background = Brushes.Transparent };
            Size size = new Size(40, 40);
            Viewbox viewBox = new Viewbox();
            viewBox.Child = textBlock;
            viewBox.Measure(size);
            viewBox.Arrange(new Rect(size));
            RenderTargetBitmap bitmap = new RenderTargetBitmap(35, 40, 80, 80, PixelFormats.Pbgra32);

            bitmap.Render(viewBox);

            return new ImageBrush(bitmap);
        }

        private void StateCubePresetation()
        {
            //Preparing Cube
            //cube = new Cube(8,new byte[]{});

            const string sth = "abc";

            //Preparing Cube
            Cube = new Cube(8, Encoding.ASCII.GetBytes(sth));

            RefreshModelView();

            HashedCube = true?!HashedCube:HashedCube;
        }

        private void SlicePresetation()
        {
            const string sth = "abc";

            //Preparing Cube
            Cube = new Cube(1, Encoding.ASCII.GetBytes(sth));

            RefreshModelView();
            HashedCube = true ? !HashedCube : HashedCube;
        }

        private void LanePresetation()
        {
            const string sth = "abc";

            //Preparing Cube
            Cube = new Cube(1, 1, 8, Encoding.ASCII.GetBytes(sth));

            RefreshModelView();
            HashedCube = true ? !HashedCube : HashedCube;
        }

        private void RowPresetation()
        {
            const string sth = "abc";

            //Preparing Cube
            Cube = new Cube(5, 1, 1, Encoding.ASCII.GetBytes(sth));

            RefreshModelView();
            HashedCube = true ? !HashedCube : HashedCube;
        }

        private void ColumnPresentation()
        {
            const string sth = "abc";

            //Preparing Cube
            Cube = new Cube(1, 5, 1, Encoding.ASCII.GetBytes(sth));

            RefreshModelView();
            HashedCube = true ? !HashedCube : HashedCube;
        }

        private void PlusX_Click(object sender, RoutedEventArgs e)
        {
            _cameraPhi += CameraDPhi;
            if (_cameraPhi < Math.PI / 2.0) _cameraPhi = Math.PI / 2.0;
            PositionCamera();
        }

        private void MinusX_Click(object sender, RoutedEventArgs e)
        {
            _cameraPhi -= CameraDPhi;
            if (_cameraPhi < Math.PI / 2.0) _cameraPhi = Math.PI / 2.0;
            PositionCamera();
        }

        private void PlusY_Click(object sender, RoutedEventArgs e)
        {
            _cameraTheta += CameraDTheta;
            PositionCamera();
        }

        private void MinusY_Click(object sender, RoutedEventArgs e)
        {
            _cameraTheta -= CameraDTheta;
            PositionCamera();
        }

        private void PlusZ_Click(object sender, RoutedEventArgs e)
        {
            _cameraR += 10*CameraDr;
            PositionCamera();
        }

        private void MinusZ_Click(object sender, RoutedEventArgs e)
        {
            _cameraR -= 10*CameraDr;
            PositionCamera();
        }


    }
}
