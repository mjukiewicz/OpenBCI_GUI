using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBCI_GUI
{
    class Filters
    {
        static double[,] prev_x_notch = new double[8, 5];
        static double[,] prev_y_notch = new double[8, 5];
        static double[,] prev_x_standard = new double[8, 5];
        static double[,] prev_y_standard = new double[8, 5];

        public static double FiltersSelect(int standard, int notch, double data, int nrk)
        {
            double[] b;
            double[] a;
            double[] b2;
            double[] a2;

            if (standard == 1) //1-50Hz
            {
                b = new double[5] { 0.2001387256580675, 0, -0.4002774513161350, 0, 0.2001387256580675 };
                a = new double[5] { 1, -2.355934631131582, 1.941257088655214, -0.7847063755334187, 0.1999076052968340 };
            }
            else if (standard == 2) //7-13Hz
            {
                b = new double[5] { 0.005129268366104263, 0, -0.01025853673220853, 0, 0.005129268366104263 };
                a = new double[5] { 1, -3.678895469764040, 5.179700413522124, -3.305801890016702, 0.8079495914209149 };
            }
            else if (standard == 3) //15-50Hz
            {
                b = new double[5] { 0.1173510367246093, 0, -0.2347020734492186, 0, 0.1173510367246093 };
                a = new double[5] { 1, -2.137430180172061, 2.038578008108517, -1.070144399200925, 0.2946365275879138 };
            }
            else if (standard == 4) //5-50Hz
            {
                b = new double[5] { 0.1750876436721012, 0, -0.3501752873442023, 0, 0.1750876436721012 };
                a = new double[5] { 1, -2.299055356038497, 1.967497759984450, -0.8748055564494800, 0.2196539839136946 };
            }
            else
            {
                b = new double[5] { 1, 1, 1, 1, 1 };
                a = new double[5] { 1, 1, 1, 1, 1 };
            }

            if (notch == 1) // 50Hz
            {
                b2 = new double[5] { 0.96508099, -1.19328255, 2.29902305, -1.19328255, 0.96508099 };
                a2 = new double[5] { 1, -1.21449347931898, 2.29780334191380, -1.17207162934772, 0.931381682126902 };
            }

            else if (notch == 2) // 60Hz
            {
                b2 = new double[5] { 0.9650809863447347, -0.2424683201757643, 1.945391494128786, -0.2424683201757643, 0.9650809863447347 };
                a2 = new double[5] { 1, -0.2467782611297853, 1.944171784691352, -0.2381583792217435, 0.9313816821269039 };
            }
            else 
            {
                b2 = new double[5] { 1, 1, 1, 1, 1 };
                a2 = new double[5] { 1, 1, 1, 1, 1 };
            }
            double result = filterIIR(a2, b2, a, b, data, nrk);
            return result;
        }

        private static double filterIIR(double[] a2, double[] b2, double[] a, double[] b, double data, int nrk)
        {
            for (int j = 5 - 1; j > 0; j--)
            {
                prev_x_notch[nrk, j] = prev_x_notch[nrk, j - 1];
                prev_y_notch[nrk, j] = prev_y_notch[nrk, j - 1];
                prev_x_standard[nrk, j] = prev_x_standard[nrk, j - 1];
                prev_y_standard[nrk, j] = prev_y_standard[nrk, j - 1];
            }
            prev_x_notch[nrk, 0] = data;

            double wynik = 0;
            for (int j = 0; j < 5; j++)
            {
                wynik += b2[j] * prev_x_notch[nrk, j];
                if (j > 0)
                {
                    wynik -= a2[j] * prev_y_notch[nrk, j];
                }
            }
            prev_y_notch[nrk, 0] = wynik;
            prev_x_standard[nrk, 0] = wynik;
            wynik = 0;
            for (int j = 0; j < 5; j++)
            {
                wynik += b[j] * prev_x_standard[nrk, j];
                if (j > 0)
                {
                    wynik -= a[j] * prev_y_standard[nrk, j];
                }
            }
            prev_y_standard[nrk, 0] = wynik;
            return wynik;
        }
    }
}
