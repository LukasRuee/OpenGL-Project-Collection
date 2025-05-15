// Decompiled with JetBrains decompiler
// Type: OpenGL.UI.TextInput
// Assembly: OpenGL.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC4D2A31-3565-43A0-AA41-FD0F6533BC9E
// Assembly location: D:\Dropbox (w.wittwer ag)\Extern\SAE\GPR5300\Projects\Graphics Programming\SAE - GPR - OpenGL Bindings\opengl\OpenGL.UI.dll

using OpenGL.Platform;
using System;

namespace OpenGL.UI
{
  public class TextInput : UIContainer
  {
    private bool hasFocus = false;
    private Text text;

    public TextInput.OnTextEvent OnCarriageReturn { get; set; }

    public TextInput.OnTextEvent OnTextEntry { get; set; }

    public string String
    {
      get
      {
        return this.text.String;
      }
    }

    public TextInput(BMFont font, string s = "")
      : base(new Point(0, 0), new Point(200, font.Height), "TextEntry" + (object) UserInterface.GetUniqueElementID())
    {
      this.text = new Text(Shaders.FontShader, font, s, BMFont.Justification.Left);
      this.text.RelativeTo = Corner.Fill;
      this.text.Padding = new Point(5, 0);
      this.OnMouseClick = (OnMouse) ((o, e) => this.text.OnMouseClick(o, e));
      this.text.OnMouseClick = (OnMouse) ((o, e) =>
      {
        if (this.hasFocus)
          return;
        this.hasFocus = true;
        Input.PushKeyBindings();
        Input.SubscribeAll(new Event((Event.KeyEvent) ((key, state) =>
        {
          if (!state || this.text.TextSize.X > this.Size.X - 16)
            return;
          this.text.String += key.ToString();
          if (this.OnTextEntry == null)
            return;
          this.OnTextEntry(this, this.String);
        })));
        Input.Subscribe('\b', new Event((Action) (() =>
        {
          if (this.text.String.Length == 0)
            return;
          this.text.String = this.text.String.Substring(0, this.text.String.Length - 1);
          if (this.OnTextEntry == null)
            return;
          this.OnTextEntry(this, this.String);
        })));
        Input.Subscribe('\r', new Event((Action) (() =>
        {
          if (this.OnCarriageReturn == null)
            return;
          this.OnCarriageReturn(this, this.String);
        })));
        Input.Subscribe('\x001B', new Event((Action) (() => this.text.OnLoseFocus((object) null, (IMouseInput) null))));
        this.text.OnLoseFocus = (OnFocus) ((sender, newFocus) =>
        {
          if (!this.hasFocus)
            return;
          this.hasFocus = false;
          Input.PopKeyBindings();
        });
      });
      this.AddElement((UIElement) this.text);
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (!this.hasFocus)
        return;
      this.hasFocus = false;
    }

    public void Clear()
    {
      this.text.String = string.Empty;
    }

    public delegate void OnTextEvent(TextInput entry, string text);
  }
}
