// Decompiled with JetBrains decompiler
// Type: OpenGL.UI.Invokable
// Assembly: OpenGL.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC4D2A31-3565-43A0-AA41-FD0F6533BC9E
// Assembly location: D:\Dropbox (w.wittwer ag)\Extern\SAE\GPR5300\Projects\Graphics Programming\SAE - GPR - OpenGL Bindings\opengl\OpenGL.UI.dll

namespace OpenGL.UI
{
  public struct Invokable
  {
    public OnInvoke Method;
    public object Parameter;

    public Invokable(OnInvoke Method, object arg)
    {
      this.Method = Method;
      this.Parameter = arg;
    }
  }
}
