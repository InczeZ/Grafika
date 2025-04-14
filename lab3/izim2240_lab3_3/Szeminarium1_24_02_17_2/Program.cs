using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using System.Numerics;

namespace Szeminarium1_24_02_17_2
{
    internal static class Program
    {
        private static CameraDescriptor cameraDescriptor = new();

        // private static Character character = new();

        // private static Controller controller;

        private static CubeArrangementModel cubeArrangementModel = new();

        private static IWindow window;

        private static GL Gl;

        private static ImGuiController controller;

        private static uint program;

        private static GlCube[] glCubes;

        private static int cubeCount;

        private static float Shininess = 50;
        private static Vector3 LightColor = new Vector3(1.0f, 1.0f, 1.0f);
        private static Vector3 LightPosition = new Vector3(0f, 2f, 0f);
        private const string ModelMatrixVariableName = "uModel";

        private const string NormalMatrixVariableName = "uNormal";
        private const string ViewMatrixVariableName = "uView";
        private const string ProjectionMatrixVariableName = "uProjection";

        private static readonly string VertexShaderSource = @"#version 330 core
            layout (location = 0) in vec3 vPos;
            layout (location = 1) in vec4 vCol;
            layout (location = 2) in vec3 vNorm;

            uniform mat4 uModel;
            uniform mat3 uNormal;
            uniform mat4 uView;
            uniform mat4 uProjection;

            out vec4 outCol;
            out vec3 outNormal;
            out vec3 outWorldPosition;

            void main()
            {
                outCol = vCol;
                gl_Position = uProjection * uView * uModel * vec4(vPos, 1.0);
                outNormal = uNormal * vNorm;
                outWorldPosition = vec3(uModel * vec4(vPos, 1.0));
            }
        ";

        private const string LightColorVariableName = "lightColor";
        private const string LightPositionVariableName = "lightPos";
        private const string ViewPosVariableName = "viewPos";
        private const string ShininessVariableName = "shininess";


        private static readonly string FragmentShaderSource = @"#version 330 core
        uniform vec3 lightColor;
        uniform vec3 lightPos;
        uniform vec3 viewPos;
        uniform float shininess;

        in vec4 outCol;
        in vec3 outNormal;
        in vec3 outWorldPosition;

        out vec4 FragColor;

        void main()
        {
            // Ambient
            float ambientStrength = 0.2;
            vec3 ambient = ambientStrength * lightColor;

            // Diffuse
            vec3 norm = normalize(outNormal);
            vec3 lightDir = normalize(lightPos - outWorldPosition);
            float diff = max(dot(norm, lightDir), 0.0);
            vec3 diffuse = diff * lightColor;

            // Specular
            float specularStrength = 0.5;
            vec3 viewDir = normalize(viewPos - outWorldPosition);
            vec3 reflectDir = reflect(-lightDir, norm);
            float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
            vec3 specular = specularStrength * spec * lightColor;

            vec3 result = (ambient + diffuse + specular) * outCol.rgb;
            FragColor = vec4(result, outCol.a);
        }
        ";

        static void Main(string[] args)
        {
            WindowOptions windowOptions = WindowOptions.Default;
            windowOptions.Title = "2. labor rubik kocka";
            windowOptions.Size = new Vector2D<int>(500, 500);

            glCubes = new GlCube[27];
            cubeCount = 27;

            // on some systems there is no depth buffer by default, so we need to make sure one is created
            windowOptions.PreferredDepthBufferBits = 24;
            window = Window.Create(windowOptions);

            window.Load += Window_Load;
            window.Update += Window_Update;
            window.Render += Window_Render;
            window.Closing += Window_Closing;

            window.Run();
        }

        private static void Window_Load()
        {
            //Console.WriteLine("Load");

            // set up input handling
            IInputContext inputContext = window.CreateInput();
            foreach (var keyboard in inputContext.Keyboards)
            {
                keyboard.KeyDown += Keyboard_KeyDown;
            }

            Gl = window.CreateOpenGL();

            controller = new ImGuiController(Gl, window, inputContext);

            Gl.ClearColor(System.Drawing.Color.White);

            SetUpObjects();

            LinkProgram();

            Gl.Enable(EnableCap.CullFace);

            Gl.Enable(EnableCap.DepthTest);
            Gl.DepthFunc(DepthFunction.Lequal);
        }

