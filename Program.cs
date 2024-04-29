namespace Fasor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // constants
            Fasor R1 = new Fasor(2200, 0);
            Fasor R2 = new Fasor(1000, 0);
            Fasor Vt = new Fasor(3, 0);
            double[] frequency = { 100, 500, 1000, 2000, 10000 };
            double c11_constant = 0.01e-6;//microfarad
            double c15_constant = 0.05e-6;
            double c2_constant = 0.022e-6;



            List<Fasor> C11_list = new List<Fasor>();
            List<Fasor> C15_list = new List<Fasor>();
            List<Fasor> C2_list = new List<Fasor>();



            //initialize the capacitors
            for(int i = 0; i < 5; i++)
            {
                double XC11_absolute = 1 / (2 * Math.PI * frequency[i] * c11_constant);
                Fasor XC11 = new Fasor(XC11_absolute, -90);
                C11_list.Add(XC11);

                double XC15_absolute = 1 / (2 * Math.PI * frequency[i] * c15_constant);
                Fasor XC15 = new Fasor(XC15_absolute, -90);
                C15_list.Add(XC15);

                double XC2_absolute = 1 / (2 * Math.PI * frequency[i] * c2_constant);
                Fasor XC2 = new Fasor(XC2_absolute, -90);
                C2_list.Add(XC2);
            }

            List<Fasor> Z11Req_list = new List<Fasor>();
            List<Fasor> Z15Req_list = new List<Fasor>();
            List<Fasor> Z2Req_list = new List<Fasor>();

            for(int i = 0; i < 5;i++)
            {
                Fasor R1_C11 = Parallel(R1, C11_list[i]);
                //R1_C11.Print($"Z11 eq: {frequency[i]}Hz");
                Fasor R1_C15 = Parallel(R1, C15_list[i]);
                //R1_C15.Print($"Z15 eq: {frequency[i]}Hz");
                Fasor R2_C2 = Parallel(R2, C2_list[i]);
                //R2_C2.Print($"Z2 eq: {frequency[i]}Hz");

                Z11Req_list.Add(R1_C11);
                Z15Req_list.Add(R1_C15);
                Z2Req_list.Add(R2_C2);

            }

            List<Fasor> V0_list = new List<Fasor>();

            for(int i = 0; i < 5;i++)
            {
                Fasor Queda_V = Tension_Divisor(Z2Req_list[i], Z11Req_list[i], Vt);
                V0_list.Add(Queda_V);
                V0_list[i].Print($"V0 em C1 = 0.01uF; Frequencia = {frequency[i]}Hz");
            }

            for (int i = 0; i < 5; i++)
            {
                Fasor Queda_V = Tension_Divisor(Z2Req_list[i], Z15Req_list[i], Vt);
                V0_list.Add(Queda_V);
                V0_list[i+5].Print($"V0 em C1 = 0.05uF; Frequencia = {frequency[i]}Hz");

            }
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
    }
}