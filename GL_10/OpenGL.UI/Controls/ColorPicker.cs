// Decompiled with JetBrains decompiler
// Type: OpenGL.UI.ColorGradient
// Assembly: OpenGL.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC4D2A31-3565-43A0-AA41-FD0F6533BC9E
// Assembly location: D:\Dropbox (w.wittwer ag)\Extern\SAE\GPR5300\Projects\Graphics Programming\SAE - GPR - OpenGL Bindings\opengl\OpenGL.UI.dll

using OpenGL.Platform;
using System;

namespace OpenGL.UI
{
  public class ColorGradient : UIElement
  {
    private float selx = 0.0f;
    private float sely = 1f;
    private float h = 1f;
    private bool mouseDown = false;
    private VAO gradientQuad;
    private Vector3 color;

    public float Hue
    {
      get
      {
        return this.h;
      }
      set
      {
        this.h = value;
        this.Program.Use();
        this.Program["hue"].SetValue(new HSLColor(value, 1f, 0.5f).ToVector());
        this.UpdateColor();
      }
    }

    public Vector3 Color
    {
      get
      {
        return this.color;
      }
      set
      {
      }
    }

    public OnMouse OnColorChange { get; set; }

    public ColorGradient()
    {
      if (UserInterface.GetElement(nameof (ColorGradient)) != null)
        throw new Exception("Only one color picker can currently exist at once.  This is a limitation I intend to remove soon.");
      this.Program = Shaders.GradientShader;
      this.Program.Use();
      this.Program["hue"].SetValue(new Vector3(this.h, 0.0f, 0.0f));
      this.Program["sel"].SetValue(new Vector2(this.selx, this.sely));
      this.gradientQuad = Geometry.CreateQuad(this.Program, Vector2.Zero, new Vector2(150f, 150f));
      this.RelativeTo = Corner.TopLeft;
      this.Position = new Point(30, 50);
      this.Size = new Point(150, 150);
      this.Name = nameof (ColorGradient);
      this.OnMouseDown = (OnMouse) ((sender, eventArgs) =>
      {
        this.mouseDown = eventArgs.Button == MouseButton.Left;
        this.UpdateMousePosition(eventArgs.Location.X, eventArgs.Location.Y);
      });
      this.OnMouseUp = (OnMouse) ((sender, eventArgs) => this.mouseDown = eventArgs.Button != MouseButton.Left && this.mouseDown);
      this.OnMouseLeave = (OnMouse) ((sender, eventArgs) => this.mouseDown = false);
      this.OnMouseMove = (OnMouse) ((sender, eventArgs) => this.UpdateMousePosition(eventArgs.Location.X, eventArgs.Location.Y));
      this.UpdateColor();
    }

    private void UpdateMousePosition(int x, int y)
    {
      if (!this.mouseDown)
        return;
      this.selx = Math.Min(1f, (float) (x - this.CorrectedPosition.X) / (float) this.Size.X);
      this.sely = Math.Min(1f, (float) (UserInterface.Height - y - this.CorrectedPosition.Y) / (float) this.Size.Y);
      this.Program.Use();
      this.Program["sel"].SetValue(new Vector2(this.selx, this.sely));
      this.UpdateColor();
    }

    private void UpdateColor()
    {
      this.color = (new HSLColor(this.h, 1f, 0.5f).ToVector() * this.selx + Vector3.One * (1f - this.selx)) * this.sely;
      if (this.OnColorChange == null)
        return;
      this.OnColorChange((object) this, new MouseEventArgs());
    }

    public override void Draw()
    {
      this.Program.Use();
      this.gradientQuad.Draw();
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
      this.gradientQuad.DisposeChildren = true;
      this.gradientQuad.Dispose();
    }
  }
}
