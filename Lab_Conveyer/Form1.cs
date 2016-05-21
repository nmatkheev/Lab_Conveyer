using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;

namespace Lab_Conveyer
{
    public partial class Form1 : Form
    {
        public FacilityCPU instance;

        public float tact;
        public float swit;
        public int tasknum;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            initButtonLogic();
        }

        private void initButtonLogic()
        {
            calculateButton.Enabled = false;
            checkAnswerButton.Enabled = false;
            resetDataFields.Enabled = false;
            loadNumBtn.Enabled = true;
        }

        private void loadNumBtn_Click(object sender, EventArgs e)
        {
            int quorum = 0;
            foreach (TextBox ctrl in groupBox1.Controls.OfType<TextBox>())
            {
                try
                {
                    var value = (dynamic)null; ;
                    if (ctrl.Name == "taskNumberField")
                    {
                        value = int.Parse(ctrl.Text);
                    }
                    else
                    {
                        value = float.Parse(ctrl.Text);
                    }
                    switch (ctrl.Name)
                    {
                        case "taskNumberField":
                            tasknum = value;
                            break;
                        case "tactValueField":
                            tact = value;
                            break;
                        case "switchValueField":
                            swit = value;
                            break;
                    }
                    //ctrl.Enabled = false;
                    quorum++;
                }
                catch (Exception)
                {
                    quorum--;
                }
            }
            if (quorum == 3)
            {
                GenerateTaskFields(tasknum);
                calculateButton.Enabled = true;
                loadNumBtn.Enabled = false;
            }
                
            else MessageBox.Show(@"Давай по новой", @"ConveyerLab", MessageBoxButtons.OK);
        }

        private void GenerateTaskFields(int tasknum)
        {
            var font = label1.Font.FontFamily;
            for (var i = 0; i < tasknum; i++)
            {
                var label = new Label
                {
                    Name = $"labelTask{i}",
                    Text = $"Задание {i+1}",
                    AutoSize = true,
                    Font = new Font(font, 8)
                };
                label.Location = new Point(10, 23 + i * 30);
                groupBox3.Controls.Add(label);

                var box = new TextBox
                {
                    Name = $"task{i}Field",
                    AutoSize = true
                };
                box.Location = new Point(70, 20 + i*30);
                groupBox3.Controls.Add(box);
            }
        }


        private void calculateButton_Click(object sender, EventArgs e)
        {
            int quorum = 0;
            var data = new List<int>();
            Regex rgxField = new Regex(@"task(\d){1}Field");
            foreach (TextBox ctrl in groupBox3.Controls.OfType<TextBox>().Where(ctrl => rgxField.IsMatch(ctrl.Name)))
            {
                try
                {
                    var arg = int.Parse(ctrl.Text);
                    data.Add(arg);
                    quorum++;
                }
                catch (Exception)
                {
                    quorum--;
                }
            }

            if (quorum != tasknum)
            {
                MessageBox.Show(@"Давай по новой", @"ConveyerLab", MessageBoxButtons.OK);
                return;
            }
            instance = new FacilityCPU(tact, swit, data);
                
            for (int i = 0; i < tasknum; i++)
            {
                chart1.Series.Add($"Series{i + 1}");
                chart1.Series[$"Series{i + 1}"].ChartType = SeriesChartType.RangeBar;
                chart1.Series[$"Series{i + 1}"].ChartArea = "ChartArea1";

                chart1.Legends.Add(new Legend($"Legend{i + 1}"));
                chart1.Legends[$"Legend{i + 1}"].DockedToChartArea = "ChartArea1";
                chart1.Series[$"Series{i + 1}"].Legend = $"Legend{i + 1}";
                chart1.Series[$"Series{i + 1}"].IsVisibleInLegend = true;
                chart1.Series[$"Series{i + 1}"].LegendText = $"Task #{i + 1}";
            }

            for (var i = 0; i < instance.dur_index.Count; i++)
            {
                if (instance.dur_index[i] == int.MaxValue) continue;

                var seriesIndex = $"Series{instance.dur_index[i] + 1}";
                chart1.Series[seriesIndex].Points.AddXY(1, i*20, i*20 + 20);
            }
            chart1.DataBind();
            checkAnswerButton.Enabled = true;
            calculateButton.Enabled = false;
            resetDataFields.Enabled = true;
            generateTestBoxes();
            MessageBox.Show(@"Ответ округляй до десятых!", @"ConveyerLab", MessageBoxButtons.OK);
        }

        private void generateTestBoxes()
        {
            var font = label1.Font.FontFamily;
            for (var i = 0; i < tasknum; i++)
            {
                var label = new Label
                {
                    Name = $"labelTask{i}",
                    Text = $"Задание {i + 1}",
                    AutoSize = true,
                    Font = new Font(font, 8)
                };
                label.Location = new Point(10, 23 + i * 30);
                groupBox2.Controls.Add(label);

                var box = new TextBox
                {
                    Name = $"check{i}Field",
                    AutoSize = true
                };
                box.Location = new Point(70, 20 + i * 30);
                groupBox2.Controls.Add(box);
            }
        }


        private void resetDataFields_Click(object sender, EventArgs e)
        {
            var boxes = groupBox3.Controls.OfType<Control>().ToList();
            foreach (Control ctrl in boxes)
            {
                groupBox3.Controls.Remove(ctrl);
                ctrl.Dispose();
            }

            var boxes2 = groupBox2.Controls.OfType<Control>().ToList();
            foreach (Control ctrl in boxes2)
            {
                groupBox2.Controls.Remove(ctrl);
                ctrl.Dispose();
            }

            chart1.Series.Clear();
            chart1.Legends.Clear();

            foreach (Control ctrl in groupBox1.Controls)
            {
                if (!(ctrl is TextBox)) continue;
                ctrl.Enabled = true;
            }

            instance = null;
            initButtonLogic();
        }

        private void checkAnswerButton_Click(object sender, EventArgs e)
        {
            List<float> userAnswer = new List<float>();
            int quorum = 0;
            foreach (TextBox ctrl in groupBox2.Controls.OfType<TextBox>())
            {
                try
                {
                    var value = float.Parse(ctrl.Text);
                    userAnswer.Add(value);
                    quorum++;
                }
                catch (Exception)
                {
                    quorum--;
                }
            }
            if (quorum == tasknum)
            {
                for (int x = 0; x < userAnswer.Count; x++)
                {
                    if (Math.Round(userAnswer[x]) == Math.Round(instance.res[x])) quorum--;
                }
                if (quorum == 0)
                    MessageBox.Show(@"Красава! Четкий подгон!", @"ConveyerLab", MessageBoxButtons.OK);
            }
            else MessageBox.Show("Полная ерунда, считай лучше!", @"ConveyerLab", MessageBoxButtons.OK);
        }
    }
}
