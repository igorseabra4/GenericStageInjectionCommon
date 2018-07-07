using System.IO;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using GenericStageInjectionCommon.Structs;

namespace GenericStageInjectionCommon.Shared.Parsers
{
    /// <summary>
    /// Provides useful utilities used for working with Wavefront .obj files for the collision exporter/generator.
    /// </summary>
    public class ObjParser
    {
        /// <summary>
        /// Stores the contents of the Wavefront OBJ File.
        /// </summary>
        private string[] _objFileString;

        /// <summary>
        /// Match object for storing the results of each regex capture groups.
        /// </summary>
        private Match _regexMatch;

        /// <summary>
        /// [Constructor] Initializes the class.
        /// </summary>
        public ObjParser(string objFilePath)
        {
            // Load Actual OBJ File
            _objFileString = File.ReadAllLines(objFilePath);
        }

        /// <summary>
        /// Returns the complete version of the OBJ File with the necessary information to generate collision.
        /// After running, use GetCollisionFile to retrieve the information. (or GetVertices/GetTriangles)
        /// </summary>
        public List<Triangle> ReadObjFile()
        {
            // Calculate the vertices and triangles and return each of them.
            List<Vector> vertices = ReadVertices();
            return ReadTriangles(vertices);
        }

        /// <summary>
        /// Works out all of the vertices from the Wavefront OBJ File.
        /// After running, use GetVertices to retrieve the information. 
        /// </summary>
        public List<Vector> ReadVertices()
        {
            List<Vector> vertices = new List<Vector>();

            // Compile Regular Expressions for stripping spaces and definitions from faces and vertices.
            // Intended data goes into the first capture group.
            // Regex CheatSheet: https://www.cheatography.com/davechild/cheat-sheets/regular-expressions/
            // Learn Regex at: https://regexone.com/
            Regex vertexRegex = new Regex(@"v[ ]*(.*)", RegexOptions.Compiled);

            // In the case there are too many vertices.
            try
            {
                // Parse the file line by line.
                foreach (String line in _objFileString)
                {
                    // If the line defines a vertex.
                    if (line.StartsWith("v"))
                    {
                        // Get Regular Expression Matches.
                        _regexMatch = vertexRegex.Match(line);

                        // Group 0 contains entire matched expression, we only want first group.
                        string vertexCoordinates = _regexMatch.Groups[1].Value;

                        // Add vertex onto vertex list.
                        vertices.Add(GetVertex(vertexCoordinates));
                    }
                }
            }
            catch(Exception Ex)
            {
                throw new InvalidDataException("ERROR READING OBJ FILE VERTICES " + Ex.Message);
            }

            return vertices;
        }

        
        /// <summary>
        /// Splits a string which defines three vertices and adds a vertex onto the vertex list.
        /// </summary>
        private Vector GetVertex(string vertexCoordinates)
        {
            // Split the vertex coordinates by spaces.
            string[] verticesString = vertexCoordinates.Split(' ');

            // Declare and assign the individual XYZ Positions
            return new Vector()
            {
                X = Convert.ToSingle(verticesString[0]),
                Y = Convert.ToSingle(verticesString[1]),
                Z = Convert.ToSingle(verticesString[2])
            };
        }

        /// <summary>
        /// Works out all of the triangles' vertices from the Wavefront OBJ File.
        /// After running, use GetTriangles to retrieve the information. 
        /// </summary>
        public List<Triangle> ReadTriangles(List<Vector> vertices)
        {
            List<Triangle> triangles = new List<Triangle>();

            // Compile Regular Expressions for stripping spaces and definitions from faces and vertices.
            // Intended data goes into the first capture group.
            // Regex CheatSheet: https://www.cheatography.com/davechild/cheat-sheets/regular-expressions/
            // Learn Regex at: https://regexone.com/
            Regex faceRegex = new Regex(@"f[ ]*(.*)", RegexOptions.Compiled);

            // In the case there are too many triangles.
            try
            {
                // Parse the file line by line.
                foreach (String line in _objFileString)
                {
                    // If the line defines a face.
                    if (line.StartsWith("f"))
                    {
                        // Get Regular Expression Matches.
                        _regexMatch = faceRegex.Match(line);

                        // Group 0 contains entire matched expression, we only want first group.
                        string faceCoordinates = _regexMatch.Groups[1].Value;

                        // Work out triangle faces.
                        triangles.Add(GetTriangle(faceCoordinates, vertices));
                    }
                }
            }
            catch (Exception Ex)
            {
                throw new InvalidDataException("ERROR READING OBJ FILE VERTICES " + Ex.Message);
            }

            return triangles;
        }

        /// <summary>
        /// Splits a string which defines three vertices and adds a triangle onto the triangle list.
        /// </summary>
        private Triangle GetTriangle(string triangleVertices, List<Vector> vertices)
        {
            // Separate each triangle entry
            string[] trianglesString = triangleVertices.Split(' ');

            // Check if it's face vertex index value only, if it contains texture coordinates or normals, strip them from the string.
            for (int x = 0; x < trianglesString.Length; x++)
            {
                if (trianglesString[x].Contains("/"))
                {
                    trianglesString[x] = trianglesString[x].Substring(0, trianglesString[x].IndexOf("/"));
                }
            }

            // Stores the individual vertex information.
            int vertexOne;
            int vertexTwo;
            int vertexThree;

            // NOTE: Our vertices array starts at 0, but the triangle vertices in the OBJ file start at 1, make sure this index subtraction is correct.
            // NOTE: Some OBJ Exporters may not assign a vertex to some faces, set them to 1 if that should turn to be so, try/catch.
            try { vertexOne = (ushort)(Convert.ToUInt16(trianglesString[0]) - 1); } catch { vertexOne = 1; }
            try { vertexTwo = (ushort)(Convert.ToUInt16(trianglesString[1]) - 1); } catch { vertexTwo = 1; }
            try { vertexThree = (ushort)(Convert.ToUInt16(trianglesString[2]) - 1); } catch { vertexThree = 1; }

            // Declare and assign the individual triangle vertices
            return new Triangle()
            {
                Vertex1 = vertices[vertexOne],
                Vertex2 = vertices[vertexTwo],
                Vertex3 = vertices[vertexThree]
            };
        }
    }
}