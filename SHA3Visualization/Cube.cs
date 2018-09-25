namespace SHA3Visualization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;

    public class Cube
    {
        // The main object model group.
        private Model3DGroup MainModel3Dgroup = new Model3DGroup();

        // Materials used for normal and selected models.
        private Material NormalMaterial;

        // The list of selectable models.
        private Dictionary<GeometryModel3D, string> SelectableModels = new Dictionary<GeometryModel3D, string>();

        private Material SelectedMaterial;

        // The currently selected model.
        private GeometryModel3D SelectedModel = null;

        public Cube()
        {
        }

        public Cube(int size, byte[] values)
        {
            this.DefineModel(5, 5, size, values);
        }

        public Cube(int x, int y, int z, byte[] values)
        {
            this.DefineModel(x, y, z, values);
        }

        public Cube(float x, float y, float z, float dx, float dy, float dz, string value)
        {
            this.FillCube(x, y, z, dx, dy, dz, value);
        }

        // delegate
        private delegate Brush ActionBrush(string value);

        public void FillCube(float x, float y, float z, float dx, float dy, float dz, string value)
        {
            // declaration of main mesh
            var mesh = new MeshGeometry3D();
            {
                // Front
                mesh = new MeshGeometry3D();
                mesh.Positions.Add(new Point3D(x, y, z + dz));
                mesh.Positions.Add(new Point3D(x + dx, y, z + dz));
                mesh.Positions.Add(new Point3D(x + dx, y + dy, z + dz));
                mesh.Positions.Add(new Point3D(x, y + dy, z + dz));
                AddingMeshProperties(ref mesh);
                var geommodel3D = new GeometryModel3D(
                    mesh,
                    new DiffuseMaterial(
                        (Brush)Application.Current.Dispatcher.Invoke(new ActionBrush(PrepareBrush), value)));

                // MainModel3Dgroup.Dispatcher.Invoke(new Action(() =>
                // {
                this.MainModel3Dgroup.Children.Add(geommodel3D);

                // }));

                // Remember that this model is selectable.
                this.SelectableModels.Add(geommodel3D, value);
            }
            {
                // Back
                mesh = new MeshGeometry3D();
                mesh.Positions.Add(new Point3D(x + dx, y, z));
                mesh.Positions.Add(new Point3D(x, y, z));
                mesh.Positions.Add(new Point3D(x, y + dy, z));
                mesh.Positions.Add(new Point3D(x + dx, y + dy, z));
                AddingMeshProperties(ref mesh);
                var geommodel3D = new GeometryModel3D(
                    mesh,
                    new DiffuseMaterial(
                        (Brush)Application.Current.Dispatcher.Invoke(new ActionBrush(PrepareBrush), value)));

                // GeometryModel3D geommodel3d = new GeometryModel3D(mesh, new DiffuseMaterial(PrepareBrush(value)));
                // MainModel3Dgroup.Dispatcher.Invoke(new Action(() => { 
                this.MainModel3Dgroup.Children.Add(geommodel3D); // }));

                // Remember that this model is selectable.
                this.SelectableModels.Add(geommodel3D, value);
            }

            //// Right
            // {
            // mesh = new MeshGeometry3D();
            // mesh.Positions.Add(new Point3D(x + dx , y , z + dz));
            // mesh.Positions.Add(new Point3D(x + dx, y , z));
            // mesh.Positions.Add(new Point3D(x + dx, y + dy, z ));
            // mesh.Positions.Add(new Point3D(x + dx, y + dy , z + dz ));
            // AddingMeshProperties(ref mesh);
            // Brush brush = new ImageBrush(bitmap);
            // GeometryModel3D geommodel3d = new GeometryModel3D(mesh, new DiffuseMaterial(brush));
            // MainModel3Dgroup.Children.Add(geommodel3d);
            // }

            //// Left
            // {
            // mesh = new MeshGeometry3D();
            // mesh.Positions.Add(new Point3D(x , y, z));
            // mesh.Positions.Add(new Point3D(x, y, z + dz));
            // mesh.Positions.Add(new Point3D(x, y + dy, z + dz));
            // mesh.Positions.Add(new Point3D(x, y + dy, z));
            // AddingMeshProperties(ref mesh);
            // Brush brush = new ImageBrush(bitmap);
            // GeometryModel3D geommodel3d = new GeometryModel3D(mesh, new DiffuseMaterial(brush));
            // MainModel3Dgroup.Children.Add(geommodel3d);
            // }

            //// Top
            // {
            // mesh = new MeshGeometry3D();
            // mesh.Positions.Add(new Point3D(x, y + dy, z + dz));
            // mesh.Positions.Add(new Point3D(x + dx, y + dy, z + dz));
            // mesh.Positions.Add(new Point3D(x + dx, y + dy, z));
            // mesh.Positions.Add(new Point3D(x, y + dy, z));

            // AddingMeshProperties(ref mesh);
            // Brush brush = new ImageBrush(bitmap);
            // GeometryModel3D geommodel3d = new GeometryModel3D(mesh, new DiffuseMaterial(brush));
            // MainModel3Dgroup.Children.Add(geommodel3d);
            // }

            //// Bottom
            // {
            // mesh = new MeshGeometry3D();
            // mesh.Positions.Add(new Point3D(x, y, z));
            // mesh.Positions.Add(new Point3D(x + dx, y, z));
            // mesh.Positions.Add(new Point3D(x + dx, y, z + dz));
            // mesh.Positions.Add(new Point3D(x, y, z + dz));
            // AddingMeshProperties(ref mesh);
            // Brush brush = new ImageBrush(bitmap);
            // var geommodel3d = new GeometryModel3D(mesh, new DiffuseMaterial(brush));
            // MainModel3Dgroup.Children.Add(geommodel3d);

            // // Remember that this model is selectable.
            // SelectableModels.Add(geommodel3d);
            // }
        }

        public Dictionary<GeometryModel3D, string> ReturnListOfModels()
        {
            return this.SelectableModels;
        }

        public Model3DGroup ReturnMainModel()
        {
            return this.MainModel3Dgroup;
        }

        private static void AddingMeshProperties(ref MeshGeometry3D mesh)
        {
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(0);
            mesh.TextureCoordinates.Add(new Point(0, 1));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(1, 0));
            mesh.TextureCoordinates.Add(new Point(0, 0));
        }

        private static Brush PrepareBrush(string value)
        {
            var textBlock = new TextBlock
                                {
                                    Text = value,
                                    Background = value == "1" ? Brushes.LawnGreen : Brushes.Yellow
                                };
            var size = new Size(40, 40);
            var viewBox = new Viewbox();
            viewBox.Child = textBlock;
            viewBox.Measure(size);
            viewBox.Arrange(new Rect(size));
            var bitmap = new RenderTargetBitmap(35, 40, 80, 80, PixelFormats.Pbgra32);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            bitmap.Render(viewBox);

            return new ImageBrush(bitmap);
        }

        private void DefineModel(int width, int heigth, int depth, byte[] values)
        {
            // clear out the existing geometry XAML
            this.MainModel3Dgroup.Children.Clear();
            this.SelectableModels.Clear();

            // Make the normal and selected materials.
            this.NormalMaterial = new DiffuseMaterial(Brushes.LightGreen);
            this.SelectedMaterial = new DiffuseMaterial(Brushes.Red);

            var bits = new BitArray(values);
            var listString = new List<string>();

            for (var i = 0; i < bits.Length; i++)
            {
                var builder = new StringBuilder(bits.Length);
                builder.Append(!bits[i] ? "0" : "1");
                listString.Add(builder.ToString());
                builder.Clear();
            }

            // if empty list
            if (listString.Count == 0) listString.Add("0");

            Action<float, float, float, float, float, float, string> actionFillCube = this.FillCube;

            // Lights
            this.MainModel3Dgroup.Dispatcher.Invoke(
                () =>
                    {
                        this.MainModel3Dgroup.Children.Add(new AmbientLight(Colors.Gray));
                        this.MainModel3Dgroup.Children.Add(new DirectionalLight(Colors.Gray, new Vector3D(1, -2, -3)));
                        this.MainModel3Dgroup.Children.Add(new DirectionalLight(Colors.Gray, new Vector3D(-1, 2, 3)));
                    });

            // list iterator
            var listIterator = 0;

            // Create cubes.
            for (var x = 2; x < 2 * width + 2; x += 2)
            for (var y = 2; y < 2 * heigth + 2; y += 2)
            for (var z = 2; z < 2 * depth + 2; z += 2)
            {
                // Make a cube with lower left corner (x, y, z).
                // Task.Run(() =>
                actionFillCube(x, y, z, 1, 1, 1, listString[listIterator]);

                // );
                listIterator = listIterator < listString.Count - 1 ? ++listIterator : 0;
            }

            // X axis.
            var meshX = MeshExtensions.XAxisArrow(6);

            this.MainModel3Dgroup.Children.Add(meshX.SetMaterial(Brushes.Red, false));

            // Y axis.
            var meshY = MeshExtensions.YAxisArrow(6);

            this.MainModel3Dgroup.Children.Add(meshY.SetMaterial(Brushes.Green, false));

            // Z axis.
            var meshZ = MeshExtensions.ZAxisArrow(6);

            this.MainModel3Dgroup.Children.Add(meshZ.SetMaterial(Brushes.Blue, false));
        }
    }
}