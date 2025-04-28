// Decompiled with JetBrains decompiler
// Type: OpenGL.UI.BMFont
// Assembly: OpenGL.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC4D2A31-3565-43A0-AA41-FD0F6533BC9E
// Assembly location: D:\Dropbox (w.wittwer ag)\Extern\SAE\GPR5300\Projects\Graphics Programming\SAE - GPR - OpenGL Bindings\opengl\OpenGL.UI.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace OpenGL.UI
{
  public class BMFont
  {
    private static Dictionary<string, BMFont> loadedFonts = new Dictionary<string, BMFont>();
    private static int maxStringLength = 200;
    private static Vector3[] vertices = new Vector3[BMFont.maxStringLength * 4];
    private static Vector2[] uvs = new Vector2[BMFont.maxStringLength * 4];
    private static int[] indices = new int[BMFont.maxStringLength * 6];
    private Dictionary<char, BMFont.Character> characters = new Dictionary<char, BMFont.Character>();
    public Dictionary<char, Dictionary<char, int>> kerning = new Dictionary<char, Dictionary<char, int>>();

    public static BMFont LoadFont(string file)
    {
      if (!BMFont.loadedFonts.ContainsKey(file) || BMFont.loadedFonts[file] == null)
      {
        BMFont bmFont;
        try
        {
          bmFont = new BMFont(file);
        }
        catch (Exception ex)
        {
          throw new FileNotFoundException(string.Format("Could not load the BMFont file: {0}", (object) file), file);
        }
        BMFont.loadedFonts.Add(file, bmFont);
      }
      return BMFont.loadedFonts[file];
    }

    public static void Dispose()
    {
      foreach (KeyValuePair<string, BMFont> loadedFont in BMFont.loadedFonts)
        loadedFont.Value.FontTexture.Dispose();
      BMFont.loadedFonts.Clear();
    }

    public Texture FontTexture { get; private set; }

    public int Height { get; private set; }

    public BMFont(string descriptorPath)
    {
      string directoryName = new FileInfo(descriptorPath).DirectoryName;
      using (StreamReader streamReader = new StreamReader(descriptorPath))
      {
        while (!streamReader.EndOfStream)
        {
          string str1 = streamReader.ReadLine();
          if (str1.StartsWith("page"))
          {
            string[] strArray = str1.Split(new char[1]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int index = 0; index < strArray.Length; ++index)
            {
              if (strArray[index].Contains("="))
              {
                string str2 = strArray[index].Substring(0, strArray[index].IndexOf('='));
                string str3 = strArray[index].Substring(strArray[index].IndexOf('=') + 1);
                if (str2 == "id" && str3 != "0")
                  throw new Exception("Currently we only support loading one texture at a time.");
                if (str2 == "file")
                  this.FontTexture = new Texture(directoryName + "/" + str3.Trim('"'));
              }
            }
          }
          else if (str1.StartsWith("char "))
          {
            string[] strArray = str1.Split(new char[1]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int num1 = 0;
            float _x1 = 0.0f;
            float _y1 = 0.0f;
            float _x2 = 0.0f;
            float _y2 = 0.0f;
            float _w = 0.0f;
            float _h = 0.0f;
            float _xoffset = 0.0f;
            float _yoffset = 0.0f;
            float _xadvance = 0.0f;
            for (int index = 0; index < strArray.Length; ++index)
            {
              if (strArray[index].Contains("="))
              {
                string str2 = strArray[index].Substring(0, strArray[index].IndexOf('='));
                int val2 = int.Parse(strArray[index].Substring(strArray[index].IndexOf('=') + 1));
                Size size;
                if (str2 == "id")
                  num1 = val2;
                else if (str2 == "x")
                {
                  double num2 = (double) val2;
                  size = this.FontTexture.Size;
                  double width = (double) size.Width;
                  _x1 = (float) (num2 / width);
                }
                else if (str2 == "y")
                {
                  double num2 = 1.0;
                  double num3 = (double) val2;
                  size = this.FontTexture.Size;
                  double height = (double) size.Height;
                  double num4 = num3 / height;
                  _y1 = (float) (num2 - num4);
                }
                else if (str2 == "width")
                {
                  _w = (float) val2;
                  double num2 = (double) _x1;
                  double num3 = (double) _w;
                  size = this.FontTexture.Size;
                  double width = (double) size.Width;
                  double num4 = num3 / width;
                  _x2 = (float) (num2 + num4);
                }
                else if (str2 == "height")
                {
                  _h = (float) val2;
                  double num2 = (double) _y1;
                  double num3 = (double) _h;
                  size = this.FontTexture.Size;
                  double height = (double) size.Height;
                  double num4 = num3 / height;
                  _y2 = (float) (num2 - num4);
                  this.Height = Math.Max(this.Height, val2);
                }
                else if (str2 == "xoffset")
                  _xoffset = (float) val2;
                else if (str2 == "yoffset")
                  _yoffset = (float) val2;
                else if (str2 == "xadvance")
                  _xadvance = (float) val2;
              }
            }
            BMFont.Character character = new BMFont.Character((char) num1, _x1, _y1, _x2, _y2, _w, _h, _xoffset, _yoffset, _xadvance);
            if (!this.characters.ContainsKey(character.id))
              this.characters.Add(character.id, character);
          }
          else if (str1.StartsWith(nameof (kerning)))
          {
            string[] strArray = str1.Split(new char[1]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
            char key = ' ';
            char index1 = ' ';
            int num1 = 0;
            for (int index2 = 0; index2 < strArray.Length; ++index2)
            {
              if (strArray[index2].Contains("="))
              {
                string str2 = strArray[index2].Substring(0, strArray[index2].IndexOf('='));
                int num2 = int.Parse(strArray[index2].Substring(strArray[index2].IndexOf('=') + 1));
                if (str2 == "first")
                  key = (char) num2;
                else if (str2 == "second")
                  index1 = (char) num2;
                else if (str2 == "amount")
                  num1 = num2;
              }
            }
            if (!this.kerning.ContainsKey(key))
              this.kerning.Add(key, new Dictionary<char, int>());
            this.kerning[key][index1] = num1;
          }
        }
      }
    }

    public int GetWidth(char c)
    {
      return (int) this.characters[this.characters.ContainsKey(c) ? c : ' '].xadvance + 1;
    }

    public int GetWidth(string text)
    {
      int num = 0;
      for (int index = 0; index < text.Length; ++index)
        num += (int) this.characters[this.characters.ContainsKey(text[index]) ? text[index] : ' '].xadvance + 1;
      return num;
    }

    private void CreateStringInternal(string text, Vector3 color, BMFont.Justification justification, float scale)
    {
      int num1 = 0;
      switch (justification)
      {
        case BMFont.Justification.Center:
          num1 = -this.GetWidth(text) / 2;
          break;
        case BMFont.Justification.Right:
          num1 = -this.GetWidth(text);
          break;
      }
      Vector3 vector3 = Vector3.One * scale;
      for (int index = 0; index < text.Length; ++index)
      {
        BMFont.Character character = this.characters[this.characters.ContainsKey(text[index]) ? text[index] : ' '];
        float y = (float) this.Height - character.yoffset;
        int num2 = num1 + 1;
        BMFont.vertices[index * 4] = vector3 * new Vector3((float) num2, y, 0.0f);
        BMFont.vertices[index * 4 + 1] = vector3 * new Vector3((float) num2, y - character.height, 0.0f);
        BMFont.vertices[index * 4 + 2] = vector3 * new Vector3((float) num2 + character.width, y, 0.0f);
        BMFont.vertices[index * 4 + 3] = vector3 * new Vector3((float) num2 + character.width, y - character.height, 0.0f);
        num1 = num2 + (int) character.xadvance;
        if ((int) text[index] == 95)
          num1 += 3;
        BMFont.uvs[index * 4] = new Vector2(character.x1, character.y1);
        BMFont.uvs[index * 4 + 1] = new Vector2(character.x1, character.y2);
        BMFont.uvs[index * 4 + 2] = new Vector2(character.x2, character.y1);
        BMFont.uvs[index * 4 + 3] = new Vector2(character.x2, character.y2);
        BMFont.indices[index * 6] = index * 4 + 2;
        BMFont.indices[index * 6 + 1] = index * 4;
        BMFont.indices[index * 6 + 2] = index * 4 + 1;
        BMFont.indices[index * 6 + 3] = index * 4 + 3;
        BMFont.indices[index * 6 + 4] = index * 4 + 2;
        BMFont.indices[index * 6 + 5] = index * 4 + 1;
      }
    }

    public void CreateString(VAO<Vector3, Vector2> vao, string text, Vector3 color, BMFont.Justification justification = BMFont.Justification.Left, float scale = 1f)
    {
      if (vao == null || (int) vao.vaoID == 0)
        return;
      if (vao.VertexCount != text.Length * 6)
        throw new InvalidOperationException("Text length did not match the length of the current vertex array object.");
      this.CreateStringInternal(text, color, justification, scale);
      Gl.BufferSubData<Vector3>(vao.vbos[0].vboID, BufferTarget.ArrayBuffer, BMFont.vertices, text.Length * 4);
      Gl.BufferSubData<Vector2>(vao.vbos[1].vboID, BufferTarget.ArrayBuffer, BMFont.uvs, text.Length * 4);
    }

    public VAO<Vector3, Vector2> CreateString(Material program, string text, Vector3 color, BMFont.Justification justification = BMFont.Justification.Left, float scale = 1f)
    {
      if (text.Length > BMFont.maxStringLength)
      {
        BMFont.maxStringLength = (int) Math.Min((double) int.MaxValue, (double) text.Length * 1.5);
        BMFont.vertices = new Vector3[BMFont.maxStringLength * 4];
        BMFont.uvs = new Vector2[BMFont.maxStringLength * 4];
        BMFont.indices = new int[BMFont.maxStringLength * 6];
      }
      this.CreateStringInternal(text, color, justification, scale);
      return new VAO<Vector3, Vector2>(program, new VBO<Vector3>(BMFont.vertices, text.Length * 4, BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw), new VBO<Vector2>(BMFont.uvs, text.Length * 4, BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw), new string[2]{ "in_position", "in_uv" }, new VBO<int>(BMFont.indices, text.Length * 6, BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticDraw));
    }

    private struct Character
    {
      public char id;
      public float x1;
      public float y1;
      public float x2;
      public float y2;
      public float width;
      public float height;
      public float xoffset;
      public float yoffset;
      public float xadvance;

      public Character(char _id, float _x1, float _y1, float _x2, float _y2, float _w, float _h, float _xoffset, float _yoffset, float _xadvance)
      {
        this.id = _id;
        this.x1 = _x1;
        this.y1 = _y1;
        this.x2 = _x2;
        this.y2 = _y2;
        this.width = _w;
        this.height = _h;
        this.xoffset = _xoffset;
        this.yoffset = _yoffset;
        this.xadvance = _xadvance;
      }
    }

    public enum Justification
    {
      Left,
      Center,
      Right,
    }
  }
}
