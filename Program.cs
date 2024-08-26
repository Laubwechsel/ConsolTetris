
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
                using(StreamReader stream = File.OpenText(Globals.HighscoresPath)) 
                {
                    string? line;
                    while((line = stream.ReadLine()) != null)
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
            Menu menu = new Menu(display,game);
            game.SetMenu(menu);
            Thread inputThread = new Thread(() =>
            {
                while (true)
                {
                    ConsoleKeyInfo input = Console.ReadKey(true);
                    if(game.Running) game.HandleInput(input);
                    else menu.HandleIndput(input);
                }
            });
            Console.Write('|');
            Console.Write(Enumerable.Repeat('-', Display.s_totalWidht - 2).ToArray());
            Console.Write('|');
            Console.WriteLine("Resize Window so that top\nbar is continous and this\ntext is on a new line");
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
            //RenderBenchmark(display);
            Thread.Sleep(30);
            int frameCounter = 0;
            while (true)
            {
                Console.SetCursorPosition(0, 0);
                    Globals.DrawFunction.Invoke();
                    Console.Write(frameCounter);
                    frameCounter++;
                Thread.Sleep(10);
            }
        }
        /*
        static void RenderBenchmark(Display display)
        {

            Console.SetCursorPosition(0, 0);
            Stopwatch witColor = Stopwatch.StartNew();
            display.DrawWithColor();
            witColor.Stop();
            Console.SetCursorPosition(0, 0);
            Stopwatch noColor = Stopwatch.StartNew();
            Console.Write(display.GetBuffer());
            noColor.Stop();

            double quot = ((double)witColor.ElapsedTicks) / ((double)noColor.ElapsedTicks);
            Console.SetCursorPosition(0, 0);
            Stopwatch naive = Stopwatch.StartNew();
            Render(display);
            naive.Stop();

            double quot2 = ((double)naive.ElapsedTicks) / ((double)noColor.ElapsedTicks);

            Stopwatch noWrite = Stopwatch.StartNew();
            string text = display.GetBuffer();
            noWrite.Stop();
            double quot3 = ((double)noWrite.ElapsedTicks) / ((double)noColor.ElapsedTicks);
            Console.Clear();
            Console.Write(
                $"No Color:   {noColor.Elapsed}\n" +
                $"With Color: {witColor.Elapsed}\n" +
                $"quote:      {quot.ToString("0.###")}\n" +
                $"Naive:      {naive.Elapsed}\n" +
                $"quote:      {quot2.ToString("0.###")}\n" +
                $"No Write:   {noWrite.Elapsed}\n" +
                $"quote:      {quot3.ToString("0.###")}");
            Console.ReadLine();
        }
        */
        static void Render(Display display)
        {
            string toRender = display.GetBuffer();

            for (int x = 0; x < toRender.Length; x++)
            {
                char next = toRender[x];
                if (!display._charColorMap.TryGetValue(next, out ConsoleColor color))
                {
                    color = ConsoleColor.Gray;
                }

                Console.ForegroundColor = color;

                Console.Write(next);
            }

        }
    }

}
