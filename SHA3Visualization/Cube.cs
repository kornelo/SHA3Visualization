using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace SHA3Visualization
{
    public class Cube
    {

        #region Initializations

        // The main object model group.
        private Model3DGroup MainModel3Dgroup = new Model3DGroup();

        // The currently selected model.
        private GeometryModel3D SelectedModel = null;

        // Materials used for normal and selected models.
        private Material NormalMaterial, SelectedMaterial;

        // The list of selectable models.
        private List<GeometryModel3D> SelectableModels =
            new List<GeometryModel3D>();

        //delegate
        private delegate Brush ActionBrush(string value);


        #endregion

        public Cube()
        { }

        public Cube(int size, byte[] values )
        {
            DefineModel(5,5,size, values);
        }

        public Model3DGroup ReturnMainModel()
        {
            return MainModel3Dgroup;
        }

        public List<GeometryModel3D> ReturnListOfModels()
        {
            return SelectableModels;
        }

        private void DefineModel(int width, int heigth, int depth, byte[] values)
        {
            // clear out the existing geometry XAML
            MainModel3Dgroup.Children.Clear();
            SelectableModels.Clear();

            // Make the normal and selected materials.
            NormalMaterial = new DiffuseMaterial(Brushes.LightGreen);
            SelectedMaterial = new DiffuseMaterial(Brushes.Red);

            List<string> listString = new List<string>();

            for(var i=0; i<values.Length; i++)
            {
                var builder = new StringBuilder(values.Length);
                builder.Append(values[i].ToString("X2"));
                listString.Add(builder.ToString());
                builder.Clear();
            }


            Action<float,float,float,float,float,float, string> actionFillCube = FillCube;

            // Lights
            MainModel3Dgroup.Dispatcher.Invoke(new Action(() =>
            {
                MainModel3Dgroup.Children.Add(new AmbientLight(Colors.Gray));
                MainModel3Dgroup.Children.Add(new DirectionalLight(Colors.Gray, new Vector3D(1, -2, -3)));
                MainModel3Dgroup.Children.Add(new DirectionalLight(Colors.Gray, new Vector3D(-1, 2, 3)));

            }));

            //list iterator
            var listIterator = 0;
            
            // Create some cubes.
            for (int x = -(width); x < (width); x += 2)
            {
                for (int y = -(heigth); y < (heigth) ; y += 2)
                {
                    for (int z = -(depth); z < (depth); z += 2)
                    {
                        // Make a cube with lower left corner (x, y, z).
                        //Task.Run(() =>
                        actionFillCube(x, y, z, 1, 1, 1, listString[listIterator]);
                        //);
                        listIterator = (listIterator < listString.Count-1) ? ++listIterator : 0;
                    }
                }
            }

            // X axis.
            MeshGeometry3D mesh_x = MeshExtensions.XAxisArrow(6);
           // MainModel3Dgroup.Dispatcher.Invoke(new Action(() =>
          //  {
                MainModel3Dgroup.Children.Add(mesh_x.SetMaterial(Brushes.Red, false));
           // }));
            
            // Y axis.
            MeshGeometry3D mesh_y = MeshExtensions.YAxisArrow(6);
          //  MainModel3Dgroup.Dispatcher.Invoke(new Action(() =>
          //  {
                MainModel3Dgroup.Children.Add(mesh_y.SetMaterial(Brushes.Green, false));
           // }));

            // Z axis.
            MeshGeometry3D mesh_z = MeshExtensions.ZAxisArrow(6);
          //  MainModel3Dgroup.Dispatcher.Invoke(new Action(() =>
           // {
                MainModel3Dgroup.Children.Add(mesh_z.SetMaterial(Brushes.Blue, false));
            //}));
        }

        public void FillCube( float x, float y, float z, float dx, float dy, float dz, string value)
        {
            
            //declaration of main mesh
            var mesh = new MeshGeometry3D();
            
            // Front
            {
                mesh = new MeshGeometry3D();
                mesh.Positions.Add(new Point3D(x, y, z + dz));
                mesh.Positions.Add(new Point3D(x + dx, y, z + dz));
                mesh.Positions.Add(new Point3D(x + dx, y + dy, z + dz));
                mesh.Positions.Add(new Point3D(x, y + dy, z + dz));
                AddingMeshProperties(ref mesh);
                GeometryModel3D geommodel3d = new GeometryModel3D(mesh, new DiffuseMaterial((Brush)Application.Current.Dispatcher.Invoke((new ActionBrush(PrepareBrush)),value)));

               //MainModel3Dgroup.Dispatcher.Invoke(new Action(() =>
               //{
                   MainModel3Dgroup.Children.Add(geommodel3d);
               //}));

                // Remember that this model is selectable.
                SelectableModels.Add(geommodel3d);
            }

            // Back
            {
                mesh = new MeshGeometry3D();
                mesh.Positions.Add(new Point3D(x + dx, y, z));
                mesh.Positions.Add(new Point3D(x, y, z));
                mesh.Positions.Add(new Point3D(x, y + dy, z));
                mesh.Positions.Add(new Point3D(x + dx, y + dy, z));
                AddingMeshProperties(ref mesh);
                GeometryModel3D geommodel3d = new GeometryModel3D(mesh, new DiffuseMaterial((Brush)Application.Current.Dispatcher.Invoke((new ActionBrush(PrepareBrush)), value)));
                //GeometryModel3D geommodel3d = new GeometryModel3D(mesh, new DiffuseMaterial(PrepareBrush(value)));
                //MainModel3Dgroup.Dispatcher.Invoke(new Action(() => { 
                MainModel3Dgroup.Children.Add(geommodel3d); //}));
                // MainModel3Dgroup.Children.Add(geommodel3d);

                // Remember that this model is selectable.
                SelectableModels.Add(geommodel3d);

            }

            #region additionalSites

            //// Right
            //{
            //    mesh = new MeshGeometry3D();
            //    mesh.Positions.Add(new Point3D(x + dx , y , z + dz));
            //    mesh.Positions.Add(new Point3D(x + dx, y , z));
            //    mesh.Positions.Add(new Point3D(x + dx, y + dy, z ));
            //    mesh.Positions.Add(new Point3D(x + dx, y + dy , z + dz ));
            //    AddingMeshProperties(ref mesh);
            //    Brush brush = new ImageBrush(bitmap);
            //    GeometryModel3D geommodel3d = new GeometryModel3D(mesh, new DiffuseMaterial(brush));
            //    MainModel3Dgroup.Children.Add(geommodel3d);
            //}

            //// Left
            //{
            //    mesh = new MeshGeometry3D();
            //    mesh.Positions.Add(new Point3D(x , y, z));
            //    mesh.Positions.Add(new Point3D(x, y, z + dz));
            //    mesh.Positions.Add(new Point3D(x, y + dy, z + dz));
            //    mesh.Positions.Add(new Point3D(x, y + dy, z));
            //    AddingMeshProperties(ref mesh);
            //    Brush brush = new ImageBrush(bitmap);
            //    GeometryModel3D geommodel3d = new GeometryModel3D(mesh, new DiffuseMaterial(brush));
            //    MainModel3Dgroup.Children.Add(geommodel3d);
            //}

            //// Top
            //{
            //    mesh = new MeshGeometry3D();
            //    mesh.Positions.Add(new Point3D(x, y + dy, z + dz));
            //    mesh.Positions.Add(new Point3D(x + dx, y + dy, z + dz));
            //    mesh.Positions.Add(new Point3D(x + dx, y + dy, z));
            //    mesh.Positions.Add(new Point3D(x, y + dy, z));

            //    AddingMeshProperties(ref mesh);
            //    Brush brush = new ImageBrush(bitmap);
            //    GeometryModel3D geommodel3d = new GeometryModel3D(mesh, new DiffuseMaterial(brush));
            //    MainModel3Dgroup.Children.Add(geommodel3d);
            //}

            //// Bottom
            //{
            //    mesh = new MeshGeometry3D();
            //    mesh.Positions.Add(new Point3D(x, y, z));
            //    mesh.Positions.Add(new Point3D(x + dx, y, z));
            //    mesh.Positions.Add(new Point3D(x + dx, y, z + dz));
            //    mesh.Positions.Add(new Point3D(x, y, z + dz));
            //    AddingMeshProperties(ref mesh);
            //    Brush brush = new ImageBrush(bitmap);
            //    var geommodel3d = new GeometryModel3D(mesh, new DiffuseMaterial(brush));
            //    MainModel3Dgroup.Children.Add(geommodel3d);

            //    // Remember that this model is selectable.
            //    SelectableModels.Add(geommodel3d);
            //}

            #endregion

        }

        private void AddingMeshProperties(ref MeshGeometry3D mesh)
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

        private Brush PrepareBrush(string value)
        { 
            TextBlock textBlock = new TextBlock() { Text = value, Background = Brushes.Transparent };
            Size size = new Size(40, 40);
            Viewbox viewBox = new Viewbox();
            viewBox.Child = textBlock;
            viewBox.Measure(size);
            viewBox.Arrange(new Rect(size));
            RenderTargetBitmap bitmap = new RenderTargetBitmap(35, 40, 80, 80, PixelFormats.Pbgra32);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            bitmap.Render(viewBox);

            return new ImageBrush(bitmap);
        }

    }
}
