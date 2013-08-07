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

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string line;           
                    int i = 0;
                    List<double> freq = new List<double>();
                    List<Complex> s11a = new List<Complex>();
                    List<Complex> s21a = new List<Complex>();
                    List<Complex> s22a = new List<Complex>();
                    List<Complex> s12a = new List<Complex>();
                
                  


                    System.IO.StreamReader sr = new
                       System.IO.StreamReader(openFileDialog1.FileName);
                    while ((line = sr.ReadLine()) != null)
                    {
                        textBox1.Text = line;

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

                            textBox5.Text = Z0; 

                        }
                        else
                        {

                            string[] servalspl = line.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );

                            string freqStrn = servalspl[0];
                            string s11reStrn = servalspl[1];
                            string s11imagStrn = servalspl[2];
                            string s21reStrn = servalspl[3];
                            string s21imagStrn = servalspl[4];
                            string s22reStrn = servalspl[5];
                            string s22imagStrn = servalspl[6];
                            string s12reStrn = servalspl[7];
                            string s12imagStrn = servalspl[8];

                            double freqa = Convert.ToDouble(freqStrn, CultureInfo.InvariantCulture);

                            float s11re = Convert.ToSingle(s11reStrn, CultureInfo.InvariantCulture);
                            float s11imag = Convert.ToSingle(s11imagStrn, CultureInfo.InvariantCulture);
                            float s21re = Convert.ToSingle(s11reStrn, CultureInfo.InvariantCulture);
                            float s21imag = Convert.ToSingle(s11imagStrn, CultureInfo.InvariantCulture);
                            float s22re = Convert.ToSingle(s11reStrn, CultureInfo.InvariantCulture);
                            float s22imag = Convert.ToSingle(s11imagStrn, CultureInfo.InvariantCulture);
                            float s12re = Convert.ToSingle(s11reStrn, CultureInfo.InvariantCulture);
                            float s12imag = Convert.ToSingle(s11imagStrn, CultureInfo.InvariantCulture);
                           
                            
                            freq.Add(freqa);

                            Complex s11b = new Complex(s11re, s11imag);
                            Complex s21b = new Complex(s21re, s21imag);
                            Complex s22b = new Complex(s22re, s22imag);
                            Complex s12b = new Complex(s12re, s12imag);

                            s11a.Add(s11b);
                            s12a.Add(s12b);
                            s21a.Add(s21b);
                            s22a.Add(s22b);
                            i = i + 1;
                        }
                        vectorLength = i;
                        
                    }
                   
                    sr.Close();

                    Complex[] s11 = s11a.ToArray();
                    Complex[] s21 = s21a.ToArray();
                    Complex[] s12 = s12a.ToArray();
                    Complex[] s22 = s22a.ToArray();
                    convertVector();
                }
            }
            catch (Exception ex)
            {
               textBox2.Text= ("The file could not be read:");
               textBox1.Text=(ex.Message);
            }

                    }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void convertVector()
        {
            if (paramtype != "S")
            {
                label2.Text = "PARAMETER NOT S-PARAMTERS!";
            }

            if (measure == "MA")
            {

            }

            if (measure == "RI")
            {
                
            }
            if (measure == "DB")
            {
               
                // 10^db/20 for each real part. then got polar.
            }


            /*
             * Complex s11b = new Complex(s11re, s11imag);
               Complex s21b = new Complex(s21re, s21imag);
               Complex s22b = new Complex(s22re, s22imag);
               Complex s12b = new Complex(s12re, s12imag);
             * 
             *  Complex[] s11 = s11a.ToArray();
                    Complex[] s21 = s21a.ToArray();
                    Complex[] s12 = s12a.ToArray();
                    Complex[] s22 = s22a.ToArray();
              
             */

        }

    }
}
