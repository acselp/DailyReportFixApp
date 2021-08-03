using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace DailyReportFixApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Excel ex = new Excel();
        SaveFileDialog saveFileDialog = new SaveFileDialog();

        static string folderPath;

        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void Panel_DragDrop(object sender, DragEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            progressBar.Visible = true;
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            
            progressBar.Value = 0;
            
            string[] files;
            files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (System.IO.Path.GetExtension(files[0]).Equals(".txt", StringComparison.InvariantCultureIgnoreCase))
            {
                ex.path = files[0];
            }
            else
            {
                MessageBox.Show("Introduceti un fisier .txt");
                return;
            }
            ModifyFile();

            SaveBtn.Enabled = true;
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                ex.SaveAs(saveFileDialog.FileName);
                ex.Close();
            }
            SaveBtn.Enabled = false;
        }

        public void ModifyFile()
        {
            string str = System.IO.File.ReadAllText(ex.path);
            string[] words = str.Split(new char[] { ' ', '\r', '\t', '\n' }).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            string[] money = new string[str.Split('\n').Length - 2];
            string[][] finalMoney = new string[str.Split('\n').Length - 4][];
            int nrLines = str.Split('\n').Length - 2;
            string[][] table = new string[nrLines][];
            int i, j = 0, apaSum = 0, gunoiSum = 0;
            MessageBox mb;

            progressBar.Value = 25;

            //Extrage plata pe apa si salubrizare (ex 120/12)
            for (i = 0; i < words.Length; i++)
            {
                if (words[i].Contains('/'))
                {
                    money[j] = words[i];
                    j++;
                }
            }

            progressBar.Value = 30;

            //Initializam matricea
            //Calculam suma totala pentru banii incasati
            for (i = 0; i < nrLines - 2; i++)
            {
                finalMoney[i] = money[i].Split('/').ToArray();
                apaSum += Int32.Parse(finalMoney[i][0]);
                gunoiSum += Int32.Parse(finalMoney[i][1]);
                table[i] = new string[4];
            }
            table[i] = new string[4];

            progressBar.Value = 35;

            //Denumirile rubricilor
            table[0][0] = words[1];
            table[0][1] = words[2];
            table[0][2] = words[3] + " " + words[4];
            table[0][3] = words[5];

            //Introducerea datelor in matrice
            int k = 7;
            for (i = 1; i < nrLines - 1; i++)
            {
                if ((k + 2) % 8 == 0)
                    k++;

                for (j = 0; j < 4; j++)
                {

                    if (j == 1)
                    {
                        table[i][j] = words[k] + " " + words[++k] + " " + words[++k];
                        k++;
                    }
                    else if(j == 2)
                    {
                        table[i][j] = words[k] + " " + words[++k];
                        k++;
                    }
                    else
                    {
                        table[i][j] = words[k];
                        k++;
                    }
                }
            }

            progressBar.Value = 45;

            folderPath = System.IO.Path.GetDirectoryName(ex.path);

            string n = "";
            int ind = 0;

            ex.CreateFile(nrLines);

            for(i = 0; i < nrLines - 2; i++)
            {
                ex.WriteFile(i + 1, 5, finalMoney[i][0]);
                ex.WriteFile(i + 1, 6, finalMoney[i][1]);
                ex.WriteFile(i + 1, 7, Convert.ToString(Int32.Parse(finalMoney[i][0]) + Int32.Parse(finalMoney[i][1])));
                for (j = 0; j < 4; j++)
                {
                    ex.WriteFile(i, j, table[i][j]);
                }
            }

            progressBar.Value = 55;

            for (j = 0; j < 4; j++)
            {
                ex.WriteFile(i, j, table[i][j]);
            }

            progressBar.Value = 85;

            ex.WriteFile(nrLines, 1, words[words.Length - 3] + "  " + words[words.Length - 2] + "    " + words[words.Length - 1]);
            ex.WriteFile(0, 5, "Apa");
            ex.WriteFile(0, 6, "Salubrizarea");
            ex.WriteFile(0, 7, "Total");
            ex.WriteFile(nrLines, 4, "Total Suma");
            ex.WriteFile(nrLines, 5, Convert.ToString(apaSum));
            ex.WriteFile(nrLines, 6, Convert.ToString(gunoiSum));
            ex.WriteFile(nrLines, 7, Convert.ToString(apaSum + gunoiSum));


            progressBar.Value = 100;
            Task.Delay(500);
            MessageBox.Show("Fisierul a fost procesat cu succes.");
            progressBar.Value = 0;
            Cursor.Current = Cursors.Default;
        }
    }
}
