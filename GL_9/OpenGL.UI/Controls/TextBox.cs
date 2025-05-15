// Decompiled with JetBrains decompiler
// Type: OpenGL.UI.TextBox
// Assembly: OpenGL.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC4D2A31-3565-43A0-AA41-FD0F6533BC9E
// Assembly location: D:\Dropbox (w.wittwer ag)\Extern\SAE\GPR5300\Projects\Graphics Programming\SAE - GPR - OpenGL Bindings\opengl\OpenGL.UI.dll

using OpenGL.Platform;
using System;
using System.Collections.Generic;
using System.Threading;

namespace OpenGL.UI
{
  public class TextBox : UIElement
  {
    private List<TextBox.TextBoxEntry> text = new List<TextBox.TextBoxEntry>();
    private List<List<TextBox.TextBoxEntry>> lines = new List<List<TextBox.TextBoxEntry>>();
    private List<Text> vaos = new List<Text>();
    private bool dirty = false;
    private int scrollBarDown = -1;
    private float visibilityTime = 0.0f;
    private float currentTime = 0.0f;
    private int totalCharacters = 0;
    private VAO selectedVAO;
    private int currentLine;
    private BMFont font;
    private int selectedLine;
    private static Texture scrollbarTexture;
    private bool allowScrollBar;
    private bool scrollBarMouseDown;
    private Button scrollBar;

    public int MaximumLines { get; private set; }

    public int LineCount
    {
      get
      {
        return this.lines.Count;
      }
    }

    public int CurrentLine
    {
      get
      {
        return this.currentLine;
      }
      set
      {
        this.currentLine = value;
        this.dirty = true;
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
        this.font = value;
        this.MaximumLines = (int) Math.Round((double) this.Size.Y / ((double) this.font.Height * 1.2 + 1.0));
      }
    }

    public string SelectedLineText
    {
      get
      {
        int index = this.selectedLine == -1 ? -1 : this.selectedLine;
        if (index < 0 || index >= this.lines.Count || this.lines[index].Count == 0)
          return "";
        return this.lines[index][0].Text;
      }
    }

    public int SelectedLine
    {
      get
      {
        return this.selectedLine;
      }
      set
      {
        this.selectedLine = value;
        if (this.OnSelectionChanged == null)
          return;
        this.OnSelectionChanged((object) this, new MouseEventArgs());
      }
    }

    public Vector4 SelectedColor { get; set; }

    public OnMouse OnSelectionChanged { get; set; }

    public bool AllowSelection { get; set; }

    public Button ScrollBar
    {
      get
      {
        return this.scrollBar;
      }
    }

    public bool AllowScrollBar
    {
      get
      {
        return this.allowScrollBar;
      }
      set
      {
        this.allowScrollBar = value;
        if (this.Parent == null || this.scrollBar == null)
          return;
        if (this.allowScrollBar && this.LineCount > this.MaximumLines)
          this.Parent.AddElement((UIElement) this.scrollBar);
        else
          this.Parent.RemoveElement((UIElement) this.scrollBar);
      }
    }

    public void UpdateScrollBar()
    {
      if (this.LineCount <= this.MaximumLines || this.Parent == null || this.scrollBar == null)
        return;
      this.scrollBar.RelativeTo = Corner.BottomLeft;
      int y = this.Size.Y - this.scrollBar.Size.Y - (int) Math.Round((double) ((float) this.CurrentLine / (float) (this.LineCount - this.MaximumLines)) * (double) (this.Size.Y - this.scrollBar.Size.Y));
      this.scrollBar.RelativeTo = Corner.BottomLeft;
      this.scrollBar.Position = this.CorrectedPosition - this.Parent.CorrectedPosition + new Point(this.Size.X, y);
      this.scrollBar.OnResize();
    }

    public Point Padding { get; set; }

