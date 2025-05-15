using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenGL.Mathematics;
using OpenGL.Platform;
using OpenGL.Game;
using OpenGL;
using System.IO;
using static OpenGL.GenericVAO;
using System.Diagnostics;

namespace SAE.GPR.P6
{
    public enum Colors
    {
        Red,
        Green,
        Blue,
        Yellow,
        Magenta,
        Cyan,
        White,
        Gray
    }

    static class Program
    {
        private static Vector3[] vertices = new Vector3[]
        {
            // Near face
            new Vector3(-0.5f, -0.5f,  0.5f),
            new Vector3( 0.5f, -0.5f,  0.5f),
            new Vector3( 0.5f,  0.5f,  0.5f),
            new Vector3(-0.5f,  0.5f,  0.5f),

            // Far face
            new Vector3( 0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f,  0.5f, -0.5f),
            new Vector3( 0.5f,  0.5f, -0.5f),

            // Left face
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f,  0.5f),
            new Vector3(-0.5f,  0.5f,  0.5f),
            new Vector3(-0.5f,  0.5f, -0.5f),

            // Right face
            new Vector3( 0.5f, -0.5f,  0.5f),
            new Vector3( 0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f,  0.5f, -0.5f),
            new Vector3( 0.5f,  0.5f,  0.5f),

            // Bottom face
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f, -0.5f, -0.5f),
            new Vector3( 0.5f, -0.5f,  0.5f),
            new Vector3(-0.5f, -0.5f,  0.5f),

