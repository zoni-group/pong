// Program.cs
using System;
using System.Drawing;
using System.Windows.Forms;
using FormsTimer = System.Windows.Forms.Timer;

namespace WinFormsPong
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GameForm());
        }
    }

    public class GameForm : Form
    {
        /*────────── constants ──────────*/
        private const int PaddleWidth   = 10;
        private const int PaddleHeight  = 60;
        private const int BallSize      = 10;
        private const int WinningScore  = 10;      // first to 10 wins

        /*────────── timers ──────────*/
        private readonly FormsTimer _gameTimer      = new FormsTimer();   // ~60 FPS
        private readonly FormsTimer _countdownTimer = new FormsTimer();   // 1 Hz for “3-2-1”

        /*────────── state ──────────*/
        private Rectangle _leftPaddle, _rightPaddle, _ball;
        private int  _leftScore  = 0,
                     _rightScore = 0;
        private float _ballVelX  = 4,
                      _ballVelY  = 4;

        private bool _wDown, _sDown;          // human controls
        private bool _isCountingDown = false; // are we in “3-2-1” phase?
        private int  _countdown      = -1;    // 3 → 2 → 1
        private bool _gameOver       = false; // someone reached 10
        private string _winnerText   = "";

        private readonly Random _rand   = new Random();
        private readonly bool   _aiMode = true;   // right paddle AI enabled

        public GameForm()
        {
            /* window setup */
            Text           = "Zoni Pong (Single Player)";
            ClientSize     = new Size(800, 600);
            BackColor      = Color.Black;
            DoubleBuffered = true;

            /* create paddles & ball */
            _leftPaddle  = new Rectangle(30,                (ClientSize.Height - PaddleHeight) / 2, PaddleWidth, PaddleHeight);
            _rightPaddle = new Rectangle(ClientSize.Width - 30 - PaddleWidth, (ClientSize.Height - PaddleHeight) / 2,
                                         PaddleWidth, PaddleHeight);
            _ball        = new Rectangle((ClientSize.Width - BallSize) / 2, (ClientSize.Height - BallSize) / 2,
                                         BallSize, BallSize);

            /* timers */
            _gameTimer.Interval      = 16;   // ~60 FPS
            _gameTimer.Tick         += GameLoop;

            _countdownTimer.Interval = 1000; // 1 second
            _countdownTimer.Tick    += CountdownTick;

            /* events */
            KeyDown += OnKeyDown;
            KeyUp   += OnKeyUp;
            Resize  += (s, e) => ResetPositions();

            /* start first serve with countdown */
            StartCountdown();
        }

        /*───────────────── game loop ─────────────────*/
        private void GameLoop(object? sender, EventArgs e)
        {
            if (_isCountingDown || _gameOver) return;

            /* human paddle */
            if (_wDown && _leftPaddle.Y > 0)
                _leftPaddle.Y -= 6;
            if (_sDown && _leftPaddle.Y < ClientSize.Height - PaddleHeight)
                _leftPaddle.Y += 6;

            /* AI paddle (simple follow) */
            if (_aiMode)
            {
                int paddleCenter = _rightPaddle.Y + PaddleHeight / 2;
                if (paddleCenter < _ball.Y)        _rightPaddle.Y += 5;
                else if (paddleCenter > _ball.Y)   _rightPaddle.Y -= 5;

                _rightPaddle.Y = Math.Clamp(_rightPaddle.Y, 0, ClientSize.Height - PaddleHeight);
            }

            /* move ball */
            _ball.X += (int)_ballVelX;
            _ball.Y += (int)_ballVelY;

            /* bounce off top/bottom */
            if (_ball.Y <= 0 || _ball.Y >= ClientSize.Height - BallSize)
                _ballVelY = -_ballVelY;

            /* bounce off paddles */
            if (_ball.IntersectsWith(_leftPaddle)  && _ballVelX < 0) _ballVelX = -_ballVelX;
            if (_ball.IntersectsWith(_rightPaddle) && _ballVelX > 0) _ballVelX = -_ballVelX;

            /* scoring */
            if (_ball.X < 0)                       { _rightScore++; CheckWin(); }
            else if (_ball.X > ClientSize.Width - BallSize) { _leftScore++;  CheckWin(); }

            Invalidate(); // request repaint
        }

        /*───────────────── countdown ─────────────────*/
        private void StartCountdown()
        {
            ResetBall();
            _gameTimer.Stop();

            _countdown       = 3;
            _isCountingDown  = true;
            _countdownTimer.Start();
            Invalidate();
        }

        private void CountdownTick(object? sender, EventArgs e)
        {
            _countdown--;
            if (_countdown <= 0)
            {
                _countdownTimer.Stop();
                _isCountingDown = false;
                _gameTimer.Start();
            }
            Invalidate();
        }

        /*───────────────── scoring / win ─────────────────*/
        private void CheckWin()
        {
            if (_leftScore >= WinningScore)
                EndGame("Left Player Wins!");
            else if (_rightScore >= WinningScore)
                EndGame(_aiMode ? "AI Wins!" : "Right Player Wins!");
            else
                StartCountdown(); // next serve
        }

        private void EndGame(string winner)
        {
            _gameTimer.Stop();
            _countdownTimer.Stop();
            _isCountingDown = false;

            _gameOver    = true;
            _winnerText  = winner;
            Invalidate();
        }

        /*───────────────── utility ─────────────────*/
        private void ResetBall()
        {
            _ball.X = (ClientSize.Width  - BallSize) / 2;
            _ball.Y = (ClientSize.Height - BallSize) / 2;

            _ballVelX = _rand.Next(0, 2) == 0 ? 4 : -4;
            _ballVelY = _rand.Next(0, 2) == 0 ? 4 : -4;
        }

        private void ResetPositions()
        {
            _leftPaddle.Y  = (ClientSize.Height - PaddleHeight) / 2;
            _rightPaddle.X = ClientSize.Width - 30 - PaddleWidth;
            _rightPaddle.Y = (ClientSize.Height - PaddleHeight) / 2;

            if (!_gameOver) StartCountdown();
        }

        /*───────────────── drawing ─────────────────*/
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            /* paddles & ball */
            g.FillRectangle(Brushes.White, _leftPaddle);
            g.FillRectangle(Brushes.White, _rightPaddle);
            g.FillEllipse  (Brushes.White, _ball);

            /* dashed mid-line */
            using var pen = new Pen(Color.Gray) { DashPattern = new float[] { 4, 4 } };
            g.DrawLine(pen, ClientSize.Width / 2, 0, ClientSize.Width / 2, ClientSize.Height);

            /* score */
            string scoreText = $"{_leftScore}   {_rightScore}";
            var sf = new StringFormat { Alignment = StringAlignment.Center };
            g.DrawString(scoreText, Font, Brushes.White, ClientSize.Width / 2, 20, sf);

            /* countdown number */
            if (_isCountingDown && _countdown > 0)
            {
                string txt   = _countdown.ToString();
                using var f  = new Font(Font.FontFamily, 48, FontStyle.Bold);
                SizeF  size  = g.MeasureString(txt, f);
                g.DrawString(txt, f, Brushes.White,
                             (ClientSize.Width - size.Width) / 2,
                             (ClientSize.Height - size.Height) / 2);
            }

            /* winner overlay */
            if (_gameOver)
            {
                using var big = new Font(Font.FontFamily, 36, FontStyle.Bold);
                SizeF winSize = g.MeasureString(_winnerText, big);
                g.DrawString(_winnerText, big, Brushes.Yellow,
                             (ClientSize.Width - winSize.Width) / 2,
                             (ClientSize.Height - winSize.Height) / 2 - 30);

                const string prompt = "Press SPACE to Restart";
                using var small = new Font(Font.FontFamily, 18, FontStyle.Regular);
                SizeF pSize = g.MeasureString(prompt, small);
                g.DrawString(prompt, small, Brushes.Gray,
                             (ClientSize.Width - pSize.Width) / 2,
                             (ClientSize.Height + winSize.Height) / 2);
            }
        }

        /*───────────────── input ─────────────────*/
        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) _wDown = true;
            if (e.KeyCode == Keys.S) _sDown = true;

            /* restart after game over */
            if (e.KeyCode == Keys.Space && _gameOver)
            {
                _leftScore  = 0;
                _rightScore = 0;
                _gameOver   = false;
                _winnerText = "";
                ResetPositions();
            }
        }

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) _wDown = false;
            if (e.KeyCode == Keys.S) _sDown = false;
        }
    }
}
