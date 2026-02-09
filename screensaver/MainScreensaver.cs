using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
namespace screensaver
{ 
    /// <summary>
    /// Главная форма скринсейвера "Снегопад"
    /// </summary>
    public partial class MainScreensaver : Form
    {
        private Bitmap backgroundBitmap;
        private Bitmap overlayBitmap;
        private Bitmap bufferBitmap;
        private Graphics bufferGraphics;


        private const int MinSnowflakeSize = 18;
        private const int MaxSnowflakeSize = 40;
        private const float MinSnowflakeSpeed = 4.0f;
        private const float MaxSnowflakeSpeed = 12.0f;
        private const int CountSnowflake = 120;
        private float deltaTime;
        private DateTime lastFrameTime;
        private Random random = new Random();
        private System.Windows.Forms.Timer animationTimer;

        private List<Snowflake> snowflakes = new List<Snowflake>();

        /// <summary>
        /// Конструктор главной формы скринсейвера
        /// </summary>
        public MainScreensaver()
        {
            backgroundBitmap = new Bitmap(Properties.Resources.village);
            overlayBitmap = new Bitmap(Properties.Resources.snowflake);

            InitializeComponent();


            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            InitializeBuffer();

            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 40;
            animationTimer.Tick += AnimationTimer_Tick;

            this.Paint += Form1_Paint;
            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
            this.KeyDown += Form1_KeyDown;
            this.KeyPreview = true;
            this.Click += Form1_Click;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            animationTimer?.Stop();
            animationTimer?.Dispose();
            backgroundBitmap?.Dispose();
            overlayBitmap?.Dispose();
            bufferBitmap?.Dispose();
            bufferGraphics?.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lastFrameTime = DateTime.Now;
            

            DrawFrame();
            UpdateScreen();
            animationTimer.Start();
            this.Focus();
        
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(bufferBitmap, 0, 0);
        }

        
        private void InitializeBuffer()
        {
            bufferBitmap?.Dispose();
            bufferGraphics?.Dispose();

            bufferBitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
            bufferGraphics = Graphics.FromImage(bufferBitmap);

            bufferGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            bufferGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            bufferGraphics.Clear(Color.Black);

        }

        private void InitializeSnowflakes()
        {
            var width = ClientSize.Width;
            var height = ClientSize.Height;


            for (var flakecount = 0; flakecount < CountSnowflake; flakecount++)
            {
                var size = random.Next(MinSnowflakeSize, MaxSnowflakeSize + 1);
                var sizeRatio = (float)(size - MinSnowflakeSize) / (MaxSnowflakeSize - MinSnowflakeSize);
                var speed = MinSnowflakeSpeed + sizeRatio * (MaxSnowflakeSpeed - MinSnowflakeSpeed);
                var x = random.Next(0, width);
                var y = random.Next(-height * 3, -size);

                snowflakes.Add(new Snowflake
                {
                    X = x,
                    Y = y,
                    Size = size,
                    Speed = speed,

                });
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            var currenttime = DateTime.Now;
            deltaTime = (float)(currenttime - lastFrameTime).TotalSeconds;
            lastFrameTime = currenttime;

            if (deltaTime > 0.1f)
            {
                deltaTime = 0.1f;
            }
            var frameMultiplier = deltaTime * 60f;
            
            MoveSnowfleks(frameMultiplier);
            DrawFrame();
            UpdateScreen();

        }

        private void MoveSnowfleks(float frameMultiplier)
        {
            var height = ClientSize.Height;
            var width = ClientSize.Width;

            foreach (var flake in snowflakes)
            {
                flake.Y += flake.Speed * frameMultiplier;
                
                if (flake.Y > height + 100)
                {
                    flake.Y = random.Next(-height * 3, -(int)flake.Size);
                    flake.X = random.Next(0, width);
                }

            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if(ClientSize.Width > 1 && ClientSize.Height > 1)
            {
                InitializeSnowflakes();
                DrawFrame();
                UpdateScreen();
            }
        }

        private void DrawFrame()
        {
            bufferGraphics.Clear(Color.Black);
            if (backgroundBitmap  != null)
            {
                var width = ClientSize.Width;
                var height = ClientSize.Height;

                bufferGraphics.DrawImage(backgroundBitmap, 
                    new Rectangle(0, 0, ClientSize.Width, ClientSize.Height), 
                    new Rectangle(0, 0, backgroundBitmap.Width, backgroundBitmap.Height), 
                    GraphicsUnit.Pixel);
            }
            if (overlayBitmap != null)
            {
                int overlayWidth = overlayBitmap.Width;
                int overlayHeight = overlayBitmap.Height;
                foreach (var flake in snowflakes)
                {
                    var size = (int)flake.Size;
                    if (flake.Y + size > 0)
                    {
                        bufferGraphics.DrawImage(overlayBitmap,
                        new Rectangle((int)flake.X, (int)flake.Y, size, size),
                        new Rectangle(0, 0, overlayBitmap.Width, overlayBitmap.Height),
                        GraphicsUnit.Pixel);
                    }
                }
            }
        }

        private void UpdateScreen()
        {
            using (Graphics screenGraphics = this.CreateGraphics())
            {
                screenGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                screenGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                screenGraphics.DrawImage(bufferBitmap, 0, 0);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            InitializeBuffer();
            UpdateScreen();
        }

    }
}
