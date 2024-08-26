namespace ConsoleTetris
{
    internal class Piece
    {
        public (int X, int Y)[] Segments = { (0, 0), (0, 0), (0, 0), (0, 0) };
        protected (int X, int Y)[] _initialPosition;
        protected int _rotation = 0;
        public char DisplayChar;
        object _piecePositionLock = new object();
        protected (int X, int Y)[][] _rotationDeltas;

        protected Display _display;

        public Piece(Display display, char displayChar, (int X, int Y)[][] rotationDeltas, (int X, int Y)[] initialPosition)
        {
            _initialPosition = initialPosition;
            _display = display;
            DisplayChar = displayChar;
            _rotationDeltas = rotationDeltas;
            ResetPosition();
        }
        public bool Move((int X, int Y) direction)
        {
            return Move(Enumerable.Repeat(direction, 4).ToArray());
        }
        public bool Move((int X, int Y)[] deltas)
        {
            lock (_piecePositionLock)
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
        }
        public void Show()
        {
            foreach (var item in Segments)
            {
                _display.DrawOnPlayArea(item.X, item.Y, DisplayChar);
            }
        }
        public void Hide()
        {
            foreach (var item in Segments)
            {
                _display.SetEmptyOnPlayArea(item.X, item.Y);
            }
        }
        public void RotateCCW()
        {
            (int X, int Y)[] deltas = _rotationDeltas[(_rotation + 1) % 4];

            bool rotated = Rotate(deltas);
            if (rotated)
            {
                _rotation--;
                _rotation = (_rotation + 4) % 4;
            }

        }
        public void RotateCW()
        {
            (int X, int Y)[] deltas = _rotationDeltas[_rotation];

            bool rotated = Rotate(deltas);
            if (rotated)
            {
                _rotation++;
                _rotation = (_rotation + 4) % 4;
            }

        }
        public void ResetPosition()
        {
            _rotation = _initialPosition[4].X;
            for (int i = 0; i < Segments.Length; i++)
            {
                Segments[i].X = _initialPosition[i].X;
                Segments[i].Y = _initialPosition[i].Y;
            }
        }

        private bool SegmentsCanMovenDirection((int X, int Y)[] direction)
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
        private bool Rotate((int X, int Y)[] deltas)
        {
            lock (_piecePositionLock)
            {
                lock (_display)
                {
                    bool moved = Move(deltas);
                    if (moved)
                    {
                        return true;
                    }
                    moved = Move(ShiftDelta(deltas, (1, 0)));
                    if (moved)
                    {
                        return true;
                    }
                    moved = Move(ShiftDelta(deltas, (-1, 0)));
                    if (moved)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }
        private (int X, int Y)[] ShiftDelta((int X, int Y)[] deltas, (int X, int Y) shift)
        {
            (int X, int Y)[] result = { (0, 0), (0, 0), (0, 0), (0, 0) };
            for (int i = 0; i < deltas.Length; i++)
            {
                result[i].X = deltas[i].X + shift.X;
                result[i].Y = deltas[i].Y + shift.Y;
            }
            return result;
        }
        #region I Piece
        public static (int X, int Y)[] IInitialPositions =
        [(0, Display.s_height - 1), (1, Display.s_height - 1), (2, Display.s_height - 1), (3, Display.s_height - 1), (0, 0)];

        static readonly (int X, int Y)[] s_icw0 = { (2, 1), (1, 0), (0, -1), (-1, -2) };
        static readonly (int X, int Y)[] s_icw1 = { (1, -2), (0, -1), (-1, 0), (-2, 1) };
        static readonly (int X, int Y)[] s_icw2 = { (-2, -1), (-1, 0), (0, 1), (1, 2) };
        static readonly (int X, int Y)[] s_icw3 = { (-1, 2), (0, 1), (1, 0), (2, -1) };
        public static readonly (int X, int Y)[][] IRotationDeltas = { s_icw0, s_icw1, s_icw2, s_icw3 };
        #endregion
        #region B Piece
        public static (int X, int Y)[] BInitialPositions =
        [(0, Display.s_height - 1), (1, Display.s_height - 1), (1, Display.s_height - 2), (0, Display.s_height - 2), (0, 0)];

        static readonly (int X, int Y)[] s_bcw0 = { (1, 0), (0, -1), (-1, 0), (0, 1) };
        static readonly (int X, int Y)[] s_bcw1 = { (0, -1), (-1, 0), (0, 1), (1, 0) };
        static readonly (int X, int Y)[] s_bcw2 = { (-1, 0), (0, 1), (1, 0), (0, -1) };
        static readonly (int X, int Y)[] s_bcw3 = { (0, 1), (1, 0), (0, -1), (-1, 0) };
        public static readonly (int X, int Y)[][] BRotationDeltas = { s_bcw0, s_bcw1, s_bcw2, s_bcw3 };
        #endregion
        #region L Piece
        public static (int X, int Y)[] LInitialPositions =
        [(0, Display.s_height - 2), (1, Display.s_height - 2), (2, Display.s_height - 2), (2, Display.s_height - 1), (0, 0)];

        static readonly (int X, int Y)[] s_lcw0 = { (1, 1), (0, 0), (-1, -1), (0, -2) };
        static readonly (int X, int Y)[] s_lcw1 = { (1, -1), (0, 0), (-1, 1), (-2, 0) };
        static readonly (int X, int Y)[] s_lcw2 = { (-1, -1), (0, 0), (1, 1), (0, 2) };
        static readonly (int X, int Y)[] s_lcw3 = { (-1, 1), (0, 0), (1, -1), (2, 0) };
        public static readonly (int X, int Y)[][] LRotationDeltas = { s_lcw0, s_lcw1, s_lcw2, s_lcw3 };
        #endregion
        #region J Piece
        public static (int X, int Y)[] JInitialPositions =
        [(2, Display.s_height - 2), (1, Display.s_height - 2), (0, Display.s_height - 2), (0, Display.s_height - 1), (0, 0)];

        static readonly (int X, int Y)[] s_jcw0 = { (-1, -1), (0, 0), (1, 1), (2, 0) };
        static readonly (int X, int Y)[] s_jcw1 = { (-1, 1), (0, 0), (1, -1), (0, -2) };
        static readonly (int X, int Y)[] s_jcw2 = { (1, 1), (0, 0), (-1, -1), (-2, 0) };
        static readonly (int X, int Y)[] s_jcw3 = { (1, -1), (0, 0), (-1, 1), (0, 2) };
        public static readonly (int X, int Y)[][] JRotationDeltas = { s_jcw0, s_jcw1, s_jcw2, s_jcw3 };
        #endregion
        #region T Piece
        public static (int X, int Y)[] TInitialPositions =
        [(0, Display.s_height - 2), (1, Display.s_height - 2), (1, Display.s_height - 1), (2, Display.s_height - 2), (0, 0)];

        static readonly (int X, int Y)[] s_tcw0 = { (1, 1), (0, 0), (1, -1), (-1, -1) };
        static readonly (int X, int Y)[] s_tcw1 = { (1, -1), (0, 0), (-1, -1), (-1, 1) };
        static readonly (int X, int Y)[] s_tcw2 = { (-1, -1), (0, 0), (-1, 1), (1, 1) };
        static readonly (int X, int Y)[] s_tcw3 = { (-1, 1), (0, 0), (1, 1), (1, -1) };
        public static readonly (int X, int Y)[][] TRotationDeltas = { s_tcw0, s_tcw1, s_tcw2, s_tcw3 };
        #endregion
        #region Z Piece
        public static (int X, int Y)[] ZInitialPositions =
        [(2, Display.s_height - 2), (1, Display.s_height - 2), (1, Display.s_height - 1), (0, Display.s_height - 1), (0, 0)];

        static readonly (int X, int Y)[] s_zcw0 = { (-1, -1), (0, 0), (1, -1), (2, 0) };
        static readonly (int X, int Y)[] s_zcw1 = { (-1, 1), (0, 0), (-1, -1), (0, -2) };
        static readonly (int X, int Y)[] s_zcw2 = { (1, 1), (0, 0), (-1, 1), (-2, 0) };
        static readonly (int X, int Y)[] s_zcw3 = { (1, -1), (0, 0), (1, 1), (0, 2) };
        public static readonly (int X, int Y)[][] ZRotationDeltas = { s_zcw0, s_zcw1, s_zcw2, s_zcw3 };
        #endregion
        #region S Piece
        public static (int X, int Y)[] SInitialPositions =
        [(0, Display.s_height - 2), (1, Display.s_height - 2), (1, Display.s_height - 1), (2, Display.s_height - 1), (0, 0)];

        static readonly (int X, int Y)[] s_scw0 = { (1, 1), (0, 0), (1, -1), (0, -2) };
        static readonly (int X, int Y)[] s_scw1 = { (1, -1), (0, 0), (-1, -1), (-2, 0) };
        static readonly (int X, int Y)[] s_scw2 = { (-1, -1), (0, 0), (-1, 1), (0, 2) };
        static readonly (int X, int Y)[] s_scw3 = { (-1, 1), (0, 0), (1, 1), (2, 0) };
        public static readonly (int X, int Y)[][] SRotationDeltas = { s_scw0, s_scw1, s_scw2, s_scw3 };
        #endregion

    }
}
