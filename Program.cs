using System;
using static SDL2.SDL;

namespace Sdl2AsciiEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Engine engine = new Engine();
            Font font = new Font();
            Console.WriteLine("---- Starting ----");
            Console.WriteLine("init");
            engine.Init();
            engine.LoadContent();
            while (engine.running) {
                engine.Update();
                engine.Render();
            }
            engine.Exit();
            Console.WriteLine("exit");
        }
    }
}