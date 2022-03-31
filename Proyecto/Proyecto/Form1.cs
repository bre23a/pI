using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Video;

namespace Proyecto
{
    public partial class Form1 : Form
    {
        private bool Camaradetect;
        private FilterInfoCollection Camaras;
        private VideoCaptureDevice Camaraselect;

        private Bitmap original;
        private Bitmap resultante;
        private int ancho, alto;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button10_Click(object sender, EventArgs e)
        {
          
            if (openFileDialog1.ShowDialog()== DialogResult.OK)
            {
               
                pictureBox1.Image = (Bitmap)(Bitmap.FromFile(openFileDialog1.FileName));
                original = (Bitmap)(Bitmap.FromFile(openFileDialog1.FileName));

                this.Invalidate();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int x = 0;
            int y = 0;
            resultante = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height);
            Color rColor = new Color();
            Color gColor = new Color();

            for (x = 0; x < pictureBox1.Image.Width; x++)
            {
                for (y = 0; y < pictureBox1.Image.Height; y++)
                {
                    gColor = original.GetPixel(x, y);
                    rColor = Color.FromArgb(0, gColor.G, 0);
                    resultante.SetPixel(x, y, rColor);
                }
                pictureBox2.Image = resultante;
                
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            resultante = new Bitmap(227, 202);
            pictureBox2.Image = resultante;
            CargaCamaras();
        }
        public void CargaCamaras()
        {
            Camaras = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (Camaras.Count > 0)
            {
                Camaradetect= true;
                for (int i = 0; i < Camaras.Count; i++)
                    comboBox1.Items.Add(Camaras[i].Name.ToString());
                    comboBox1.Text = Camaras[0].Name.ToString();
            }
            else{
                Camaradetect=false;
            }
        }
        public void ApagarCamara()
        {
            if (Camaraselect!=null&& Camaraselect.IsRunning)
            {
                Camaraselect.SignalToStop();
                Camaraselect = null;
            }
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            ApagarCamara();
            int i=comboBox1.SelectedIndex;
            string nomCam = Camaras[i].MonikerString;
            Camaraselect=new VideoCaptureDevice(nomCam);
            Camaraselect.NewFrame += new NewFrameEventHandler(Send);
            Camaraselect.Start();

        }
        private void Send(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap imagen= (Bitmap)eventArgs.Frame.Clone();
            pictureBox3.Image = imagen;
        }
        

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {           
                ApagarCamara();           
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.Image.Save(saveFileDialog1.FileName + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetBrightness(50);
        }

        public void SetBrightness(int brightness)
        {
            Bitmap temp = (Bitmap)pictureBox1.Image;
            Bitmap bmap = (Bitmap)temp.Clone();
            if (brightness < -255) brightness = -255;
            if (brightness > 255) brightness = 255;
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    int cR = c.R + brightness;
                    int cG = c.G + brightness;
                    int cB = c.B + brightness;

                    if (cR < 0) cR = 1;
                    if (cR > 255) cR = 255;

                    if (cG < 0) cG = 1;
                    if (cG > 255) cG = 255;

                    if (cB < 0) cB = 1;
                    if (cB > 255) cB = 255;

                    bmap.SetPixel(i, j,
        Color.FromArgb((byte)cR, (byte)cG, (byte)cB));
                }
            }
            pictureBox2.Image = (Bitmap)bmap.Clone();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SetInvert();
        }
        public void SetInvert()
        {
            Bitmap temp = (Bitmap)pictureBox1.Image;
            Bitmap bmap = (Bitmap)temp.Clone();
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    bmap.SetPixel(i, j,
      Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                }
            }
            pictureBox2.Image = (Bitmap)bmap.Clone();
        }
        public void SetGrayscale()
        {
            Bitmap temp = (Bitmap)pictureBox1.Image;
            Bitmap bmap = (Bitmap)temp.Clone();
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    byte gray = (byte)(.299 * c.R + .587 * c.G + .114 * c.B);

                    bmap.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                }
            }
            pictureBox2.Image = (Bitmap)bmap.Clone();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int x = 0;
            int y = 0;
            int porcentaje = 25;
            int rangoMin = 90;
            int rangoMax = 255;
            
            Random random = new Random();
            Color rcolor;          
            resultante = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height);
            for (x = 0; x < pictureBox1.Image.Width; x++)
            {
                for (y = 0; y < pictureBox1.Image.Height; y++)
                {
                    if (random.Next(1, 100) <= porcentaje)
                    {
                        rcolor = Color.FromArgb(random.Next(rangoMin, rangoMax), random.Next(rangoMin, rangoMax), random.Next(rangoMin, rangoMax));
                    }
                    else
                    {
                        rcolor= original.GetPixel(x, y);
                    }
                    resultante.SetPixel(x, y, rcolor);  

                }
                pictureBox2.Image = resultante;
               
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            float r1 = 122;
            float g1 = 151;
            float b1 = 248;

            float r2 = 134;
            float g2 = 64;
            float b2 = 178;   

            int r = 0;
            int g = 0;
            int b = 0;

            float dr=(r2 - r1)/original.Width;
            float dg = (g2 - g1) / original.Width;
            float db = (b2 - b1) / original.Width;

            int x = 0;
            int y = 0;

            Color ocolor;
            button15_Click( sender, e);

            for (x = 0; x < pictureBox1.Image.Width; x++)
            {
                for (y = 0; y < pictureBox1.Image.Height; y++)
                {
                    ocolor = resultante.GetPixel(x, y);
                    r=(int)((r1/255.0f)*ocolor.R);
                    g = (int)((g1 / 255.0f * ocolor.G));
                    b = (int)((b1 / 255.0f) * ocolor.B);

                    if (r > 255) r = 255;
                    else if (r < 0) r = 0;

                    if (g > 255) g = 255;
                    else if (g < 0) g = 0;

                    if (b > 255) b = 255;
                    else if(b < 0) b = 0;

                    resultante.SetPixel(x,y,Color.FromArgb(r,g,b));
                }
                r1=(r1 + dr);
                g1=(g1+ dg);
                b1=(b1 + db);           

            }
            pictureBox2.Image = resultante;

        }

        private void button14_Click(object sender, EventArgs e)
        {
            SetGrayscale();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            int x = 0;
            int y= 0;
            resultante = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height);
            Color rcolor = new Color();
            Color gcolor = new Color();
            float g = 0;
            for (x = 0; x < pictureBox1.Image.Width; x++)
            {
                for (y = 0; y < pictureBox1.Image.Height; y++)
                {
                    gcolor = original.GetPixel(x, y);
                    g= gcolor.R * 0.2116f + gcolor.G * 0.7152f + gcolor.B * 0.0722f;
                    rcolor = Color.FromArgb((int)g, (int)g, (int)g);
                    resultante.SetPixel(x, y, rcolor);

                }


            }
            pictureBox2.Image = resultante;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (resultante != null)
            {
                Graphics g= e.Graphics;
                AutoScrollMinSize = new Size(ancho, alto);
                g.DrawImage(resultante, new Rectangle(  this.AutoScrollPosition.X,
                                                        this.AutoScrollPosition.Y +30 ,
                                                        ancho, alto));
                g.Dispose();

            }
        }
    }
}
