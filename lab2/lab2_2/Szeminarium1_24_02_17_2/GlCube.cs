﻿using Silk.NET.OpenGL;


namespace Szeminarium1_24_02_17_2
{
    internal class GlCube
    {
        public uint Vao { get; }
        public uint Vertices { get; }
        public uint Colors { get; }
        public uint Indices { get; }
        public uint IndexArrayLength { get; }

        private GL Gl;

        private GlCube(uint vao, uint vertices, uint colors, uint indeces, uint indexArrayLength, GL gl)
        {
            this.Vao = vao;
            this.Vertices = vertices;
            this.Colors = colors;
            this.Indices = indeces;
            this.IndexArrayLength = indexArrayLength;
            this.Gl = gl;
        }

        public static unsafe GlCube CreateCubeWithFaceColors(GL Gl, float x, float y, float z, float[][] faceColors)
        {
            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            // counter clockwise is front facing
            float[] vertexArray = new float[] {
                x-0.5f, y+0.5f, z+0.5f,
                x+0.5f, y+0.5f, z+0.5f,
                x+0.5f, y+0.5f, z-0.5f,
                x-0.5f, y+0.5f, z-0.5f,

                x-0.5f, y+0.5f, z+0.5f,
                x-0.5f, y-0.5f, z+0.5f,
                x+0.5f, y-0.5f, z+0.5f,
                x+0.5f, y+0.5f, z+0.5f,

                x-0.5f, y+0.5f, z+0.5f,
                x-0.5f, y+0.5f, z-0.5f,
                x-0.5f, y-0.5f, z-0.5f,
                x-0.5f, y-0.5f, z+0.5f,

                x-0.5f, y-0.5f, z+0.5f,
                x+0.5f, y-0.5f, z+0.5f,
                x+0.5f, y-0.5f, z - 0.5f,
                x-0.5f, y-0.5f, z - 0.5f,

                x+0.5f, y+0.5f, z - 0.5f,
                x-0.5f, y+0.5f, z - 0.5f,
                x-0.5f, y-0.5f, z - 0.5f,
                x+0.5f, y-0.5f, z - 0.5f,
                    
                x+0.5f, y+0.5f, z + 0.5f,
                x+0.5f, y+0.5f, z - 0.5f,
                x+0.5f, y-0.5f, z - 0.5f,
                x+0.5f, y-0.5f, z + 0.5f,

            };

            List<float> colorsList = new List<float>();
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    colorsList.AddRange(faceColors[i]);
                }
            }


            float[] colorArray = colorsList.ToArray();

            uint[] indexArray = new uint[] {
                0, 1, 2,
                0, 2, 3,

                4, 5, 6,
                4, 6, 7,

                8, 9, 10,
                10, 11, 8,

                12, 14, 13,
                12, 15, 14,

                17, 16, 19,
                17, 19, 18,

                20, 22, 21,
                20, 23, 22
            };

            uint vertices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, vertices);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)vertexArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(0);

            uint colors = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, colors);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)colorArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(1);

            uint indices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, indices);
            Gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)indexArray.AsSpan(), GLEnum.StaticDraw);

            // release array buffer
            Gl.BindBuffer(GLEnum.ArrayBuffer, 0);
            uint indexArrayLength = (uint)indexArray.Length;

            return new GlCube(vao, vertices, colors, indices, indexArrayLength, Gl);
        }

        internal void ReleaseGlCube()
        {
            // always unbound the vertex buffer first, so no halfway results are displayed by accident
            Gl.DeleteBuffer(Vertices);
            Gl.DeleteBuffer(Colors);
            Gl.DeleteBuffer(Indices);
            Gl.DeleteVertexArray(Vao);
        }
    }
}
