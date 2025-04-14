using System;
using System.Collections.Generic;
using Silk.NET.OpenGL;

namespace Szeminarium1_24_02_17_2
{
    internal class GlBarrel
    {
        public uint Vao { get; }
        public uint Vertices { get; }
        public uint Colors { get; }
        public uint Indices { get; }
        public uint Normals { get; }
        public uint IndexArrayLength { get; }

        private GL Gl;

        private GlBarrel(uint vao, uint vertices, uint colors, uint indices, uint normals, uint indexArrayLength, GL gl)
        {
            Vao = vao;
            Vertices = vertices;
            Colors = colors;
            Indices = indices;
            Normals = normals;
            IndexArrayLength = indexArrayLength;
            Gl = gl;
        }

        public static unsafe GlBarrel CreateBarrel(GL Gl, float[] faceColor, bool rotatedNormals)
        {
            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            const int numSides = 18;
            const float radius = 0.5f;
            const float height = 1.0f;
            const float halfHeight = height / 2.0f;
            const float angleIncrement = (float)(2 * Math.PI / numSides);

            List<float> vertexList = new List<float>();
            List<uint> indexList = new List<uint>();
            List<float> colorList = new List<float>();
            List<float> normalList = new List<float>();

            // Rotation matrix for 10 degrees around y-axis
            float cosTheta = (float)Math.Cos(Math.PI / 18);
            float sinTheta = (float)Math.Sin(Math.PI / 18);

            // Generate vertices, colors, and normals for the barrel
            for (int i = 0; i < numSides; i++)
            {
                float angle = i * angleIncrement;
                float nextAngle = (i + 1) * angleIncrement;

                // Current side vertices
                float x1 = radius * (float)Math.Cos(angle);
                float z1 = radius * (float)Math.Sin(angle);
                float x2 = radius * (float)Math.Cos(nextAngle);
                float z2 = radius * (float)Math.Sin(nextAngle);

                // Add vertices for the rectangle (2 triangles)
                vertexList.AddRange(new float[]
                {
                    x1, halfHeight, z1,
                    x2, halfHeight, z2,
                    x2, -halfHeight, z2,
                    x1, -halfHeight, z1
                });

                // Add corresponding colors
                for (int j = 0; j < 4; j++)
                {
                    colorList.AddRange(faceColor);
                }

                // Calculate normals
                float[] normal = CalculateNormal(x1, z1, x2, z2, rotatedNormals, cosTheta, sinTheta);
                for (int j = 0; j < 4; j++)
                {
                    normalList.AddRange(normal);
                }

                // Indices for the two triangles that make up the rectangle
                uint baseIndex = (uint)(i * 4);
                indexList.AddRange(new uint[]
                {
                    baseIndex, baseIndex + 1, baseIndex + 2,
                    baseIndex, baseIndex + 2, baseIndex + 3
                });
            }

            float[] vertexArray = vertexList.ToArray();
            uint[] indexArray = indexList.ToArray();
            float[] colorArray = colorList.ToArray();
            float[] normalArray = normalList.ToArray();

            uint vertices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, vertices);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)vertexArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);
            Gl.EnableVertexAttribArray(0);

            uint colors = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, colors);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)colorArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), (void*)0);
            Gl.EnableVertexAttribArray(1);

            uint normals = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, normals);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)normalArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), (void*)0);
            Gl.EnableVertexAttribArray(2);

            uint indices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, indices);
            Gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)indexArray.AsSpan(), GLEnum.StaticDraw);

            Gl.BindBuffer(GLEnum.ArrayBuffer, 0);
            uint indexArrayLength = (uint)indexArray.Length;

            return new GlBarrel(vao, vertices, colors, indices, normals, indexArrayLength, Gl);
        }

        private static float[] CalculateNormal(float x1, float z1, float x2, float z2, bool rotated, float cosTheta, float sinTheta)
        {
            // Normal for the face
            float nx = z2 - z1;
            float nz = x1 - x2;
            float length = (float)Math.Sqrt(nx * nx + nz * nz);
            nx /= length;
            nz /= length;
            float ny = 0.0f;

            if (rotated)
            {
                float newNx = nx * cosTheta - nz * sinTheta;
                float newNz = nx * sinTheta + nz * cosTheta;
                nx = newNx;
                nz = newNz;
            }

            return new float[] { nx, ny, nz };
        }

        internal void ReleaseGlBarrel()
        {
            Gl.DeleteBuffer(Vertices);
            Gl.DeleteBuffer(Colors);
            Gl.DeleteBuffer(Indices);
            Gl.DeleteBuffer(Normals);
            Gl.DeleteVertexArray(Vao);
        }
    }
}
