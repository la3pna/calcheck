using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using ZedGraph;
using System.Numerics;


namespace calcheck
{
    public partial class Form1 : Form
    {

        string def;
        int vectorLength;
        string freqBase;
        string paramtype;
        string measure;
        string Z0;
        Complex[] s11;
        Complex[] s12;
        Complex[] s21;
        Complex[] s22;
        double[] freq;
        double[] dataArray;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label4.Text = "";
           
            label14.Text = "";
            label15.Text = "";
            label16.Text = "";

        }

        private void loadSparameterToolStripMenuItem_Click(object sender, EventArgs e) 
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string line;
                    int i = 0;
                    List<double> freql = new List<double>();
                    List<Complex> s11a = new List<Complex>();
                    List<Complex> s21a = new List<Complex>();
                    List<Complex> s22a = new List<Complex>();
                    List<Complex> s12a = new List<Complex>();

                    System.IO.StreamReader sr = new
                       System.IO.StreamReader(openFileDialog1.FileName);
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.ToLowerInvariant().Contains('!'))
                        {
                        }
                        else if (line.ToLowerInvariant().Contains('#'))
                        {
                            def = line;
                            string[] defstrn = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            freqBase = defstrn[1];
                            paramtype = defstrn[2];
                            measure = defstrn[3];
                            Z0 = defstrn[5];

                            label4.Text = Z0;
                            label14.Text = freqBase;

                            if (paramtype != "S")
                            {
                               // label2.Text = "PARAMETER NOT S-PARAMETERS!";
                            }

                        }

                        else
                        {

                            Complex s11b;
                            Complex s21b;
                            Complex s12b;
                            Complex s22b;

                            string[] servalspl = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            string freqStrn = servalspl[0];
                            string s11reStrn = servalspl[1];
                            string s11imagStrn = servalspl[2];
                            string s21reStrn = servalspl[3];
                            string s21imagStrn = servalspl[4];
                            string s22reStrn = servalspl[7];
                            string s22imagStrn = servalspl[8];
                            string s12reStrn = servalspl[5];
                            string s12imagStrn = servalspl[6];

                            float freqa = Convert.ToSingle(freqStrn, CultureInfo.InvariantCulture);

                            float s11re = Convert.ToSingle(s11reStrn, CultureInfo.InvariantCulture);
                            float s11imag = Convert.ToSingle(s11imagStrn, CultureInfo.InvariantCulture);
                            float s21re = Convert.ToSingle(s21reStrn, CultureInfo.InvariantCulture);
                            float s21imag = Convert.ToSingle(s21imagStrn, CultureInfo.InvariantCulture);
                            float s22re = Convert.ToSingle(s22reStrn, CultureInfo.InvariantCulture);
                            float s22imag = Convert.ToSingle(s22imagStrn, CultureInfo.InvariantCulture);
                            float s12re = Convert.ToSingle(s12reStrn, CultureInfo.InvariantCulture);
                            float s12imag = Convert.ToSingle(s12imagStrn, CultureInfo.InvariantCulture);

                            if (measure == "MA")
                            {
                                //convert to cartesian
                                s11b = Complex.FromPolarCoordinates(s11re, s11imag * Math.PI / 180);
                                s12b = Complex.FromPolarCoordinates(s12re, s12imag * Math.PI / 180);
                                s21b = Complex.FromPolarCoordinates(s21re, s21imag * Math.PI / 180);
                                s22b = Complex.FromPolarCoordinates(s22re, s22imag * Math.PI / 180);
                                //Complex c1 = Complex.FromPolarCoordinates(10, 45 * Math.PI / 180)
                            }

                            if (measure == "RI")
                            {
                                //leave as is
                                s11b = new Complex(s11re, s11imag);
                                s21b = new Complex(s21re, s21imag);
                                s22b = new Complex(s22re, s22imag);
                                s12b = new Complex(s12re, s12imag);
                            }

                            else //(measure == "DB")
                            {  
                                // 10^db/20 for each real part. then got polar.
                                s11b = Complex.FromPolarCoordinates( Math.Pow(10, (s11re/20)), s11imag * Math.PI / 180);
                                s12b = Complex.FromPolarCoordinates( Math.Pow(10, (s12re/20)), s12imag * Math.PI / 180);
                                s21b = Complex.FromPolarCoordinates( Math.Pow(10, (s21re/20)), s21imag * Math.PI / 180);
                                s22b = Complex.FromPolarCoordinates( Math.Pow(10, (s22re/20)), s22imag * Math.PI / 180);
                            }

                            freql.Add(freqa);
                            s11a.Add(s11b);
                            s12a.Add(s12b);
                            s21a.Add(s21b);
                            s22a.Add(s22b);
                            i = i + 1;

                        }
                        vectorLength = i;
                    }
                    sr.Close();
                    freq = freql.ToArray();
                    s11 = s11a.ToArray();
                    s21 = s21a.ToArray();
                    s12 = s12a.ToArray();
                    s22 = s22a.ToArray();
                }
                calculate();
                }
            
            catch (Exception ex)
            {
             // label2.Text= ("The file could not be read:" + ex.Message);
               
            }
     }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void calculate()
        {
            
            List<double> e = new List<double>();

            for (int i = 0; i < vectorLength ; i++)
            {


           // abs(s11*conj(s21)+s12*conj(s22))/(sqrt((1-abs(s11)^2-abs(s12)^2)*(1-abs(s21)^2-abs(s22)^2)))
          
           Complex a = Complex.Abs(Complex.Add(Complex.Multiply(s11[i], Complex.Conjugate(s21[i])), Complex.Multiply(s12[i], Complex.Conjugate(s22[i]))));
          //A works
          Complex b = Complex.Subtract(Complex.Subtract(1,Complex.Pow(Complex.Abs(s11[i]), 2.00)), Complex.Pow(Complex.Abs(s12[i]), 2.00));
           Complex c = Complex.Subtract(Complex.Subtract(1,Complex.Pow(Complex.Abs(s21[i]), 2.00)), Complex.Pow(Complex.Abs(s22[i]), 2.00));
           Complex d = Complex.Divide(a, Complex.Sqrt( Complex.Multiply(b,c)));

           // the simplified code for an reciprocal device works:

            //abs(s11*conj(s21) + s21*conj(s11)) / (1 - abs(s11)^2 - abs(s21)^2)
            //    Complex a = Complex.Abs(Complex.Add(Complex.Multiply(s11[i], Complex.Conjugate(s21[i])),Complex.Multiply(s21[i], Complex.Conjugate(s11[i]))));
             //   Complex b = Complex.Subtract(Complex.Subtract(1.0, Complex.Pow(Complex.Abs(s11[i]),2.0)), Complex.Pow(Complex.Abs(s21[i]),2.0));
             //   Complex d = Complex.Divide(a, b);

                double f = (double)( d.Magnitude );

            
                e.Add(f);               
            }

           dataArray = e.ToArray();

            //abs(s11*conj(s21)+s12*conj(s22))/(sqrt((1-abs(s11)^2-abs(s12)^2)*(1-abs(s21)^2-abs(s22)^2)))

           plotdata();

            label15.Text = Convert.ToString(string.Format("{0:0.00}", (dataArray.Max() * 100) - 100));
            label16.Text = Convert.ToString(string.Format("{0:0.00}",(100-(dataArray.Min()*100))));


        }

        private void saveOpenTcheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
           //
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Comma separated (*.csv)|*.csv";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    var extension = Path.GetExtension(saveFileDialog1.FileName);
                    switch (extension.ToLower())
                    {
                         case ".csv":
                         StreamWriter wText = new StreamWriter(myStream);
                         wText.WriteLine("OpenT-Check ");
                         int length = dataArray.Length;
                         for (int i = 0; i <= length - 1; i++)
                            {
                                wText.WriteLine(Convert.ToString(freq[i], CultureInfo.InvariantCulture ) + ',' + Convert.ToString(dataArray[i], CultureInfo.InvariantCulture ));
                            }
                        wText.Flush();
                        wText.Close();
                        break;
                        default:
                        throw new ArgumentOutOfRangeException(extension);
                    }
                }
                // Bitmap bmp = new Bitmap(panel1.Width, panel1.Height);
                // panel1.DrawToBitmap(bmp, panel1.Bounds);
                // bmp.Save(@"C:\Temp\Test.bmp");
            }
        }

        private void plotdata() 
        {
            // This is to remove all plots
            zedGraphControl1.GraphPane.CurveList.Clear();

            // GraphPane object holds one or more Curve objects (or plots)
            GraphPane myPane = zedGraphControl1.GraphPane;

            // PointPairList holds the data for plotting, X and Y arrays 
            PointPairList spl1 = new PointPairList(freq, dataArray);
         //   PointPairList spl2 = new PointPairList(x, z);

            // Add cruves to myPane object
            LineItem myCurve1 = myPane.AddCurve("T-check 1", spl1, Color.Blue, SymbolType.None);
          //  LineItem myCurve2 = myPane.AddCurve("Cosine Wave", spl2, Color.Red, SymbolType.None);

            myCurve1.Line.Width = 3.0F;
          //  myCurve2.Line.Width = 3.0F;
            myPane.Title.Text = "T-check";
            myPane.YAxis.Title.Text = "Error";
            myPane.XAxis.Title.Text = "Frequency";

            // I add all three functions just to be sure it refeshes the plot.   
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();

        }
           
    }
}