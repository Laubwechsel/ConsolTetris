using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTetris
{
    internal class Globals
    {
        public static Action? DrawFunction;
        public static int LastGameScore = 0;
        public static char[] PlayerInitials = ['-', '-', '-'];
        public static List<(int score, char[] inital)> LeaderBoard = new();
        public static string HighscoresPath = Path.Combine(Directory.GetCurrentDirectory(), "highscores.txt");
    }
}
