// Decompiled with JetBrains decompiler
// Type: OpenGL.UI.CheckBox
// Assembly: OpenGL.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC4D2A31-3565-43A0-AA41-FD0F6533BC9E
// Assembly location: D:\Dropbox (w.wittwer ag)\Extern\SAE\GPR5300\Projects\Graphics Programming\SAE - GPR - OpenGL Bindings\opengl\OpenGL.UI.dll

using OpenGL.Platform;
using System.Drawing;

namespace OpenGL.UI
{
  public class CheckBox : UIContainer
  {
    private OpenGL.UI.Text text;
    private BMFont font;
    private string textString;
    private Button checkBox;
    private bool isChecked;

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
        this.Text = this.textString;
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
          this.text = new OpenGL.UI.Text(Shaders.FontShader, this.font, this.textString, BMFont.Justification.Left);
          this.text.Size = new OpenGL.Point(0, this.Size.Y);
          this.text.RelativeTo = Corner.BottomLeft;
          this.text.Position = new OpenGL.Point(this.UncheckedTexture.Size.Width + 6, this.Size.Y / 2 - this.text.TextSize.Y / 2);
          this.AddElement((UIElement) this.text);
        }
        else
          this.text.String = this.textString;
      }
    }

    public OnMouse OnCheckedChanged { get; set; }

    public Texture UncheckedTexture { get; set; }

    public Texture CheckedTexture { get; set; }

    public bool Checked
    {
      get
      {
        return this.isChecked;
      }
      set
      {
        this.isChecked = value;
        if (this.OnCheckedChanged != null)
          this.OnCheckedChanged((object) this, new MouseEventArgs());
        if (this.isChecked)
          this.checkBox.BackgroundTexture = this.CheckedTexture;
        else
          this.checkBox.BackgroundTexture = this.UncheckedTexture;
      }
    }

    public CheckBox(Texture uncheckedTexture, Texture checkedTexture, BMFont font, string text) : base(new OpenGL.Point(0, 0), new OpenGL.Point(uncheckedTexture.Size.Width, uncheckedTexture.Size.Height), nameof(CheckBox) + (object)UserInterface.GetUniqueElementID())
    {

      this.UncheckedTexture = uncheckedTexture;
      this.CheckedTexture = checkedTexture;
      this.checkBox = new Button(this.UncheckedTexture);
      this.checkBox.BackgroundColor = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
      this.checkBox.RelativeTo = Corner.BottomLeft;
      Button checkBox = this.checkBox;
      Size size2 = uncheckedTexture.Size;
      int width2 = size2.Width;
      size2 = uncheckedTexture.Size;
      int height2 = size2.Height;
      OpenGL.Point point = new OpenGL.Point(width2, height2);
      checkBox.Size = point;
      this.AddElement((UIElement) this.checkBox);
      this.Font = font;
      this.Text = text;
      this.checkBox.OnMouseClick = (OnMouse) ((o, e) => this.Checked = !this.Checked);
    }
  }
}
