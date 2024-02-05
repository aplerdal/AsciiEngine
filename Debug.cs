using SDL2;
using static SDL2.SDL;
namespace Sdl2AsciiEngine{
    class Debug{
        public Engine engine;
        public bool Enabled = false;
        bool pressed = false;
        string currentInput = "";
        LinkedList<String> output = new LinkedList<string>();

        public void HandleInput(Dictionary<int,bool> keyPressed){
            if(keyPressed[(int)SDL_Keycode.SDLK_RETURN]){
                ExecuteCommand(currentInput);
            }
        }
        public void ExecuteCommand(string cmd){
            string[] command = cmd.Split(' ');
            switch (command[0]){
                case "var":
                    output.RemoveFirst();
                    output.AddLast(typeof(Engine).GetProperty(command[1]).GetValue(engine)?.ToString()??"noval");
                    break;
                default:
                    break;
            }
        }
    }
}