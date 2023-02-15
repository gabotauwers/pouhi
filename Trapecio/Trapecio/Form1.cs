using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Calculus;
using System.Windows.Forms.DataVisualization.Charting;

namespace Trapecio
{
    public partial class Form1 : Form
    {
        bool _mayorMenor;
        double a, b, h, res1, res2;
        int n, i = 0;
        string operacion, ecuacion;
        Calculo calcular = new Calculo();

        public Form1()
        {
            InitializeComponent();
        }
        public void Ecuacion()
        {
            ecuacion = "";
            operacion = txtEcuacion.Text;
            for (int k = 0; k < operacion.Length; k++)
            {
                if (k != 0)
                {
                    if (operacion[k] == 'x' && (operacion[k - 1] != '(' && operacion[k - 1] != '^'))
                    {
                        ecuacion += "*x";
                    }
                    else if (operacion[k] == 'x' && (operacion[k - 1] == '(' || operacion[k - 1] == '^'))
                        ecuacion += 'x';
                    else
                        ecuacion += operacion[k];
                }
                else
                {
                    if (operacion[k] == 'e')
                        ecuacion += "2.718281828";
                    else
                        ecuacion += operacion[k];
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Series();
            btnCalcular.Enabled = false;
        }
        public void Series()
        {
            _grafica.Series.Add("Series1");
            _grafica.Series["Series1"].ChartType = SeriesChartType.Line;
            _grafica.Series["Series1"].BorderWidth = 5;
            _grafica.Series["Series1"].Color = Color.Purple;
            _grafica.Series["Series1"].LegendText = "'f(x)'";

        }
        public bool tieneSolucion()
        {
            bool haySolucion = true;
            

            if (calcular.Sintaxis(ecuacion,'x'))
            {
                haySolucion = true;
            }
            else
            {
                MessageBox.Show("La ecuacion ingresada tiene un error en la sintaxis, ingresa una diferente.","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                txtEcuacion.Text = "";
                haySolucion = false;
            }
            return haySolucion;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult seleccion = MessageBox.Show("¿Está seguro que desea salir?", "Salir", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (seleccion != DialogResult.Yes)
            {
                e.Cancel = true;
                Focus();
            }
        }

        public void CalcularIntegral(double [] x, double[] fx)
        {
            res1 = h / 2;
            for (i = 0; i <= n; i++)
            {
                if (i == 0)
                {
                    res2 = fx[i];
                }
                else if (i == n)
                {
                    res2 += fx[i];
                }
                else
                {
                    res2 += (2 * fx[i]);
                }
            }
            res1 *= res2;
            lblResultado.Text = "El resultado es " + res1;
        } 
        public void BloquearTB()
        {
            txtEcuacion.Enabled = false;
            txtA.Enabled = false;
            txtB.Enabled = false;
            txtN.Enabled = false;
        }
        public void DesbloquearTB()
        {
            txtEcuacion.Enabled = true;
            txtA.Enabled = true;
            txtB.Enabled = true;
            txtN.Enabled = true;
        }
        public void AsignarValores()
        {
            Ecuacion();
            n = Convert.ToInt32(txtN.Text);
            a = Convert.ToDouble(txtA.Text);
            b = Convert.ToDouble(txtB.Text);
        }
        private void btnCalcular_Click(object sender, EventArgs e)
        {
            btnCalcular.Enabled = false;
            BloquearTB();
            try
            {
                AsignarValores();
                double[] x = new double[n + 1];
                double[] fx = new double[n + 1];
                if (tieneSolucion())
                {
                    if (bMayora())
                    {
                        Tabular(x, fx);
                        CalcularIntegral(x, fx);
                    }
                    else
                    {
                        MessageBox.Show("El rango introducido es incorrecto (B debe ser mayor que A)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        DesbloquearTB();
                        txtA.Text = "";
                        txtB.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("La ecuación tiene un error en la sintaxis, ingresa otra por favor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtEcuacion.Text = "";
                    DesbloquearTB();
                }
            }
            catch
            {
                MessageBox.Show("El sistema no tiene solución, ingrese nuevos valores","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                Limpiar();
            }
        }
        public bool bMayora()
        {
            if ((b > a))
                _mayorMenor = true;
            else
                _mayorMenor = false;
            return _mayorMenor;
        }
        public void Tabular(double[] x, double[] fx)
        {
            h = (b - a) / n;
            for (i = 0; i <= n; i++)
            {
                if (i == 0)
                    x[i] = a;
                else
                {
                    x[i] = x[i - 1] + h;
                }
                fx[i] = calcular.EvaluaFx(x[i]);
                _tabla.Rows.Add(i, x[i], fx[i]);
                _grafica.Series["Series1"].Points.AddXY(x[i], fx[i]);
            }
        }
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }
        public void Limpiar()
        {
            txtEcuacion.Text = "";
            txtN.Text = "";
            txtA.Text = "";
            txtB.Text = "";
            _tabla.Rows.Clear();
            _grafica.Series.Clear();
            Series();
            lblResultado.Text = "";
            txtEcuacion.Enabled = true;
            txtA.Enabled = true;
            txtB.Enabled = true;
            txtN.Enabled = true;
        }
        private void TextBoxes_TextChanged(object sender, EventArgs e)
        {
            bool cajaVacia = false;
            foreach (Control control in Controls)
            {
                if (control is TextBox)
                {
                    if (control.Text == String.Empty)
                        cajaVacia = true;
                }
            }

            if (cajaVacia)
                btnCalcular.Enabled = false;
            else
                btnCalcular.Enabled = true;
        }
        private void txtN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar))
                e.Handled = false;
            else
                e.Handled = true;
        }
        private void txtA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar) || e.KeyChar == '-' || e.KeyChar == '.')
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
