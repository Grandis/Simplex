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
        int variables = 0;
        int bounds = 0;
        String[] eqData = { "<=", "=>" };
        double[,] mainMatrix = null;
        
        private void button1_Click(object sender, EventArgs e)
        {
            mainMatrix = null;
            simplexCount();
        }

        public void simplexCount()
        {
            
            // исходное уравнение
            //L = 6 * x1 + 5 * x2 + 9 * x3;
            /*
              {5, 2, 3, 1, 0, 0,10},
              {1, 6, 2, 0, 1, 0, 20},
              {4, 0, 3, 0, 0, 1, 18},
            */
            // Узнаём необходимые размеры массива и создаем его.
            variables = Convert.ToInt32(variablesNumber.Text);
            bounds = Convert.ToInt32(boundsNumber.Text);
            x = bounds + 1;
            y = x + variables;
            double[,] firstMatrix = new double[x, y];

            // Добавляем в массив коэфициенты переменных ограничений.
            for (int i = 0; i < bounds; i++)
            {
                for (int j = 0; j < variables; j++)
                {
                    firstMatrix[i, j] = Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value);
                }
            }

            // Добавляем в массив единицы базисных переменных
            for (int i = 0; i < bounds; i++)
            {
                firstMatrix[i, variables + i] = 1;
            }

            // Добавляем в массив ограничения.
            for (int i = 0; i < bounds; i++)
            {
                firstMatrix[i, y - 1] = Convert.ToDouble(dataGridView1.Rows[i].Cells[dataGridView1.Columns.Count-1].Value);
            }

            // Добавляем в массив исходное уравнение (с обратными знаками!).
            for (int j = 0; j < variables; j++)
            {
                firstMatrix[x-1, j] = Convert.ToDouble(dataGridView2.Rows[0].Cells[j].Value) * -1;
            }

            String matrix = "";
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                    matrix += firstMatrix[i, j] + " ";
                matrix += "\n";
            }
            MessageBox.Show(matrix);

//////////////////////////////////////////////////////////////////////////////////////////////
            
            // Значение функции L для начального решения
            //double[] xBegin = { 0, 0, 0, 25, 20, 18 };

            if (mainMatrix != null) firstMatrix = mainMatrix;

            // наименьший элемент в L строке
            double minCollumn = firstMatrix[bounds, 0];
            int minCollumnIndex = 0;
            for (int j = 0; j < variables; j++)
            {
                if (firstMatrix[bounds, j] < minCollumn)
                {
                    minCollumn = firstMatrix[bounds, j];
                    minCollumnIndex = j;
                }
            }

            // наименьший эллемент в найденном столбце
            int minRowIndex = 0;
            double minRow = Double.MaxValue;//firstMatrix[0, y-1] / firstMatrix[0, minCollumnIndex];
            for (int i = 0; i < bounds; i++)
            {
                if (firstMatrix[i, minCollumnIndex] > 0 && firstMatrix[i, y - 1] / firstMatrix[i, minCollumnIndex] < minRow)
                {
                    minRow = firstMatrix[i, y-1] / firstMatrix[i, minCollumnIndex];
                    minRowIndex = i;
                }
            }

            double minRowCol = firstMatrix[minRowIndex, minCollumnIndex];

            // Делим элементы строки
            for (int j = 0; j < y; j++)
            {
                firstMatrix[minRowIndex, j] /= minRowCol;
            }

            // Считаем
            for (int i = 0; i < x; i++)
            {
                if (i != minRowIndex)
                {
                    double tempMinCol = firstMatrix[i, minCollumnIndex];
                    for (int j = 0; j < y; j++)
                    {
                        firstMatrix[i, j] -= firstMatrix[minRowIndex, j] * tempMinCol;
                    }
                }
            }

            // Проверяем, все ли элементы функции L положительны.
            bool check = true;
            for (int j = 0; j < y - 1; j++)
            {
                if (firstMatrix[x - 1, j] < 0) check = false;
            }

            // Если да, то задача решена.
            if (check)
            {
                L = firstMatrix[x - 1, y - 1];
                MessageBox.Show("Целевая функция равна " + L.ToString());
            }

            // Если нет, то повторяем функцию simplexCount().
            else
            {
                mainMatrix = firstMatrix;
                simplexCount();
            }
             
        }

        // Указываем количество переменных.
        private void variablesNumber_TextChanged(object sender, EventArgs e)
        {
            DataGridViewComboBoxColumn cbEqual = new DataGridViewComboBoxColumn();
            cbEqual.DataSource = eqData;

            if (variablesNumber.Text != "" && Convert.ToInt32(variablesNumber.Text) > 0)
            {
                dataGridView1.Columns.Clear();
                dataGridView2.Columns.Clear();
                
                for (int i = 0; i < Convert.ToInt32(variablesNumber.Text); i++)
                {
                    string name = "x" + (i + 1);
                    dataGridView1.Columns.Add(name, name);
                    dataGridView2.Columns.Add(name, name);
                    dataGridView2.Rows.Clear();
                    dataGridView2.Rows.Add(1);
                }
                dataGridView1.Columns.Add(cbEqual);
                dataGridView1.Columns.Add("result", "");
            }
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].Width = 50;
            }
            for (int i = 0; i < dataGridView2.Columns.Count; i++)
            {
                dataGridView1.Columns[i].Width = 50;
            }
        }

        // Указываем количество ограничений.
        private void boundsNumber_TextChanged(object sender, EventArgs e)
        {
            try
                {
                    if (variablesNumber.Text != "" && Convert.ToInt32(variablesNumber.Text) > 0 && boundsNumber.Text != "" && Convert.ToInt32(boundsNumber.Text) > 0)
                    {
                            dataGridView1.Rows.Clear();
                            dataGridView1.Rows.Add(Convert.ToInt32(boundsNumber.Text));
                    }
                }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
