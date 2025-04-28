using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenGL.Mathematics;
using OpenGL.Platform;
using OpenGL.Game;
using OpenGL;
using System.IO;

namespace SAE.GPR.P4
{
    static class Program
    {
        private static int width = 800;
        private static int height = 600;

        static void Main()
        {

            Game game = new Game();

            #region shape
        Vector3[] vertices = new Vector3[]
            {
                // Bottom face (near)
                new Vector3(-0.5f, -0.5f,  0.5f),
                new Vector3( 0.5f, -0.5f,  0.5f),
                new Vector3( 0.5f,  0.5f,  0.5f),
                new Vector3(-0.5f,  0.5f,  0.5f),

                // Top face (far)
                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3( 0.5f, -0.5f, -0.5f),
                new Vector3( 0.5f,  0.5f, -0.5f),
                new Vector3(-0.5f,  0.5f, -0.5f)
            };
            uint[] indices = new uint[]
            {
                // Near face
                0, 1, 2, 2, 3, 0,

                // Far face
                4, 5, 1, 1, 0, 4,

                // Left face
                7, 6, 5, 5, 4, 7,

                // Right face
                3, 2, 6, 6, 7, 3,

                // Bottom face
                1, 5, 6, 6, 2, 1,

                // Top face
                4, 0, 3, 3, 7, 4
            };
            #endregion

            Time.Initialize();
            Window.CreateWindow("OpenGL P4", 800, 600);

            // add a reshape callback to update the UI
            Window.OnReshapeCallbacks.Add(OnResize);

            // add a close callback to make sure we dispose of everythng properly
            Window.OnCloseCallbacks.Add(OnClose);
            // Enable depth testing to ensure correct z-ordering of our fragments
            Gl.Enable(EnableCap.DepthTest);
            Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            // Load shader files
            Material material = Material.Create("shaders\\vert.vs", "shaders\\frag.fs");
            material["color"].SetValue(new Vector3(1, 0, 1));

            Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            //TODO: Create VAO Data
            var cube = new VAO(material, new VBO<Vector3>(vertices), new VBO<uint>(indices, BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticRead));

            //TODO: Create game object
            GameObject obj = new GameObject("Object", new MeshRenderer(material, cube));

            //TODO: Add to scene
            obj.Transform.Position = new Vector3(0, 0, -10);
            obj.Transform.Rotation = new Vector3(45, 45, 45);
            obj.Transform.Scale = new Vector3(1, 2, 0.5f);
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
            // TODO: Uncomment code below

            float r = Mathf.Sin(Time.TimeSinceStart*10);

            Matrix4 view = GetViewMatrix();
            Matrix4 projection = GetProjectionMatrix();

            //--------------------------
            // Data passing to shader
            //--------------------------
            Material material = obj.Renderer.Material;

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
    }
}
