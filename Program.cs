// Program.cs
using System;
using System.Drawing;
using System.Windows.Forms;
// Alias the WinForms timer to avoid conflicts
using FormsTimer = System.Windows.Forms.Timer;

namespace WinFormsPong
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GameForm());
        }
    }

    public class GameForm : Form
    {
        private readonly FormsTimer _gameTimer = new FormsTimer();
        private Rectangle _leftPaddle, _rightPaddle, _ball;
        private const int PaddleWidth = 10, PaddleHeight = 60, BallSize = 10;
        private int _leftScore = 0, _rightScore = 0;
        private float _ballVelX = 4, _ballVelY = 4;
        private bool _wDown, _sDown, _upDown, _downDown;
        private readonly Random _rand = new Random();

        public GameForm()
        {
            Text = "Zoni Pong";
            DoubleBuffered = true;
            ClientSize = new Size(800, 600);
            BackColor = Color.Black;

            // Initial positions
            _leftPaddle  = new Rectangle(30, (Height - PaddleHeight) / 2, PaddleWidth, PaddleHeight);
            _rightPaddle = new Rectangle(Width - 30 - PaddleWidth, (Height - PaddleHeight) / 2, PaddleWidth, PaddleHeight);
            _ball        = new Rectangle((Width - BallSize) / 2, (Height - BallSize) / 2, BallSize, BallSize);

            // Game loop ~60 FPS
            _gameTimer.Interval = 16;
            _gameTimer.Tick += GameLoop;
            _gameTimer.Start();

            KeyDown += OnKeyDown;
            KeyUp   += OnKeyUp;
            Resize  += (s, e) => ResetPositions();
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            // Move paddles
            if (_wDown && _leftPaddle.Y > 0)
                _leftPaddle.Y -= 6;
            if (_sDown && _leftPaddle.Y < ClientSize.Height - PaddleHeight)
                _leftPaddle.Y += 6;
            if (_upDown && _rightPaddle.Y > 0)
                _rightPaddle.Y -= 6;
            if (_downDown && _rightPaddle.Y < ClientSize.Height - PaddleHeight)
                _rightPaddle.Y += 6;    // <-- fixed from _downPaddle

            // Move ball
            _ball.X += (int)_ballVelX;
            _ball.Y += (int)_ballVelY;

            // Bounce off top/bottom
            if (_ball.Y <= 0 || _ball.Y >= ClientSize.Height - BallSize)
                _ballVelY = -_ballVelY;

            // Bounce off paddles
            if (_ball.IntersectsWith(_leftPaddle)  && _ballVelX < 0) _ballVelX = -_ballVelX;
            if (_ball.IntersectsWith(_rightPaddle) && _ballVelX > 0) _ballVelX = -_ballVelX;

            // Score
            if (_ball.X < 0)
            {
                _rightScore++;
                ResetBall();
            }
            else if (_ball.X > ClientSize.Width - BallSize)
            {
                _leftScore++;
                ResetBall();
            }

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            // Draw paddles & ball
            g.FillRectangle(Brushes.White, _leftPaddle);
            g.FillRectangle(Brushes.White, _rightPaddle);
            g.FillEllipse(Brushes.White, _ball);

            // Draw center dashed line
            using var pen = new Pen(Color.Gray) { DashPattern = new float[] { 4, 4 } };
            g.DrawLine(pen, ClientSize.Width/2, 0, ClientSize.Width/2, ClientSize.Height);

            // Draw score
            string score = $"{_leftScore}   { _rightScore}";
            var sf = new StringFormat { Alignment = StringAlignment.Center };
            g.DrawString(score, Font, Brushes.White, ClientSize.Width/2, 20, sf);
        }

        private void ResetBall()
        {
            _ball.X = (ClientSize.Width - BallSize) / 2;
            _ball.Y = (ClientSize.Height - BallSize) / 2;
            // Randomize direction
            _ballVelX = _rand.Next(0, 2) == 0 ? 4 : -4;
            _ballVelY = _rand.Next(0, 2) == 0 ? 4 : -4;
        }

        private void ResetPositions()
        {
            _leftPaddle.Y  = (ClientSize.Height - PaddleHeight) / 2;
            _rightPaddle.X = ClientSize.Width - 30 - PaddleWidth;
            _rightPaddle.Y = (ClientSize.Height - PaddleHeight) / 2;
            ResetBall();
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)    _wDown    = true;
            if (e.KeyCode == Keys.S)    _sDown    = true;
            if (e.KeyCode == Keys.Up)   _upDown   = true;
            if (e.KeyCode == Keys.Down) _downDown = true;
        }

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)    _wDown    = false;
            if (e.KeyCode == Keys.S)    _sDown    = false;
            if (e.KeyCode == Keys.Up)   _upDown   = false;
            if (e.KeyCode == Keys.Down) _downDown = false;
        }
    }
}
