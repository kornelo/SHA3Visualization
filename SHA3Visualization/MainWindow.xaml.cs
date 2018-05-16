using System;
using System.Collections.Generic;
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
        private Model3DGroup MainModel3Dgroup = new Model3DGroup();

        // The camera.
        private PerspectiveCamera TheCamera;

        // public cube
        public Cube cube;

        // The camera's current location.
        private double CameraPhi = 14.5 * Math.PI / 18.0; // 30 degrees
        private double CameraTheta =  8.0 * Math.PI / 18.0; // 30 degrees
        private double CameraR = 30.0;

        // The change in CameraPhi when you press the up and down arrows.
        private const double CameraDPhi = 0.1;

        // The change in CameraTheta when you press the left and right arrows.
        private const double CameraDTheta = 0.1;

        // The change in CameraR when you press + or -.
        private const double CameraDR = 0.1;

        // The currently selected model.
        private GeometryModel3D SelectedModel;

        // Materials used for normal and selected models.
        private Material NormalMaterial, SelectedMaterial;

        // The list of selectable models.
        private Dictionary<GeometryModel3D, string> SelectableModels;

        private void SHA3Visualizer_Loaded(object sender, RoutedEventArgs e)
        {

            // Give the camera its initial position.
            TheCamera = new PerspectiveCamera
            {
                FieldOfView = 60
            };
            MainTabView.Camera = TheCamera;
            PositionCamera();

            // Define lights.
            DefineLights(MainModel3Dgroup);

            // Create the model.
            //PerformHashing();
            // StateCubePresetation();


            //Loading View
            RefreshModelView();
        }

        public void RefreshModelView()
        {
            //Collection of Models
            SelectableModels = (cube != null) ? cube.ReturnListOfModels() : null;

            // Add the group of models to a ModelVisual3D.
            ModelVisual3D model_visual = new ModelVisual3D
            {
                Content = (cube != null) ? cube.ReturnMainModel() : null
            };

            // Display the main visual to the viewportt.
            MainTabView.Children.Add(model_visual);
        }

        #endregion

        private void VerticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TheCamera.Transform =
                new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), VerticalScrollBar.Value));
        }

        private void HorizontalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TheCamera.Transform =
                new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), HorizontalScrollBar.Value));
        }


        private void ZoomScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        //    Cube.Transform =
        //        new ScaleTransform3D(ZoomScrollBar.Value, ZoomScrollBar.Value, ZoomScrollBar.Value);
        }


        // Position the camera.
        private void PositionCamera()
        {
            // Calculate the camera's position in Cartesian coordinates.
            double y = CameraR * Math.Sin(CameraPhi);
            double hyp = CameraR * Math.Cos(CameraPhi);
            double x = hyp * Math.Cos(CameraTheta);
            double z = hyp * Math.Sin(CameraTheta);

            // Moving camera
            double xMove = 6;
            double yMove = 7;
            double zMove = 0;

            TheCamera.Position = new Point3D(x +xMove , y + yMove, z + zMove);

            // Look toward the origin.
            TheCamera.LookDirection = new Vector3D(-x , -y, -z);

            // Set the Up direction.

            xValue.Text = TheCamera.Position.X.ToString();
            yValue.Text = TheCamera.Position.Y.ToString();
            zValue.Text = TheCamera.Position.Z.ToString();

        }

        #region Hit Testing Code

        // See what was clicked.

        #endregion Hit Testing Code

        private void SHA3Visualizer_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    CameraPhi += CameraDPhi;
                    if (CameraPhi < Math.PI / 2.0) CameraPhi = Math.PI / 2.0;
                    break;
                case Key.Down:
                    CameraPhi -= CameraDPhi;
                    if (CameraPhi <  -Math.PI / 2.0) CameraPhi = -Math.PI / 2.0;
                    break;
                case Key.Left:
                    CameraTheta += CameraDTheta;
                    break;
                case Key.Right:
                    CameraTheta -= CameraDTheta;
                    break;
                case Key.Add:
                case Key.OemPlus:
                    CameraR -= CameraDR;
                    if (CameraR < CameraDR) CameraR = CameraDR;
                    break;
                case Key.Subtract:
                case Key.OemMinus:
                    CameraR += CameraDR;
                    break;
            }

            // Update the camera's position.
            PositionCamera();
        }

        private void SHA3Visualizer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            

            // Deselect the prevously selected model.
            if (SelectedModel != null)
            {
                var value = SelectableModels[SelectedModel];

                // Make the normal and selected materials.
                NormalMaterial = new DiffuseMaterial(SelectedBrush(value));
                
                SelectedModel
                    .Material = NormalMaterial;

                //back site of number
                var index = SelectableModels[SelectedModel].IndexOf(value);
                if (SelectedModel.Bounds.Z % 2 == 0)
                    SelectedModel = SelectableModels.ElementAt(index + 1).Key;
                else
                    SelectedModel = SelectableModels.ElementAt(index - 1).Key;

                SelectedModel.Material = NormalMaterial;
                SelectedModel = null;
            }

            // Get the mouse's position relative to the viewport.
            Point mouse_pos = e.GetPosition(MainTabView);

            // Perform the hit test.
            HitTestResult result =
                VisualTreeHelper.HitTest(MainTabView, mouse_pos);

            // See if we hit a model.
            RayMeshGeometry3DHitTestResult mesh_result =
                result as RayMeshGeometry3DHitTestResult;
            if (mesh_result != null)
            {
                GeometryModel3D model = (GeometryModel3D)mesh_result.ModelHit;
                if (SelectableModels.ContainsKey(model))
                {
                    SelectedModel = model;
                    SelectedModel.Material = SelectedMaterial;

                    var value = SelectableModels[SelectedModel];

                    // Make the selected materials.
                    SelectedMaterial = new DiffuseMaterial(SelectedBrush(value, true));

                    //back site of number selection
                    var index = SelectableModels[SelectedModel].IndexOf(value);
                    if (SelectedModel.Bounds.Z % 2 == 0)
                        SelectedModel = SelectableModels.ElementAt(index + 1).Key;
                    else
                    SelectedModel = SelectableModels.ElementAt(index - 1).Key;

                    SelectedModel.Material = SelectedMaterial;
                }
            }
        }

        private void DefineLights(Model3DGroup model_group)
        {
            model_group.Children.Add(new AmbientLight(Colors.DarkSlateGray));
            model_group.Children.Add(
                new DirectionalLight(Colors.Gray,
                    new Vector3D(3.0, -2.0, 1.0)));
            model_group.Children.Add(
                new DirectionalLight(Colors.DarkGray,
                    new Vector3D(-3.0, -2.0, -1.0)));
        }


        public void PerformHashing()
        {
            var alghorithm = new SHA3.SHA3(224);
            byte[] data = { };
            alghorithm.ComputeHash(data);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            switch (presentationMenuComboBox.SelectedValue)
            {

                default:
                    //StateCubePresetation();
                    //SlicePresetation();
                    //LanePresetation();
                    //RowPresetation();
                    ColumnPresentation();
                    break;
            }
        }

        private Brush SelectedBrush(string value, bool selected = false)
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
            cube = new Cube(8,new byte[]{});
            
            RefreshModelView();
        }

        private void SlicePresetation()
        {
            //Preparing Cube
            cube = new Cube(1, new byte[] { });

            RefreshModelView();
            
        }

        private void LanePresetation()
        {
            //Preparing Cube
            cube = new Cube(1,1,8, new byte[] { });

            RefreshModelView();

        }

        private void RowPresetation()
        {
            //Preparing Cube
            cube = new Cube(5, 1, 1, new byte[] { });

            RefreshModelView();

        }

        private void ColumnPresentation()
        {
            var sth = "abc";

            //Preparing Cube
            cube = new Cube(1, 5, 1, Encoding.ASCII.GetBytes(sth));

            RefreshModelView();

        }
    }
}
