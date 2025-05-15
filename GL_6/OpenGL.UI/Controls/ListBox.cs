// Decompiled with JetBrains decompiler
// Type: OpenGL.UI.ListBox
// Assembly: OpenGL.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC4D2A31-3565-43A0-AA41-FD0F6533BC9E
// Assembly location: D:\Dropbox (w.wittwer ag)\Extern\SAE\GPR5300\Projects\Graphics Programming\SAE - GPR - OpenGL Bindings\opengl\OpenGL.UI.dll

using System;

namespace OpenGL.UI
{
  public class ListBox : UIContainer
  {
    private string[] items;
    private Text text;
    private BMFont font;
    private Button dropDownToggle;
    private TextBox dropDownBox;
    private bool dropDownVisible;

    public string[] GetItems()
    {
      return (string[]) this.items.Clone();
    }

    public int SelectedLine
    {
      get
      {
        return this.dropDownBox.SelectedLine;
      }
      set
      {
        this.dropDownBox.CurrentLine = Math.Max(0, Math.Min(this.dropDownBox.LineCount - 4, value));
        this.dropDownBox.SelectedLine = value;
      }
    }

    public string SelectedLineText
    {
      get
      {
        return this.dropDownBox.SelectedLineText;
      }
    }

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
        this.text = new Text(Shaders.FontShader, this.font, this.dropDownBox.SelectedLineText, BMFont.Justification.Left);
        this.text.RelativeTo = Corner.TopLeft;
        this.text.Position = new Point(0, this.text.TextSize.Y);
      }
    }

    public ListBox(Texture dropDownIcon, Texture scrollTexture, BMFont font, string[] items, int selectedLine = 0)
    {
      ListBox listBox = this;
      this.items = items;
      this.dropDownToggle = new Button(dropDownIcon);
      this.dropDownToggle.RelativeTo = Corner.TopRight;
      this.dropDownToggle.Position = new Point(0, (this.Size.Y - this.dropDownToggle.Size.Y) / 2);
      this.AddElement((UIElement) this.dropDownToggle);
      this.dropDownBox = new TextBox(font, scrollTexture, selectedLine);
      foreach (string message in items)
        this.dropDownBox.WriteLine(message);
      this.dropDownBox.CurrentLine = 0;
      this.dropDownBox.AllowSelection = true;
      this.dropDownToggle.OnMouseClick = (OnMouse) ((o, e) =>
      {
        listBox.dropDownVisible = !listBox.dropDownVisible;
        if (listBox.dropDownVisible)
        {
          // ISSUE: explicit non-virtual call
          listBox.Parent.AddElement((UIElement) listBox.dropDownBox);
          listBox.dropDownBox.AllowScrollBar = items.Length > 4;
        }
        else
        {
          // ISSUE: explicit non-virtual call
          listBox.Parent.RemoveElement((UIElement) listBox.dropDownBox);
          listBox.dropDownBox.AllowScrollBar = false;
        }
      });
      this.dropDownBox.OnSelectionChanged = (OnMouse) ((o, e) => listBox.text.String = listBox.dropDownBox.SelectedLineText);
      this.dropDownToggle.OnLoseFocus = new OnFocus(this.OnLoseFocusInternal);
      this.dropDownBox.OnLoseFocus = new OnFocus(this.OnLoseFocusInternal);
      this.OnLoseFocus = new OnFocus(this.OnLoseFocusInternal);
      this.Font = font;
      this.SelectedLine = selectedLine;
      this.AddElement((UIElement) this.text);
    }

    private void OnLoseFocusInternal(object sender, IMouseInput newFocus)
    {
      if (newFocus == this.dropDownToggle || newFocus == this.dropDownBox || newFocus == this.dropDownBox.ScrollBar)
        return;
      if (this.dropDownVisible)
        this.Parent.RemoveElement((UIElement) this.dropDownBox);
      this.dropDownBox.AllowScrollBar = false;
      this.dropDownToggle.Enabled = false;
      this.dropDownVisible = false;
    }

    public override void Invalidate()
    {
      base.Invalidate();
      int num = Math.Min(this.items.Length, 4);
      this.dropDownBox.RelativeTo = this.RelativeTo;
      this.dropDownBox.Size = new Point(this.Size.X - 8, (int) Math.Round((double) (this.font.Height * num) * 1.2));
      this.dropDownBox.Position = new Point(this.Position.X, this.Position.Y + this.Size.Y);
      if (this.dropDownBox.RelativeTo == Corner.Center)
        this.dropDownBox.Position = new Point(this.dropDownBox.Position.X, (-this.Size.Y - this.dropDownBox.Size.Y) / 2);
      this.dropDownToggle.Position = new Point(0, (this.Size.Y - this.dropDownToggle.Size.Y) / 2);
    }
  }
}