        private static void LinkProgram()
        {
            uint vshader = Gl.CreateShader(ShaderType.VertexShader);
            uint fshader = Gl.CreateShader(ShaderType.FragmentShader);

            Gl.ShaderSource(vshader, VertexShaderSource);
            Gl.CompileShader(vshader);
            Gl.GetShader(vshader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + Gl.GetShaderInfoLog(vshader));

            Gl.ShaderSource(fshader, FragmentShaderSource);
            Gl.CompileShader(fshader);

            program = Gl.CreateProgram();
            Gl.AttachShader(program, vshader);
            Gl.AttachShader(program, fshader);
            Gl.LinkProgram(program);
            Gl.GetProgram(program, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader {Gl.GetProgramInfoLog(program)}");
            }
            Gl.DetachShader(program, vshader);
            Gl.DetachShader(program, fshader);
            Gl.DeleteShader(vshader);
            Gl.DeleteShader(fshader);
        }

        private static void Keyboard_KeyDown(IKeyboard keyboard, Key key, int arg3)
        {
            switch (key)
            {
                case Key.Left:
                    cameraDescriptor.DecreaseZYAngle();
                    break;
                    ;
                case Key.Right:
                    cameraDescriptor.IncreaseZYAngle();
                    break;
                case Key.I:
                    cameraDescriptor.IncreaseDistance();
                    break;
                case Key.O:
                    cameraDescriptor.DecreaseDistance();
                    break;
                case Key.Up:
                    cameraDescriptor.IncreaseZXAngle();
                    break;
                case Key.Down:
                    cameraDescriptor.DecreaseZXAngle();
                    break;
                case Key.Backspace:
                    cubeArrangementModel.RotateLeft();
                    window.Render += Window_Render;
                    break;
                case Key.Space:
                    cubeArrangementModel.RotateRight();
                    window.Render += Window_Render;
                    break;

            }

            // controller.Update(keyboard);
            SetViewMatrix();
        }

        private static void Window_Update(double deltaTime)
        {
            //Console.WriteLine($"Update after {deltaTime} [s].");
            // multithreaded
            // make sure it is threadsafe
            // NO GL calls
            cubeArrangementModel.AdvanceTime(deltaTime);
            controller.Update((float)deltaTime);
        }

        private static unsafe void Window_Render(double deltaTime)
        {
            //Console.WriteLine($"Render after {deltaTime} [s].");

            // GL here
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            Gl.Clear(ClearBufferMask.DepthBufferBit);


            Gl.UseProgram(program);

            SetViewMatrix();
            SetProjectionMatrix();
            SetLightColor();
            SetLightPosition();
            SetViewerPosition();
            DrawCubes();

            ImGui.Begin("Lighting Controls");
            ImGui.ColorEdit3("Light Color", ref LightColor);
            ImGui.InputFloat3("Light Position", ref LightPosition);
            if (ImGui.Button("Rotate Left"))
                cubeArrangementModel.RotateLeft();
            if (ImGui.Button("Rotate Right"))
                cubeArrangementModel.RotateRight();
            ImGui.End();
            controller.Render();

        }

        private static unsafe void DrawCubes()
        {

            for (int i = 0; i < cubeCount; i++)
            {

                var modelMatrixForCenterCube = Matrix4X4.CreateRotationZ(0.0f);
                if (i % 3 == 2)
                {
                    modelMatrixForCenterCube = Matrix4X4.CreateRotationZ((float)cubeArrangementModel.CubeRotation);
                    if (cubeArrangementModel.CubeRotation != 0f)
                    {
                        modelMatrixForCenterCube = Matrix4X4.CreateTranslation(-1.0f, -1.0f, -1.0f) * modelMatrixForCenterCube * Matrix4X4.CreateTranslation(1.0f, 1.0f, 1.0f);
                    }
                }
                SetModelMatrix(modelMatrixForCenterCube);
                Gl.BindVertexArray(glCubes[i].Vao);
                Gl.DrawElements(GLEnum.Triangles, glCubes[i].IndexArrayLength, GLEnum.UnsignedInt, null);
                Gl.BindVertexArray(0);
            }
        }

        private static unsafe void SetModelMatrix(Matrix4X4<float> modelMatrix)
        {
            // Set model matrix
            int modelLoc = Gl.GetUniformLocation(program, ModelMatrixVariableName);
            Gl.UniformMatrix4(modelLoc, 1, false, (float*)&modelMatrix);

            // Compute normal matrix
            Matrix4X4<float> modelInv;
            Matrix4X4.Invert(modelMatrix, out modelInv);
            Matrix3X3<float> normalMatrix = Matrix3X3.Transpose(new Matrix3X3<float>(modelInv));

            int normalLoc = Gl.GetUniformLocation(program, NormalMatrixVariableName);
            Gl.UniformMatrix3(normalLoc, 1, false, (float*)&normalMatrix);
        }