    public TextBox(BMFont font, Texture scrollTexture, int selectedLine = -1)
    {
      this.Font = font;
      this.SelectedColor = new Vector4(0.3f, 0.9f, 0.3f, 1f);
      this.OnMouseDown = (OnMouse) ((sender, eventArgs) => this.SelectedLine = this.currentLine + (int) ((double) (this.CorrectedPosition.Y + this.Size.Y - (UserInterface.Height - eventArgs.Location.Y)) / ((double) this.Font.Height * 1.2)));
      if (TextBox.scrollbarTexture == null)
        TextBox.scrollbarTexture = scrollTexture;
      this.scrollBar = new Button(TextBox.scrollbarTexture);
      this.scrollBar.BackgroundColor = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
      this.scrollBar.Size = new Point(this.scrollBar.Size.X, this.scrollBar.Size.Y / 2);
      this.scrollBar.OnMouseUp = (OnMouse) ((sender, eventArgs) => this.scrollBarMouseDown = false);
      this.scrollBar.OnMouseDown = (OnMouse) ((sender, eventArgs) =>
      {
        this.scrollBarMouseDown = eventArgs.Button == MouseButton.Left;
        this.scrollBarDown = eventArgs.Location.Y;
      });
      this.scrollBar.OnMouseMove = (OnMouse) ((sender, eventArgs) =>
      {
        if (!this.scrollBarMouseDown)
          return;
        int num = this.scrollBarDown - eventArgs.Location.Y;
        int val1_1 = this.CorrectedPosition.Y - this.Parent.CorrectedPosition.Y;
        int val1_2 = val1_1 + this.Size.Y - this.ScrollBar.Size.Y;
        int y = Math.Min(val1_2, Math.Max(val1_1, this.scrollBar.Position.Y + num));
        if (y == this.scrollBar.Position.Y)
          return;
        this.scrollBarDown = eventArgs.Location.Y;
        this.scrollBar.Position = new Point(this.scrollBar.Position.X, y);
        this.scrollBar.OnResize();
        this.CurrentLine = (int) Math.Round((double) (this.LineCount - this.MaximumLines) * ((double) (val1_2 - y) / ((double) this.Size.Y - (double) this.scrollBar.Size.Y)));
      });
      this.scrollBar.OnLoseFocus = (OnFocus) ((o, e) =>
      {
        if (this.OnLoseFocus == null)
          return;
        this.OnLoseFocus(o, e);
      });
      this.OnMouseMove = (OnMouse) ((sender, eventArgs) => this.scrollBar.OnMouseMove(sender, eventArgs));
    }

    private void ParseText()
    {
      this.MaximumLines = (int) Math.Round((double) this.Size.Y / ((double) this.Font.Height * 1.2 + 1.0));
      this.lines.Clear();
      List<TextBox.TextBoxEntry> textBoxEntryList = new List<TextBox.TextBoxEntry>();
      int position = 0;
      for (int index1 = 0; index1 < this.text.Count; ++index1)
      {
        if (this.text[index1] != null && this.text[index1].Text != null)
        {
          int width = this.text[index1].Font.GetWidth(this.text[index1].Text);
          if (position + width + this.Padding.X * 2 <= this.Size.X)
          {
            textBoxEntryList.Add(this.text[index1]);
            this.text[index1].Position = position;
            position += width;
            if (this.text[index1].NewLine)
            {
              this.lines.Add(textBoxEntryList);
              textBoxEntryList = new List<TextBox.TextBoxEntry>();
              position = 0;
            }
          }
          else
          {
            string text = this.text[index1].Text;
            while (text.Length > 0)
            {
              int num = position + this.Padding.X * 2;
              int index2;
              for (index2 = 0; index2 < text.Length; ++index2)
              {
                num += this.text[index1].Font.GetWidth(text[index2]);
                if (num > this.Size.X)
                {
                  --index2;
                  break;
                }
              }
              if (index2 <= 0)
                return;
              int index3 = index2;
              if (text.Length > index2)
              {
                while (index3 > 0 && ((int) text[index3] != 32 && (int) text[index3] != 9))
                  --index3;
                if (index3 == 0)
                  index3 = index2;
              }
              if (index3 == -1)
              {
                textBoxEntryList.Add(new TextBox.TextBoxEntry(this.text[index1].Color, text, this.text[index1].Font, true, position));
                text = string.Empty;
              }
              else if (index3 != index2 || textBoxEntryList.Count == 0)
              {
                textBoxEntryList.Add(new TextBox.TextBoxEntry(this.text[index1].Color, text.Substring(0, index3), this.text[index1].Font, true, position));
                text = text.Substring(index3).TrimStart(Array.Empty<char>());
              }
              if ((uint) textBoxEntryList.Count > 0U)
              {
                this.lines.Add(textBoxEntryList);
                textBoxEntryList = new List<TextBox.TextBoxEntry>();
                position = 0;
              }
            }
          }
        }
      }
      if ((uint) textBoxEntryList.Count > 0U)
        this.lines.Add(textBoxEntryList);
      this.dirty = true;
    }

