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

namespace SAE.GPR.P8
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
            new Vector3(-1f, -1f, 0f),
            new Vector3(1f, -1f, 0f),
            new Vector3(1f, 1f, 0f),
            new Vector3(-1f, 1f, 0f),
        };

        private static Vector3[] colors = new Vector3[]
        {
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 1, 0),
        };

        private static Vector2[] uvs = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1),
        };

        private static uint[] indices = new uint[]
        {
            0, 1, 2,
            2, 3, 0
        };


        private static int width = 800;
        private static int height = 600;

        static void Main()
        {
            Game game = new Game();

            Time.Initialize();
            Window.CreateWindow("OpenGL P8", 800, 600);

            Window.OnReshapeCallbacks.Add(OnResize);
            Window.OnCloseCallbacks.Add(OnClose);

            Gl.Enable(EnableCap.DepthTest);
            Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            Material material = Material.Create("shaders\\vert.vs", "shaders\\frag.fs");
            material["color"].SetValue(new Vector3(1, 1, 1));

            Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            //Bind testure
            var crateTexture = new Texture("textures/crate.jpg");
            // Bind texture #0
            Gl.ActiveTexture(0);
            Gl.BindTexture(crateTexture);

            // Scale up (magnify)
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
            // Scale down (minify)
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Linear);

            Vector3[] colors = new Vector3[vertices.Length]; //GenerateColors(Colors.Red);

            for (int i = 0; i < vertices.Length; i++)
            {
                // Normalize Y between 0 (bottom) and 1 (top)
                float intensity = (vertices[i].Y + 0.5f); // because Y ranges from -0.5 to +0.5
                colors[i] = new Vector3(intensity, intensity, intensity); // grayscale
            }

            //Fill shader
            List<IGenericVBO> vbos = new List<IGenericVBO>();
            vbos.Add(new GenericVBO<Vector3>(new VBO<Vector3>(vertices), "in_position"));
            vbos.Add(new GenericVBO<Vector3>(new VBO<Vector3>(colors), "in_color"));
            vbos.Add(new GenericVBO<Vector2>(new VBO<Vector2>(uvs), "in_texcoords"));
            vbos.Add(new GenericVBO<uint>(new VBO<uint>(indices, BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticRead)));

            var vbo = vbos.ToArray();
            var vao = new VAO(material, vbo);

            //TODO: Create game object
            GameObject obj = new GameObject("Object", new MeshRenderer(material, vao));

            //TODO: Add to scene
            obj.Transform.Position = new Vector3(0, 0, -10);
            obj.Transform.Rotation = Vector3.Zero;
            obj.Transform.Scale = Vector3.One;
            game.SceneGraph.Add(obj);

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
                SetTransform(obj);

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
            float rotationSpeed = 0; // degrees per second

            float deltaTime = Time.DeltaTime; // Time passed since last frame
            obj.Transform.Rotation += new Vector3(rotationSpeed * deltaTime, rotationSpeed * deltaTime, rotationSpeed * deltaTime);

            Matrix4 view = GetViewMatrix();
            Matrix4 projection = GetProjectionMatrix();

            //--------------------------
            // Data passing to shader
            //--------------------------
            Material material = obj.Renderer.Material;

            float r = Mathf.Sin(Time.TimeSinceStart);
            //material["brightness"].SetValue(r);
            //material["contrast"].SetValue(r);
            //material["grayscale"].SetValue(Math.Abs(r));

            // Animate HSV parameters
            float hueShift = (Time.TimeSinceStart * 30f) % 360f; // 30° per second, wraps around at 360°
            float saturation = 0.5f + 0.5f * Mathf.Sin(Time.TimeSinceStart * 2f); // oscillates between 0 and 1
            float value = 1.0f; // constant brightness

            material["hue"].SetValue(hueShift);
            material["saturation"].SetValue(saturation);
            material["value"].SetValue(value);


            material["time"].SetValue(Time.TimeSinceStart);

            material["projection"].SetValue(projection);
            material["view"].SetValue(view);
            material["model"].SetValue(obj.Transform.GetTRS());
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
