using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDL2;
using System.Timers;
using static SDL2.SDL;
using Timer = System.Timers.Timer;
using System.Diagnostics;

namespace AsciiEngine
{
    class Engine
    {
        List<IntPtr> textures = new List<IntPtr>();

        #region Keyboard
        public Dictionary<int, bool> keyDown = new Dictionary<int, bool>();
        public Dictionary<int, bool> keyPress = new Dictionary<int, bool>();
        public Dictionary<int, bool> keyReleased = new Dictionary<int, bool>();
        #endregion

        SDL_Rect tileScale;

        public long tickCount = 0;

        public float renderTime = 0f;
        public float updateTime = 0f;

        public Stopwatch activeTime = new Stopwatch();

        SDL_Rect Screen;
        IntPtr window;
        IntPtr renderer;
        
        public bool running = true;

        public static int tilesx = 64;
        public static int tilesy = 36;

        public Debug debug = new Debug(new SDL_Rect() { x = 0, y = 0, w = tilesx-1, h = (tilesy-10) });

        public Screen? screen;
        public GameManager game;

        public Engine(GameManager game){
            this.game = game;
        }
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

            game.screen = screen;
            game.debug = debug;
            game.engine = this;
            game.Init();
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
            tickCount++;
            HandleEvents();
            game.Update();
        }
        public void Render()
        {
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
        }
    }
}