    private void BuildVAOs()
    {
      for (int index = 0; index < this.vaos.Count; ++index)
      {
        this.vaos[index].Dispose();
        this.vaos[index] = (Text) null;
      }
      this.vaos.Clear();
      if (this.lines.Count > this.MaximumLines && this.allowScrollBar && this.scrollBar != null)
      {
        if (this.scrollBar.Name == null || !UserInterface.Elements.ContainsKey(this.scrollBar.Name))
          this.Parent.AddElement((UIElement) this.scrollBar);
      }
      else if (this.Parent != null && this.scrollBar != null)
        this.Parent.RemoveElement((UIElement) this.scrollBar);
      for (int currentLine = this.CurrentLine; currentLine <= this.MaximumLines + this.CurrentLine && (currentLine < this.lines.Count && currentLine >= 0); ++currentLine)
      {
        if (this.lines[currentLine].Count != 0)
        {
          TextBox.TextBoxEntry entry = this.lines[currentLine][0];
          string text = "";
          for (int index = 0; index < this.lines[currentLine].Count; ++index)
          {
            if (entry.Color != this.lines[currentLine][index].Color || entry.Font != this.lines[currentLine][index].Font)
            {
              if ((uint) text.Length > 0U)
                this.BuildVAO(entry, text);
              entry = this.lines[currentLine][index];
              text = this.lines[currentLine][index].Text;
            }
            else
              text += this.lines[currentLine][index].Text;
          }
          if ((uint) text.Length > 0U)
            this.BuildVAO(entry, text);
        }
      }
      this.CalculateVisibilityTime();
      this.dirty = false;
    }

    private void CalculateVisibilityTime()
    {
      this.totalCharacters = 0;
      foreach (Text vao in this.vaos)
        this.totalCharacters += vao.String.Length;
      this.visibilityTime = this.TimePerCharacter * (float) this.totalCharacters;
    }

    private void BuildVAO(TextBox.TextBoxEntry entry, string text)
    {
      this.vaos.Add(new Text(Shaders.FontShader, entry.Font, text, entry.Color, BMFont.Justification.Left)
      {
        Padding = new Point(entry.Position, 0)
      });
    }

    public override void OnResize()
    {
      base.OnResize();
      this.UpdateScrollBar();
      if (this.selectedVAO != null)
      {
        this.selectedVAO.DisposeChildren = true;
        this.selectedVAO.Dispose();
      }
      VBO<Vector3> vertex = new VBO<Vector3>(new Vector3[4]{ new Vector3(0.0f, 0.0f, 0.0f), new Vector3((float) this.Size.X, 0.0f, 0.0f), new Vector3((float) this.Size.X, (float) this.Font.Height * 1.2f, 0.0f), new Vector3(0.0f, (float) this.Font.Height * 1.2f, 0.0f) });
      VBO<int> element = new VBO<int>(new int[6]{ 0, 1, 3, 1, 2, 3 }, BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticDraw);
      this.selectedVAO = new VAO(Shaders.SolidUIShader, vertex, element);
      this.ParseText();
    }

    public int VisibleCharacters { get; private set; }

    public float TimePerCharacter { get; set; }

    public OnMouse OnTextVisible { get; set; }

    public bool TextIsVisible
    {
      get
      {
        return this.VisibleCharacters == 0;
      }
      set
      {
        if (value)
          this.currentTime = this.visibilityTime;
        else
          this.currentTime = 0.0f;
      }
    }

