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
using Emgu.CV;
using Emgu.CV.Structure;
using AForge.Vision.Motion;


using HaarCascadeClassifer;
namespace Proyecto
{
    public partial class Form1 : Form
    {
        static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
       

        VideoCapture capture;
        bool reproduciendo=false;
        int framestotales;
        int frameactualnum;
        Mat frameactual;
        int fps;

        FilterInfoCollection dispositivos;         
        MotionDetector detector;        
        float deteccion;

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
            using (OpenFileDialog img = new OpenFileDialog() { Multiselect = false, Filter = "JPEG |*.jpg" })
            if (img.ShowDialog()== DialogResult.OK)
            {
               
                pictureBox1.Image = (Bitmap)(Bitmap.FromFile(img.FileName));
                original = (Bitmap)(Bitmap.FromFile(img.FileName));

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
     
            detector = new MotionDetector(new TwoFramesDifferenceDetector(), new MotionBorderHighlighting(Color.Blue));
            deteccion = 0;
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
        

        private void button9_Click_1(object sender, EventArgs e)
        {

            int i = comboBox1.SelectedIndex;
            string nomCam = Camaras[i].MonikerString;
            Camaraselect = new VideoCaptureDevice(nomCam);
            Camaraselect.NewFrame += new NewFrameEventHandler(Send);
            Camaraselect.Start();

            /* using (openFileDialog1 = new OpenFileDialog())
             Camaraselect = new VideoCaptureDevice(Camaras[comboBox1.SelectedIndex].MonikerString);
             videoSourcePlayer1.VideoSource = Camaraselect;
             videoSourcePlayer1.Start();
            */
        }

        private void Send(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap imagen = (Bitmap)eventArgs.Frame.Clone();
            Image<Rgb, byte> grayImage = new Image<Rgb, byte>(imagen);
            Rectangle[] rectangles = cascadeClassifier.DetectMultiScale(grayImage, 1.4, 0);

            foreach (Rectangle rectangle in rectangles)
            {
                using (Graphics graphics = Graphics.FromImage(imagen))
                {

                    using (Pen lapiz = new Pen(color: Color.Red, 5))
                    {
                        graphics.DrawRectangle(lapiz, rectangle);
                    }
                }
            }
            pictureBox3.Image = imagen;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Camaraselect.SignalToStop();
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

                    bmap.SetPixel(i, j, Color.FromArgb((byte)cR, (byte)cG, (byte)cB));
                }
            }
            pictureBox2.Image = (Bitmap)bmap.Clone();
        }

