using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace OC6
{
    public partial class Form1 : Form
    {
        Process thisProc;
        int i = 1;
        List<Thread> TList = new List<Thread>();
        List<RenderThread> RList = new List<RenderThread>();
        public Form1()
        {
            thisProc = Process.GetCurrentProcess();
            InitializeComponent();
            Color.Enabled = false;
            Suspend.Enabled = false;
            Resume.Enabled = false;
            RemoveThread.Enabled = false;
            Radius.Enabled = false;
            RefreshInterval.Enabled = false;
            Priority.Items.Add("Высокий");
            Priority.Items.Add("Выше среднего");
            Priority.Items.Add("Нормальный");
            Priority.Items.Add("Ниже среднего");
            Priority.Items.Add("Низкий");
            Priority.Enabled = false;
            MainPriority.Items.Add("High");
            MainPriority.Items.Add("AboveNormal");
            MainPriority.Items.Add("Normal");
            MainPriority.Items.Add("BelowNormal");
            MainPriority.Items.Add("Idle");
            MainPriority.Items.Add("RealTime");
            switch (thisProc.PriorityClass.ToString())
            {
                case "High" : MainPriority.SelectedItem = "High"; break;
                case "AboveNormal": MainPriority.SelectedItem = "AboveNormal"; break;
                case "Normal": MainPriority.SelectedItem = "Normal"; break;
                case "BelowNormal": MainPriority.SelectedItem = "BelowNormal"; break;
                case "Idle": MainPriority.SelectedItem = "Idle"; break;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Thread thread = null;
            RenderThread render = new RenderThread(this.pictureBox1, this, thread, colorDialog1.Color, 0, i++, 0);
            TList.Add(thread);
            RList.Add(render);
            ListThread.Items.Add(render._name);
            render.Run();
            render = null;
        }
        private void RemoveThread_Click(object sender, EventArgs e)
        {
            foreach (RenderThread ab in RList)
            {
                if (ListThread.SelectedIndex != -1 && ab._name == ListThread.SelectedItem.ToString())
                {
                    ListThread.Items.Remove(ab._name);
                    ab.Dispose();
                    i--;
                    RList.Remove(ab);
                    Color.Enabled = false;
                    Suspend.Enabled = false;
                    Resume.Enabled = false;
                    RemoveThread.Enabled = false;
                    Radius.Enabled = false;
                    RefreshInterval.Enabled = false;
                    Priority.Enabled = false;
                    break;
                }
            }
        }
        private void Color_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            foreach (RenderThread ab in RList)
                if (ListThread.SelectedIndex != -1 && ab._name == ListThread.SelectedItem.ToString())
                {
                    ab.UsedColor = colorDialog1.Color;
                }
        }
        private void Resume_Click(object sender, EventArgs e)
        {
            foreach (RenderThread ab in RList)
                if (ListThread.SelectedIndex != -1 && ab._name == ListThread.SelectedItem.ToString())
                {
                    ab.Resume();
                    Resume.Enabled = false;
                    Suspend.Enabled = true;
                }
        }
        private void Suspend_Click(object sender, EventArgs e)
        {
            foreach (RenderThread ab in RList)
                if (ListThread.SelectedIndex != -1 && ab._name == ListThread.SelectedItem.ToString())
                {
                    ab.Suspend();
                    Resume.Enabled = true;
                    Suspend.Enabled = false;
                }
        }
        private void Radius_ValueChanged(object sender, EventArgs e)
        {
            foreach (RenderThread ab in RList)
                if (ListThread.SelectedIndex != -1 && ab._name == ListThread.SelectedItem.ToString())
                    ab.Radius = Convert.ToInt32(Radius.Value);
        }
        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            if (RList.Count != 0)
            {
                foreach (RenderThread ab in RList)
                {
                    ListThread.Items.Remove(ab._name);
                    ab.Dispose();
                    i--;
                }
            }
        }
        private void RefreshInterval_ValueChanged(object sender, EventArgs e)
        {
            foreach (RenderThread ab in RList)
                if (ListThread.SelectedIndex != -1 && ab._name == ListThread.SelectedItem.ToString())
                    ab.SleepDuration = Convert.ToInt32(RefreshInterval.Value);
        }
        private void ListThread_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (RenderThread ab in RList)
                if (ListThread.SelectedIndex != -1 && ab._name == ListThread.SelectedItem.ToString())
                {
                    colorDialog1.Color = ab._color;
                    Color.Enabled = true;
                    RemoveThread.Enabled = true;
                    Priority.Enabled = true;
                    Priority.SelectedItem = ab.Priority;
                    if (ab._pauseEvent.WaitOne(0))
                    {
                        Resume.Enabled = true;
                        Suspend.Enabled = false;
                    }
                    else
                    {
                        Resume.Enabled = false;
                        Suspend.Enabled = true;
                    }
                    Radius.Enabled = true;
                    Radius.Value = ab.Radius;
                    RefreshInterval.Enabled = true;
                    RefreshInterval.Value = ab.SleepDuration;
                }
        }
        private void Priority_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (RenderThread ab in RList)
                if (ListThread.SelectedIndex != -1 && ab._name == ListThread.SelectedItem.ToString())
                    ab.Priority = Priority.SelectedItem.ToString();
        }
        private void MainPriority_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (MainPriority.SelectedItem.ToString())
            {
                case "High":
                    thisProc.PriorityClass = ProcessPriorityClass.High; break;
                case "AboveNormal":
                    thisProc.PriorityClass = ProcessPriorityClass.AboveNormal; break;
                case "Normal":
                    thisProc.PriorityClass = ProcessPriorityClass.Normal; break;
                case "BelowNormal":
                    thisProc.PriorityClass = ProcessPriorityClass.BelowNormal; break;
                case "Idle":
                    thisProc.PriorityClass = ProcessPriorityClass.Idle; break;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            foreach (RenderThread ab in RList)
                ab.ParentResize(sender, e);
        }
    }
}