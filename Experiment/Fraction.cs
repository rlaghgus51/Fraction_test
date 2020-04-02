using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment
{
    class Fraction
    {
        private int num;
        public int Num { get { return num; } set { num = value; } }
        private int den;
        public int Den { get { return den; } set { den = value; } }

        public Fraction(int num, int den)
        {
            this.Num = num;
            this.Den = den;
        }

        public Fraction(int num)
        {
            this.Num = num;
            this.Den = 1;
        }

        public Fraction(Fraction original)
        {
            this.Num = original.Num;
            this.Den = original.Den;
        }

        public override string ToString()
        {
            if (Num == 0)
                return "0";
            if (Den == 1)
                return Num.ToString();
            return Num.ToString() + "\nㅡ\n" + Den.ToString();
        }

        public Fraction Clone()
        {
            return new Fraction(this);
        }


        //기약분수를 리턴한다.
        public Fraction Irreducible()
        {
            int g = gcd(Num, Den);
            return new Fraction(Num / g, Den / g);
        }

        //최대공약수를 구한다.
        private int gcd(int a, int b)
        {
            if (b == 0)
                return a;
            return gcd(b, a % b);
        }

        // overload operator +
        public static Fraction operator +(Fraction a, Fraction b)
        {
            return new Fraction(checked(a.Num * b.Den + b.Num * a.Den), checked(a.Den * b.Den));
        }

        // overload operator -
        public static Fraction operator -(Fraction a, Fraction b)
        {
            return new Fraction(checked(a.Num * b.Den - b.Num * a.Den), checked(a.Den * b.Den));
        }

        // overload operator *
        public static Fraction operator *(Fraction a, Fraction b)
        {
            return new Fraction(checked(a.Num * b.Num), checked(a.Den * b.Den));
        }

        // overload operator /
        public static Fraction operator /(Fraction a, Fraction b)
        {
            return new Fraction(checked(a.Num * b.Den), checked(a.Den * b.Num));
        }

        // user-defined conversion from Fraction to double
        public static implicit operator double(Fraction f)
        {
            return (double)f.Num / f.Den;
        }

        // 비교 오퍼레이터 추가**
        public static bool operator >(Fraction a, Fraction b)
        {
            if (((double)a.Num / a.Den) > ((double)b.Num / b.Den))
            {
                return true;
            }
            else
                return false;
        }

        public static bool operator <(Fraction a, Fraction b)
        {
            if (((double)a.Num / a.Den) < ((double)b.Num / b.Den))
            {
                return true;
            }
            else
                return false;
        }
    }
}
