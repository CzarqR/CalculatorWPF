using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

/// <summary>
/// CO JEST ŹLE
/// Global resources do ktorych mozna sie odwolywac zamiast kolorów w kodzie w kazdym miejscu
/// przekazywanie stringow do funkcji z klasy Calculus jest głupie, mozna pewnie zrobic to lepiej
/// metoda tryparse zapisuje duze cyfry w postaci E+XX co powoduje ze działania na duzych liczbach sa niedokladne
/// </summary>

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private bool toClear = true;
        private readonly ushort minWidth = 8;
        public const int buttonClickShowTime = 165;//ms 

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdateText(string s)
        {
            if (toClear) //lbl start operation or after invalid eq
            {
                lblCurr.Content = s;
                toClear = false;
            }
            else
            {
                lblCurr.Content += s;
            }

        }

        private void Calculate()
        {
            string currEq = lblCurr.Content.ToString();
            string formatedEQ = string.Empty, result = currEq;
            if (currEq.Length > 1)
            {
                if (Calculus.CalculaceCalculator(ref result, ref formatedEQ)) //OK
                {
                    lblCurr.Content = result;
                    lblEq.Content = formatedEQ;
                    brdEq.BorderBrush = new SolidColorBrush(Color.FromRgb(239, 201, 175));

                }
                else //WRONG
                {
                    brdEq.BorderBrush = Brushes.Red;
                    toClear = false;
                    lblCurr.Content = currEq;
                    lblEq.Content = result;
                }

            }
            else if (currEq.Length == 1)
            {
                Regex regex = new Regex(@"\d");
                if (regex.IsMatch(currEq))
                {

                    lblCurr.Content = result;
                    brdEq.BorderBrush = new SolidColorBrush(Color.FromRgb(239, 201, 175));
                    lblEq.Content = currEq;
                }
            }
            SetSize();
        }

        private void Del()
        {
            if (lblCurr.Content.ToString().Length != 0)
            {
                lblCurr.Content = lblCurr.Content.ToString().Substring(0, lblCurr.Content.ToString().Length - 1);
            }
        }

        private void ShowClick(char x)
        {
            string c = x.ToString();
            foreach (Button btn in grdButtons.Children)
            {
                if (btn.Content.Equals(c))
                {
                    BtnClick(btn);
                    break;
                }

            }
        }

        private async void BtnClick(Button btn)
        {
            btn.Background = new SolidColorBrush(Color.FromRgb(16, 76, 145));
            await Task.Delay(buttonClickShowTime);
            btn.Background = new SolidColorBrush(Color.FromRgb(31, 138, 192));
        }


        private void BtnTextInput(object sender, RoutedEventArgs e)
        {
            UpdateText((sender as Button).Content.ToString());
            SetSize();

        }

        private void BtnEquals(object sender, RoutedEventArgs e)
        {
            Calculate();
            SetSize();
        }

        private void BtnClear(object sender, RoutedEventArgs e)
        {
            lblCurr.Content = string.Empty;
            SetSize();
        }

        private void BtnDel(object sender, RoutedEventArgs e)
        {
            Del();
            SetSize();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                BtnClick(btnDel);
                Del();
                SetSize();

            }
            else if (e.Key == Key.Enter)
            {
                ShowClick('=');
                Calculate();
            }
        }

        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {
            char keyChar;
            try
            {
                keyChar = (char)Encoding.ASCII.GetBytes(e.Text)[0];
            }
            catch (Exception)
            {
                return;
            }


            if ("0123456789,%*^/+-()".Contains(keyChar)) //operator or number
            {
                ShowClick(keyChar);
                UpdateText(keyChar.ToString());
                SetSize();
            }
            else if (keyChar.Equals('c') || keyChar.Equals('C')) //C - clear
            {
                BtnClick(btnClear);
                lblCurr.Content = string.Empty;
                SetSize();
            }
            else if (keyChar.Equals('='))
            {
                Calculate();
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            lblEq.Width = lblCurr.Width = this.Width - 10;
            SetSize();

        }

        private void SetSize()
        {
            double w = MeasureString(lblCurr);
            lblCurr.FontSize = (w >= minWidth ? w : minWidth);
            w = MeasureString(lblEq, 14);
            lblEq.FontSize = (w >= minWidth ? w : minWidth);

        }



        private double MeasureString(Label lbl, double fs = 26)
        {
            var ft = new FormattedText(
                lbl.Content.ToString(),
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(lbl.FontFamily, lbl.FontStyle, lbl.FontWeight, lbl.FontStretch),
                fs,
                Brushes.Black,
                new NumberSubstitution(),
                1);
            while (ft.Width > lbl.Width && fs > minWidth)
            {
                fs -= 0.1;
                ft = new FormattedText(
                lbl.Content.ToString(),
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(lbl.FontFamily, lbl.FontStyle, lbl.FontWeight, lbl.FontStretch),
                fs,
                Brushes.Black,
                new NumberSubstitution(),
                1);
            }
            return fs - 1.5;

        }
    }
}
