using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace SHA3Visualization
{
    class Cube
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

        #endregion

        public Cube(Model3DGroup model_group)
        {
            MainModel3Dgroup = model_group;
            DefineModel(20,20,20);
        }

        public Model3DGroup ReturnMainModel()
        {
            return MainModel3Dgroup;
        }

        public List<GeometryModel3D> ReturnListOfModels()
        {
            return SelectableModels;
        }

        private void DefineModel(int width, int heigth, int depth)
        {
            // Make the normal and selected materials.
            NormalMaterial = new DiffuseMaterial(Brushes.LightGreen);
            SelectedMaterial = new DiffuseMaterial(Brushes.Red);

            // Create some cubes.
            for (int x = -(width/2); x <= (width/2); x += 2)
            {
                for (int y = -(heigth/2); y <= (heigth/2); y += 2)
                {
                    for (int z = -(depth/2); z <= (depth/2); z += 2)
                    {
                        // Make a cube with lower left corner (x, y, z).
                        var mesh = new MeshGeometry3D();
                        FillCube(mesh, x, y, z, 1, 1, 1, "0");

                    }
                }
            }

            // X axis.
            MeshGeometry3D mesh_x = MeshExtensions.XAxisArrow(6);
            MainModel3Dgroup.Children.Add(mesh_x.SetMaterial(Brushes.Red, false));

            // Y axis.
            MeshGeometry3D mesh_y = MeshExtensions.YAxisArrow(6);
            MainModel3Dgroup.Children.Add(mesh_y.SetMaterial(Brushes.Green, false));

            // Z axis.
            MeshGeometry3D mesh_z = MeshExtensions.ZAxisArrow(6);
            MainModel3Dgroup.Children.Add(mesh_z.SetMaterial(Brushes.Blue, false));
        }

        public void FillCube(MeshGeometry3D mesh, float x, float y, float z, float dx, float dy, float dz, string value)
        {
            // clear out the existing geometry XAML
            //MainModel3Dgroup.Children.Clear(); 

            // Lights

            MainModel3Dgroup.Children.Add(new AmbientLight(Colors.Gray));
            MainModel3Dgroup.Children.Add(new DirectionalLight(Colors.Gray, new Vector3D(1, -2, -3)));
            MainModel3Dgroup.Children.Add(new DirectionalLight(Colors.Gray, new Vector3D(-1, 2, 3)));

            // Front
            {
                mesh = new MeshGeometry3D();
                mesh.Positions.Add(new Point3D(x, y, z + dz));
                mesh.Positions.Add(new Point3D(x + dx, y, z + dz));
                mesh.Positions.Add(new Point3D(x + dx, y + dy, z + dz));
                mesh.Positions.Add(new Point3D(x, y + dy, z + dz));
                AddingMeshProperties(ref mesh);
                GeometryModel3D geommodel3d = new GeometryModel3D(mesh, new DiffuseMaterial(PrepareBrush(value)));
                MainModel3Dgroup.Children.Add(geommodel3d);

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
                GeometryModel3D geommodel3d = new GeometryModel3D(mesh, new DiffuseMaterial(PrepareBrush(value)));
                MainModel3Dgroup.Children.Add(geommodel3d);

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
           
            bitmap.Render(viewBox);

            return new ImageBrush(bitmap);
        }
    }
}
