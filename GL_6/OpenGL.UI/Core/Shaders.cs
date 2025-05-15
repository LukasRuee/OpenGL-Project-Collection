// Decompiled with JetBrains decompiler
// Type: OpenGL.UI.Shaders
// Assembly: OpenGL.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC4D2A31-3565-43A0-AA41-FD0F6533BC9E
// Assembly location: D:\Dropbox (w.wittwer ag)\Extern\SAE\GPR5300\Projects\Graphics Programming\SAE - GPR - OpenGL Bindings\opengl\OpenGL.UI.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGL.UI
{
  internal static class Shaders
  {
    private static List<Material> LoadedPrograms = new List<Material>();
    public static Shaders.ShaderVersion Version = Shaders.ShaderVersion.GLSL140;
    private static bool initialized = false;
    private static char[] newlineChar = new char[1]{ '\n' };
    private static char[] unixNewlineChar = new char[1]{ '\r' };
    private static string UITexturedVertexSource = "\n#version 140\n\nuniform vec3 position;\nuniform mat4 ui_projection_matrix;\n\nin vec3 in_position;\nin vec2 in_uv;\n\nout vec2 uv;\n\nvoid main(void)\n{\n  uv = in_uv;\n  \n  gl_Position = ui_projection_matrix * vec4(position + in_position, 1);\n}";
    private static string UITexturedFragmentSource = "\nuniform sampler2D active_texture;\n\nin vec2 uv;\n\nvoid main(void)\n{\n  gl_FragColor = texture2D(active_texture, uv);\n}";
    private static string UISolidVertexSource = "\n#version 140\n\nuniform vec3 position;\nuniform mat4 ui_projection_matrix;\n\nin vec3 in_position;\n\nvoid main(void)\n{\n  gl_Position = ui_projection_matrix * vec4(position + in_position, 1);\n}";
    private static string UISolidFragmentSource = "\n#version 140\n\nuniform vec4 color;\n\nvoid main(void)\n{\n  gl_FragColor = color;\n}";
    private static string FontVertexSource = "\n#version 140\n\nuniform vec2 position;\nuniform mat4 ui_projection_matrix;\n\nin vec3 in_position;\nin vec2 in_uv;\n\nout vec2 uv;\n\nvoid main(void)\n{\n  uv = in_uv;\n  gl_Position = ui_projection_matrix * vec4(in_position.x + position.x, in_position.y + position.y, 0, 1);\n}";
    private static string FontFragmentSource = "\n#version 140\n\nuniform sampler2D active_texture;\nuniform vec3 color;\n\nin vec2 uv;\n\nvoid main(void)\n{\n  vec4 t = texture2D(active_texture, uv);\n  gl_FragColor = vec4(t.rgb * color, t.a);\n}";
    public static string gradientVertexShaderSource = "\nuniform mat4 ui_projection_matrix;\nuniform mat4 model_matrix;\n\nattribute vec3 in_position;\nattribute vec3 in_uv;\n\nvarying vec3 uv;\nvarying vec3 position;\n\nvoid main(void)\n{\n  position = in_position;\n  uv = in_uv;\n  gl_Position = ui_projection_matrix * model_matrix * vec4(in_position, 1);\n}";
    private static string gradientFragmentShaderSource = "\nuniform vec3 hue;\nuniform vec2 sel;\nvarying vec3 uv;\nvarying vec3 position;\n\nvoid main(void)\n{\n  int posx = int(position.x);\n  int posy = int(position.y);\n\n  if (posx == 0 || posx == 149 || posy == 0 || posy == 149) gl_FragColor = vec4(0, 0, 0, 1);\n  else\n  {\n    vec3 gradient = mix(vec3(1, 1, 1), hue, uv.x);\n    float distance = (uv.x - sel.x) * (uv.x - sel.x) + (uv.y - sel.y) * (uv.y - sel.y);\n    bool ring3 = (distance >= 0.0005 && distance < 0.001);\n    bool ring2 = (distance >= 0.00025 && distance < 0.0005);\n    bool ring1 = (distance >= 0.0001 && distance < 0.00025);\n    gl_FragColor = (ring3 || ring1 ? vec4(0, 0, 0, 1) : (ring2 ? vec4(1, 1, 1, 1) : vec4(mix(vec3(0, 0, 0), gradient, uv.y), 1)));\n  }\n}";
    public static string hueVertexShaderSource = "\nuniform mat4 ui_projection_matrix;\nuniform mat4 model_matrix;\n\nattribute vec3 in_position;\nattribute vec3 in_uv;\n\nvarying vec3 uv;\nvarying vec3 position;\n\nvoid main(void)\n{\n  position = in_position;\n  uv = in_uv;\n  gl_Position = ui_projection_matrix * model_matrix * vec4(in_position, 1);\n}";
    private static string hueFragmentShaderSource = "\nvarying vec3 uv;\nvarying vec3 position;\n\nuniform float hue;\n\nfloat HUE2RGB(float p, float q, float t)\n{\n  if (t < 0.0) t += 1.0;\n  if (t > 1.0) t -= 1.0;\n  if (t < 1.0 / 6.0) return p + (q - p) * 6.0 * t;\n  if (t < 1.0 / 2.0) return q;\n  if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6.0;\n  return p;\n}\n\nvec3 HSL2RGB(float h, float s, float l)\n{\n  float r, g, b;\n\n  if (s == 0.0) r = g = b = l;\n  else\n  {\n    float q = (l < 0.5 ? l * (1.0 + s) : l + s - l * s);\n    float p = 2.0 * l - q;\n    r = HUE2RGB(p, q, h + 1.0 / 3.0);\n    g = HUE2RGB(p, q, h);\n    b = HUE2RGB(p, q, h - 1.0 / 3.0);\n  }\n\n  return vec3(r, g, b);\n}\n\nvoid main(void)\n{\n  int posx = int(position.x);\n  int posy = int(position.y);\n\n  if (posx == 10 || posx == 25) gl_FragColor = vec4(0, 0, 0, 1);\n  else if (position.x >= 10.0 && position.x <= 25.0) gl_FragColor = (posy == 0 || posy == 149 ? vec4(0, 0, 0, 1) : vec4(HSL2RGB(uv.y, 1.0, 0.5), 1));\n  else if (position.x < 8.0)\n  {\n    float distance = abs(position.y - hue);\n    if (int(6.0 - position.x) == int(distance)) gl_FragColor = vec4(0, 0, 0, 1);\n    else if (6.0 - position.x > distance) gl_FragColor = (int(position.x) == 0 ? vec4(0, 0, 0, 1) : vec4(HSL2RGB(hue / 150.0, 1.0, 0.5), 1));\n    else gl_FragColor = vec4(0, 0, 0, 0);\n  }\n  else gl_FragColor = vec4(0, 0, 0, 0);\n}";
    public static Material SolidUIShader;
    public static Material TexturedUIShader;
    public static Material FontShader;
    public static Material GradientShader;
    public static Material HueShader;

    public static bool Init(Shaders.ShaderVersion shaderVersion = Shaders.ShaderVersion.GLSL140)
    {
      if (Shaders.initialized)
        return true;
      Shaders.Version = shaderVersion;
      try
      {
        Shaders.SolidUIShader = Shaders.InitShader(Shaders.UISolidVertexSource, Shaders.UISolidFragmentSource);
        Shaders.TexturedUIShader = Shaders.InitShader(Shaders.UITexturedVertexSource, Shaders.UITexturedFragmentSource);
        Shaders.FontShader = Shaders.InitShader(Shaders.FontVertexSource, Shaders.FontFragmentSource);
        Shaders.GradientShader = Shaders.InitShader(Shaders.gradientVertexShaderSource, Shaders.gradientFragmentShaderSource);
        Shaders.HueShader = Shaders.InitShader(Shaders.hueVertexShaderSource, Shaders.hueFragmentShaderSource);
        Shaders.initialized = true;
      }
      catch (Exception ex)
      {
      }
      return Shaders.initialized;
    }

    public static void Dispose()
    {
      if (!Shaders.initialized)
        return;
      if (Shaders.SolidUIShader != null)
        Shaders.SolidUIShader.Dispose();
      if (Shaders.TexturedUIShader != null)
        Shaders.TexturedUIShader.Dispose();
      if (Shaders.FontShader != null)
        Shaders.FontShader.Dispose();
      if (Shaders.GradientShader != null)
        Shaders.GradientShader.Dispose();
      if (Shaders.HueShader == null)
        return;
      Shaders.HueShader.Dispose();
    }

    public static string ConvertShader(string shader, bool vertexShader)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string[] strArray = shader.Split(Shaders.newlineChar);
      for (int index = 0; index < strArray.Length; ++index)
      {
        strArray[index] = strArray[index].Trim(Shaders.unixNewlineChar);
        if (strArray[index].StartsWith("uniform Camera"))
        {
          index += 3;
          stringBuilder.AppendLine("uniform mat4 projection_matrix;");
          stringBuilder.AppendLine("uniform mat4 view_matrix;");
        }
        else if (strArray[index].StartsWith("#version 140"))
          stringBuilder.AppendLine("#version 130");
        else if (strArray[index].StartsWith("in "))
          stringBuilder.AppendLine((vertexShader ? "attribute " : "varying ") + strArray[index].Substring(3));
        else if (strArray[index].StartsWith("out ") & vertexShader)
          stringBuilder.AppendLine("varying " + strArray[index].Substring(4));
        else
          stringBuilder.AppendLine(strArray[index]);
      }
      return stringBuilder.ToString();
    }

    public static Material InitShader(string vertexSource, string fragmentSource)
    {
      if (Shaders.Version == Shaders.ShaderVersion.GLSL120)
      {
        vertexSource = Shaders.ConvertShader(vertexSource, true);
        fragmentSource = Shaders.ConvertShader(fragmentSource, false);
      }
      Material shaderProgram = new Material(vertexSource, fragmentSource);
      Shaders.LoadedPrograms.Add(shaderProgram);
      return shaderProgram;
    }

    public static void UpdateUIProjectionMatrix(Matrix4 projectionMatrix)
    {
      foreach (Material loadedProgram in Shaders.LoadedPrograms)
      {
        if (loadedProgram["ui_projection_matrix"] != null)
        {
          loadedProgram.Use();
          loadedProgram["ui_projection_matrix"].SetValue(projectionMatrix);
        }
      }
    }

    public enum ShaderVersion
    {
      GLSL120,
      GLSL140,
    }
  }
}
