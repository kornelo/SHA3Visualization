//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SHA3Visualization
//{
//    class Useless
//    {

//        private void SHA3Visualizer_Loaded(object sender, RoutedEventArgs e)
//        {
//            // Give the camera its initial position.
//            this._theCamera = new PerspectiveCamera { FieldOfView = 60 };
//            this.MainTabView.Camera = this._theCamera;
//            this.PositionCamera();

//            // Define lights.
//            this.DefineLights(this.MainModel3Dgroup);

//            // Fill up for menu ComboBox
//            this.ComboBoxDynamicFill();

//            // Create the model.
//            // PerformHashing();
//            // StateCubePresetation();

//            // Loading View
//            this.RefreshModelView();
//        }

//        private void SHA3Visualizer_MouseDown(object sender, MouseButtonEventArgs e)
//        {
//            // Deselect the prevously selected model.
//            if (this._selectedModel != null)
//            {
//                var value = this._selectableModels[this._selectedModel];

//                // Make the normal and selected materials.
//                this._normalMaterial = new DiffuseMaterial(SelectedBrush(value));

//                this._selectedModel.Material = this._normalMaterial;

//                // back site of number
//                var index = this._selectableModels[this._selectedModel].IndexOf(value, StringComparison.Ordinal);
//                if (Math.Abs(this._selectedModel.Bounds.Z % 2) < 1)
//                    this._selectedModel = this._selectableModels.ElementAt(index + 1).Key;
//                else this._selectedModel = this._selectableModels.ElementAt(index - 1).Key;

//                this._selectedModel.Material = this._normalMaterial;
//                this._selectedModel = null;
//            }

//            // Get the mouse's position relative to the viewport.
//            var mousePos = e.GetPosition(this.MainTabView);

//            // Perform the hit test.
//            var result = VisualTreeHelper.HitTest(this.MainTabView, mousePos);

//            // See if we hit a model.
//            var meshResult = result as RayMeshGeometry3DHitTestResult;
//            if (meshResult != null)
//            {
//                var model = (GeometryModel3D)meshResult.ModelHit;
//                if (this._selectableModels.ContainsKey(model))
//                {
//                    this._selectedModel = model;
//                    this._selectedModel.Material = this._selectedMaterial;

//                    var value = this._selectableModels[this._selectedModel];

//                    // Make the selected materials.
//                    this._selectedMaterial = new DiffuseMaterial(SelectedBrush(value, true));

//                    // back site of number selection
//                    var index = this._selectableModels[this._selectedModel].IndexOf(value, StringComparison.Ordinal);
//                    if (Math.Abs(this._selectedModel.Bounds.Z % 2) < 1)
//                        this._selectedModel = this._selectableModels.ElementAt(index + 1).Key;
//                    else this._selectedModel = this._selectableModels.ElementAt(index - 1).Key;

//                    this._selectedModel.Material = this._selectedMaterial;
//                }
//            }
//        }

//private void SHA3Visualizer_KeyDown(object sender, KeyEventArgs e)
//{
//switch (e.Key)
//{
//case Key.Up:
//this._cameraPhi += CameraDPhi;
//if (this._cameraPhi < Math.PI / 2.0) this._cameraPhi = Math.PI / 2.0;
//break;
//case Key.Down:
//this._cameraPhi -= CameraDPhi;
//if (this._cameraPhi < -Math.PI / 2.0) this._cameraPhi = -Math.PI / 2.0;
//break;
//case Key.Left:
//this._cameraTheta += CameraDTheta;
//break;
//case Key.Right:
//this._cameraTheta -= CameraDTheta;
//break;
//case Key.Add:
//case Key.OemPlus:
//this._cameraR -= CameraDr;
//if (this._cameraR < CameraDr) this._cameraR = CameraDr;
//break;
//case Key.Subtract:
//case Key.OemMinus:
//this._cameraR += CameraDr;
//break;
//}

//// Update the camera's position.
//this.PositionCamera();
//}
//    }
//}
