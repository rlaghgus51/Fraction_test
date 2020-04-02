using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment
{
    class Quiz
    {
        private Fraction frac1;
        private Fraction frac2;
        private bool isfirstbigger;

        public string FirstFraction  { get { return frac1.ToString(); } }
        public string SecondFraction { get { return frac2.ToString(); } }
        public bool isFirstBigger { get { return isfirstbigger; } set { isfirstbigger = value; } }

        public Quiz(int num1, int den1, int num2, int den2, Random rnd)
        {
            if (rnd.Next(0, 2) == 0)
            {
                this.frac1 = new Fraction(num1, den1);
                this.frac2 = new Fraction(num2, den2);
            }
            else
            {
                this.frac1 = new Fraction(num2, den2);
                this.frac2 = new Fraction(num1, den1);
            }

            if (frac1 > frac2)
                this.isFirstBigger = true;
            else
                this.isFirstBigger = false;
        }
    }
}
