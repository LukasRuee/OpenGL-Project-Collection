// Decompiled with JetBrains decompiler
// Type: OpenGL.UI.IUserInterface
// Assembly: OpenGL.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC4D2A31-3565-43A0-AA41-FD0F6533BC9E
// Assembly location: D:\Dropbox (w.wittwer ag)\Extern\SAE\GPR5300\Projects\Graphics Programming\SAE - GPR - OpenGL Bindings\opengl\OpenGL.UI.dll

using System;

namespace OpenGL.UI
{
  public interface IUserInterface : IDisposable
  {
    float Alpha { get; set; }

    Point Position { get; set; }

    Point Size { get; set; }

    Point MinSize { get; set; }

    Point MaxSize { get; set; }

    Corner RelativeTo { get; set; }

    UIContainer Parent { get; set; }

    Material Program { get; }

    string Name { get; set; }

    void Draw();

    void OnResize();

    void Update();

    void Invalidate();

    void Invoke(OnInvoke Method, object arg);
  }
}
