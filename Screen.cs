using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl2AsciiEngine
{
    public struct Color
    {
        public byte r;
        public byte g;
        public byte b;
        public Color(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
        public static Color White { get { return new Color(255, 255, 255); } }
        public static Color Black { get { return new Color(0, 0, 0); } }
        public static Color Red { get { return new Color(255, 0, 0); } }
        public static Color Green {get { return new Color(0, 255, 0); } }
        public static Color Blue { get { return new Color(0, 0, 255); } }
        public static Color Yellow { get { return new Color(255, 255, 0); } }
        public static Color Aqua { get { return new Color(0, 255, 255); } }
        public static Color Magenta { get { return new Color(255, 0, 255); } }
    }
    public struct Tile{
        public int index;
        public Color color;
        public Color bgColor;
        public Tile(int index, Color color, Color bgColor)
        {
            this.index = index;
            this.color = color;
            this.bgColor = bgColor;
        }
    }
    public class Screen
    {
        public int xsize;
        public int ysize;
        public Tile[,] framebuffer;
        public Color color;
        public Color bgColor;

        public Screen(int xsize, int ysize)
        {
            this.ysize = ysize;
            this.xsize = xsize;
            this.framebuffer = new Tile[xsize, ysize];
        }
        public void Clear()
        {
            for (int y = 0; y < ysize; y++)
            {
                for (int x = 0; x < xsize; x++)
                {
                    framebuffer[x, y] = new Tile(0, color, bgColor);
                }
            }
        }
        public void SetColor(Color color)
        {
            this.color = color;
        }
        public void SetBgColor(Color bgColor)
        {
            this.bgColor = bgColor;
        }
        public void WriteString(string text, int x, int y)
        {
            int[] indicies = Font.TilesFromString(text);

            for (int _x = 0; _x < indicies.Length; _x++)
            {
                if ((x + _x < framebuffer.GetLength(0))) { if (ValidPos(x + _x, y)) framebuffer[x + _x, y] = new Tile(indicies[_x], color, bgColor);}
            }
        }
        public void ClearArea(int x, int y, int w, int h)
        {
            for (int i = y; i < y+h; i++)
            {
                for (var j = x; j < x+w; j++)
                {
                    if (ValidPos(j, i)) framebuffer[j, i] = new Tile((int)Font.Character.FULL_BLOCK, bgColor, color);
                }
            }
        }
        public void WriteLightRectangle(int x, int y, int w, int h)
        {
            //corners
            if (ValidPos(x, y)) framebuffer[x, y]     = new Tile((int)Font.Character.BOX_DRAWINGS_LIGHT_DOWN_AND_RIGHT, color, bgColor);
            if (ValidPos(x + w, y)) framebuffer[x+w, y]   = new Tile((int)Font.Character.BOX_DRAWINGS_LIGHT_DOWN_AND_LEFT, color, bgColor);
            if (ValidPos(x, y + h)) framebuffer[x, y+h]   = new Tile((int)Font.Character.BOX_DRAWINGS_LIGHT_UP_AND_RIGHT, color, bgColor);
            if (ValidPos(x + w, y + h)) framebuffer[x+w, y+h] = new Tile((int)Font.Character.BOX_DRAWINGS_LIGHT_UP_AND_LEFT, color, bgColor);

            for (int i = 1; i < w; i++)
            {
                if (ValidPos(x + i, y)) framebuffer[x+i, y] = new Tile((int)Font.Character.BOX_DRAWINGS_LIGHT_HORIZONTAL, color, bgColor);
                if (ValidPos(x + i, y + h)) framebuffer[x+i, y+h] = new Tile((int)Font.Character.BOX_DRAWINGS_LIGHT_HORIZONTAL, color, bgColor);
            }
            for (int i = 1; i < h; i++)
            {
                if (ValidPos(x, y + i)) framebuffer[x ,  y + i] = new Tile((int)Font.Character.BOX_DRAWINGS_LIGHT_VERTICAL, color, bgColor);
                if (ValidPos(x + w, y + i)) framebuffer[x+w, y + i] = new Tile((int)Font.Character.BOX_DRAWINGS_LIGHT_VERTICAL, color, bgColor);
            }
        }
        public void WriteDoubleRectangle(int x, int y, int w, int h)
        {
            //corners
            if (ValidPos(x, y)) framebuffer[x, y] = new Tile((int)Font.Character.BOX_DRAWINGS_DOUBLE_DOWN_AND_RIGHT, color, bgColor);
            if (ValidPos(x + w, y)) framebuffer[x + w, y] = new Tile((int)Font.Character.BOX_DRAWINGS_DOUBLE_DOWN_AND_LEFT, color, bgColor);
            if (ValidPos(x, y + h)) framebuffer[x, y + h] = new Tile((int)Font.Character.BOX_DRAWINGS_DOUBLE_UP_AND_RIGHT, color, bgColor);
            if (ValidPos(x + w, y + h)) framebuffer[x + w, y + h] = new Tile((int)Font.Character.BOX_DRAWINGS_DOUBLE_UP_AND_LEFT, color, bgColor);

            for (int i = 1; i < w; i++)
            {
                if (ValidPos(x + i, y)) framebuffer[x + i, y] = new Tile((int)Font.Character.BOX_DRAWINGS_DOUBLE_HORIZONTAL, color, bgColor);
                if (ValidPos(x + i, y + h)) framebuffer[x + i, y + h] = new Tile((int)Font.Character.BOX_DRAWINGS_DOUBLE_HORIZONTAL, color, bgColor);
            }
            for (int i = 1; i < h; i++)
            {
                if (ValidPos(x, y + i)) framebuffer[x, y + i] = new Tile((int)Font.Character.BOX_DRAWINGS_DOUBLE_VERTICAL, color, bgColor);
                if (ValidPos(x + w, y + i)) framebuffer[x + w, y + i] = new Tile((int)Font.Character.BOX_DRAWINGS_DOUBLE_VERTICAL, color, bgColor);
            }
        }
        public bool ValidPos(int x, int y)
        {
            if( x >= 0 && y >= 0 && x < xsize && y < ysize)
            {
                return true;
            }
            return false;
        }
    }
}
