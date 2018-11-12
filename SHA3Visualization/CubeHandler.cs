namespace SHA3Visualization
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Security.Cryptography;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;

    using SHA3Visualization.SHA3;

    internal class CubeHandler : INotifyPropertyChanged
    {
        // The change in CameraPhi when you press the up and down arrows.
        private const double CameraDPhi = 0.1;

        // The change in CameraR when you press + or -.
        private const double CameraDr = 0.1;

        // The change in CameraTheta when you press the left and right arrows.
        private const double CameraDTheta = 0.1;

        public static Cube Cube1;

        // public cube
        public Cube Cube;

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

        public event PropertyChangedEventHandler PropertyChanged;

        public static List<ulong> State { get; set; }

        public SHA3Managed Alghorithm { get; set; }

        public List<string> FinalState { get; set; }

        public string Hash { get; set; }

        public void ColumnPresentation()
        {
            const string sth = "abc";

            // Preparing Cube
            this.Cube = new Cube(1, 5, 1, Encoding.ASCII.GetBytes(sth));
        }

        public void LanePresetation()
        {
            const string sth = "abc";

            // Preparing Cube
            this.Cube = new Cube(1, 1, 8, Encoding.ASCII.GetBytes(sth));
        }

        public void MinusX_Click(object sender, RoutedEventArgs e)
        {
            this._cameraPhi -= CameraDPhi;
            if (this._cameraPhi < Math.PI / 2.0) this._cameraPhi = Math.PI / 2.0;
            this.PositionCamera();
        }

        public void MinusY_Click(object sender, RoutedEventArgs e)
        {
            this._cameraTheta -= CameraDTheta;
            this.PositionCamera();
        }

        public void MinusZ_Click(object sender, RoutedEventArgs e)
        {
            this._cameraR -= 10 * CameraDr;
            this.PositionCamera();
        }

        public void PerformHashing()
        {
            State = new List<ulong>();
            this.Alghorithm = new SHA3Managed();
            var data = Encoding.ASCII.GetBytes("abc");
            data = this.Alghorithm.ComputeHash(data);
            this.Hash = BitConverter.ToString(data, 0, data.Length);

            // foreach (var state in Alghorithm.State)
            // {
            // State.Add(state);
            // }
            this.ConvertHashState();

            this.OnPropertyChanged("FinalState");
            this.OnPropertyChanged("Hash");
        }

        public void PlusX_Click(object sender, RoutedEventArgs e)
        {
            this._cameraPhi += CameraDPhi;
            if (this._cameraPhi < Math.PI / 2.0) this._cameraPhi = Math.PI / 2.0;
            this.PositionCamera();
        }

        public void PlusY_Click(object sender, RoutedEventArgs e)
        {
            this._cameraTheta += CameraDTheta;
            this.PositionCamera();
        }

        public void PlusZ_Click(object sender, RoutedEventArgs e)
        {
            this._cameraR += 10 * CameraDr;
            this.PositionCamera();
        }

        public void RowPresetation()
        {
            const string sth = "abc";

            // Preparing Cube
            this.Cube = new Cube(5, 1, 1, Encoding.ASCII.GetBytes(sth));
        }

        public void SlicePresetation()
        {
            const string sth = "abc";

            // Preparing Cube
            this.Cube = new Cube(1, Encoding.ASCII.GetBytes(sth));
        }

        public void StateCubePresetation()
        {
            // Preparing Cube
            // cube = new Cube(8,new byte[]{});
            const string sth = "abc";

            // Preparing Cube
            this.Cube = new Cube(8, Encoding.ASCII.GetBytes(sth));
        }

        public void UpdateFinalState()
        {
            this.OnPropertyChanged("FinalState");
            this.OnPropertyChanged("Hash");
        }

        internal void PlusY_Click()
        {
            throw new NotImplementedException();
        }

        internal void PlusZ_Click()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
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

        private void ConvertHashState()
        {
            this.FinalState = State.ConvertAll(delegate(ulong i) { return i.ToString("X2"); });
        }

        private void DefineLights(Model3DGroup modelGroup)
        {
            modelGroup.Children.Add(new AmbientLight(Colors.DarkSlateGray));
            modelGroup.Children.Add(new DirectionalLight(Colors.Gray, new Vector3D(3.0, -2.0, 1.0)));
            modelGroup.Children.Add(new DirectionalLight(Colors.DarkGray, new Vector3D(-3.0, -2.0, -1.0)));
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
            // this.xValue.Text = this._theCamera.Position.X.ToString(CultureInfo.InvariantCulture);
            // this.yValue.Text = this._theCamera.Position.Y.ToString(CultureInfo.InvariantCulture);
            // this.zValue.Text = this._theCamera.Position.Z.ToString(CultureInfo.InvariantCulture);
        }
    }
}