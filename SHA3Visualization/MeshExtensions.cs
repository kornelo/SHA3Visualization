namespace SHA3Visualization
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    public static class MeshExtensions
    {
        // Make an arrow.
        public static void AddArrow(
            this MeshGeometry3D mesh,
            Point3D point1,
            Point3D point2,
            Vector3D up,
            double barb_length)
        {
            // Make the shaft.
            AddSegment(mesh, point1, point2, 0.05, true);

            // Get a unit vector in the direction of the segment.
            var v = point2 - point1;
            v.Normalize();

            // Get a perpendicular unit vector in the plane of the arrowhead.
            var perp = Vector3D.CrossProduct(v, up);
            perp.Normalize();

            // Calculate the arrowhead end points.
            var v1 = ScaleVector(-v + perp, barb_length);
            var v2 = ScaleVector(-v - perp, barb_length);

            // Draw the arrowhead.
            AddSegment(mesh, point2, point2 + v1, up, 0.05);
            AddSegment(mesh, point2, point2 + v2, up, 0.05);
        }

        // Make an axis with tic marks.
        public static void AddAxis(
            this MeshGeometry3D mesh,
            Point3D point1,
            Point3D point2,
            Vector3D up,
            double tic_diameter,
            double tic_separation)
        {
            // Make the shaft.
            AddSegment(mesh, point1, point2, 0.05, true);

            // Get a unit vector in the direction of the segment.
            var v = point2 - point1;
            var length = v.Length;
            v.Normalize();

            // Find the position of the first tic mark.
            var tic_point1 = point1 + v * (tic_separation - 0.025);

            // Make tic marks.
            var num_tics = (int)(length / tic_separation) - 1;
            for (var i = 0; i < num_tics; i++)
            {
                var tic_point2 = tic_point1 + v * 0.05;
                AddSegment(mesh, tic_point1, tic_point2, tic_diameter);
                tic_point1 += v * tic_separation;
            }
        }

        // Make a box with the indicated dimensions and
        // corner with minimal X, Y, and Z coordinates.
        public static void AddBox(this MeshGeometry3D mesh, float x, float y, float z, float dx, float dy, float dz)
        {
            // Bottom (min Y).
            AddTriangle(mesh, new Point3D(x, y, z), new Point3D(x + dx, y, z), new Point3D(x + dx, y, z + dz));
            AddTriangle(mesh, new Point3D(x, y, z), new Point3D(x + dx, y, z + dz), new Point3D(x, y, z + dz));

            // Top (max Y).
            AddTriangle(
                mesh,
                new Point3D(x, y + dy, z),
                new Point3D(x, y + dy, z + dz),
                new Point3D(x + dx, y + dy, z + dz));
            AddTriangle(
                mesh,
                new Point3D(x, y + dy, z),
                new Point3D(x + dx, y + dy, z + dz),
                new Point3D(x + dx, y + dy, z));

            // Left (min X).
            AddTriangle(mesh, new Point3D(x, y, z), new Point3D(x, y, z + dz), new Point3D(x, y + dy, z + dz));
            AddTriangle(mesh, new Point3D(x, y, z), new Point3D(x, y + dy, z + dz), new Point3D(x, y + dy, z));

            // Right (max X).
            AddTriangle(
                mesh,
                new Point3D(x + dx, y, z),
                new Point3D(x + dx, y + dy, z),
                new Point3D(x + dx, y + dy, z + dz));
            AddTriangle(
                mesh,
                new Point3D(x + dx, y, z),
                new Point3D(x + dx, y + dy, z + dz),
                new Point3D(x + dx, y, z + dz));

            // Front (max Z).
            AddTriangle(
                mesh,
                new Point3D(x, y, z + dz),
                new Point3D(x + dx, y, z + dz),
                new Point3D(x + dx, y + dy, z + dz));
            AddTriangle(
                mesh,
                new Point3D(x, y, z + dz),
                new Point3D(x + dx, y + dy, z + dz),
                new Point3D(x, y + dy, z + dz));

            // Back (min Z).
            AddTriangle(mesh, new Point3D(x, y, z), new Point3D(x, y + dy, z), new Point3D(x + dx, y + dy, z));
            AddTriangle(mesh, new Point3D(x, y, z), new Point3D(x + dx, y + dy, z), new Point3D(x + dx, y, z));
        }

        // Add a cage.
        public static void AddCage(
            this MeshGeometry3D mesh,
            float x,
            float y,
            float z,
            float dx,
            float dy,
            float dz,
            double thickness)
        {
            var xmin = x;
            var xmax = x + dx;
            var ymin = y;
            var ymax = y + dy;
            var zmin = z;
            var zmax = z + dz;

            // Top.
            var up = new Vector3D(0, 1, 0);
            AddSegment(mesh, new Point3D(xmax, ymax, zmax), new Point3D(xmax, ymax, -zmax), up, thickness, true);
            AddSegment(mesh, new Point3D(xmax, ymax, -zmax), new Point3D(xmin, ymax, -zmax), up, thickness, true);
            AddSegment(mesh, new Point3D(xmin, ymax, -zmax), new Point3D(xmin, ymax, zmax), up, thickness, true);
            AddSegment(mesh, new Point3D(xmin, ymax, zmax), new Point3D(xmax, ymax, zmax), up, thickness, true);

            // Bottom.
            AddSegment(mesh, new Point3D(xmax, ymin, zmax), new Point3D(xmax, ymin, -zmax), up, thickness, true);
            AddSegment(mesh, new Point3D(xmax, ymin, -zmax), new Point3D(xmin, ymin, -zmax), up, thickness, true);
            AddSegment(mesh, new Point3D(xmin, ymin, -zmax), new Point3D(xmin, ymin, zmax), up, thickness, true);
            AddSegment(mesh, new Point3D(xmin, ymin, zmax), new Point3D(xmax, ymin, zmax), up, thickness, true);

            // Sides.
            var right = new Vector3D(1, 0, 0);
            AddSegment(mesh, new Point3D(xmax, ymin, zmax), new Point3D(xmax, ymax, zmax), right, thickness, true);
            AddSegment(mesh, new Point3D(xmax, ymin, -zmax), new Point3D(xmax, ymax, -zmax), right, thickness, true);
            AddSegment(mesh, new Point3D(xmin, ymin, zmax), new Point3D(xmin, ymax, zmax), right, thickness, true);
            AddSegment(mesh, new Point3D(xmin, ymin, -zmax), new Point3D(xmin, ymax, -zmax), right, thickness, true);
        }

        // Make a thin rectangular prism between the two points.
        // If extend is true, extend the segment by half the
        // thickness so segments with the same end points meet nicely.
        // If up is missing, create a perpendicular vector to use.
        public static void AddSegment(
            MeshGeometry3D mesh,
            Point3D point1,
            Point3D point2,
            double thickness,
            bool extend)
        {
            // Find an up vector that is not colinear with the segment.
            // Start with a vector parallel to the Y axis.
            var up = new Vector3D(0, 1, 0);

            // If the segment and up vector point in more or less the
            // same direction, use an up vector parallel to the X axis.
            var segment = point2 - point1;
            segment.Normalize();
            if (Math.Abs(Vector3D.DotProduct(up, segment)) > 0.9) up = new Vector3D(1, 0, 0);

            // Add the segment.
            AddSegment(mesh, point1, point2, up, thickness, extend);
        }

        public static void AddSegment(MeshGeometry3D mesh, Point3D point1, Point3D point2, double thickness)
        {
            AddSegment(mesh, point1, point2, thickness, false);
        }

        public static void AddSegment(
            MeshGeometry3D mesh,
            Point3D point1,
            Point3D point2,
            Vector3D up,
            double thickness)
        {
            AddSegment(mesh, point1, point2, up, thickness, false);
        }

        public static void AddSegment(
            MeshGeometry3D mesh,
            Point3D point1,
            Point3D point2,
            Vector3D up,
            double thickness,
            bool extend)
        {
            // Get the segment's vector.
            var v = point2 - point1;

            if (extend)
            {
                // Increase the segment's length on both ends by thickness / 2.
                var n = ScaleVector(v, thickness / 2.0);
                point1 -= n;
                point2 += n;
            }

            // Get the scaled up vector.
            var n1 = ScaleVector(up, thickness / 2.0);

            // Get another scaled perpendicular vector.
            var n2 = Vector3D.CrossProduct(v, n1);
            n2 = ScaleVector(n2, thickness / 2.0);

            // Make a skinny box.
            // p1pm means point1 PLUS n1 MINUS n2.
            var p1pp = point1 + n1 + n2;
            var p1mp = point1 - n1 + n2;
            var p1pm = point1 + n1 - n2;
            var p1mm = point1 - n1 - n2;
            var p2pp = point2 + n1 + n2;
            var p2mp = point2 - n1 + n2;
            var p2pm = point2 + n1 - n2;
            var p2mm = point2 - n1 - n2;

            // Sides.
            AddTriangle(mesh, p1pp, p1mp, p2mp);
            AddTriangle(mesh, p1pp, p2mp, p2pp);

            AddTriangle(mesh, p1pp, p2pp, p2pm);
            AddTriangle(mesh, p1pp, p2pm, p1pm);

            AddTriangle(mesh, p1pm, p2pm, p2mm);
            AddTriangle(mesh, p1pm, p2mm, p1mm);

            AddTriangle(mesh, p1mm, p2mm, p2mp);
            AddTriangle(mesh, p1mm, p2mp, p1mp);

            // Ends.
            AddTriangle(mesh, p1pp, p1pm, p1mm);
            AddTriangle(mesh, p1pp, p1mm, p1mp);

            AddTriangle(mesh, p2pp, p2mp, p2mm);
            AddTriangle(mesh, p2pp, p2mm, p2pm);
        }

        // Calculate a triangle's normal vector.
        public static Vector3D FindTriangleNormal(Point3D point1, Point3D point2, Point3D point3)
        {
            // Get two edge vectors.
            var v1 = point2 - point1;
            var v2 = point3 - point2;

            // Get the cross product.
            var n = Vector3D.CrossProduct(v1, v2);

            // Normalize.
            n.Normalize();

            return n;
        }

        // Set the vector's length.
        public static Vector3D ScaleVector(Vector3D vector, double length)
        {
            var scale = length / vector.Length;
            return new Vector3D(vector.X * scale, vector.Y * scale, vector.Z * scale);
        }

        // Give the mesh a diffuse material.
        public static GeometryModel3D SetMaterial(this MeshGeometry3D mesh, Brush brush, bool set_back_material)
        {
            var material = new DiffuseMaterial(brush);
            var model = new GeometryModel3D(mesh, material);
            if (set_back_material) model.BackMaterial = material;
            return model;
        }

        // Return a MeshGeometry3D representing this mesh's triangle normals.
        public static MeshGeometry3D ToTriangleNormals(this MeshGeometry3D mesh, double length, double thickness)
        {
            // Make a mesh to hold the normals.
            var normals = new MeshGeometry3D();

            // Loop through the mesh's triangles.
            for (var triangle = 0; triangle < mesh.TriangleIndices.Count; triangle += 3)
            {
                // Get the triangle's vertices.
                var point1 = mesh.Positions[mesh.TriangleIndices[triangle]];
                var point2 = mesh.Positions[mesh.TriangleIndices[triangle + 1]];
                var point3 = mesh.Positions[mesh.TriangleIndices[triangle + 2]];

                // Make the triangle's normal
                AddTriangleNormal(mesh, normals, point1, point2, point3, length, thickness);
            }

            return normals;
        }

        // Return a MeshGeometry3D representing this mesh's triangle normals.
        public static MeshGeometry3D ToVertexNormals(this MeshGeometry3D mesh, double length, double thickness)
        {
            // Copy existing vertex normals.
            var vertex_normals = new Vector3D[mesh.Positions.Count];
            for (var i = 0; i < mesh.Normals.Count; i++) vertex_normals[i] = mesh.Normals[i];

            // Calculate missing vetex normals.
            for (var vertex = mesh.Normals.Count; vertex < mesh.Positions.Count; vertex++)
            {
                var total_vector = new Vector3D(0, 0, 0);
                var num_triangles = 0;

                // Find the triangles that contain this vertex.
                for (var triangle = 0; triangle < mesh.TriangleIndices.Count; triangle += 3)
                {
                    // See if this triangle contains the vertex.
                    var vertex1 = mesh.TriangleIndices[triangle];
                    var vertex2 = mesh.TriangleIndices[triangle + 1];
                    var vertex3 = mesh.TriangleIndices[triangle + 2];
                    if (vertex1 == vertex || vertex2 == vertex || vertex3 == vertex)
                    {
                        // This triangle contains this vertex.
                        // Calculate its surface normal.
                        var normal = FindTriangleNormal(
                            mesh.Positions[vertex1],
                            mesh.Positions[vertex2],
                            mesh.Positions[vertex3]);

                        // Add the new normal to the total.
                        total_vector = new Vector3D(
                            total_vector.X + normal.X,
                            total_vector.Y + normal.Y,
                            total_vector.Z + normal.Z);
                        num_triangles++;
                    }
                }

                // Set the vertex's normal.
                if (num_triangles > 0)
                    vertex_normals[vertex] = new Vector3D(
                        total_vector.X / num_triangles,
                        total_vector.Y / num_triangles,
                        total_vector.Z / num_triangles);
            }

            // Make a mesh to hold the normals.
            var normals = new MeshGeometry3D();

            // Convert the normal vectors into segments.
            for (var i = 0; i < mesh.Positions.Count; i++)
            {
                // Set the normal vector's length.
                vertex_normals[i] = ScaleVector(vertex_normals[i], length);

                // Find the other end point.
                var endpoint = mesh.Positions[i] + vertex_normals[i];

                // Create the segment.
                AddSegment(normals, mesh.Positions[i], endpoint, thickness);
            }

            return normals;
        }

        // Return a MeshGeometry3D representing this mesh's wireframe.
        public static MeshGeometry3D ToWireframe(this MeshGeometry3D mesh, double thickness)
        {
            // Make a dictionary in case triangles share segments
            // so we don't draw the same segment twice.
            var already_drawn = new Dictionary<int, int>();

            // Make a mesh to hold the wireframe.
            var wireframe = new MeshGeometry3D();

            // Loop through the mesh's triangles.
            for (var triangle = 0; triangle < mesh.TriangleIndices.Count; triangle += 3)
            {
                // Get the triangle's corner indices.
                var index1 = mesh.TriangleIndices[triangle];
                var index2 = mesh.TriangleIndices[triangle + 1];
                var index3 = mesh.TriangleIndices[triangle + 2];

                // Make the triangle's three segments.
                AddTriangleSegment(mesh, wireframe, already_drawn, index1, index2, thickness);
                AddTriangleSegment(mesh, wireframe, already_drawn, index2, index3, thickness);
                AddTriangleSegment(mesh, wireframe, already_drawn, index3, index1, thickness);
            }

            return wireframe;
        }

        // Add axes with the given lengths.
        public static MeshGeometry3D XAxisArrow(float length)
        {
            var mesh = new MeshGeometry3D();
            mesh.AddArrow(new Point3D(12, 0, 0), new Point3D(6, 0, 0), new Vector3D(0, 1, 0), 0.5);
            return mesh;
        }

        public static MeshGeometry3D YAxisArrow(float length)
        {
            var mesh = new MeshGeometry3D();
            mesh.AddArrow(new Point3D(12, 0, 0), new Point3D(12, 6, 0), new Vector3D(1, 0, 0), 0.5);
            return mesh;
        }

        public static MeshGeometry3D ZAxisArrow(float length)
        {
            var mesh = new MeshGeometry3D();
            mesh.AddArrow(new Point3D(12, 0, 0), new Point3D(12, 0, 6), new Vector3D(0, 1, 0), 0.5);
            return mesh;
        }

        // Add a triangle to the indicated mesh.
        // Do not reuse points so triangles don't share normals.
        private static void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
        {
            // Create the points.
            var index1 = mesh.Positions.Count;
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);

            // Create the triangle.
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1);
        }

        // Add a segment representing the triangle's normal to the normals mesh.
        private static void AddTriangleNormal(
            MeshGeometry3D mesh,
            MeshGeometry3D normals,
            Point3D point1,
            Point3D point2,
            Point3D point3,
            double length,
            double thickness)
        {
            // Get the triangle's normal.
            var n = FindTriangleNormal(point1, point2, point3);

            // Set the length.
            n = ScaleVector(n, length);

            // Find the center of the triangle.
            var endpoint1 = new Point3D(
                (point1.X + point2.X + point3.X) / 3.0,
                (point1.Y + point2.Y + point3.Y) / 3.0,
                (point1.Z + point2.Z + point3.Z) / 3.0);

            // Find the segment's other end point.
            var endpoint2 = endpoint1 + n;

            // Create the segment.
            AddSegment(normals, endpoint1, endpoint2, thickness);
        }

        // Add the triangle's three segments to the wireframe mesh.
        private static void AddTriangleSegment(
            MeshGeometry3D mesh,
            MeshGeometry3D wireframe,
            Dictionary<int, int> already_drawn,
            int index1,
            int index2,
            double thickness)
        {
            // Get a unique ID for a segment connecting the two points.
            if (index1 > index2)
            {
                var temp = index1;
                index1 = index2;
                index2 = temp;
            }

            var segment_id = index1 * mesh.Positions.Count + index2;

            // If we've already added this segment for
            // another triangle, do nothing.
            if (already_drawn.ContainsKey(segment_id)) return;
            already_drawn.Add(segment_id, segment_id);

            // Create the segment.
            AddSegment(wireframe, mesh.Positions[index1], mesh.Positions[index2], thickness);
        }
    }
}