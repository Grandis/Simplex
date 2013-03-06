using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;

namespace Simplex
{
    public partial class Count : UserControl
    {
        private Microsoft.Office.Interop.Excel.Application objExcel;
        private Microsoft.Office.Interop.Excel.Workbook objWorkBook;
        private Microsoft.Office.Interop.Excel.Worksheet objWorkSheet;


        public Count()
        {
            InitializeComponent();
        }

        double L;
        int x, y, tempMinColumn = -1, tempMinRow = -1;
        int variables = 0;
        int bounds = 0;
        String[] eqData = { "<=", "=>" };
        double[,] mainMatrix = null;
        double[] xArray;
        bool check = true;

        private void button1_Click(object sender, EventArgs e)
        {
            tempMinColumn = -1;
            tempMinRow = -1;
            xArray = null;
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

            try
            {
                // Узнаём необходимые размеры массива и создаем его.
                variables = Convert.ToInt32(variablesNumber.Text);
                bounds = Convert.ToInt32(boundsNumber.Text);
                x = bounds + 1;
                y = x + variables;
                double[,] firstMatrix = new double[x, y];

                if (xArray == null) xArray = new double[bounds + variables];

                if (mainMatrix == null)
                {
                    // Добавляем в массив коэфициенты переменных ограничений.
                    for (int i = 0; i < bounds; i++)
                    {
                        for (int j = 0; j < variables; j++)
                        {
                            if (dataGridView1.Rows[i].Cells[variables].Value.ToString() == "<=")
                            {
                                firstMatrix[i, j] = Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value);
                            }
                            else
                            {
                                firstMatrix[i, j] = Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value) * -1;
                            }
                        }
                    }

                    // Добавляем в массив единицы базисных переменных
                    for (int i = 0; i < bounds; i++)
                    {
                        firstMatrix[i, variables + i] = 1;
                    }

                    // Добавляем в массив ограничения и заполняем массив переменных.
                    for (int i = 0; i < bounds; i++)
                    {
                        if (dataGridView1.Rows[i].Cells[variables].Value.ToString() == "<=")
                        {
                            firstMatrix[i, y - 1] = Convert.ToDouble(dataGridView1.Rows[i].Cells[dataGridView1.Columns.Count - 1].Value);
                        }
                        else
                        {
                            firstMatrix[i, y - 1] = Convert.ToDouble(dataGridView1.Rows[i].Cells[dataGridView1.Columns.Count - 1].Value) * -1;
                        }
                        xArray[variables + i] = Convert.ToDouble(dataGridView1.Rows[i].Cells[dataGridView1.Columns.Count - 1].Value);
                    }

                    // Добавляем в массив исходное уравнение (с обратными знаками!).
                    for (int j = 0; j < variables; j++)
                    {
                        if (comboBox1.SelectedItem.Equals("Min")) firstMatrix[x - 1, j] = Convert.ToDouble(dataGridView2.Rows[0].Cells[j].Value);
                        else firstMatrix[x - 1, j] = Convert.ToDouble(dataGridView2.Rows[0].Cells[j].Value) * -1;
                    }

                    String matrix = "Начальная матрица\n";
                    for (int i = 0; i < x; i++)
                    {
                        for (int j = 0; j < y; j++)
                            matrix += firstMatrix[i, j] + " ";
                        matrix += "\n";
                    }
                    MessageBox.Show(matrix);

                    // Проверяем, все ли элементы функции L положительны.
                    for (int j = 0; j < y - 1; j++)
                    {
                        if (firstMatrix[x - 1, j] < 0) check = false;
                    }
                    if (check)
                    {
                        MessageBox.Show("Целевая функция L = 0.");
                        return;
                    }
                }


    //////////////////////////////////////////////////////////////////////////////////////////////


                else firstMatrix = mainMatrix;

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
                        minRow = firstMatrix[i, y - 1] / firstMatrix[i, minCollumnIndex];
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

                //matrix = "";
                //for (int i = 0; i < x; i++)
                //{
                //    for (int j = 0; j < y; j++)
                //        matrix += firstMatrix[i, j] + " ";
                //    matrix += "\n";
                //}
                //MessageBox.Show(matrix);

                if (tempMinRow != -1 && tempMinColumn != -1) xArray[tempMinColumn] = firstMatrix[tempMinRow, y - 1];
                xArray[minCollumnIndex] = firstMatrix[minRowIndex, y - 1];
                tempMinColumn = minCollumnIndex;
                tempMinRow = minRowIndex;

                // Проверяем, все ли элементы функции L положительны.
                check = true;
                for (int j = 0; j < y - 1; j++)
                {
                    if (firstMatrix[x - 1, j] < 0) check = false;
                }

                // Если да, то задача решена.
                if (check)
                {
                    if (comboBox1.SelectedItem.Equals("Min")) L = firstMatrix[x - 1, y - 1] * -1;
                    else L = firstMatrix[x - 1, y - 1];
                    String xLast = "Переменные: X(";
                    for (int i = 0; i < variables; i++) xLast += xArray[i] + ";  ";
                    xLast = xLast.Trim() + ").";
                    MessageBox.Show("Целевая функция равна " + L.ToString() + ".\n" + xLast);
                }

                // Если нет, то повторяем функцию simplexCount().
                else
                {
                    mainMatrix = firstMatrix;
                    simplexCount();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        // Указываем количество переменных.
        private void variablesNumber_TextChanged(object sender, EventArgs e)
        {
            DataGridViewComboBoxColumn cbEqual = new DataGridViewComboBoxColumn();
            cbEqual.DataSource = eqData;
            try
            {
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                variablesNumber.Text = "";
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
                boundsNumber.Text = "";
            }
        }

        public void excel()
        {
            try
            {
                objExcel = new Microsoft.Office.Interop.Excel.Application();
                objWorkBook = objExcel.Workbooks.Add(System.Reflection.Missing.Value);
                objWorkSheet = objWorkBook.Sheets[1];

                for (int i = 0; i < Convert.ToInt32(variablesNumber.Text); i++)
                {
                    objExcel.Cells[1, i + 2] = dataGridView2.Rows[0].Cells[i].Value;
                    
                }

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        DataGridViewRow row = dataGridView1.Rows[i]; // строки
                        for (int j = 0; j < row.Cells.Count; j++) //цикл по ячейкам строки
                        {
                        objExcel.Cells[i + 3, j + 1] = row.Cells[j].Value;
                        }
                    }
                objWorkBook.SaveAs("check.xls");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                {
                    objWorkBook.Close();
                    objExcel.Quit();
                    objWorkBook = null;
                    objWorkSheet = null;
                    objExcel = null;
                    //System.Diagnostics.Process.Start("check.xls");
                }
            }
            
        }
    }
}
