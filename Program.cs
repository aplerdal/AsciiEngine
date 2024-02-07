using TwC;

namespace AsciiEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Engine engine = new Engine(new Game()); 
            engine.Init();
            engine.LoadContent();

            // Set to false to disable the debug console
            Debug.Allowed = true;

            while (engine.running) {
                engine.Update();
                engine.Render();
            }
            engine.Exit();
        }
    }
}