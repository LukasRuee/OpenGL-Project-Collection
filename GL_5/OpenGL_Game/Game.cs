using System;
using System.Collections.Generic;
using System.Xml.Linq;
using OpenGL;
using OpenGL.Mathematics; 

namespace OpenGL_Game
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

        public GameObject()
        {
            Name = string.Empty;
            Transform = new Transform();
            Renderer = new MeshRenderer();
        }

        public void Initialize()
        {
            Renderer?.Initialize();
        }

        public void Update()
        {
            // Update
        }

        public void Render()
        {
            Renderer?.Render(Transform);
        }
    }

    public class MeshRenderer
    {
        public Material Material { get; set; }
        public VAO Geometry { get; set; }

        public void Initialize()
        {
            Geometry?.Initialize();
        }

        public void Render(Transform transform)
        {
            Material?.Use();
            Material?.SetMatrix4("model", transform.GetTRS());
            Geometry?.Render();
        }
    }

    public class Material : IDisposable
    {
        private int shaderProgram;

        public Material(int shader)
        {
            shaderProgram = shader;
        }

        public void Use()
        {
            //Gl.UseProgram(shaderProgram);
        }

        public void SetMatrix4(string name, Matrix4 matrix)
        {
            //int location = Gl.GetUniformLocation(shaderProgram, name);
            //Gl.UniformMatrix4(location, false, ref matrix);
        }

        public void Dispose()
        {
            if (shaderProgram != 0)
            {
                //Gl.DeleteProgram(shaderProgram);
            }
        }
    }

    public class VAO : IDisposable
    {
        public int Handle { get; private set; }

        public void Initialize()
        {
            //Handle = Gl.GenVertexArray();
            //Gl.BindVertexArray(Handle);

            // Setup buffers here...

            //Gl.BindVertexArray(0);
        }

        public void Render()
        {
            //Gl.BindVertexArray(Handle);
            //Gl.DrawArrays(PrimitiveType.Triangles, 0, 3); // Example draw call
            //Gl.BindVertexArray(0);
        }

        public void Dispose()
        {
            /*
            if (Handle != 0)
            {
                Gl.DeleteVertexArray(Handle);
            }*/
        }
    }
}
