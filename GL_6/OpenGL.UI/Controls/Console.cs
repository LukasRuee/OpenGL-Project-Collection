// Decompiled with JetBrains decompiler
// Type: OpenGL.UI.Console
// Assembly: OpenGL.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC4D2A31-3565-43A0-AA41-FD0F6533BC9E
// Assembly location: D:\Dropbox (w.wittwer ag)\Extern\SAE\GPR5300\Projects\Graphics Programming\SAE - GPR - OpenGL Bindings\opengl\OpenGL.UI.dll

using System;
using System.Collections.Generic;

namespace OpenGL.UI
{
  public class Console : UIContainer
  {
    public Dictionary<string, Console.OnCommand> commands = new Dictionary<string, Console.OnCommand>();
    private TextBox textBox;
    private TextInput textEntry;

    public Console(BMFont font)
      : base(new Point(0, 0), new Point(500, 300), nameof (Console) + (object) UserInterface.GetUniqueElementID())
    {
      this.textBox = new TextBox(font, (Texture) null, -1);
      this.textBox.RelativeTo = Corner.TopLeft;
      this.textBox.BackgroundColor = new Vector4(0.0f, 0.0f, 0.0f, 0.5f);
      this.textBox.AllowScrollBar = true;
      this.AddElement((UIElement) this.textBox);
      this.textEntry = new TextInput(font, "");
      this.textEntry.RelativeTo = Corner.BottomLeft;
      this.textEntry.BackgroundColor = new Vector4(0.0f, 0.0f, 0.0f, 0.7f);
      this.AddElement((UIElement) this.textEntry);
      this.textEntry.OnCarriageReturn = new TextInput.OnTextEvent(this.ExecuteCommand);
    }

    public override void OnResize()
    {
      base.OnResize();
      this.textBox.Size = new Point(this.Size.X, this.Size.Y - this.textBox.Font.Height);
      this.textEntry.Size = new Point(this.Size.X, this.textBox.Font.Height);
    }

    private void ExecuteCommand(TextInput entry, string command)
    {
      if (command.Length == 0)
        return;
      entry.Clear();
      this.textBox.Write(new Vector3(1f, 1f, 1f), "> ");
      this.textBox.WriteLine(new Vector3(0.0f, 0.6f, 0.9f), command);
      try
      {
        if (command.Contains(" "))
        {
          int length = command.IndexOf(" ");
          string key = command.Substring(0, length);
          if (this.commands.ContainsKey(key))
            this.commands[key](command.Substring(length + 1));
          else
            this.textBox.WriteLine(new Vector3(1f, 0.0f, 0.0f), "Unknown command");
        }
        else if (this.commands.ContainsKey(command))
          this.commands[command]("");
        else
          this.textBox.WriteLine(new Vector3(1f, 0.0f, 0.0f), "Unknown command");
      }
      catch (Exception ex)
      {
        this.textBox.WriteLine(new Vector3(1f, 0.0f, 0.0f), "Exception while running command.  " + ex.Message);
      }
    }

    public void Write(string message)
    {
      this.Write(Vector3.One, message);
    }

    public void WriteLine(string message)
    {
      this.textBox.WriteLine(message);
    }

    public void Write(Vector3 color, string message)
    {
      this.textBox.Write(color, message);
    }

    public void WriteLine(Vector3 color, string message)
    {
      this.textBox.WriteLine(color, message);
    }

    public void Write(Vector3 color, string message, BMFont customFont)
    {
      this.textBox.Write(color, message, customFont);
    }

    public void WriteLine(Vector3 color, string message, BMFont customFont)
    {
      this.textBox.WriteLine(color, message, customFont);
    }

    public void Clear()
    {
      this.textBox.Clear();
    }

    public delegate void OnCommand(string args);
  }
}
