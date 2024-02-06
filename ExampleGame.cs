using AsciiEngine;
using static SDL2.SDL;

namespace ExampleGame{
    class Game : GameManager{
        public override void Init()
        {
            base.Init();
            //handle initialization here
        }
        public override void Update()
        {
            //handle game logic here
            base.Update();
            
            screen.SetColor(Color.White);
            screen.SetBgColor(new Color(0, 0, 0));
            screen.Clear();

            screen.SetColor(new Color(255, 255, 0));
            if (engine.keyDown[(int)SDL_Keycode.SDLK_w])
            {
                screen.WriteString("W", 4, 3);
            }
            if (engine.keyDown[(int)SDL_Keycode.SDLK_a])
            {
                screen.WriteString("A", 3, 4);
            }
            if (engine.keyDown[(int)SDL_Keycode.SDLK_s])
            {
                screen.WriteString("S", 4, 4);
            }
            if (engine.keyDown[(int)SDL_Keycode.SDLK_d])
            {
                screen.WriteString("D", 5, 4);
            }
            if (engine.keyPress[(int)SDL_Keycode.SDLK_BACKQUOTE]){
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
                debug.HandleInput(engine.keyPress);
                debug.Draw(engine.activeTime);
            }
        }
    }
}