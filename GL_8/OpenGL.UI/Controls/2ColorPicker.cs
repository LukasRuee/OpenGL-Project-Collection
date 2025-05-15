// Decompiled with JetBrains decompiler
// Type: OpenGL.UI.HSLColor
// Assembly: OpenGL.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: CC4D2A31-3565-43A0-AA41-FD0F6533BC9E
// Assembly location: D:\Dropbox (w.wittwer ag)\Extern\SAE\GPR5300\Projects\Graphics Programming\SAE - GPR - OpenGL Bindings\opengl\OpenGL.UI.dll

using System;
using System.Drawing;

namespace OpenGL.UI
{
  public class HSLColor
  {
    public float H { get; private set; }

    public float S { get; private set; }

    public float L { get; private set; }

    public HSLColor(float h, float s, float l)
    {
      this.H = h;
      this.S = s;
      this.L = l;
    }

    public HSLColor(Vector3 c)
    {
      float x = c.X;
      float y = c.Y;
      float z = c.Z;
      float num1 = Math.Min(Math.Min(x, y), z);
      float num2 = Math.Max(Math.Max(x, y), z);
      float num3 = num2 - num1;
      this.H = 0.0f;
      this.S = 0.0f;
      this.L = (float) (((double) num2 + (double) num1) / 2.0);
      if ((double) num3 != 0.0)
      {
        this.S = (double) this.L >= 0.5 ? num3 / (2f - num2 - num1) : num3 / (num2 + num1);
        if ((double) x == (double) num2)
          this.H = (y - z) / num3;
        else if ((double) y == (double) num2)
          this.H = (float) (2.0 + ((double) z - (double) x) / (double) num3);
        else if ((double) z == (double) num2)
          this.H = (float) (4.0 + ((double) x - (double) y) / (double) num3);
      }
      this.H *= 60f;
      if ((double) this.H < 0.0)
        this.H += 360f;
      this.H /= 360f;
    }

    public HSLColor(Color c)
      : this((float) c.R / (float) byte.MaxValue, (float) c.G / (float) byte.MaxValue, (float) c.B / (float) byte.MaxValue)
    {
    }

    public Vector3 ToVector()
    {
      float z;
      float y;
      float x;
      if ((double) this.S == 0.0)
      {
        double l;
        z = (float) (l = (double) this.L);
        y = (float) l;
        x = (float) l;
      }
      else
      {
        float q = (double) this.L < 0.5 ? this.L * (1f + this.S) : (float) ((double) this.L + (double) this.S - (double) this.L * (double) this.S);
        float p = 2f * this.L - q;
        x = HSLColor.HUE2RGB(p, q, this.H + 0.3333333f);
        y = HSLColor.HUE2RGB(p, q, this.H);
        z = HSLColor.HUE2RGB(p, q, this.H - 0.3333333f);
      }
      return new Vector3(x, y, z);
    }

    private static float HUE2RGB(float p, float q, float t)
    {
      if ((double) t < 0.0)
        ++t;
      if ((double) t > 1.0)
        --t;
      if ((double) t < 1.0 / 6.0)
        return p + (float) (((double) q - (double) p) * 6.0) * t;
      if ((double) t < 0.5)
        return q;
      if ((double) t < 2.0 / 3.0)
        return p + (float) (((double) q - (double) p) * (0.666666686534882 - (double) t) * 6.0);
      return p;
    }
  }
}
