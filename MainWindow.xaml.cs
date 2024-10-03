using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace GameMaker
{
    public partial class MainWindow : Window
    {
        private bool isRightMouseDown = false;
        private Point lastMousePosition;
        private List<SceneObject> sceneObjects = new List<SceneObject>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NewObject_Click(object sender, RoutedEventArgs e)
        {
            AddCube();
        }

        private void AddCube()
        {
            var cube = new ModelVisual3D();
            var geometry = new GeometryModel3D
            {
                Geometry = new MeshGeometry3D
                {
                    Positions = new Point3DCollection
                    {
                        new Point3D(-0.5, -0.5, -0.5),
                        new Point3D(0.5, -0.5, -0.5),
                        new Point3D(0.5, 0.5, -0.5),
                        new Point3D(-0.5, 0.5, -0.5),
                        new Point3D(-0.5, -0.5, 0.5),
                        new Point3D(0.5, -0.5, 0.5),
                        new Point3D(0.5, 0.5, 0.5),
                        new Point3D(-0.5, 0.5, 0.5)
                    },
                    TriangleIndices = new Int32Collection
                    {
                        0, 1, 2, 0, 2, 3,
                        4, 5, 6, 4, 6, 7,
                        0, 1, 5, 0, 5, 4,
                        2, 3, 7, 2, 7, 6,
                        1, 2, 6, 1, 6, 5,
                        0, 3, 7, 0, 7, 4
                    }
                },
                Material = new DiffuseMaterial(new SolidColorBrush(Colors.Blue))
            };

            cube.Content = geometry;
            var sceneObject = new SceneObject(cube);
            sceneObjects.Add(sceneObject);
            viewport.Children.Add(cube);
            SceneHierarchyList.Items.Add("Cube " + (sceneObjects.Count - 1));
        }

        private void AddRigidbody_Click(object sender, RoutedEventArgs e)
        {
            if (SceneHierarchyList.SelectedItem != null)
            {
                int selectedIndex = SceneHierarchyList.SelectedIndex;
                var selectedObject = sceneObjects[selectedIndex];

                // Check if the object already has a Rigidbody
                if (selectedObject.Rigidbody == null)
                {
                    selectedObject.Rigidbody = new Rigidbody();
                    MessageBox.Show($"Rigidbody added to Cube {selectedIndex}");
                    UpdatePropertiesPanel(selectedObject);
                }
                else
                {
                    MessageBox.Show("This object already has a Rigidbody.");
                }
            }
            else
            {
                MessageBox.Show("Select an object in the hierarchy to add a Rigidbody.");
            }
        }

        private void SceneHierarchyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SceneHierarchyList.SelectedItem != null)
            {
                int selectedIndex = SceneHierarchyList.SelectedIndex;
                UpdatePropertiesPanel(sceneObjects[selectedIndex]);
            }
        }

        private void UpdatePropertiesPanel(SceneObject selectedObject)
        {
            PropertiesPanel.Children.Clear();

            if (selectedObject.Rigidbody != null)
            {
                // Mass
                var massLabel = new TextBlock { Text = "Mass:" };
                var massSlider = new Slider { Minimum = 0.1, Maximum = 10, Value = selectedObject.Rigidbody.Mass };
                massSlider.ValueChanged += (s, e) => selectedObject.Rigidbody.Mass = massSlider.Value;
                PropertiesPanel.Children.Add(massLabel);
                PropertiesPanel.Children.Add(massSlider);

                // Drag
                var dragLabel = new TextBlock { Text = "Drag:" };
                var dragSlider = new Slider { Minimum = 0, Maximum = 5, Value = selectedObject.Rigidbody.Drag };
                dragSlider.ValueChanged += (s, e) => selectedObject.Rigidbody.Drag = dragSlider.Value;
                PropertiesPanel.Children.Add(dragLabel);
                PropertiesPanel.Children.Add(dragSlider);
            }
        }

        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            isRightMouseDown = true;
            lastMousePosition = Mouse.GetPosition(this);
            Mouse.Capture(this);
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            isRightMouseDown = false;
            Mouse.Capture(null);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (isRightMouseDown)
            {
                Point currentMousePosition = Mouse.GetPosition(this);
                Vector delta = currentMousePosition - lastMousePosition;

                // Adjust camera rotation based on mouse movement
                camera.Position += new Vector3D(delta.X * 0.01, -delta.Y * 0.01, 0);
                lastMousePosition = currentMousePosition;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Implement movement controls if needed
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            // Implement key up actions if needed
        }
    }

    // Simple class for Rigidbody properties
    public class Rigidbody
    {
        public double Mass { get; set; } = 1.0; // Default mass
        public double Drag { get; set; } = 0.0; // Default drag
    }

    // Class to encapsulate scene objects and their properties
    public class SceneObject
    {
        public ModelVisual3D Model { get; }
        public Rigidbody Rigidbody { get; set; }

        public SceneObject(ModelVisual3D model)
        {
            Model = model;
        }
    }
}
