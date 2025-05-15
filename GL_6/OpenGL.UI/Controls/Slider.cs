// Decompiled with JetBrains decompiler
// Type: OpenGL.UI.Slider
// Assembly: OpenGL.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC4D2A31-3565-43A0-AA41-FD0F6533BC9E
// Assembly location: D:\Dropbox (w.wittwer ag)\Extern\SAE\GPR5300\Projects\Graphics Programming\SAE - GPR - OpenGL Bindings\opengl\OpenGL.UI.dll

using OpenGL.Platform;
using System;

namespace OpenGL.UI
{
  public class Slider : UIContainer
  {
    private int min = 0;
    private int max = 10;
    private int value = 0;
    private bool sliderMouseDown = false;
    private int sliderDown = -1;
    private Button sliderButton;

    public int Minimum
    {
      get
      {
        return this.min;
      }
      set
      {
        if (value < 0 || value >= this.max)
          throw new ArgumentOutOfRangeException(nameof (Minimum));
        this.min = value;
        if (this.Value >= this.min)
          return;
        this.Value = this.min;
      }
    }

    public int Maximum
    {
      get
      {
        return this.max;
      }
      set
      {
        if (value < 0 || value <= this.min)
          throw new ArgumentOutOfRangeException(nameof (Maximum));
        this.max = value;
        if (this.Value <= this.max)
          return;
        this.Value = this.max;
      }
    }

    public int Value
    {
      get
      {
        return this.value;
      }
      set
      {
        if (value < this.min || value > this.max)
          throw new ArgumentOutOfRangeException(nameof (Value));
        if (value != this.value && this.OnValueChanged != null)
          this.OnValueChanged((object) this, new MouseEventArgs());
        this.value = value;
        this.sliderButton.Position = new Point((value - this.min) * (this.Size.X - this.sliderButton.Size.X) / (this.max - this.min), 0);
        this.sliderButton.OnResize();
      }
    }

    public bool LockToSteps { get; set; }

    public OnMouse OnValueChanged { get; set; }

    public Slider(Texture sliderTexture, int min = 0, int max = 10, int value = 0)
      : base(new Point(0, 0), new Point(200, sliderTexture.Size.Height), nameof (Slider) + (object) UserInterface.GetUniqueElementID())
    {
      this.min = min;
      this.max = max;
      this.value = value;
      this.sliderButton = new Button(sliderTexture);
      this.sliderButton.BackgroundColor = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
      this.sliderButton.OnMouseUp = (OnMouse) ((sender, eventArgs) => this.sliderMouseDown = false);
      this.sliderButton.OnMouseDown = (OnMouse) ((sender, eventArgs) =>
      {
        this.sliderMouseDown = eventArgs.Button == MouseButton.Left;
        this.sliderDown = eventArgs.Location.X;
      });
      this.sliderButton.OnMouseMove = (OnMouse) ((sender, eventArgs) => this.OnMouseMove(sender, eventArgs));
      this.OnMouseMove = (OnMouse) ((sender, eventArgs) =>
      {
        if (!this.sliderMouseDown)
          return;
        if (eventArgs.Location.X < this.CorrectedPosition.X)
        {
          this.sliderButton.Position = new Point(0, 0);
          this.Value = this.Minimum;
        }
        else if (eventArgs.Location.X > this.CorrectedPosition.X + this.Size.X)
        {
          this.sliderButton.Position = new Point(this.Size.X - this.sliderButton.Size.X, 0);
          this.Value = this.Maximum;
        }
        else
        {
          int num1 = eventArgs.Location.X - this.sliderDown;
          int val2 = eventArgs.Location.X - this.CorrectedPosition.X - this.sliderButton.Size.X / 2;
          double num2 = Math.Max(0.0, (double) val2 / (double) (this.Size.X - this.sliderButton.Size.X));
          int x = !this.LockToSteps ? Math.Max(0, Math.Min(this.Size.X - this.sliderButton.Size.X, val2)) : (int) (Math.Round(num2 * (double) (this.Maximum - this.Minimum)) * (double) (this.Size.X - this.sliderButton.Size.X) / (double) (this.Maximum - this.Minimum));
          if (x == this.sliderButton.Position.X)
            return;
          this.sliderButton.Position = new Point(x, 0);
          this.sliderDown = eventArgs.Location.X;
          int num3 = Math.Max(this.Minimum, Math.Min(this.Maximum, (int) Math.Round((double) (this.Maximum - this.Minimum) * num2) + this.Minimum));
          if (this.value != num3)
          {
            this.value = num3;
            if (this.OnValueChanged != null)
              this.OnValueChanged((object) this, new MouseEventArgs());
          }
        }
        this.sliderButton.OnResize();
      });
      this.sliderButton.RelativeTo = Corner.BottomLeft;
      this.sliderButton.Position = new Point(value * (this.Size.X - this.sliderButton.Size.X) / (max - min), 0);
      this.AddElement((UIElement) this.sliderButton);
    }
  }
}
