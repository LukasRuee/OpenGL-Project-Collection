using System;
using System.Collections.Generic;
using OpenGL.Mathematics;
using OpenGL;

namespace OpenGL.Game
{
    public class Game
    {
        public List<GameObject> SceneGraph { get; private set; }

        public Game()
        {
            SceneGraph = new List<GameObject>();
        }

        public void Render()
        {
            foreach (var obj in SceneGraph)
            {
                obj.Render();
            }
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