    public override void Draw()
    {
      base.Draw();
      if (this.CurrentLine < 0)
        return;
      if (this.dirty)
        this.BuildVAOs();
      if ((double) this.TimePerCharacter > 0.0 && (double) this.currentTime < (double) this.visibilityTime)
      {
        this.VisibleCharacters = (int) Math.Max(1f, this.currentTime / this.visibilityTime * (float) this.totalCharacters);
      }
      else
      {
        if (this.OnTextVisible != null)
          this.OnTextVisible((object) this, new MouseEventArgs(new Point(0, 0)));
        this.VisibleCharacters = 0;
      }
      int num1 = 0;
      int num2 = 0;
      for (int index1 = 0; num2 < this.lines.Count - this.CurrentLine && index1 < this.vaos.Count; ++num2)
      {
        if (this.AllowSelection && this.currentLine + num2 == this.SelectedLine && this.selectedVAO != null)
        {
          Shaders.SolidUIShader.Use();
          Shaders.SolidUIShader["position"].SetValue(new Vector3((float) this.CorrectedPosition.X, (float) this.CorrectedPosition.Y - (1.2f * (float) (num2 + 1) * (float) this.Font.Height - (float) this.Size.Y), 0.0f));
          Shaders.SolidUIShader["color"].SetValue(this.SelectedColor);
          this.selectedVAO.Draw();
        }
        for (int index2 = 0; index2 < this.lines[num2 + this.CurrentLine].Count && index1 < this.vaos.Count; ++index2)
        {
          this.vaos[index1].CorrectedPosition = new Point(this.CorrectedPosition.X + this.Padding.X, (int) ((double) this.CorrectedPosition.Y - (1.2 * (double) (num2 + 1) * (double) this.Font.Height - (double) this.Size.Y)));
          if (this.VisibleCharacters <= 0 || num1 + this.vaos[index1].String.Length <= this.VisibleCharacters)
          {
            this.vaos[index1].Draw();
            num1 += this.vaos[index1].String.Length;
          }
          else
          {
            this.vaos[index1].DrawWithCharacterCount(this.VisibleCharacters - num1);
            num1 = this.VisibleCharacters;
          }
          ++index1;
        }
      }
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (this.scrollBar != null)
      {
        this.scrollBar.Dispose();
        this.scrollBar = (Button) null;
      }
      if (this.selectedVAO != null)
      {
        this.selectedVAO.DisposeChildren = true;
        this.selectedVAO.Dispose();
        this.selectedVAO = (VAO) null;
      }
      if (TextBox.scrollbarTexture != null)
      {
        TextBox.scrollbarTexture.Dispose();
        TextBox.scrollbarTexture = (Texture) null;
      }
      foreach (UIElement vao in this.vaos)
        vao.Dispose();
      this.vaos.Clear();
    }

    public void ScrollToEnd()
    {
      if (Thread.CurrentThread.ManagedThreadId != UserInterface.MainThreadID)
        throw new InvalidOperationException("An attempt was made to modify a UI element off the main thread.");
      this.ParseText();
      if (this.LineCount > this.MaximumLines)
        this.CurrentLine = this.LineCount - this.MaximumLines;
      this.UpdateScrollBar();
    }

    public void Write(string message)
    {
      if (Thread.CurrentThread.ManagedThreadId != UserInterface.MainThreadID)
        throw new InvalidOperationException("An attempt was made to modify a UI element off the main thread.");
      this.Write(Vector3.One, message);
      this.totalCharacters += message.Length;
    }

    public void Write(Vector3 color, string message)
    {
      if (Thread.CurrentThread.ManagedThreadId != UserInterface.MainThreadID)
        throw new InvalidOperationException("An attempt was made to modify a UI element off the main thread.");
      this.text.Add(new TextBox.TextBoxEntry(color, message, this.Font, false, 0));
    }

    public void WriteLine(string message)
    {
      if (Thread.CurrentThread.ManagedThreadId == UserInterface.MainThreadID)
        this.WriteLineSafe((object) message);
      else
        this.Invoke(new OnInvoke(this.WriteLineSafe), (object) message);
    }

    public void WriteLine(Vector3 color, string message)
    {
      if (Thread.CurrentThread.ManagedThreadId != UserInterface.MainThreadID)
        throw new InvalidOperationException("An attempt was made to modify a UI element off the main thread.");
      this.text.Add(new TextBox.TextBoxEntry(color, message, this.Font, true, 0));
      this.ScrollToEnd();
    }

    public void Write(Vector3 color, string message, BMFont customFont)
    {
      if (Thread.CurrentThread.ManagedThreadId != UserInterface.MainThreadID)
        throw new InvalidOperationException("An attempt was made to modify a UI element off the main thread.");
      this.text.Add(new TextBox.TextBoxEntry(color, message, customFont, false, 0));
    }

    public void WriteLine(Vector3 color, string message, BMFont customFont)
    {
      if (Thread.CurrentThread.ManagedThreadId != UserInterface.MainThreadID)
        throw new InvalidOperationException("An attempt was made to modify a UI element off the main thread.");
      this.text.Add(new TextBox.TextBoxEntry(color, message, customFont, true, 0));
      this.ScrollToEnd();
    }

    private void WriteLineSafe(object message)
    {
      this.WriteLine(Vector3.One, (string) message);
    }

    public void Clear()
    {
      this.text.Clear();
      this.dirty = true;
      this.currentTime = 0.0f;
      this.totalCharacters = 0;
      this.visibilityTime = 0.0f;
    }

    private class TextBoxEntry
    {
      public Vector3 Color;
      public string Text;
      public bool NewLine;
      public int Position;
      public BMFont Font;

      public TextBoxEntry(Vector3 color, string text, BMFont font, bool newLine = true, int position = 0)
      {
        this.Color = color;
        this.Text = text;
        this.NewLine = newLine;
        this.Position = position;
        this.Font = font;
      }
    }
  }
}
