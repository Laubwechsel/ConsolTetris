namespace ConsoleTetris
{
    internal class GhostPiece
    {
        public (int X, int Y)[] Segments = { (0, 0), (0, 0), (0, 0), (0, 0) };
        Display _display;
        public char DisplayChar;
        public GhostPiece(Display display)
        {
            _display = display;
            DisplayChar = _display.EmptyChars[1];
        }
        public void Show()
        {
            foreach (var item in Segments)
            {
                if (_display.IsPlayareaEmptyThere(item))
                {
                    _display.DrawOnPlayArea(item.X, item.Y, DisplayChar);

                }
            }

        }
        public void Hide()
        {
            foreach (var item in Segments)
            {
                if (_display.GetPlayAreaChar(item) == DisplayChar)
                {
                    _display.SetEmptyOnPlayArea(item.X, item.Y);

                }
            }

        }
        public bool Move((int X, int Y) direction)
        {
            return Move(Enumerable.Repeat(direction, 4).ToArray());
        }

        public bool Move((int X, int Y)[] deltas)
        {
            if (!SegmentsCanMovenDirection(deltas))
            {
                return false;
            }
            lock (_display)
            {
                Hide();
                for (int i = 0; i < Segments.Length; i++)
                {
                    Segments[i].Y += deltas[i].Y;
                    Segments[i].X += deltas[i].X;
                }
                Show();
            }
            return true;

        }
        protected bool SegmentsCanMovenDirection((int X, int Y)[] direction)
        {
            for (int i = 0; i < Segments.Length; i++)
            {
                (int X, int Y) nextLocation = (direction[i].X + Segments[i].X, direction[i].Y + Segments[i].Y);
                if (HittingSelf(nextLocation))
                {
                    continue;
                }
                bool failedBoundCheck = nextLocation.Y < 0 || nextLocation.X < 0 || nextLocation.X >= Display.s_playAreaWidth;
                bool failedAreaCheck = !_display.IsPlayareaEmptyThere(nextLocation);
                if (failedBoundCheck || failedAreaCheck)
                {
                    return false;
                }
            }
            return true;
        }
        private bool HittingSelf((int X, int Y) nextPosition)
        {
            foreach (var item in Segments)
            {
                if (item.X == nextPosition.X && item.Y == nextPosition.Y)
                {
                    return true;
                }
            }
            return false;
        }


    }
}