        private static unsafe void SetUpObjects()
        {

            int counter = 0;
            float[][] colors = [[1.0f, 0.4f, 0.0f, 1.0f],  /*ORANGE*/
                                [0.0f, 0.4f, 0.0f, 1.0f],   /*GREEN*/
                                [1.0f, 0.0f, 0.0f, 1.0f],   /*RED*/  
                                [1.0f, 1.0f, 0.0f, 1.0f],   /*YELLOW*/
                                [0.0f, 0.0f, 1.0f, 1.0f],   /*BLUE*/
                                [1.0f, 0.9f, 0.9f, 1.0f], /*WHITE*/
                                [0.2f, 0.2f, 0.2f, 1.0f]];  /*BLACK*/

            float[][] colorsCube = new float[6][];
            for (int i = 0; i < 6; i++)
            {
                colorsCube[i] = new float[4];
            }

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    for (int z = 0; z < 3; z++)
                    {

                        for (int c = 0; c < 6; c++)
                        {
                            colorsCube[c] = colors[6];
                        }

                        //fent elol bal lent hatul jobb
                        if (x == 0)
                        {
                            colorsCube[2] = colors[3];
                        }

                        if (y == 0)
                        {
                            colorsCube[3] = colors[4];
                        }

                        if (z == 0)
                        {
                            colorsCube[4] = colors[2];
                        }

                        if (z == 2)
                        {
                            colorsCube[1] = colors[0];
                        }

                        if (x == 2)
                        {
                            colorsCube[5] = colors[1];
                        }
                        if (y == 2)
                        {
                            colorsCube[0] = colors[5];
                        }

                        glCubes[counter] = GlCube.CreateCubeWithFaceColors(Gl, x + x * 0.1f, y + y * 0.1f, z + z * 0.1f, colorsCube);
                        counter++;
                    }
                }
            }
        }



        private static void Window_Closing()
        {
            controller?.Dispose();

            for (int i = 0; i < cubeCount; i++)
            {
                glCubes[i].ReleaseGlCube();
            }
        }

        private static unsafe void SetProjectionMatrix()
        {
            var projectionMatrix = Matrix4X4.CreatePerspectiveFieldOfView<float>((float)Math.PI / 4f, 1024f / 768f, 0.1f, 100);
            int location = Gl.GetUniformLocation(program, ProjectionMatrixVariableName);

            if (location == -1)
            {
                throw new Exception($"{ViewMatrixVariableName} uniform not found on shader.");
            }

            Gl.UniformMatrix4(location, 1, false, (float*)&projectionMatrix);
            CheckError();
        }

        private static unsafe void SetViewMatrix()
        {
            var viewMatrix = Matrix4X4.CreateLookAt(cameraDescriptor.Position, cameraDescriptor.Target, cameraDescriptor.UpVector);
            int location = Gl.GetUniformLocation(program, ViewMatrixVariableName);

            if (location == -1)
            {
                throw new Exception($"{ViewMatrixVariableName} uniform not found on shader.");
            }

            Gl.UniformMatrix4(location, 1, false, (float*)&viewMatrix);
            CheckError();
        }

        public static void CheckError()
        {
            var error = (ErrorCode)Gl.GetError();
            if (error != ErrorCode.NoError)
                throw new Exception("GL.GetError() returned " + error.ToString());
        }

        private static unsafe void SetLightColor()
        {
            int location = Gl.GetUniformLocation(program, LightColorVariableName);

            if (location == -1)
            {
                throw new Exception($"{LightColorVariableName} uniform not found on shader.");
            }

            Gl.Uniform3(location, LightColor.X, LightColor.Y, LightColor.Z);
            CheckError();
        }

        private static unsafe void SetLightPosition()
        {
            int location = Gl.GetUniformLocation(program, LightPositionVariableName);

            if (location == -1)
            {
                throw new Exception($"{LightPositionVariableName} uniform not found on shader.");
            }

            Gl.Uniform3(location, LightPosition.X, LightPosition.Y, LightPosition.Z);
            CheckError();
        }

        private static unsafe void SetViewerPosition()
        {
            int location = Gl.GetUniformLocation(program, ViewPosVariableName);

            if (location == -1)
            {
                throw new Exception($"{ViewPosVariableName} uniform not found on shader.");
            }

            Gl.Uniform3(location, cameraDescriptor.Position.X, cameraDescriptor.Position.Y, cameraDescriptor.Position.Z);
            CheckError();
        }

    }
}