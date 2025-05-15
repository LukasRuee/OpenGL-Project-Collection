// Decompiled with JetBrains decompiler
// Type: OpenGL.UI.UserInterface
// Assembly: OpenGL.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC4D2A31-3565-43A0-AA41-FD0F6533BC9E
// Assembly location: D:\Dropbox (w.wittwer ag)\Extern\SAE\GPR5300\Projects\Graphics Programming\SAE - GPR - OpenGL Bindings\opengl\OpenGL.UI.dll

using OpenGL.Platform;
using System.Collections.Generic;
using System.Threading;

namespace OpenGL.UI
{
  public static class UserInterface
  {
    private static uint uniqueID = 0;
    private static Stack<UIContainer> activeContainerStack = new Stack<UIContainer>();
    private static UIElement currentSelection = (UIElement) null;
    private static UIElement activeSelection = (UIElement) null;
    public static UIContainer UIWindow;
    public static Dictionary<string, UIElement> Elements;
    public static Matrix4 UIProjectionMatrix;
    public static UIElement Selection;
    private static Click mousePosition;
    private static Click lmousePosition;

    public static bool Visible { get; set; }

    public static int Width { get; set; }

    public static int Height { get; set; }

    public static int MainThreadID { get; private set; }

    public static Stack<UIContainer> ActiveContainer
    {
      get
      {
        return UserInterface.activeContainerStack;
      }
    }

    public static void InitUI(int width, int height)
    {
      if (!Shaders.Init(Shaders.ShaderVersion.GLSL140))
        return;
      UserInterface.Elements = new Dictionary<string, UIElement>();
      UserInterface.UIWindow = new UIContainer(new Point(0, 0), new Point(UserInterface.Width, UserInterface.Height), "TopLevel");
      UserInterface.UIWindow.RelativeTo = Corner.BottomLeft;
      UserInterface.Elements.Add("Screen", (UIElement) UserInterface.UIWindow);
      UserInterface.UIProjectionMatrix = Matrix4.CreateTranslation(new Vector3((float) (-UserInterface.Width / 2), (float) (-UserInterface.Height / 2), 0.0f)) * Matrix4.CreateOrthographic((float) UserInterface.Width, (float) UserInterface.Height, 0.0f, 1000f);
      UserInterface.Visible = true;
      UserInterface.MainThreadID = Thread.CurrentThread.ManagedThreadId;
      UserInterface.OnResize(width, height);
    }

    public static void AddElement(UIElement Element)
    {
      UserInterface.UIWindow.AddElement(Element);
    }

    public static void RemoveElement(UIElement Element)
    {
      UserInterface.UIWindow.RemoveElement(Element);
    }

    public static void ClearElements()
    {
      UserInterface.UIWindow.ClearElements();
    }

    public static UIElement GetElement(string name)
    {
      UIElement uiElement = (UIElement) null;
      UserInterface.Elements.TryGetValue(name, out uiElement);
      return uiElement;
    }

    public static void Draw()
    {
      if (!UserInterface.Visible || UserInterface.UIWindow == null)
        return;
      bool boolean1 = Gl.GetBoolean(GetPName.DepthTest);
      bool boolean2 = Gl.GetBoolean(GetPName.Blend);
      if (boolean1)
      {
        Gl.Disable(EnableCap.DepthTest);
        Gl.DepthMask(false);
      }
      UserInterface.UIWindow.Draw();
      if (boolean1)
      {
        Gl.DepthMask(true);
        Gl.Enable(EnableCap.DepthTest);
      }
      if (!boolean2)
        return;
      Gl.Enable(EnableCap.Blend);
    }

    public static uint GetUniqueElementID()
    {
      return UserInterface.uniqueID++;
    }

    private static void Update(float delta)
    {
      UserInterface.UIWindow.Update();
    }

    public static void OnResize(int width, int height)
    {
      UserInterface.Width = width;
      UserInterface.Height = height;
      UserInterface.UIProjectionMatrix = Matrix4.CreateTranslation(new Vector3((float) (-UserInterface.Width / 2), (float) (-UserInterface.Height / 2), 0.0f)) * Matrix4.CreateOrthographic((float) UserInterface.Width, (float) UserInterface.Height, 0.0f, 1000f);
      Shaders.UpdateUIProjectionMatrix(UserInterface.UIProjectionMatrix);
      if (UserInterface.UIWindow == null)
        return;
      UserInterface.UIWindow.Size = new Point(UserInterface.Width, UserInterface.Height);
      UserInterface.UIWindow.OnResize();
    }

