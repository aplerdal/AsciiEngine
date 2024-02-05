using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using static SDL2.SDL_image;
using static SDL2.SDL;

namespace Sdl2AsciiEngine
{
    class Engine
    {
        List<IntPtr> textures = new List<IntPtr>();
        Dictionary<int, bool> keyboard = new Dictionary<int, bool>();
        
        SDL_Rect tileScale;

        float renderps = 0f;
        float updateps = 0f;
        
        SDL_Rect Screen;
        IntPtr window;
        IntPtr renderer;
        
        public bool running = true;
        
        Random rand = new Random();
        
        int tilesx = 64;
        int tilesy = 36;
        
        Screen screen;

        bool console = false;
        bool pressed = false;

        public void Exit()
        {
            foreach (IntPtr tex in textures)
            {
                SDL_DestroyTexture(tex);
            }
            SDL_DestroyRenderer(renderer);
            SDL_DestroyWindow(window);
            SDL_Quit();
        }
        public void Init()
        {
            if (SDL_Init(SDL_INIT_VIDEO) < 0)
            {
                Console.WriteLine($"There was an issue initilizing SDL. {SDL_GetError()}");
            }
            window = SDL_CreateWindow(
                "SDL Engine",
                SDL_WINDOWPOS_UNDEFINED,
                SDL_WINDOWPOS_UNDEFINED,
                1280, 720,
                SDL_WindowFlags.SDL_WINDOW_SHOWN
            );
            Screen = new SDL_Rect {
                x=0,
                y=0,
                w=1280,
                h=720,
            };
            tileScale = new SDL_Rect { 
                x = 0,
                y = 0,
                w = (int)Math.Round((decimal)Screen.w / tilesx),
                h = (int)Math.Round((decimal)Screen.h / tilesy)
            };
            if (window == IntPtr.Zero)
            {
                Console.WriteLine($"There was an issue creating the window. {SDL_GetError()}");
            }
            renderer = SDL_CreateRenderer(
                window,
                -1,
                SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
                SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC
            );
            if (renderer == IntPtr.Zero)
            {
                Console.WriteLine($"There was an issue creating the renderer. {SDL_GetError()}");
            }
            if (IMG_Init(IMG_InitFlags.IMG_INIT_PNG) == 0)
            {
                Console.WriteLine($"There was an issue initilizing SDL2_Image {SDL_image.IMG_GetError()}");
            }
            screen = new Screen(tilesx, tilesy);
            foreach (var key in Enum.GetValues(typeof(SDL_Keycode)))
            {
                keyboard.Add((int)key, false);
            }
        }
        public void LoadContent()
        {
            textures.Add(IMG_LoadTexture(renderer, "assets/font.png"));
        }
        public void Update()
        {
            var timing = System.Diagnostics.Stopwatch.StartNew();
            screen.SetColor(Color.White);
            screen.SetBgColor(new Color(0, 0, 0));
            screen.Clear();
            while (SDL_PollEvent(out SDL_Event e) == 1)
            {
                switch (e.type)
                {
                    case SDL_EventType.SDL_QUIT:
                        running = false;
                        break;
                    case SDL_EventType.SDL_KEYDOWN:
                        keyboard [(int)e.key.keysym.sym] = true;
                        break;
                    case SDL_EventType.SDL_KEYUP:
                        keyboard[(int)e.key.keysym.sym] = false;
                        break;
                }
            }

            screen.SetColor(new Color(255, 255, 0));
            if (keyboard[(int)SDL_Keycode.SDLK_w])
            {
                screen.WriteString("W", 4, 3);
            }
            if (keyboard[(int)SDL_Keycode.SDLK_a])
            {
                screen.WriteString("A", 3, 4);
            }
            if (keyboard[(int)SDL_Keycode.SDLK_s])
            {
                screen.WriteString("S", 4, 4);
            }
            if (keyboard[(int)SDL_Keycode.SDLK_d])
            {
                screen.WriteString("D", 5, 4);
            }
            if (keyboard[(int)SDL_Keycode.SDLK_BACKQUOTE])
            {
                if (!pressed)
                {
                    pressed = true;
                    console = !console;
                }
            }else
            {
                pressed = false;
            }

            screen.SetColor(Color.White);
            screen.WriteString("Hello, World! >.<", 1, 1);
            screen.WriteDoubleRectangle(0, 0, 18, 12);

            if (console)
            {
                screen.WriteLightRectangle(0, 0, tilesx-2, tilesy-12);
                screen.ClearArea(1, 1, tilesx - 3, tilesy - 13);
            }

            //Console.WriteLine("Update");
            timing.Stop();
            updateps = timing.ElapsedMilliseconds;

            //FPS
            string timestr = Math.Round((renderps + updateps)).ToString().PadLeft(6, '0').Substring(0, 2) + "." + Math.Round((renderps + updateps)).ToString().PadLeft(6, '0').Substring(2, 4) + " seconds per cycle";
            screen.WriteString(timestr, 64-timestr.Length, 35);
        }
        public void Render()
        {
            var timing = System.Diagnostics.Stopwatch.StartNew();
            SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
            SDL_RenderClear(renderer);
            //framebuffer[1, 1] = 1;
            for (int y = 0; y < tilesy; y++)
            {
                for (int x = 0; x < tilesx; x++)
                {
                    Tile t = screen.framebuffer[x, y];
                    SDL_Rect tilePos = new SDL_Rect { x = x * tileScale.w, y = y * tileScale.h, w = tileScale.w, h = tileScale.h };

                    SDL_Rect bgtile = new SDL_Rect { x = 88, y = 104, w = 8, h = 8 };                    
                    SDL_SetTextureColorMod(textures[0], t.bgColor.r, t.bgColor.g, t.bgColor.b);
                    SDL_RenderCopy(renderer, textures[0], ref bgtile, ref tilePos);

                    SDL_Rect tile = new SDL_Rect { x = (t.index %16)*8, y = (int)Math.Floor((decimal)t.index / 16)*8, w = 8, h = 8 };
                    SDL_SetTextureColorMod(textures[0], t.color.r, t.color.g, t.color.b);
                    SDL_RenderCopy(renderer, textures[0], ref tile, ref tilePos);
                }
            }
            SDL_RenderPresent(renderer);
            timing.Stop();
            renderps = timing.ElapsedMilliseconds;
        }
    }
}
