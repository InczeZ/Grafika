using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;

namespace Szeminarium1
{
    internal static class Program
    {
        private static IWindow graphicWindow;

        private static GL Gl;

        private static uint program;

        //ha elrontom az alabbit, akkor nem rajzol ki semmit (GlEnum errort dob kesobb)
        private static readonly string VertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 vPos;
		layout (location = 1) in vec4 vCol;
		out vec4 outCol;

        void main()
        {
			outCol = vCol;
            gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
        }
        ";
        

        //ha ezt rontom el le se fordul
        private static readonly string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;
		in vec4 outCol;

        void main()
        {
            FragColor = outCol;
        }
        ";

        static void Main(string[] args)
        {
            WindowOptions windowOptions = WindowOptions.Default;
            windowOptions.Title = "1. szeminárium - háromszög";
            windowOptions.Size = new Silk.NET.Maths.Vector2D<int>(500, 500);

            graphicWindow = Window.Create(windowOptions);

            graphicWindow.Load += GraphicWindow_Load;
            
            graphicWindow.Update += GraphicWindow_Update;
            graphicWindow.Render += GraphicWindow_Render;

            graphicWindow.Run();
        }

        private static void GraphicWindow_Load()
        {
            // egszeri beallitasokat
            //Console.WriteLine("Loaded");

            Gl = graphicWindow.CreateOpenGL();

            Gl.ClearColor(System.Drawing.Color.White);

            uint vshader = Gl.CreateShader(ShaderType.VertexShader);
            uint fshader = Gl.CreateShader(ShaderType.FragmentShader);

            Gl.ShaderSource(vshader, VertexShaderSource);
            Gl.CompileShader(vshader);
            Gl.GetShader(vshader, ShaderParameterName.CompileStatus, out int vStatus); //ha ez van elobb glEnum error
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + Gl.GetShaderInfoLog(vshader));

            Gl.ShaderSource(fshader, FragmentShaderSource); //sorrend mindegy
            Gl.CompileShader(fshader);                      //

            program = Gl.CreateProgram();
            Gl.AttachShader(program, vshader); //attach sorok valamelyike nelkul fekete abrat kapok
            Gl.AttachShader(program, fshader);
            Gl.LinkProgram(program);          //link nelkul ures abrat
            Gl.DetachShader(program, vshader);
            Gl.DetachShader(program, fshader);
            Gl.DeleteShader(vshader);
            Gl.DeleteShader(fshader);

            Gl.GetProgram(program, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader {Gl.GetProgramInfoLog(program)}");
            }

        }

        private static void GraphicWindow_Update(double deltaTime)
        {
            // NO GL
            // make it threadsave
            //Console.WriteLine($"Update after {deltaTime} [s]");
        }

        private static unsafe void GraphicWindow_Render(double deltaTime)
        {
            //Console.WriteLine($"Render after {deltaTime} [s]");

            Gl.Clear(ClearBufferMask.ColorBufferBit);

            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            //ha itt kiveszek par elemet, akkor kevesebb pont koordinatam lesz mas abrat kapok
            float[] vertexArray = new float[] {
                -0.5f, -0.5f, 0.0f,
                +0.5f, -0.5f, 0.0f,
                 0.0f, +0.5f, 0.0f,
                 1f, 1f, 0f
            };

            //ha itt modositok megvaltozik a szine
            float[] colorArray = new float[] {
                1.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
            };

            //ha itt modositok, akkor a pontokat cserelem fel
            //ha sorokat cserelek vagy egy bizonyos sorban elemeket, akkor nincs valtozas
            //ha kulon sorokba cserelem osszea pontokat akkor mas abrat kapok
            uint[] indexArray = new uint[] {
                0, 1, 2,
                2, 1, 3
            };

            uint vertices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, vertices); //ha a bind-et felcserelem a dataval akkor ures abrat kapok, vagy ha kitorlom ugyanaz
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)vertexArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(0);
            //barmelyik sort cserelem fel vagy torlom ki ures abrat kapok

            uint colors = Gl.GenBuffer();
            //VertexAttribPointer
            //BindBuffer
            //BufferData
            //ebben a sorrendben teljesen mas szint kapok
            Gl.BindBuffer(GLEnum.ArrayBuffer, colors);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)colorArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(1);

            uint indices = Gl.GenBuffer();//a alenyeg hogy a Gl.BindBuffer(GLEnum.ElementArrayBuffer, indices); megelozze a BufferData()-t, maskepp ures abra
            Gl.BindBuffer(GLEnum.ArrayBuffer, 0);
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, indices);
            Gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)indexArray.AsSpan(), GLEnum.StaticDraw);
            Gl.UseProgram(program);

            Gl.BindVertexArray(vao);
            Gl.DrawElements(GLEnum.Triangles, (uint)indexArray.Length, GLEnum.UnsignedInt, null); // we used element buffer
            //ha a drawelemtset szedem ki akkor ures abrat kapok
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0); //DrawElements utan kell legyen

            // always unbound the vertex buffer first, so no halfway results are displayed by accident
            Gl.DeleteBuffer(vertices);
            Gl.DeleteBuffer(colors);
            Gl.DeleteBuffer(indices);
            Gl.DeleteVertexArray(vao);
        }
    }
}