    public static bool Pick(Point Location)
    {
      return UserInterface.UIWindow != null && (UserInterface.Selection = UserInterface.UIWindow.PickChildren(new Point(Location.X, UserInterface.Height - Location.Y))) != null && UserInterface.Selection != UserInterface.UIWindow;
    }

    public static void Dispose()
    {
      UserInterface.UIWindow?.Dispose();
      UserInterface.UIWindow = (UIContainer) null;
      UserInterface.Elements?.Clear();
      Shaders.Dispose();
    }

    public static Click MousePosition
    {
      get
      {
        return UserInterface.mousePosition;
      }
      set
      {
        UserInterface.lmousePosition = UserInterface.mousePosition;
        UserInterface.mousePosition = value;
      }
    }

    public static Click LastMousePosition
    {
      get
      {
        return UserInterface.lmousePosition;
      }
      set
      {
        UserInterface.lmousePosition = value;
      }
    }

    public static IMouseInput Focus { get; set; }

    public static bool OnMouseMove(int x, int y)
    {
      UIElement currentSelection = UserInterface.currentSelection;
      UserInterface.MousePosition = new Click(x, y, false, false, false, false);
      Point Location = new Point(UserInterface.MousePosition.X, UserInterface.MousePosition.Y);
      if (UserInterface.currentSelection != null && UserInterface.currentSelection.OnMouseMove != null)
        UserInterface.currentSelection.OnMouseMove((object) null, new MouseEventArgs(UserInterface.MousePosition, UserInterface.LastMousePosition));
      UserInterface.currentSelection = !UserInterface.Pick(Location) ? (UIElement) null : UserInterface.Selection;
      if (UserInterface.currentSelection != currentSelection)
      {
        if (currentSelection != null && currentSelection.OnMouseLeave != null)
          currentSelection.OnMouseLeave((object) null, new MouseEventArgs(UserInterface.MousePosition, UserInterface.LastMousePosition));
        if (UserInterface.currentSelection != null && UserInterface.currentSelection.OnMouseEnter != null)
          UserInterface.currentSelection.OnMouseEnter((object) null, new MouseEventArgs(UserInterface.MousePosition, UserInterface.LastMousePosition));
      }
      return UserInterface.currentSelection != null;
    }

    public static bool OnMouseClick(int button, int state, int x, int y)
    {
      UserInterface.MousePosition = new Click(x, y, (MouseButton) button, (MouseState) state);
      if (UserInterface.MousePosition.State == MouseState.Down)
      {
        if (UserInterface.Focus != null && UserInterface.currentSelection != UserInterface.Focus && UserInterface.Focus.OnLoseFocus != null)
          UserInterface.Focus.OnLoseFocus((object) null, (IMouseInput) UserInterface.currentSelection);
        UserInterface.Focus = (IMouseInput) UserInterface.currentSelection;
      }
      if (UserInterface.activeSelection != null && UserInterface.MousePosition.State == MouseState.Up)
      {
        if (UserInterface.activeSelection.OnMouseUp != null)
          UserInterface.activeSelection.OnMouseUp((object) null, new MouseEventArgs(UserInterface.MousePosition, UserInterface.LastMousePosition));
        UserInterface.activeSelection = (UIElement) null;
      }
      else if (UserInterface.currentSelection != null && !(UserInterface.currentSelection is UIContainer))
      {
        if (UserInterface.MousePosition.State == MouseState.Down)
        {
          if (UserInterface.currentSelection.OnMouseDown != null)
            UserInterface.currentSelection.OnMouseDown((object) null, new MouseEventArgs(UserInterface.MousePosition, UserInterface.LastMousePosition));
          UserInterface.activeSelection = UserInterface.currentSelection;
        }
        else
        {
          if (UserInterface.currentSelection.OnMouseUp != null)
            UserInterface.currentSelection.OnMouseUp((object) null, new MouseEventArgs(UserInterface.MousePosition, UserInterface.LastMousePosition));
          UserInterface.activeSelection = (UIElement) null;
        }
        if (UserInterface.currentSelection.OnMouseClick != null)
          UserInterface.currentSelection.OnMouseClick((object) null, new MouseEventArgs(UserInterface.MousePosition, UserInterface.LastMousePosition));
      }
      return UserInterface.activeSelection != null;
    }
  }
}
