using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDL2;
using System.Timers;
using static SDL2.SDL;
using Timer = System.Timers.Timer;
using System.Diagnostics;

namespace Sdl2AsciiEngine
{
    class Engine
    {
        List<IntPtr> textures = new List<IntPtr>();

        #region Keyboard
        Dictionary<int, bool> keyDown = new Dictionary<int, bool>();
        Dictionary<int, bool> keyPress = new Dictionary<int, bool>();
        Dictionary<int, bool> keyReleased = new Dictionary<int, bool>();
        #endregion

        SDL_Rect tileScale;

        public long tickCount = 0;

        public float renderTime = 0f;
        public float updateTime = 0f;

        Stopwatch activeTime = new Stopwatch();

        SDL_Rect Screen;
        IntPtr window;
        IntPtr renderer;
        
        public bool running = true;
        
        Random rand = new Random();

        public static int tilesx = 64;
        public static int tilesy = 36;

        Debug debug = new Debug(new SDL_Rect() { x = 0, y = 0, w = tilesx-1, h = (tilesy-10) });

        Screen? screen;

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
            screen = new Screen(tilesx, tilesy);

            debug.engine = this;
            debug.screen = screen;
            SDL_StopTextInput();

            activeTime.Start();

            foreach (var key in Enum.GetValues(typeof(SDL_Keycode)))
            {
                this.keyDown.Add((int)key, false);
                keyPress.Add((int)key,false);
                keyReleased.Add((int)key,false);
            }
        }
        public void LoadContent()
        {
            IntPtr temp = SDL_LoadBMP("assets/font.bmp");
            textures.Add(SDL_CreateTextureFromSurface(renderer,temp));  
            SDL_FreeSurface(temp);
        }
        public unsafe void HandleEvents()
        {
            foreach (var i in keyReleased.Keys.ToList())
            {
                keyReleased[i] = false;
                keyPress[i] = false;
            }
            while (SDL_PollEvent(out SDL_Event e) == 1)
            {
                switch (e.type)
                {
                    case SDL_EventType.SDL_QUIT:
                        running = false;
                        break;
                    case SDL_EventType.SDL_TEXTINPUT:
                        debug.currentInput += Encoding.UTF8.GetString(e.text.text, 1);
                        break;
                    case SDL_EventType.SDL_KEYDOWN:
                        if (keyDown[(int)e.key.keysym.sym]==false){
                            keyPress[(int)e.key.keysym.sym] = true;
                        }
                        keyDown[(int)e.key.keysym.sym] = true;
                        break;
                    case SDL_EventType.SDL_KEYUP:
                    	keyReleased[(int)e.key.keysym.sym] = true;
                        keyDown[(int)e.key.keysym.sym] = false;
                        break;
                }
            }
        }
        public void Update()
        {
            var timing = Stopwatch.StartNew();
            tickCount++;
            screen.SetColor(Color.White);
            screen.SetBgColor(new Color(0, 0, 0));
            screen.Clear();
            HandleEvents();

            screen.SetColor(new Color(255, 255, 0));
            if (keyDown[(int)SDL_Keycode.SDLK_w])
            {
                screen.WriteString("W", 4, 3);
            }
            if (keyDown[(int)SDL_Keycode.SDLK_a])
            {
                screen.WriteString("A", 3, 4);
            }
            if (keyDown[(int)SDL_Keycode.SDLK_s])
            {
                screen.WriteString("S", 4, 4);
            }
            if (keyDown[(int)SDL_Keycode.SDLK_d])
            {
                screen.WriteString("D", 5, 4);
            }
            if (keyPress[(int)SDL_Keycode.SDLK_BACKQUOTE]){
                if (!debug.Enabled)
                {
                    debug.Activate();
                } else
                {
                    debug.Deactivate();
                }
            }
            
            screen.SetColor(Color.White);
            screen.WriteString("Hello, World! >.<", 1, 1);
            screen.WriteDoubleRectangle(0, 0, 18, 12);
            if (debug.Enabled)
            {
                debug.HandleInput(keyPress);
                debug.Draw(activeTime);
            }
            

            //Console.WriteLine("Update");
            timing.Stop();
            updateTime = timing.ElapsedMilliseconds;

            //FPS
            string timestr = Math.Round(renderTime + updateTime).ToString().PadLeft(6, '0').Substring(0, 2) + "." + Math.Round((renderTime + updateTime)).ToString().PadLeft(6, '0').Substring(2, 4) + " seconds per cycle";
            screen.WriteString(timestr, 64-timestr.Length, 35);
        }
        public void Render()
        {
            var timing = Stopwatch.StartNew();
            SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
            SDL_RenderClear(renderer);
            for (int y = 0; y < tilesy; y++)
            {
                for (int x = 0; x < tilesx; x++)
                {
                    Tile t = screen.framebuffer[x, y];
                    SDL_Rect tilePos = new SDL_Rect { x = x * tileScale.w, y = y * tileScale.h, w = tileScale.w, h = tileScale.h };

                    SDL_Rect bgtile = new SDL_Rect { x = 88, y = 104, w = 8, h = 8 };                    
                    SDL_SetTextureColorMod(textures[0], t.bgColor.r, t.bgColor.g, t.bgColor.b);
                    SDL_RenderCopy(renderer, textures[0], ref bgtile, ref tilePos);

                    SDL_Rect tile = new SDL_Rect { x = t.index %16*8, y = (int)Math.Floor((decimal)t.index / 16)*8, w = 8, h = 8 };
                    SDL_SetTextureColorMod(textures[0], t.color.r, t.color.g, t.color.b);
                    SDL_RenderCopy(renderer, textures[0], ref tile, ref tilePos);
                }
            }
            SDL_RenderPresent(renderer);
            timing.Stop();
            renderTime = timing.ElapsedMilliseconds;
        }
    }
}
