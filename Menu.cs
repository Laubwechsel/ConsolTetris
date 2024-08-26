using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTetris
{
    internal class Menu
    {
        private Display _display;
        private Game _game;
        private int _scorePosition = -1;
        private int _charPosition = 0;
        private bool _enteringInitials = false;
        public Menu(Display display, Game game)
        {
            _display = display;
            _game = game;
        }

        public void HandleIndput(ConsoleKeyInfo input)
        {
            if (_enteringInitials)
            {
                if (input.Key == ConsoleKey.Enter)
                {
                    _enteringInitials = false;
                    StringBuilder sb1 = new StringBuilder();
                    foreach (var item in Globals.LeaderBoard)
                    {
                        sb1.AppendLine(item.score.ToString() + " " + new string(item.inital));
                    }
                    File.WriteAllText(Globals.HighscoresPath, sb1.ToString());

                    DisplayOptions();
                    return;
                }
                AddCharToInitials(input.KeyChar);
                return;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var item in Globals.LeaderBoard)
            {
                sb.AppendLine(item.score.ToString() + " " + new string(item.inital));
            }
            File.WriteAllText(Globals.HighscoresPath, sb.ToString());

            DisplayOptions();

            switch (input.KeyChar)
            {
                case '1':
                    _display.Reset();
                    _game.Reset();
                    Reset();
                    break;
                case '2':
                    Environment.Exit(0);
                    break;
            }
        }
        private void AddCharToInitials(char c)
        {
            if (_scorePosition < 0)
            {
                return;
            }
            if (_charPosition > 2)
            {
                _charPosition = 0;
            }
            Globals.LeaderBoard[_scorePosition].inital[_charPosition] = c;
            Globals.PlayerInitials[_charPosition] = c;
            _charPosition++;
            _display.DrawLeaderboard();
        }
        private void Reset()
        {
            _charPosition = 0;
            _scorePosition = -1;
        }
        public void DisplayGameOver()
        {
            lock (_display)
            {
                _display.ClearScoreBoard();
                string go = "Game Over";
                string score = "Score:";
                string scoreNumber = _display.FormatNumber(_game.Score);

                _display.WriteOnScoreBord(17, go);
                _display.WriteOnScoreBord(16, score);
                _display.WriteOnScoreBord(15, scoreNumber);

                List<(int score, char[] initials)> leaderBoard = Globals.LeaderBoard;
                leaderBoard = leaderBoard.OrderByDescending((x) => x.score).ToList();

                for (int i = 0; i < leaderBoard.Count && i + 7 < 17; i++)
                {
                    if (leaderBoard[i].score < _game.Score && _scorePosition == -1)
                    {
                        _scorePosition = i;
                        _enteringInitials = true;
                        leaderBoard.Insert(i, (_game.Score, (char[])Globals.PlayerInitials.Clone()));
                        string enter = "Enter Initials";
                        string ready = "Press Enter";
                        string cont = "To Continue";
                        _display.WriteOnScoreBord(Display.s_height - 18, enter);
                        _display.WriteOnScoreBord(Display.s_height - 19, ready);
                        _display.WriteOnScoreBord(Display.s_height - 20, cont);
                        break;
                    }
                }
                if (leaderBoard.Count < 10 && _scorePosition == -1)
                {
                    _scorePosition = leaderBoard.Count;
                    _enteringInitials = true;
                    leaderBoard.Add((_game.Score, (char[])Globals.PlayerInitials.Clone()));
                    string enter = "Enter Initials";
                    string ready = "Press Enter";
                    string cont = "To Continue";
                    _display.WriteOnScoreBord(Display.s_height - 18, enter);
                    _display.WriteOnScoreBord(Display.s_height - 19, ready);
                    _display.WriteOnScoreBord(Display.s_height - 20, cont);

                }
                Globals.LeaderBoard = leaderBoard;
                _display.DrawLeaderboard();
            }

        }
        private void DisplayOptions()
        {
            lock (_display)
            {
                _display.ClearScoreBoard();
                _display.DrawLeaderboard();
                string go = "Game Over";
                string score = "Score:";
                string scoreNumber = _display.FormatNumber(_game.Score);

                _display.WriteOnScoreBord(17, go);
                _display.WriteOnScoreBord(16, score);
                _display.WriteOnScoreBord(15, scoreNumber);
                string retry = $"1 Restart";
                string exit = "2 Exit";
                _display.WriteOnScoreBord(Display.s_height - 18, retry);
                _display.WriteOnScoreBord(Display.s_height - 19, exit);

            }
        }
    }
}
