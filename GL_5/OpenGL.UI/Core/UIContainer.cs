// Decompiled with JetBrains decompiler
// Type: OpenGL.UI.UIContainer
// Assembly: OpenGL.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC4D2A31-3565-43A0-AA41-FD0F6533BC9E
// Assembly location: D:\Dropbox (w.wittwer ag)\Extern\SAE\GPR5300\Projects\Graphics Programming\SAE - GPR - OpenGL Bindings\opengl\OpenGL.UI.dll

using System.Collections.Generic;

namespace OpenGL.UI
{
  public class UIContainer : UIElement
  {
    protected List<UIElement> elements;

    public new string Name
    {
      get
      {
        return base.Name;
      }
      set
      {
        base.Name = value;
        foreach (UIElement element in this.elements)
          element.Name = base.Name + (object) element.GetType();
      }
    }

    public List<UIElement> Elements
    {
      get
      {
        return this.elements;
      }
    }

    public UIContainer()
      : this(new Point(0, 0), UserInterface.UIWindow.Size, "Container" + (object) UserInterface.GetUniqueElementID())
    {
      this.RelativeTo = Corner.Fill;
    }

    public UIContainer(Point Size, string Name)
      : this(new Point(0, 0), Size, Name)
    {
    }

    public UIContainer(Point Position, Point Size, string Name)
    {
      this.elements = new List<UIElement>();
      this.Name = Name;
      this.RelativeTo = Corner.TopLeft;
      this.Position = Position;
      this.Size = Size;
    }

    public void AddElement(UIElement Element)
    {
      if (Element.Name == null || Element.Name.Length == 0)
        Element.Name = Element.ToString() + (object) UserInterface.GetUniqueElementID();
      if (UserInterface.Elements.ContainsKey(Element.Name))
        return;
      UserInterface.Elements.Add(Element.Name, Element);
      Element.Parent = this;
      this.elements.Add(Element);
      if (this != UserInterface.UIWindow)
        return;
      Element.OnResize();
    }

    public void RemoveElement(UIElement Element)
    {
      if (Element.Name != null && UserInterface.Elements.ContainsKey(Element.Name))
        UserInterface.Elements.Remove(Element.Name);
      if (!this.elements.Contains(Element))
      {
        for (int index = 0; index < this.elements.Count; ++index)
        {
          if (this.elements[index].GetType() == typeof (UIContainer) || this.elements[index].GetType().BaseType == typeof (UIContainer))
            ((UIContainer) this.elements[index]).RemoveElement(Element);
        }
      }
      else
        this.elements.Remove(Element);
    }

    public UIElement PickChildren(Point Location)
    {
      if (!this.Pick(Location))
        return (UIElement) null;
      for (int index = this.elements.Count - 1; index >= 0; --index)
      {
        if (this.elements[index].Pick(Location))
        {
          if (this.elements[index].GetType() == typeof (UIContainer) || this.elements[index].GetType().BaseType == typeof (UIContainer))
            return ((UIContainer) this.elements[index]).PickChildren(Location);
          return this.elements[index];
        }
      }
      return (UIElement) this;
    }

    public virtual void Close()
    {
      this.Dispose();
    }

    public void DrawContainerOnly()
    {
      base.Draw();
    }

    public void ClearElements()
    {
      foreach (UIElement element in this.elements)
        element.Dispose();
      this.elements.Clear();
    }

    protected override void Dispose(bool disposing)
    {
      while (this.elements.Count > 0)
        this.elements[0].Dispose();
      base.Dispose(disposing);
    }

    public override void Update()
    {
      for (int index = 0; index < this.elements.Count; ++index)
        this.elements[index].Update();
    }

    public override void Invalidate()
    {
      for (int index = 0; index < this.elements.Count; ++index)
        this.elements[index].Invalidate();
    }

    public override void Draw()
    {
      this.DrawContainerOnly();
      for (int index = 0; index < this.elements.Count; ++index)
      {
        if (this.elements[index].Visible)
          this.elements[index].Draw();
      }
    }

    public override void OnResize()
    {
      if (this != UserInterface.UIWindow && this.Parent == null)
        return;
      base.OnResize();
      for (int index = 0; index < this.elements.Count; ++index)
        this.elements[index].OnResize();
    }

    public override void DoInvoke()
    {
      base.DoInvoke();
      for (int index = 0; index < this.elements.Count; ++index)
        this.elements[index].DoInvoke();
    }
  }
}
