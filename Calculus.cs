using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Calculator
{
    class Calculus
    {
        public static bool CalculaceCalculator(ref string inEq, ref string formatedEQ)
        {
            if (NormalToReverse(ref inEq, ref formatedEQ))
            {
                if (Calculate(ref inEq))
                {
                    return true;
                }
                return false;

            }
            else
            {
                return false;
            }

        }


        private static bool NormalToReverse(ref string n, ref string reformatedStr)
        {
            n = n.Replace("()", "");
            if (n.Length == 0)
            {
                n = "Equation is in wrong format";
                return false;
            }
            if (n.Length > 1)
            {
                n = AddSpacesAfterOperator(n);
            }
            if (!ValidateCharacters(n))
            {
                n = "Equation contains invalid character";
                return false;
            }
            if (!IsEqBracketsCorrect(n))
            {
                n = "Brackets in equation are placed wrong";
                return false;
            }
            if (n[0].Equals('+'))
            {
                n = n.Remove(0, 2);
            }
            else if (n[0].Equals('-'))
            {
                n = n.Remove(1, 1);
            }
            n = n.Replace("( +", "( 0 +");
            n = n.Replace("( -", "( 0 -");
            reformatedStr = n;
            string[] s = n.Split(' ');
            if (!IsEqCorrect(s))
            {
                n = "Equation is in wrong format";
                return false;
            }

            Stack<char> stack = new Stack<char>();
            StringBuilder r = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if (double.TryParse(s[i], NumberStyles.Any, new CultureInfo("pl"), out _)) //number
                {
                    r.Append(s[i] + " ");
                }
                else //operator
                {
                    if (stack.Count == 0) //stack is empty
                    {
                        stack.Push(char.Parse(s[i]));
                    }
                    else //stack not empty
                    {
                        if (s[i].Equals(")")) //right bracket
                        {
                            while (!stack.Peek().Equals('('))
                            {
                                r.Append(stack.Pop() + " ");
                            }
                            stack.Pop();
                        }
                        else if (s[i].Equals("(")) //left bracket
                        {
                            stack.Push('(');
                        }
                        else
                        {
                            while (stack.Count > 0 && weight[stack.Peek()] >= weight[char.Parse(s[i])])
                            {
                                r.Append(stack.Pop() + " ");
                            }
                            stack.Push(char.Parse(s[i]));
                        }
                    }
                }
            }
            while (stack.Count > 0)
            {
                r.Append(stack.Pop() + " ");
            }
            n = r.ToString().Trim();
            return true;
        }

        private static bool Calculate(ref string r)
        {
            Stack<double> stack = new Stack<double>();
            string[] s = r.Split(' ');
            double a, b;
            for (int i = 0; i < s.Length; i++)
            {
                if (double.TryParse(s[i], NumberStyles.Number, new CultureInfo("pl"), out double c)) //number
                {
                    stack.Push(c);
                }
                else //operator
                {
                    b = stack.Pop();
                    a = stack.Pop();
                    switch (char.Parse(s[i]))
                    {
                        case '+':
                            stack.Push(a + b);
                            break;
                        case '-':
                            stack.Push(a - b);
                            break;
                        case '*':
                            stack.Push(a * b);
                            break;
                        case '/':
                            if (b == 0)
                            {
                                r = "You can't divide by 0";
                                return false;
                            }
                            stack.Push(a / b);
                            break;
                        case '^':
                            stack.Push(Math.Pow(a, b));
                            break;
                        case '%':
                            if (b == 0)
                            {
                                r = "You can't divide by 0";
                                return false;
                            }
                            stack.Push(a % b);
                            break;
                        default:
                            r = "Equation contains invalid character";
                            return false;

                    }

                }

            }
            r = string.Format("{0:F99}", stack.Pop()).TrimEnd('0').TrimEnd(',');
            return true;
        }
        private static bool IsEqBracketsCorrect(string x)
        {
            int s = 0;
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i].Equals('('))
                {
                    s++;
                }
                else if (x[i].Equals(')'))
                {
                    s--;
                }
                if (s < 0)
                {
                    return false;
                }
            }
            if (s != 0)
            {
                return false;
            }
            return true;
        }
        private static bool IsEqCorrect(string[] x, int min = 0, int max = 1)
        {

            int s = 0;
            foreach (string p in x)
            {
                if (double.TryParse(p, NumberStyles.Any, new CultureInfo("pl"), out _)) //number
                {
                    s++;
                }
                else if (p.Equals("(") || p.Equals(")")) //bracket
                {
                    continue;
                }
                else //operator
                {
                    s--;
                }

                if (s < min || s > max)
                {
                    return false;
                }

            }
            if (s != 1)
            {
                return false;
            }
            return true;
        }
        private static bool ValidateCharacters(string x)
        {
            Regex rgx = new Regex(@"^[0-9+/*% \-^(),]+$"); //every operator and number must be separated by a space
            return rgx.IsMatch(x);
        }
        private static string AddSpacesAfterOperator(string x)
        {
            StringBuilder s = new StringBuilder();
            switch (x[0])
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '%':
                case '(':
                case ')':
                case '^':
                    if (!x[1].Equals(' '))
                    {
                        s.Append(x[0] + " ");
                    }
                    else
                    {
                        s.Append(x[0]);
                    }
                    break;
                default:
                    s.Append(x[0]);
                    break;
            }
            int max = x.Length - 1;
            for (int i = 1; i < max; i++)
            {
                switch (x[i])
                {
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '%':
                    case '(':
                    case ')':
                    case '^':
                        if (!s[s.Length - 1].Equals(' '))
                        {
                            s.Append(" " + x[i]);
                        }
                        else
                        {
                            s.Append(x[i]);
                        }
                        if (!x[i + 1].Equals(' '))
                        {
                            s.Append(" ");
                        }
                        break;
                    default:
                        s.Append(x[i]);
                        break;
                }
            }
            switch (s[s.Length - 1])
            {
                case ' ':
                    s.Append(x[x.Length - 1]);
                    break;
                case '+':
                case '-':
                case '*':
                case '/':
                case '%':
                case '(':
                case ')':
                case '^':
                    s.Append(" " + x[x.Length - 1]);
                    break;
                default: //number
                    if (x[x.Length - 1].Equals(')'))
                    {
                        s.Append(" " + x[x.Length - 1]);
                    }
                    else
                    {
                        s.Append(x[x.Length - 1]);
                    }
                    break;
            }
            return s.ToString();
        }

        private static readonly Dictionary<char, byte> weight = new Dictionary<char, byte>
        {
                { '(', 0 },
                { ')', 1 },
                { '+', 1 },
                { '-', 1 },
                { '*', 2 },
                { '/', 2 },
                { '%', 2 },
                { '^', 3 }
        };

    }
}
