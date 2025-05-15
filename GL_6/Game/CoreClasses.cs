using System;
using System.Collections.Generic;
using OpenGL.Mathematics;
using OpenGL;
using System.Diagnostics;
using OpenGL.Platform;

namespace OpenGL.Game
{
    public class Game
    {
        private static int width = 800;
        private static int height = 600;

        public List<GameObject> SceneGraph { get; private set; }

        public Game()
        {
            SceneGraph = new List<GameObject>();
        }

        public void Render()
        {
            Debug.WriteLine(SceneGraph.Count);
            foreach (var obj in SceneGraph)
            {
                var material = obj.Renderer.Material;
                var vao = obj.Renderer.Geometry;

                material.Use(); // Binds shader
                vao.BindAttributes(material);     // Binds VAO

                // Apply transforms and set uniforms
                material["model"].SetValue(obj.Transform.GetTRS());
                material["view"].SetValue(GetViewMatrix());
                material["projection"].SetValue(GetProjectionMatrix());

                // Draw call
                vao.Draw();

                obj.Render();
            }
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

        public void Update()
        {
            foreach (var obj in SceneGraph)
            {
                obj.Update();
            }
        }
    }

    public class GameObject
    {
        public string Name { get; set; }
        public Transform Transform { get; set; }
        public MeshRenderer Renderer { get; set; }

        public GameObject(String name, MeshRenderer renderer)
        {
            Name = name;
            Transform = new Transform();
            Renderer = renderer;
        }

        virtual public void Initialize()
        {
            // Init
        }

        virtual public void Update()
        {
            // Update
        }

        public void Render()
        {
            Renderer.Render(Transform);
        }
    }

    public class MeshRenderer
    {
        public Material Material { get; set; }
        public VAO Geometry { get; set; }

        public MeshRenderer(Material material, VAO vao)
        {
            Material = material;
            Geometry = vao;
        }

        public void Render(Transform transform)
        {
            Geometry.Program.Use();
            Geometry.Draw();
        }
    }
}
