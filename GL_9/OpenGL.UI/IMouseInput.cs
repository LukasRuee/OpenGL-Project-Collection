// Decompiled with JetBrains decompiler
// Type: OpenGL.UI.IMouseInput
// Assembly: OpenGL.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC4D2A31-3565-43A0-AA41-FD0F6533BC9E
// Assembly location: D:\Dropbox (w.wittwer ag)\Extern\SAE\GPR5300\Projects\Graphics Programming\SAE - GPR - OpenGL Bindings\opengl\OpenGL.UI.dll

namespace OpenGL.UI
{
  public interface IMouseInput
  {
    OnMouse OnMouseClick { get; set; }

    OnMouse OnMouseEnter { get; set; }

    OnMouse OnMouseLeave { get; set; }

    OnMouse OnMouseDown { get; set; }

    OnMouse OnMouseUp { get; set; }

    OnMouse OnMouseMove { get; set; }

    OnMouse OnMouseRepeat { get; set; }

    OnFocus OnLoseFocus { get; set; }
  }
}
