// Decompiled with JetBrains decompiler
// Type: OpenGL.UI.Button
// Assembly: OpenGL.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC4D2A31-3565-43A0-AA41-FD0F6533BC9E
// Assembly location: D:\Dropbox (w.wittwer ag)\Extern\SAE\GPR5300\Projects\Graphics Programming\SAE - GPR - OpenGL Bindings\opengl\OpenGL.UI.dll

using System.Drawing;

namespace OpenGL.UI
{
  public class Button : UIElement
  {
    private OpenGL.UI.Text text;
    private BMFont font;
    private string textString;

    public bool Enabled { get; set; }

    public Vector4 EnabledColor { get; set; }

    public BMFont Font
    {
      get
      {
        return this.font;
      }
      set
      {
        if (this.text != null)
          this.text.Dispose();
        this.font = value;
        if (this.textString == null || (uint) this.textString.Length <= 0U)
          return;
        this.text = new OpenGL.UI.Text(Shaders.FontShader, this.font, this.textString, BMFont.Justification.Center);
      }
    }

    public string Text
    {
      get
      {
        return this.textString;
      }
      set
      {
        this.textString = value;
        if (this.text == null)
        {
          if (this.font == null)
            return;
          this.text = new OpenGL.UI.Text(Shaders.FontShader, this.font, this.textString, BMFont.Justification.Center);
          this.text.Size = this.Size;
        }
        else
          this.text.String = this.textString;
      }
    }

    public Button(Texture texture)
    {
      this.BackgroundColor = Vector4.Zero;
      this.EnabledColor = Vector4.Zero;
      this.BackgroundTexture = texture;
      this.RelativeTo = Corner.TopLeft;
      Size size = texture.Size;
      int width = size.Width;
      size = texture.Size;
      int height = size.Height;
      this.Size = new OpenGL.Point(width, height);
    }

    public Button(int width, int height)
    {
      this.BackgroundColor = new Vector4(0.3f, 0.3f, 0.3f, 1f);
      this.EnabledColor = new Vector4(0.3f, 0.9f, 0.3f, 1f);
      this.RelativeTo = Corner.TopLeft;
      this.Size = new OpenGL.Point(width, height);
    }

    public override void OnResize()
    {
      if (this.text != null)
        this.text.Size = this.Size;
      base.OnResize();
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (this.text == null)
        return;
      this.text.Dispose();
    }

    public override void Draw()
    {
      this.DrawQuadColored(this.Enabled ? this.EnabledColor : this.BackgroundColor);
      if (this.BackgroundTexture != null)
        this.DrawQuadTextured();
      if (this.text == null)
        return;
      this.text.CorrectedPosition = this.CorrectedPosition;
      this.text.Draw();
    }
  }
}
