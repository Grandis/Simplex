using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Simplex
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        double L, x1, x2, x3, x4, x5, x6;
        int x, y;
        double[,] mainMatrix = null;
        

        private void button1_Click(object sender, EventArgs e)
        {
            simplexCount();
        }

        public void simplexCount()
        {
            // исходное уравнение
            L = 6 * x1 + 5 * x2 + 9 * x3;

            x = Convert.ToInt32(boundsNumber.Text) + 1;
            y = x + Convert.ToInt32(variablesNumber.Text);

            /*
              {5, 2, 3, 1, 0, 0,10},
              {1, 6, 2, 0, 1, 0, 20},
              {4, 0, 3, 0, 0, 1, 18},
             */

            double[,] firstMatrix = 
            {
                {Convert.ToDouble(textBox4.Text), Convert.ToDouble(textBox5.Text), Convert.ToDouble(textBox6.Text), 1, 0, 0,Convert.ToDouble(textBox7.Text)},
                {Convert.ToDouble(textBox8.Text), Convert.ToDouble(textBox9.Text), Convert.ToDouble(textBox10.Text), 0, 1, 0, Convert.ToDouble(textBox11.Text)},
                {Convert.ToDouble(textBox12.Text), Convert.ToDouble(textBox13.Text), Convert.ToDouble(textBox14.Text), 0, 0, 1, Convert.ToDouble(textBox15.Text)},
                {-Convert.ToDouble(textBox1.Text), -Convert.ToDouble(textBox2.Text), -Convert.ToDouble(textBox3.Text), 0, 0, 0, 0}
            };

            // Значение функции L для начального решения
            //double[] xBegin = { 0, 0, 0, 25, 20, 18 };

            if (mainMatrix != null) firstMatrix = mainMatrix;

            // наименьший элемент в L строке
            double minCollumn = firstMatrix[3, 0];
            int minCollumnIndex = 0;
            for (int j = 0; j < 7; j++)
            {
                if (firstMatrix[3, j] < minCollumn)
                {
                    minCollumn = firstMatrix[3, j];
                    minCollumnIndex = j;
                }
            }
            //***************

            // наименьший эллемент в найденном столбце
            int minRowIndex = 0;
            double minRow = firstMatrix[0, 6] / firstMatrix[0, minCollumnIndex];
            for (int i = 0; i < 3; i++)
            {
                if (firstMatrix[i, 6] / firstMatrix[i, minCollumnIndex] < minRow)
                {
                    minRow = firstMatrix[i, 6] / firstMatrix[i, minCollumnIndex];
                    minRowIndex = i;
                }
            }
            //********************
            double minRowCol = firstMatrix[minRowIndex, minCollumnIndex];
            //MessageBox.Show(minRowCol.ToString());

            // Делим элементы строки
            for (int j = 0; j < 7; j++)
            {
                firstMatrix[minRowIndex, j] /= minRowCol;
            }
            //*******************
            //MessageBox.Show(minRowIndex.ToString());

            for (int i = 0; i < 4; i++)
            {
                if (i != minRowIndex)
                {
                    double tempMinCol = firstMatrix[i, minCollumnIndex];
                    for (int j = 0; j < 7; j++)
                    {
                        firstMatrix[i, j] -= firstMatrix[minRowIndex, j] * tempMinCol;
                    }
                }
            }

            bool check = true;
            for (int j = 0; j < y - 1; j++)
            {
                if (firstMatrix[x - 1, j] < 0) check = false;
            }

            if (check)
            {
                L = firstMatrix[x - 1, y - 1];
                MessageBox.Show("Целевая функция равна " + L.ToString());
            }

            else
            {
                mainMatrix = firstMatrix;
                simplexCount();
            }
        }
    }
}
