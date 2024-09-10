
using System.Diagnostics;

namespace ConsoleTetris
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.Clear();
            Console.CursorVisible = false;
            Console.SetWindowSize(Display.s_totalWidht, Display.s_totalHeight + 2);
            if (OperatingSystem.IsWindows())
            {
                Console.SetBufferSize(Display.s_totalWidht, Display.s_totalHeight + 2);
            }
            if (File.Exists(Globals.HighscoresPath))
            {
                using (StreamReader stream = File.OpenText(Globals.HighscoresPath))
                {
                    string? line;
                    while ((line = stream.ReadLine()) != null)
                    {
                        string[] parts = line.Split(' ');
                        if (parts.Length == 2)
                        {
                            Globals.LeaderBoard.Add((int.Parse(parts[0]), parts[1].ToCharArray()));
                        }
                    }
                }
            }
            Display display = new Display();
            Game game = new Game(display);
            Menu menu = new Menu(display, game);
            game.SetMenu(menu);
            Thread inputThread = new Thread(() =>
            {
                while (true)
                {
                    ConsoleKeyInfo input = Console.ReadKey(true);
                    if (game.Running) game.HandleInput(input);
                    else menu.HandleIndput(input);
                }
            });
            Console.Write('|');
            Console.Write(Enumerable.Repeat('-', Display.s_totalWidht - 2).ToArray());
            Console.Write('|');
            Console.WriteLine("type 'nc' for no Color\n(better performance)");
            Console.WriteLine();
            Console.WriteLine("a/d or left/right move piece");
            Console.WriteLine("q/e rotate piece");
            Console.WriteLine("w or up store piece");
            Console.WriteLine("s or down hard drop piece");
            Console.WriteLine();
            Console.WriteLine("Press Enter To Start");

            string? inp = Console.ReadLine();
            if (inp == "nc")
            {
                Globals.DrawFunction = display.DrawWithoutColor;
            }
            else
            {
                Globals.DrawFunction = display.DrawWithColor;
            }
            Console.Clear();
            display.DrawWithColor();
            game.Start();
            inputThread.Start();
            Thread.Sleep(30);
#if DEBUG
            int frameCounter = 0;
            Stopwatch stopwatch = new Stopwatch();
#endif
            while (true)
            {
                Console.SetCursorPosition(0, 0);
#if DEBUG
                stopwatch.Restart();
#endif
                Globals.DrawFunction.Invoke();
#if DEBUG
                stopwatch.Stop();
                Console.WriteLine(frameCounter);
                Console.Write(stopwatch.ElapsedMilliseconds);
                Console.Write("   ");
                frameCounter++;
#endif
                Thread.Sleep(10);
            }
        }
        
    }

}
