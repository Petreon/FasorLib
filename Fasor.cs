using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fasor
{
    internal class Fasor
    {
        #region Properties
        public double module { get; private set; }
        public double fase_degree { get; private set; }
        public double fase_radian { get; private set; }
        public double real { get; private set; }
        public double imagy { get; private set; }

        #endregion

        public Fasor(double module, double fase) {
            this.module = module;
            this.fase_degree = fase;

            this.fase_radian = Fasor.Degree_to_Radian(this.fase_degree);

            update_rectangular();
        }

        #region Public_Static

        static public Fasor Sum(Fasor num1, Fasor num2)
        {
            double new_real = num1.real + num2.real;
            double new_imagy = num1.imagy + num2.imagy;

            double real_imagy_ele = Math.Pow(new_real,2) + Math.Pow(new_imagy,2);

            double module = Math.Sqrt(real_imagy_ele);
            double fase = Math.Atan2(new_imagy, new_real); //THIS FUNCTION RETURN IN RAD ATTENTION
            fase = Fasor.Radian_to_Degree(fase);

            Fasor sum = new Fasor(module, fase);

            return sum;
        }

        static public Fasor Minus(Fasor num1, Fasor num2)
        {
            double new_real = num1.real - num2.real;
            double new_imagy = num1.imagy - num2.imagy;

            double real_imagy_ele = Math.Pow(new_real, 2) + Math.Pow(new_imagy, 2);

            double module = Math.Sqrt(real_imagy_ele);
            double fase = Math.Atan2(new_imagy, new_real); //THIS FUNCTION RETURN IN RAD ATTENTION
            fase = Fasor.Radian_to_Degree(fase);


            Fasor sum = new Fasor(module, fase);

            return sum;
        }

        static public Fasor Multiply(Fasor num1, Fasor num2)
        {
            double new_module = num1.module * num2.module;
            double new_degree = num1.fase_degree + num2.fase_degree;

            return new Fasor(new_module, new_degree);

        }

        static public Fasor Divide(Fasor num1, Fasor num2)
        {
            double new_module = num1.module / num2.module;
            double new_degree = num1.fase_degree - num2.fase_degree;

            return new Fasor(new_module, new_degree);
        }

        public static double Degree_to_Radian(double degree)
        {
            return degree * (Math.PI / 180);
        }

        public static double Radian_to_Degree(double degree)
        {
            return degree * (180 / Math.PI);
        }

        public static Fasor Parallel(Fasor num1, Fasor num2)
        {
            Fasor upside = Fasor.Multiply(num1, num2);
            Fasor downside = Fasor.Sum(num1, num2);
            Fasor result = Fasor.Divide(upside, downside);

            return new Fasor(result.module, result.fase_degree);
        }

        public static Fasor Tension_Divisor(Fasor Z1, Fasor Z2, Fasor Tension)
        {

            Fasor upside = Z1;
            Fasor downside = Fasor.Sum(Z1, Z2);
            Fasor result1 = Fasor.Divide(upside, downside);
            Fasor result2 = Fasor.Multiply(result1, Tension);

            return new Fasor(result2.module, result2.fase_degree);

        }

        #endregion

        #region Public_Methods
        public void Print_all(string msg=null)
        {
            if (msg !=null)
            {
                Console.WriteLine($"{msg} Module: {this.module} , Fase: {this.fase_degree} // REC: Real: {this.real} , Imaginary:{this.imagy}");

            }else
            {
                Console.WriteLine($"Module: {this.module} , Fase: {this.fase_degree} // REC: Real: {this.real} , Imaginary:{this.imagy}");

            }
        }

        public void Print(string msg = null)
        {
            if (msg != null)
            {
                Console.WriteLine($"{msg} Module: {this.module} , Fase: {this.fase_degree}");

            }
            else
            {
                Console.WriteLine($"Module: {this.module} , Fase: {this.fase_degree}");

            }
        }

        public string FasorString(string unit = "")
        {
            return $"Module: {Math.Round(this.module,3).ToString()} {unit} , Fase: {Math.Round(this.fase_degree, 3).ToString()}°";
        }


        #endregion

        #region Private_Methods

        private void update_rectangular()
        {
            this.real = this.module * Math.Cos(this.fase_radian);
            this.imagy = this.module * Math.Sin(this.fase_radian);
        }

        #endregion

    }
}
