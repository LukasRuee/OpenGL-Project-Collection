// Decompiled with JetBrains decompiler
// Type: OpenGL.UI.HueGradient
// Assembly: OpenGL.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC4D2A31-3565-43A0-AA41-FD0F6533BC9E
// Assembly location: D:\Dropbox (w.wittwer ag)\Extern\SAE\GPR5300\Projects\Graphics Programming\SAE - GPR - OpenGL Bindings\opengl\OpenGL.UI.dll

using OpenGL.Platform;

namespace OpenGL.UI
{
  public class HueGradient : UIElement
  {
    private bool mouseDown = false;
    private VAO hueQuad;

    public HueGradient()
    {
      this.Program = Shaders.HueShader;
      this.Program.Use();
      this.Program["hue"].SetValue(0.0f);
      this.hueQuad = Geometry.CreateQuad(this.Program, Vector2.Zero, new Vector2(26f, 150f));
      this.RelativeTo = Corner.TopLeft;
      this.Position = new Point(185, 50);
      this.Size = new Point(26, 150);
      this.Name = nameof (HueGradient);
      this.OnMouseDown = (OnMouse) ((sender, eventArgs) =>
      {
        this.mouseDown = eventArgs.Button == MouseButton.Left;
        this.UpdateMousePosition(eventArgs.Location.X, eventArgs.Location.Y);
      });
      this.OnMouseUp = (OnMouse) ((sender, eventArgs) => this.mouseDown = eventArgs.Button != MouseButton.Left && this.mouseDown);
      this.OnMouseLeave = (OnMouse) ((sender, eventArgs) => this.mouseDown = false);
      this.OnMouseMove = (OnMouse) ((sender, eventArgs) => this.UpdateMousePosition(eventArgs.Location.X, eventArgs.Location.Y));
    }

    private void UpdateMousePosition(int x, int y)
    {
      if (!this.mouseDown)
        return;
      float num = (float) (UserInterface.Height - y - this.CorrectedPosition.Y) / (float) this.Size.Y;
      this.Program.Use();
      this.Program["hue"].SetValue((float) (UserInterface.Height - y - this.CorrectedPosition.Y));
      UIElement element = UserInterface.GetElement("ColorGradient");
      if (element == null)
        return;
      ((ColorGradient) element).Hue = num;
    }

    public override void Draw()
    {
      Gl.Enable(EnableCap.Blend);
      this.Program.Use();
      this.hueQuad.Draw();
      Gl.Disable(EnableCap.Blend);
    }

    public override void OnResize()
    {
      base.OnResize();
      this.Program.Use();
      this.Program["model_matrix"].SetValue(Matrix4.CreateTranslation(new Vector3((float) this.CorrectedPosition.X, (float) this.CorrectedPosition.Y, 0.0f)));
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      this.hueQuad.DisposeChildren = true;
      this.hueQuad.Dispose();
    }
  }
}
