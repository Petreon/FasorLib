using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML;

namespace Fasor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //RC serie

            double[] frequency = { 100, 500, 800, 1000, 1200, 2000, 5000, 10000 };
            Fasor tensionFont  = new Fasor(5,0); //5V amplitute
            Fasor Resistency1 = new Fasor (68e3,0); //68k
            double Capacitance = 0.0022e-6; // 2,2nF

            //first calculate the impedance of XC
            List<Fasor> Xc = new List<Fasor>(); //capacitance

            for(int i = 0; i < frequency.Length; i++)
            {
                Fasor bufferXC = new Fasor(1/(2 * 3.14 * frequency[i] * Capacitance), -90);
                Xc.Add(bufferXC);
            }

            //list of tension divisor
            List<Fasor> Vc = new List<Fasor>();
            for(int i = 0; i < Xc.Count; i++)
            {
                Fasor bufferVc = Fasor.Tension_Divisor(Xc[i], Resistency1, tensionFont);
                Vc.Add(bufferVc);
            }

            List<Fasor> VR = new List<Fasor>();
            for (int i = 0; i < Xc.Count; i++)
            {
                Fasor bufferVR = Fasor.Tension_Divisor(Resistency1, Xc[i], tensionFont);
                VR.Add(bufferVR);
            }

            // printing the Fasors
            for(int i = 0; i < Xc.Count; i++)
            {
                Console.WriteLine($"Freq = {frequency[i]}, VC: {Vc[i].FasorString("V")} || VR: {VR[i].FasorString("V")}");
            }
            Console.WriteLine("");


            for (int i = 0;i < frequency.Length; i++)
            {

                Console.WriteLine($"{frequency[i]}Hz -> Passa baixa(Capacitor) Decibel = {Math.Round(Decibel_RC_Capacitor(Resistency1.real, Capacitance, frequency[i]),2)}dB");
                Console.WriteLine($"{frequency[i]}Hz -> Passa Alta(Resistor) Decibel = {Math.Round(Decibel_RC_Resistor(Resistency1.real, Capacitance, frequency[i]),2)}dB");
                Console.WriteLine("");
            }

            //EXCEL PART

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sample Sheet");
                
                for(int i = 0;i < frequency.Length; i++)
                {
                    worksheet.Cell($"A{i+1}").Value = frequency[i];
                    worksheet.Cell($"B{i+1}").Value = Math.Round(Vc[i].module,2);
                    worksheet.Cell($"C{i+1}").Value = Math.Round(Vc[i].fase_degree,2);
                    worksheet.Cell($"D{i+1}").Value = Math.Round(VR[i].module,2);
                    worksheet.Cell($"E{i+1}").Value = Math.Round(VR[i].fase_degree, 2);
                    worksheet.Cell($"F{i+1}").Value = Math.Round(Decibel_RC_Capacitor(Resistency1.real, Capacitance, frequency[i]), 2);
                    worksheet.Cell($"G{i+1}").Value = Math.Round(Decibel_RC_Resistor(Resistency1.real, Capacitance, frequency[i]), 2);

                }

                workbook.SaveAs("Teste.xlsx");
            }


                return;
        }

        static double Decibel_RC_Capacitor(double ResistencyValue,double CapacitorValue,double frequencyValue )
        {
            //the H(s) = 1/(jwRC + 1) 
            //module = pow(wRC,2) + pow(wRC,2)
            double rads = 2 * Math.PI * frequencyValue;
            double constant = ResistencyValue * CapacitorValue * rads;

            double module = Math.Sqrt(Math.Pow(constant, 2) + Math.Pow(1, 2));

            return -( 20 * Math.Log10(module));

        }

        static double Decibel_RC_Resistor(double ResistencyValue, double CapacitorValue, double frequencyValue)
        {
            //passa alta
            //H(s) = RCjw / (RCjw + 1)
            // dB = 20log(w) - 20log(jw + 1/RC)

            double rads = 2 * Math.PI * frequencyValue;
            double constant = ResistencyValue * CapacitorValue * rads;

            double module = Math.Sqrt(Math.Pow(constant, 2) + Math.Pow(1, 2));

            return ( 20 * Math.Log10(constant) - 20 * Math.Log10(module));

        }

       
    }
}