            // Top face
            new Vector3(-0.5f,  0.5f,  0.5f),
            new Vector3( 0.5f,  0.5f,  0.5f),
            new Vector3( 0.5f,  0.5f, -0.5f),
            new Vector3(-0.5f,  0.5f, -0.5f)
        };
        private static uint[] indices = new uint[]
        {
            // Near face (front)
            0, 1, 2, 2, 3, 0,  // Triangle 1: (0, 1, 2), Triangle 2: (2, 3, 0)
            // Far face (back)
            4, 5, 6, 6, 7, 4,  // Triangle 1: (4, 5, 6), Triangle 2: (6, 7, 4)
            // Left face
            8, 9, 10, 10, 11, 8, // Triangle 1: (8, 9, 10), Triangle 2: (10, 11, 8)
            // Right face
            12, 13, 14, 14, 15, 12, // Triangle 1: (12, 13, 14), Triangle 2: (14, 15, 12)
            // Bottom face
            16, 17, 18, 18, 19, 16, // Triangle 1: (16, 17, 18), Triangle 2: (18, 19, 16)
            // Top face
            20, 21, 22, 22, 23, 20  // Triangle 1: (20, 21, 22), Triangle 2: (22, 23, 20)
        };
        private static Vector2[] uvs = new Vector2[]
        {
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), // Near
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), // Far
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), // Left
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), // Right
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), // Bottom
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1), // Top
        };

        private static int width = 800;
        private static int height = 600;

        static void Main()
        {
            Game game = new Game();

            Time.Initialize();
            Window.CreateWindow("OpenGL P6", 800, 600);

            Window.OnReshapeCallbacks.Add(OnResize);
            Window.OnCloseCallbacks.Add(OnClose);

            Gl.Enable(EnableCap.DepthTest);
            Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            Material materialA = Material.Create("shaders\\vert.vs", "shaders\\frag.fs");
            materialA["color"].SetValue(new Vector3(1, 1, 1));
            Material materialB = Material.Create("shaders\\vert.vs", "shaders\\frag.fs");
            materialB["color"].SetValue(new Vector3(1, 0, 1));
            Material materialC = Material.Create("shaders\\vert.vs", "shaders\\frag.fs");
            materialC["color"].SetValue(new Vector3(1, 0, 1));

            Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            //Bind testure
            var crateTexture = new Texture("textures/crate.jpg");
            Gl.ActiveTexture(0);
            Gl.BindTexture(crateTexture);

            //Make colors
            Vector3[] colors = new Vector3[vertices.Length]; //GenerateColors(Colors.Red);
            for (int i = 0; i < vertices.Length; i++)
            {
                float intensity = (vertices[i].Y + 0.5f); // because Y ranges from - .5 to +0.5
                colors[i] = new Vector3(intensity, intensity, intensity); // grayscale
            }

            //Create obj A
            List<IGenericVBO> vbosA = new List<IGenericVBO>();
            vbosA.Add(new GenericVBO<Vector3>(new VBO<Vector3>(vertices), "in_position"));
            vbosA.Add(new GenericVBO<Vector3>(new VBO<Vector3>(colors), "in_color"));
            vbosA.Add(new GenericVBO<Vector2>(new VBO<Vector2>(uvs), "in_texcoords"));
            vbosA.Add(new GenericVBO<uint>(new VBO<uint>(indices, BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticRead)));

            var vboA = vbosA.ToArray();
            var vaoA = new VAO(materialA, vboA);

            GameObject objA = new GameObject("ObjectA", new MeshRenderer(materialA, vaoA));
            objA.Initialize();

            objA.Transform.Position = new Vector3(-3, 1, -10);
            objA.Transform.Rotation = Vector3.Zero;
            objA.Transform.Scale = Vector3.One;
            game.SceneGraph.Add(objA);

            //Create obj B
            List<IGenericVBO> vbosB = new List<IGenericVBO>();
            vbosB.Add(new GenericVBO<Vector3>(new VBO<Vector3>(vertices), "in_position"));
            vbosB.Add(new GenericVBO<Vector3>(new VBO<Vector3>(colors), "in_color"));
            vbosB.Add(new GenericVBO<Vector2>(new VBO<Vector2>(uvs), "in_texcoords"));
            vbosB.Add(new GenericVBO<uint>(new VBO<uint>(indices, BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticRead)));

            var vboB = vbosB.ToArray();
            var vaoB = new VAO(materialB, vboB);

            GameObject objB = new GameObject("ObjectB", new MeshRenderer(materialB, vaoB));
            objB.Initialize();

            objB.Transform.Position = new Vector3(0, 0, -10);
            objB.Transform.Rotation = Vector3.Zero;
            objB.Transform.Scale = Vector3.One;
            game.SceneGraph.Add(objB);

            //Create obj C
            List<IGenericVBO> vbosC = new List<IGenericVBO>();
            vbosC.Add(new GenericVBO<Vector3>(new VBO<Vector3>(vertices), "in_position"));
            vbosC.Add(new GenericVBO<Vector3>(new VBO<Vector3>(colors), "in_color"));
            vbosC.Add(new GenericVBO<Vector2>(new VBO<Vector2>(uvs), "in_texcoords"));
            vbosC.Add(new GenericVBO<uint>(new VBO<uint>(indices, BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticRead)));
            
            var vboC = vbosC.ToArray();
            var vaoC = new VAO(materialC, vboC);

            GameObject objC = new GameObject("ObjectC", new MeshRenderer(materialC, vaoC));
            objC.Initialize();

            objC.Transform.Position = new Vector3(3, -1, -10);
            objC.Transform.Rotation = Vector3.Zero;
            objC.Transform.Scale = Vector3.One;
            game.SceneGraph.Add(objC);

            // Hook to the escape press event using the OpenGL.UI class library
            Input.Subscribe((char)Keys.Escape, Window.OnClose);

            // Make sure to set up mouse event handlers for the window
            Window.OnMouseCallbacks.Add(OpenGL.UI.UserInterface.OnMouseClick);
            Window.OnMouseMoveCallbacks.Add(OpenGL.UI.UserInterface.OnMouseMove);

            // Game loop
            while (Window.Open)
            {
                Window.HandleInput();
                OnPreRenderFrame();

                //TODO: Execute SetTransform(...) on game object
                SetTransform(objA);
                SetTransform(objB);
                SetTransform(objC);

                game.Render();
                game.Update();

                OnPostRenderFrame();

                Time.Update();
            }
        }


        #region Transformation

        // TODO: Add GameObject obj parameter
        private static void SetTransform(GameObject obj)
        {
            // Rotate over time
            float rotationSpeed = 50f; // degrees per second

            float deltaTime = Time.DeltaTime; // Time passed since last frame
            obj.Transform.Rotation += new Vector3(rotationSpeed * deltaTime, rotationSpeed * deltaTime, rotationSpeed * deltaTime);

            //Matrix4 view = GetViewMatrix();
            //Matrix4 projection = GetProjectionMatrix();
            //
            ////--------------------------
            //// Data passing to shader
            ////--------------------------
            //Material material = obj.Renderer.Material;
            //
            //material["projection"].SetValue(projection);
            //material["view"].SetValue(view);
            //material["model"].SetValue(obj.Transform.GetTRS());
        }

        private static Matrix4 GetProjectionMatrix()
        {
            float fov = 45;

            float aspectRatio = (float)width / (float)height;
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(Mathf.ToRad(fov), (float)width / height, 0.1f, 1000f);
            //projection = Matrix4.CreateOrthographic0.0f, (float)screenWidth, 0.0f, (float)screenHeight, 0.1f, 100.0f);

            return projection;
        }

        private static Matrix4 GetViewMatrix()
        {
            Matrix4 viewTranslation = Matrix4.Identity;
            Matrix4 viewRotation = Matrix4.Identity;
            Matrix4 viewScale = Matrix4.Identity;

            viewTranslation = Matrix4.CreateTranslation(new Vector3(0.0f, 0.0f, 0.0f));
            viewRotation = Matrix4.CreateRotation(new Vector3(0.0f, 0.0f, 1.0f), 0.0f);
            viewScale = Matrix4.CreateScaling(new Vector3(1.0f, 1.0f, 1.0f));

            //Matrix4 view = viewTranslation * viewRotation * viewScale;// TRS matrix -> scale, rotate then translate -> All applied in WORLD Coordinates
            Matrix4 view = viewRotation * viewTranslation * viewScale;// RTS matrix -> scale, rotate then translate -> All applied in LOCAL Coordinates

            return view;
        }

        #endregion

        #region Callbacks
        private static void OnResize()
        {
            width = Window.Width;
            height = Window.Height;

            OpenGL.UI.UserInterface.OnResize(Window.Width, Window.Height);
        }

        private static void OnClose()
        {
            // make sure to dispose of everything
            OpenGL.UI.UserInterface.Dispose();
            OpenGL.UI.BMFont.Dispose();
        }

        private static void OnPreRenderFrame()
        {
            // set up the OpenGL viewport and clear both the color and depth bits
            Gl.Viewport(0, 0, Window.Width, Window.Height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        private static void OnPostRenderFrame()
        {
            // Draw the user interface after everything else
            OpenGL.UI.UserInterface.Draw();

            // Swap the back buffer to the front so that the screen displays
            Window.SwapBuffers();
        }

        private static void OnMouseClick(int button, int state, int x, int y)
        {
            // take care of mapping the Glut buttons to the UI enums
            if (!OpenGL.UI.UserInterface.OnMouseClick(button + 1, (state == 0 ? 1 : 0), x, y))
            {
                // do other picking code here if necessary
            }
        }

        private static void OnMouseMove(int x, int y)
        {
            if (!OpenGL.UI.UserInterface.OnMouseMove(x, y))
            {
                // do other picking code here if necessary
            }
        }

        #endregion
        public static Vector3[] GenerateColors(Colors selectedColor)
        {
            Vector3 vector3;

            switch (selectedColor)
            {
                case Colors.Red:
                    vector3 = new Vector3(1, 0, 0);
                    break;
                case Colors.Green:
                    vector3 = new Vector3(0, 1, 0);
                    break;
                case Colors.Blue:
                    vector3 = new Vector3(0, 0, 1);
                    break;
                case Colors.Yellow:
                    vector3 = new Vector3(1, 1, 0);
                    break;
                case Colors.Magenta:
                    vector3 = new Vector3(1, 0, 1);
                    break;
                case Colors.Cyan:
                    vector3 = new Vector3(0, 1, 1);
                    break;
                case Colors.White:
                    vector3 = new Vector3(1, 1, 1);
                    break;
                case Colors.Gray:
                    vector3 = new Vector3(0.5f, 0.5f, 0.5f);
                    break;
                default:
                    vector3 = new Vector3(1, 1, 1); // Default to white
                    break;
            }

            Vector3 color = vector3;

            Vector3[] colorArray = new Vector3[24];
            for (int i = 0; i < colorArray.Length; i++)
            {
                colorArray[i] = color;
            }

            return colorArray;
        }
    }
}
