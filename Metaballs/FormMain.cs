using MorphxLibs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Metaballs {
    public partial class FormMain : Form {
        private readonly List<Blob> blobs = new List<Blob>();
        private DirectBitmap bmp;
        private double w2;
        private double h2;
        private Color[] colors;

        public FormMain() {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);

            CreateColorsCache();
            CreateBitmap();
            CreateBlobs(6);

            Task.Run(() => {
                //Stopwatch sw = new Stopwatch();
                //sw.Start();

                while(true) {
                    this.Invalidate();
                    Thread.Sleep(30);

                    //sw.Restart();
                    Render();
                    //try {
                    //    this.Invoke((MethodInvoker)delegate {
                    //        this.Text = sw.ElapsedMilliseconds.ToString();
                    //    });
                    //} catch { }
                }
            });

            this.SizeChanged += (_, __) => CreateBitmap();
        }

        private void CreateColorsCache() {
            HLSRGB hls = new HLSRGB(0.0, 0.5, 1.0);
            colors = new Color[360];
            for(hls.Hue = 0; hls.Hue < 360; hls.Hue++)
                colors[(int)hls.Hue] = hls.Color;
        }

        private void CreateBitmap() {
            bmp?.Dispose();

            bmp = new DirectBitmap(this.DisplayRectangle.Width, this.DisplayRectangle.Height);
            w2 = bmp.Width / 2;
            h2 = bmp.Height / 2;
        }

        private void CreateBlobs(int n) {
            Random rnd = new Random();
            for(int i = 0; i < n; i++) {
                blobs.Add(new Blob(rnd.NextDouble() * bmp.Width,
                                   rnd.NextDouble() * bmp.Height,
                                   50 + rnd.NextDouble() * Math.Max(bmp.Width, bmp.Height) / 4,
                                   new Vector(rnd.NextDouble() * 10,
                                              rnd.NextDouble() * 10)));
            }
        }

        private void Render() {
            double s;
            int bc = blobs.Count;
            int x, y, i;
            int bw = bmp.Width;
            int bh = bmp.Height;
            //HLSRGB color = new HLSRGB(0.0, 0.5, 1.0);

            for(y = 0; y < bh; y++) {
                for(x = 0; x < bw; x++) {
                    s = 0;
                    for(i = 0; i < bc; i++) {
                        s += 50 * blobs[i].Radius / Vector.Distance(x, y, blobs[i].Position.X1, blobs[i].Position.Y1);
                    }
                    // Better color degradation, but slower
                    //color.Hue = s > 359 ? 359 : s;
                    //bmp.SetPixelFast(x, y, color.Color);

                    // Faster (color banding)
                    bmp.SetPixelFast(x, y, colors[s > 359 ? 359 : (int)s]);
                }
            }
            blobs.ForEach(b => b.Move(bmp.Width, bmp.Height));
        }

        protected override void OnPaintBackground(PaintEventArgs e) {
        }

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.CompositingMode = CompositingMode.SourceCopy;
            e.Graphics.DrawImageUnscaled(bmp.Bitmap, 0, 0);
        }
    }
}