        public void SetBrightnessvideo(int brightness)
        {
            Bitmap temp = (Bitmap)pictureBox1.Image;
            Bitmap bmap = (Bitmap)temp.Clone();
            unsafe
            {

                System.Drawing.Imaging.BitmapData bitmapData =
                bmap.LockBits(new Rectangle(0, 0, bmap.Width, bmap.Height),System.Drawing.Imaging.ImageLockMode.ReadWrite, bmap.PixelFormat);
                int bytesporpixel = System.Drawing.Bitmap.GetPixelFormatSize(bmap.PixelFormat)/8;
                int alturadelpixel = bitmapData.Height;
                int anchodelpixel = bitmapData.Width * bytesporpixel;
                byte*ptrPrimerPixel =(byte*)bitmapData.Scan0;
                if (brightness < -255) brightness = -255;
                if (brightness > 255) brightness = 255;
                Color c;

                Parallel.For(0, alturadelpixel, y =>
                  {
                      byte* lineaactual = ptrPrimerPixel + (y * bitmapData.Stride);

                      for (int i = 0; i < anchodelpixel; i = i + bytesporpixel)
                      {
                         
                          int azul = lineaactual[i];
                          int verde = lineaactual[i + 1];
                          int rojo = lineaactual[i + 2];

                          
                          azul = azul + brightness;
                          verde = verde + brightness;
                          rojo = rojo + brightness;

                          if (azul < 0) azul = 1;
                          if (azul > 255) azul = 255;

                          if (verde < 0) verde = 1;
                          if (verde > 255) verde = 255;

                          if (rojo < 0) rojo = 1;
                          if (rojo > 255) rojo = 255;

                          lineaactual[i] = (byte)azul;
                          lineaactual[i + 1] = (byte)verde;
                          lineaactual[i + 2] = (byte)rojo;

                      }
                  });       
                bmap.UnlockBits(bitmapData);
                pictureBox2.Image = (Bitmap)bmap.Clone();
            }
            
          
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
        public void gradientevideo()
        {
         
           
            Bitmap temp = (Bitmap)pictureBox1.Image;
            Bitmap bmap = (Bitmap)temp.Clone();
            grisvideo();
            unsafe
            {
               

                System.Drawing.Imaging.BitmapData bitmapData =
                bmap.LockBits(new Rectangle(0, 0, bmap.Width, bmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmap.PixelFormat);
                int bytesporpixel = System.Drawing.Bitmap.GetPixelFormatSize(bmap.PixelFormat) / 8;
                int alturadelpixel = bitmapData.Height;
                int anchodelpixel = bitmapData.Width * bytesporpixel;
                byte* ptrPrimerPixel = (byte*)bitmapData.Scan0;
                float r1 = 122;
                float g1 = 151;
                float b1 = 248;

                float r2 = 134;
                float g2 = 64;
                float b2 = 178;

                int r = 0;
                int g = 0;
                int b = 0;

                float dr = (r2 - r1) / anchodelpixel;
                float dg = (g2 - g1) / anchodelpixel;
                float db = (b2 - b1) / anchodelpixel;


                Parallel.For(0, alturadelpixel, y =>
                {
                    byte* lineaactual = ptrPrimerPixel + (y * bitmapData.Stride);
                  
                    for (int i = 0; i < anchodelpixel; i = i + bytesporpixel)
                    {
                       
                        int azul = lineaactual[i];
                        int verde = lineaactual[i + 1];
                        int rojo = lineaactual[i + 2];

                        r = (int)((r1 / 255.0f) * azul);
                        g = (int)((g1 / 255.0f * verde));
                        b = (int)((b1 / 255.0f) * rojo);

                        if (r > 255) r = 255;
                        else if (r < 0) r = 0;

                        if (g > 255) g = 255;
                        else if (g < 0) g = 0;

                        if (b > 255) b = 255;
                        else if (b < 0) b = 0;                     

                        lineaactual[i] = (byte)azul;
                        lineaactual[i + 1] = (byte)verde;
                        lineaactual[i + 2] = (byte)rojo;

                    }
                    r1 = (r1 + dr);
                    g1 = (g1 + dg);
                    b1 = (b1 + db);
                });
                bmap.UnlockBits(bitmapData);
                pictureBox2.Image = (Bitmap)bmap.Clone();
            }


        }

        private void button14_Click(object sender, EventArgs e)
        {
            SetBrightnessvideo(50);
            

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

        private void button16_Click(object sender, EventArgs e)
        {
            upvideo();

        }
        private void upvideo()
        {

            using (OpenFileDialog hi = new OpenFileDialog() { Multiselect = false, Filter = "MP4 |*.mp4" })
             
            if (hi.ShowDialog() == DialogResult.OK)
            {

                capture = new VideoCapture(hi.FileName);
                framestotales = Convert.ToInt32(capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameCount));
                fps = Convert.ToInt32(capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps));
                reproduciendo = true;
                frameactual = new Mat();
                frameactualnum = 0;
                reproducirvideos();
            }
          
        }
        private async void reproducirvideos()
        {
            if (capture == null)
            {
                return;
            }
            try { 
                while (reproduciendo==true && frameactualnum< framestotales)
                {
                    capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.PosFrames, frameactualnum);
                    capture.Read(frameactual);
                    pictureBox1.Image = frameactual.Bitmap;
                    frameactualnum += 1;
                    await Task.Delay(1000/fps);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
          

            Bitmap temp = (Bitmap)pictureBox1.Image;
            Bitmap bmap = (Bitmap)temp.Clone();
            unsafe
            {

                System.Drawing.Imaging.BitmapData bitmapData =
                bmap.LockBits(new Rectangle(0, 0, bmap.Width, bmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmap.PixelFormat);
                int bytesporpixel = System.Drawing.Bitmap.GetPixelFormatSize(bmap.PixelFormat) / 8;
                int alturadelpixel = bitmapData.Height;
                int anchodelpixel = bitmapData.Width * bytesporpixel;
                byte* ptrPrimerPixel = (byte*)bitmapData.Scan0;
              
                Color c;

                Parallel.For(0, alturadelpixel, y =>
                {
                    byte* lineaactual = ptrPrimerPixel + (y * bitmapData.Stride);

                    for (int i = 0; i < anchodelpixel; i = i + bytesporpixel)
                    {

                        int azul = lineaactual[i];
                        int verde = lineaactual[i + 1];
                        int rojo = lineaactual[i + 2];                  
                                             

                        lineaactual[i] = 0;
                        lineaactual[i + 1] = (byte)verde;
                        lineaactual[i + 2] = 0;

                    }
                });
                bmap.UnlockBits(bitmapData);
                pictureBox2.Image = (Bitmap)bmap.Clone();
            }



        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            SetInvertVideo();
        }
        public void SetInvertVideo()
        {
            Bitmap temp = (Bitmap)pictureBox1.Image;
            Bitmap bmap = (Bitmap)temp.Clone();
            unsafe
            {

                System.Drawing.Imaging.BitmapData bitmapData =
                bmap.LockBits(new Rectangle(0, 0, bmap.Width, bmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmap.PixelFormat);
                int bytesporpixel = System.Drawing.Bitmap.GetPixelFormatSize(bmap.PixelFormat) / 8;
                int alturadelpixel = bitmapData.Height;
                int anchodelpixel = bitmapData.Width * bytesporpixel;
                byte* ptrPrimerPixel = (byte*)bitmapData.Scan0;


                Parallel.For(0, alturadelpixel, y =>
                {
                    byte* lineaactual = ptrPrimerPixel + (y * bitmapData.Stride);

                    for (int i = 0; i < anchodelpixel; i = i + bytesporpixel)
                    {

                        int azul = lineaactual[i];
                        int verde = lineaactual[i + 1];
                        int rojo = lineaactual[i + 2];
                        azul = 255-azul;
                        verde =255- verde ;
                        rojo = 255-rojo;

                        lineaactual[i] = (byte)azul;
                        lineaactual[i + 1] = (byte)verde;
                        lineaactual[i + 2] = (byte)rojo;

                    }
                });
                bmap.UnlockBits(bitmapData);
                pictureBox2.Image = (Bitmap)bmap.Clone();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ruidovideo();

            
        }
        public void ruidovideo()
        {
                /**/
            Bitmap temp = (Bitmap)pictureBox1.Image;
            Bitmap bmap = (Bitmap)temp.Clone();
     
            int porcentaje = 5;
            int rangoMin = 85;
            int rangoMax = 115;
            float pbrillo = 0;
            Random random = new Random();
           int r = 0;
            int g = 0;
            int b = 0;
           
            unsafe
            {

                System.Drawing.Imaging.BitmapData bitmapData =
                bmap.LockBits(new Rectangle(0, 0, bmap.Width, bmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmap.PixelFormat);
                int bytesporpixel = System.Drawing.Bitmap.GetPixelFormatSize(bmap.PixelFormat) / 8;
                int alturadelpixel = bitmapData.Height;
                int anchodelpixel = bitmapData.Width * bytesporpixel;
                byte* ptrPrimerPixel = (byte*)bitmapData.Scan0;
                Parallel.For(0, alturadelpixel, y =>
                {
                    byte* lineaactual = ptrPrimerPixel + (y * bitmapData.Stride);

                    for (int i = 0; i < anchodelpixel; i = i + bytesporpixel)
                    {

                        int azul = lineaactual[i];
                        int verde = lineaactual[i + 1];
                        int rojo = lineaactual[i + 2];                     

                        if (random.Next(1, 100) <= porcentaje)
                        {
                            /*   azul= random.Next(rangoMin, rangoMax);
                               verde=random.Next(rangoMin, rangoMax);
                               rojo = random.Next(rangoMin, rangoMax);*/
                            pbrillo = random.Next(rangoMin, rangoMax) / 100.0f;
                            r = (int)(azul * pbrillo);
                            g = (int)(verde * pbrillo);
                            b = (int)(rojo * pbrillo);

                            if(r>255)r = 255;
                            else if(r<0)r = 0;
                            if(g>255)g=255;
                            else if(g<0)g=0;
                            if(b>255)b=255;
                            else if (b<0)b=0;


                        }
                     
                        lineaactual[i] = (byte)r;
                        lineaactual[i + 1] = (byte)g;
                        lineaactual[i + 2] = (byte)rojo;
                    }
                });
                bmap.UnlockBits(bitmapData);
                pictureBox2.Image = (Bitmap)bmap.Clone();
            }
        }

        private void button7_Click_1(object sender, EventArgs e)
        {

         
            gradientevideo();
        }
        public void grisvideo()
        {
           
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Bitmap temp = (Bitmap)pictureBox1.Image;
            Bitmap bmap = (Bitmap)temp.Clone();

            unsafe
            {

                System.Drawing.Imaging.BitmapData bitmapData =
                bmap.LockBits(new Rectangle(0, 0, bmap.Width, bmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmap.PixelFormat);
                int bytesporpixel = System.Drawing.Bitmap.GetPixelFormatSize(bmap.PixelFormat) / 8;
                int alturadelpixel = bitmapData.Height;
                int anchodelpixel = bitmapData.Width * bytesporpixel;
                byte* ptrPrimerPixel = (byte*)bitmapData.Scan0;



                Random random = new Random();
                float g = 0;

                Parallel.For(0, alturadelpixel, y =>
                {
                    byte* lineaactual = ptrPrimerPixel + (y * bitmapData.Stride);

                    for (int i = 0; i < anchodelpixel; i = i + bytesporpixel)
                    {

                        int azul = lineaactual[i];
                        int verde = lineaactual[i + 1];
                        int rojo = lineaactual[i + 2];

                        g = azul * 0.2116f + verde * 0.7152f + rojo * 0.0722f;

                        azul = (int)g;
                        verde = (int)g;
                        rojo = (int)g;


                        lineaactual[i] = (byte)azul;
                        lineaactual[i + 1] = (byte)verde;
                        lineaactual[i + 2] = (byte)rojo;



                    }

                });
                bmap.UnlockBits(bitmapData);
                pictureBox2.Image = (Bitmap)bmap.Clone();
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            Camaraselect.SignalToStop();
            pictureBox3.Image.Dispose();
            pictureBox3.Image = null;
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            pictureBox2.Image.Dispose();
            pictureBox2.Image = null;
            pictureBox1.Image.Dispose();
            pictureBox1.Image = null;
        }
    
        private void videoSourcePlayer1_Click(object sender, EventArgs e)
        {

        }

        private void videoSourcePlayer1_NewFrame_1(object sender, ref Bitmap image)
        {
            deteccion = detector.ProcessFrame(image);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofg = new OpenFileDialog())
            {
                if (ofg.ShowDialog() == DialogResult.OK)
                {
                
                    /**/
                    pictureBox1.Image=Image.FromFile(ofg.FileName);
                    Bitmap bitmap=new Bitmap (pictureBox1.Image);
                    Image<Rgb,byte> grayImage= new Image<Rgb, byte> (bitmap);
                    Rectangle[] rectangles =  cascadeClassifier.DetectMultiScale(grayImage, 1.4, 0);

                    foreach (Rectangle rectangle in rectangles)
                    {
                        using(Graphics graphics = Graphics.FromImage(bitmap))
                        {
                            using(Pen lapiz=new Pen (color:Color.Red,5))
                            {
                                graphics.DrawRectangle(lapiz, rectangle);
                            }
                        }
                    }
                    pictureBox3.Image = bitmap;
                }
            }
        }

        private void pictureBox3_Click_1(object sender, EventArgs e)
        {

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
