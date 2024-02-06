using SDL2;
using System.Diagnostics;
using static SDL2.SDL;
namespace Sdl2AsciiEngine{
    class Debug {
        public Engine engine;
        public Screen screen;
        public bool Enabled = false;
        bool pressed = false;
        public string currentInput = "";
        LinkedList<String> output = new LinkedList<string>();
        SDL_Rect rect;

        public Debug(SDL_Rect rect)
        {
            this.rect = rect;
            Init();
        }

        public void Init()
        {
            for(int i = 0; i < rect.h-3; i++)
            {
                output.AddLast("");
            }
        }
        public void Activate()
        {
            Enabled = true;
            SDL_StartTextInput();
        }
        public void Deactivate()
        {
            currentInput = currentInput[..(currentInput.Length - 1)];
            SDL_StopTextInput();
            Enabled = false;
        }
        public void Draw(Stopwatch time)
        {
            screen.ClearArea(0,0,rect.w,rect.h);
            screen.WriteLightRectangle(0, 0, rect.w, rect.h);
            string disp = currentInput.Length > rect.w - 1 ? currentInput[..(rect.w - 1)] : currentInput;
            screen.WriteString(disp, 1, rect.h - 1);
            if (Math.Floor((decimal)(time.ElapsedMilliseconds % 1000) / 500) == 1)
            {
                if (disp.Length < rect.w - 1)
                {
                    screen.WriteString("\u2588", disp.Length+1, rect.h-1);
                }
            }
            for (int i = 0; i < output.Count; i++)
            {
                screen.WriteString(output.ElementAt(i), 1, rect.h - 2 - (output.Count-i));
            }
        }
        public unsafe void HandleInput(Dictionary<int, bool> keyPress)
        {
            if (keyPress[(int)SDL_Keycode.SDLK_RETURN]) {
                ExecuteCommand(currentInput);
                currentInput = "";
            }
            if (keyPress[(int)SDL_Keycode.SDLK_BACKSPACE])
            {
                currentInput = currentInput.Substring(0,Math.Clamp(currentInput.Length-1,0,int.MaxValue));
            }
        }
        public void ExecuteCommand(string cmd){
            string[] command = cmd.Split(' ');
            switch (command[0].ToLower()){
                case "var":
                    if (command.Length < 2) {
                        output.RemoveFirst();
                        output.AddLast("Variables");
                        foreach (var i in typeof(Engine).GetFields())
                        {
                            output.RemoveFirst();
                            output.AddLast($" {i.Name}");
                        }

                    } else
                    {
                        output.RemoveFirst();
                        output.AddLast(typeof(Engine).GetField(command[1])?.GetValue(engine)?.ToString() ?? "noval");
                    }
                    break;
                default:
                    output.AddLast("Command not found");
                    break;
            }
        }
    }
}