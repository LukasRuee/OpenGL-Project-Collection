// Decompiled with JetBrains decompiler
// Type: OpenGL.UI.UIElement
// Assembly: OpenGL.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC4D2A31-3565-43A0-AA41-FD0F6533BC9E
// Assembly location: D:\Dropbox (w.wittwer ag)\Extern\SAE\GPR5300\Projects\Graphics Programming\SAE - GPR - OpenGL Bindings\opengl\OpenGL.UI.dll

using System;
using System.Collections;

namespace OpenGL.UI
{
  public abstract class UIElement : IUserInterface, IDisposable, IMouseInput
  {
    private bool visible = true;
    private Queue InvokeQueue = (Queue) null;
    private Point u_size;
    private VAO uiQuad;

    public float Alpha { get; set; }

    public Point Position { get; set; }

    public Point Size
    {
      get
      {
        return this.u_size;
      }
      set
      {
        if (this.MaxSize.X == 0 || this.MaxSize.Y == 0)
          this.MaxSize = new Point(1000000, 1000000);
        this.u_size = Point.Max(this.MinSize, Point.Min(this.MaxSize, value));
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.uiQuad != null)
      {
        this.uiQuad.DisposeChildren = true;
        this.uiQuad.Dispose();
        this.uiQuad = (VAO) null;
      }
      UserInterface.RemoveElement(this);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public Point MinSize { get; set; }

    public Point MaxSize { get; set; }

    public Point CorrectedPosition { get; set; }

    public Corner RelativeTo { get; set; }

    public string Name { get; set; }

    public OnMouse OnMouseClick { get; set; }

    public OnMouse OnMouseEnter { get; set; }

    public OnMouse OnMouseLeave { get; set; }

    public OnMouse OnMouseDown { get; set; }

    public OnMouse OnMouseUp { get; set; }

    public OnMouse OnMouseMove { get; set; }

    public OnMouse OnMouseRepeat { get; set; }

    public OnFocus OnLoseFocus { get; set; }

    public UIContainer Parent { get; set; }

    public bool DisablePicking { get; set; }

    public Material Program { get; protected set; }

    public bool Visible
    {
      get
      {
        return this.visible;
      }
      set
      {
        this.visible = value;
      }
    }

    public virtual void Draw()
    {
      this.DoInvoke();
      if (this.BackgroundTexture != null)
        this.DrawQuadTextured();
      else
        this.DrawQuadColored();
    }

    public virtual void OnResize()
    {
      if (this.Parent == null)
      {
        if (this.RelativeTo == Corner.BottomLeft)
          this.CorrectedPosition = this.Position;
        else if (this.RelativeTo == Corner.TopLeft)
          this.CorrectedPosition = new Point(this.Position.X, this.Position.Y);
        else if (this.RelativeTo == Corner.BottomRight)
          this.CorrectedPosition = new Point(UserInterface.Width - this.Position.X - this.Size.X, this.Position.Y);
        else if (this.RelativeTo == Corner.TopRight)
          this.CorrectedPosition = new Point(UserInterface.Width - this.Position.X - this.Size.X, -this.Position.Y - this.Size.Y);
        else if (this.RelativeTo == Corner.Bottom)
          this.CorrectedPosition = new Point(UserInterface.Width / 2 - this.Size.X / 2 + this.Position.X, this.Position.Y);
        else if (this.RelativeTo == Corner.Top)
          this.CorrectedPosition = new Point(UserInterface.Width / 2 - this.Size.X / 2 + this.Position.X, -this.Position.Y - this.Size.Y);
        else if (this.RelativeTo == Corner.Center)
          this.CorrectedPosition = new Point(UserInterface.Width / 2 - this.Size.X / 2 + this.Position.X, UserInterface.Height / 2 - this.Size.Y / 2 + this.Position.Y);
      }
      else
      {
        if (this.RelativeTo == Corner.BottomLeft)
          this.CorrectedPosition = this.Position;
        else if (this.RelativeTo == Corner.TopLeft)
          this.CorrectedPosition = new Point(this.Position.X, this.Parent.Size.Y - this.Position.Y - this.Size.Y);
        else if (this.RelativeTo == Corner.BottomRight)
          this.CorrectedPosition = new Point(this.Parent.Size.X - this.Position.X - this.Size.X, this.Position.Y);
        else if (this.RelativeTo == Corner.TopRight)
          this.CorrectedPosition = new Point(this.Parent.Size.X - this.Position.X - this.Size.X, this.Parent.Size.Y - this.Position.Y - this.Size.Y);
        else if (this.RelativeTo == Corner.Bottom)
          this.CorrectedPosition = new Point(this.Parent.Size.X / 2 - this.Size.X / 2 + this.Position.X, this.Position.Y);
        else if (this.RelativeTo == Corner.Top)
          this.CorrectedPosition = new Point(this.Parent.Size.X / 2 - this.Size.X / 2 + this.Position.X, this.Parent.Size.Y - this.Position.Y - this.Size.Y);
        else if (this.RelativeTo == Corner.Fill)
        {
          this.CorrectedPosition = new Point(0, 0);
          this.Size = this.Parent.Size;
        }
        else if (this.RelativeTo == Corner.Center)
          this.CorrectedPosition = new Point(this.Parent.Size.X / 2 - this.Size.X / 2 + this.Position.X, this.Parent.Size.Y / 2 - this.Size.Y / 2 + this.Position.Y);
        this.CorrectedPosition += this.Parent.CorrectedPosition;
      }
      if (this.BackgroundColor != Vector4.Zero || this.BackgroundTexture != null)
      {
        if (this.uiQuad != null)
        {
          this.uiQuad.DisposeChildren = true;
          this.uiQuad.Dispose();
        }
        this.uiQuad = Geometry.CreateQuad(Shaders.SolidUIShader, Vector2.Zero, new Vector2((float) this.Size.X, (float) this.Size.Y), Vector2.Zero, new Vector2(1f, 1f));
      }
      this.Invalidate();
    }

    public virtual void Update()
    {
    }

    public virtual bool Pick(Point Location)
    {
      if (this.DisablePicking)
        return false;
      return Location.X >= this.CorrectedPosition.X && Location.X <= this.CorrectedPosition.X + this.Size.X && Location.Y >= this.CorrectedPosition.Y && Location.Y <= this.CorrectedPosition.Y + this.Size.Y;
    }

    public virtual void Invalidate()
    {
    }

    public void Invoke(OnInvoke Method, object arg)
    {
      if (this.InvokeQueue == null)
        this.InvokeQueue = new Queue();
      Queue.Synchronized(this.InvokeQueue).Enqueue((object) new Invokable(Method, arg));
    }

    public virtual void DoInvoke()
    {
      if (this.InvokeQueue == null || this.InvokeQueue.Count == 0)
        return;
      for (int index = 0; index < Queue.Synchronized(this.InvokeQueue).Count; ++index)
      {
        Invokable invokable = (Invokable) Queue.Synchronized(this.InvokeQueue).Dequeue();
        invokable.Method(invokable.Parameter);
      }
    }

    public static bool Intersects(Point Position, Point Size, Point Location)
    {
      return Location.X >= Position.X && Location.X <= Position.X + Size.X && Location.Y >= Position.Y && Location.Y <= Position.Y + Size.Y;
    }

    public Texture BackgroundTexture { get; set; }

    public Vector4 BackgroundColor { get; set; }

    public void DrawQuadTextured()
    {
      if (this.BackgroundTexture == null)
        return;
      if (this.uiQuad == null)
        this.uiQuad = Geometry.CreateQuad(Shaders.SolidUIShader, Vector2.Zero, new Vector2((float) this.Size.X, (float) this.Size.Y), Vector2.Zero, new Vector2(1f, 1f));
      Gl.Enable(EnableCap.Blend);
      Gl.ActiveTexture(TextureUnit.Texture0);
      Gl.BindTexture(this.BackgroundTexture);
      Shaders.TexturedUIShader.Use();
      Shaders.TexturedUIShader["position"].SetValue(new Vector3((float) this.CorrectedPosition.X, (float) this.CorrectedPosition.Y, 0.0f));
      this.uiQuad.DrawProgram(Shaders.TexturedUIShader);
      Gl.Disable(EnableCap.Blend);
    }

    public void DrawQuadColored()
    {
      if (this.BackgroundColor == Vector4.Zero)
        return;
      this.DrawQuadColored(this.BackgroundColor);
    }

    public void DrawQuadColored(Vector4 color)
    {
      if (this.uiQuad == null)
        this.uiQuad = Geometry.CreateQuad(Shaders.SolidUIShader, Vector2.Zero, new Vector2((float) this.Size.X, (float) this.Size.Y), Vector2.Zero, new Vector2(1f, 1f));
      Gl.Enable(EnableCap.Blend);
      Shaders.SolidUIShader.Use();
      Shaders.SolidUIShader["position"].SetValue(new Vector3((float) this.CorrectedPosition.X, (float) this.CorrectedPosition.Y, 0.0f));
      Shaders.SolidUIShader[nameof (color)].SetValue(color);
      this.uiQuad.Draw();
      Gl.Disable(EnableCap.Blend);
    }
  }
}
