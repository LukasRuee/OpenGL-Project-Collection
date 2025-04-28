// Decompiled with JetBrains decompiler
// Type: OpenGL.UI.Text
// Assembly: OpenGL.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC4D2A31-3565-43A0-AA41-FD0F6533BC9E
// Assembly location: D:\Dropbox (w.wittwer ag)\Extern\SAE\GPR5300\Projects\Graphics Programming\SAE - GPR - OpenGL Bindings\opengl\OpenGL.UI.dll

using System;

namespace OpenGL.UI
{
  public class Text : UIElement
  {
    private string text;
    private BMFont bitmapFont;
    private Vector3 color;
    private BMFont.Justification justification;
    private OpenGL.VAO<Vector3, Vector2> VAO;

    public static BMFont FontFromSize(Text.FontSize font)
    {
      switch (font)
      {
        case Text.FontSize._12pt:
          return BMFont.LoadFont("fonts/font12.fnt");
        case Text.FontSize._14pt:
          return BMFont.LoadFont("fonts/font14.fnt");
        case Text.FontSize._16pt:
          return BMFont.LoadFont("fonts/font16.fnt");
        case Text.FontSize._24pt:
          return BMFont.LoadFont("fonts/font24.fnt");
        case Text.FontSize._32pt:
          return BMFont.LoadFont("fonts/font32.fnt");
        case Text.FontSize._48pt:
          return BMFont.LoadFont("fonts/font48.fnt");
        default:
          return BMFont.LoadFont("fonts/font12.fnt");
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
        this.color = value;
      }
    }

    public BMFont.Justification Justification
    {
      get
      {
        return this.justification;
      }
      set
      {
        if (this.justification == value)
          return;
        this.justification = value;
        this.bitmapFont.CreateString(this.VAO, this.text, this.color, this.justification, 1f);
      }
    }

    public Point Padding { get; set; }

    public Point TextSize { get; private set; }

    public string String
    {
      get
      {
        return this.text;
      }
      set
      {
        if (this.text == value || value == null)
          return;
        if (this.text != null && this.text.Length == value.Length)
        {
          this.bitmapFont.CreateString(this.VAO, value, this.Color, this.Justification, 1f);
        }
        else
        {
          if (this.VAO != null)
            this.VAO.Dispose();
          this.VAO = this.bitmapFont.CreateString(this.Program, value, this.Color, this.Justification, 1f);
          this.VAO.DisposeChildren = true;
        }
        this.text = value;
        this.TextSize = new Point(this.bitmapFont.GetWidth(this.text), this.bitmapFont.Height);
      }
    }

    public Text(Material program, BMFont font, string text, BMFont.Justification justification = BMFont.Justification.Left)
      : this(program, font, text, new Vector3(1f, 1f, 1f), justification)
    {
    }

    public Text(Text.FontSize font, string text, BMFont.Justification justification = BMFont.Justification.Left)
      : this(Shaders.FontShader, Text.FontFromSize(font), text, Vector3.One, justification)
    {
    }

    public Text(Text.FontSize font, string text, Vector3 color, BMFont.Justification justification = BMFont.Justification.Left)
      : this(Shaders.FontShader, Text.FontFromSize(font), text, color, justification)
    {
    }

    public Text(Material program, BMFont font, string text, Vector3 color, BMFont.Justification justification = BMFont.Justification.Left)
    {
      this.bitmapFont = font;
      this.Program = program;
      this.Justification = justification;
      this.Color = color;
      this.String = text;
      this.Position = new Point(0, 0);
    }

    public void UpdateFontSize(Text.FontSize font)
    {
      this.bitmapFont = Text.FontFromSize(font);
      if (string.IsNullOrEmpty(this.String))
        return;
      if (this.VAO != null)
      {
        this.bitmapFont.CreateString(this.VAO, this.String, this.Color, this.Justification, 1f);
      }
      else
      {
        this.VAO = this.bitmapFont.CreateString(this.Program, this.String, this.Color, this.Justification, 1f);
        this.VAO.DisposeChildren = true;
      }
      this.TextSize = new Point(this.bitmapFont.GetWidth(this.text), this.bitmapFont.Height);
    }

    public void DrawWithCharacterCount(int count)
    {
      int count1 = Math.Min(count * 6, this.VAO.VertexCount);
      Gl.ActiveTexture(TextureUnit.Texture0);
      Gl.BindTexture(this.bitmapFont.FontTexture);
      Gl.Enable(EnableCap.Blend);
      this.Program.Use();
      this.Program["position"].SetValue(new Vector2((float) (this.CorrectedPosition.X + this.Padding.X), (float) (this.CorrectedPosition.Y + this.Padding.Y)));
      this.Program["color"].SetValue(this.color);
      this.VAO.BindAttributes(this.Program);
      Gl.DrawElements(BeginMode.Triangles, count1, DrawElementsType.UnsignedInt, IntPtr.Zero);
      Gl.Disable(EnableCap.Blend);
    }

    public override void Draw()
    {
      base.Draw();
      Gl.ActiveTexture(TextureUnit.Texture0);
      Gl.BindTexture(this.bitmapFont.FontTexture);
      int num = 0;
      if (this.Size.Y > this.TextSize.Y)
        num = (this.Size.Y - this.TextSize.Y) / 2;
      Gl.Enable(EnableCap.Blend);
      this.Program.Use();
      if (this.Justification == BMFont.Justification.Center)
        this.Program["position"].SetValue(new Vector2((float) (this.CorrectedPosition.X + this.Padding.X + this.Size.X / 2), (float) (this.CorrectedPosition.Y + this.Padding.Y + num)));
      else
        this.Program["position"].SetValue(new Vector2((float) (this.CorrectedPosition.X + this.Padding.X), (float) (this.CorrectedPosition.Y + this.Padding.Y + num)));
      this.Program["color"].SetValue(this.color);
      this.VAO.Draw();
      Gl.Disable(EnableCap.Blend);
    }

    protected override void Dispose(bool disposing)
    {
      if (this.VAO != null)
      {
        this.VAO.Dispose();
        this.VAO = (OpenGL.VAO<Vector3, Vector2>) null;
      }
      base.Dispose(true);
    }

    public enum FontSize
    {
      _12pt,
      _14pt,
      _16pt,
      _24pt,
      _32pt,
      _48pt,
    }
  }
}
