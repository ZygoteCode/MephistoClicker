using MetroSuite;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Security.Cryptography;
using System.Windows.Forms;
using System;
using InterceptionSharp;

public partial class c : MetroForm
{
    [DllImport("User32.dll")]
    private static extern short GetAsyncKeyState(System.Int32 vKey);

    public Input input;
    private char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    private Thread clickThread;
    private byte checkStatus = 0;
    private System.Windows.Forms.Keys startClicking = System.Windows.Forms.Keys.F7, stopClicking = System.Windows.Forms.Keys.F8;
    private bool canClick = true;
    private ProtoRandom.ProtoRandom random;

    public c()
    {
        InitializeComponent();
        CheckForIllegalCrossThreadCalls = false;
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
        random = new ProtoRandom.ProtoRandom(5);

        input = new Input();
        input.Load();

        Thread thread = new Thread(CheckKey);
        thread.Priority = ThreadPriority.Highest;
        thread.Start();

        Thread thread2 = new Thread(CheckKey1);
        thread2.Priority = ThreadPriority.Highest;
        thread2.Start();

        try
        {
            if (!System.IO.File.Exists("keys.txt"))
            {
                System.IO.File.WriteAllText("keys.txt", "F7" + Environment.NewLine + "F8");
            }
            else
            {
                System.Collections.Generic.List<string> keys = new System.Collections.Generic.List<string>();

                foreach (string line in Utils.SplitToLines(System.IO.File.ReadAllText("keys.txt")))
                {
                    keys.Add(line);
                }

                if (keys.Count != 2)
                {
                    Process.GetCurrentProcess().Kill();
                    return;
                }

                if (keys[0].Length > 10)
                {
                    Process.GetCurrentProcess().Kill();
                    return;
                }

                if (keys[1].Length > 10)
                {
                    Process.GetCurrentProcess().Kill();
                    return;
                }

                try
                {
                    startClicking = Utils.ConvertStringToKeys(keys[0]);
                }
                catch
                {

                }

                try
                {
                    stopClicking = Utils.ConvertStringToKeys(keys[1]);
                }
                catch
                {

                }
            }
        }
        catch
        {

        }

        UpdateAll();
    }

    public void UpdateAll()
    {
        System.IO.File.WriteAllText("keys.txt", Utils.ConvertKeysToString(startClicking) + Environment.NewLine + Utils.ConvertKeysToString(stopClicking));
        gunaButton4.Text = "Start Clicking (" + Utils.ConvertKeysToString(startClicking) + ")";
        gunaButton1.Text = "Stop Clicking (" + Utils.ConvertKeysToString(stopClicking) + ")";
    }

    private void gunaButton4_Click(object sender, System.EventArgs e)
    {
        gunaButton3.Enabled = false;
        gunaButton2.Enabled = false;
        gunaButton4.Enabled = false;
        gunaButton1.Enabled = true;

        if (checkStatus == 0)
        {
            siticoneRadioButton11.Checked = true;
        }
        else if (checkStatus == 1)
        {
            siticoneRadioButton1.Checked = true;
        }
        else if (checkStatus == 2)
        {
            siticoneRadioButton2.Checked = true;
        }

        clickThread = new Thread(AutoClick);
        clickThread.Priority = ThreadPriority.Highest;
        clickThread.Start();
    }

    private void gunaButton1_Click(object sender, System.EventArgs e)
    {
        gunaButton1.Enabled = false;
        gunaButton4.Enabled = true;
        gunaButton3.Enabled = true;
        gunaButton2.Enabled = true;

        try
        {
            clickThread.Abort();
        }
        catch
        {

        }
    }

    public void CheckKey()
    {
        while (true)
        {
            Thread.Sleep(10);

            if (canClick)
            {
                if ((GetAsyncKeyState(((int)startClicking)) & 0x8000) > 0)
                {
                    if (gunaButton4.Enabled)
                    {
                        gunaButton4.PerformClick();
                    }
                }
                else if ((GetAsyncKeyState(((int)stopClicking)) & 0x8000) > 0)
                {
                    if (gunaButton1.Enabled)
                    {
                        gunaButton1.PerformClick();
                    }
                }
            }
        }
    }

    public void CheckKey1()
    {
        while (true)
        {
            Thread.Sleep(10);

            if ((GetAsyncKeyState(((int)startClicking)) & 0x8000) > 0 || (GetAsyncKeyState(((int)stopClicking)) & 0x8000) > 0 || gunaButton1.Enabled)
            {
                continue;
            }

            if (gunaButton3.Text == "Waiting for key")
            {
                for (int i = 0; i < 255; i++)
                {
                    if ((GetAsyncKeyState(i) & 0x8000) > 0)
                    {
                        canClick = false;
                        startClicking = (System.Windows.Forms.Keys)i;
                        gunaButton3.Text = "Change your key for Start Clicking";
                        gunaButton2.Enabled = true;
                        UpdateAll();
                        new Thread(ReEnable).Start();
                        continue;
                    }
                }
            }
            else if (gunaButton2.Text == "Waiting for key")
            {
                for (int i = 0; i < 255; i++)
                {
                    if ((GetAsyncKeyState(i) & 0x8000) > 0)
                    {
                        canClick = false;
                        stopClicking = (System.Windows.Forms.Keys)i;
                        gunaButton2.Text = "Change your key for Stop Clicking";
                        gunaButton3.Enabled = true;
                        UpdateAll();
                        new Thread(ReEnable).Start();
                        continue;
                    }
                }
            }
        }
    }

    public void ReEnable()
    {
        Thread.Sleep(500);
        canClick = true;
    }

    private void c_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
    {
        Process.GetCurrentProcess().Kill();
    }

    private void siticoneCheckBox22_CheckedChanged(object sender, System.EventArgs e)
    {
        TopMost = siticoneCheckBox22.Checked;
    }

    public int GetNumberByText(string text)
    {
        string theNum = "";

        foreach (char c in text)
        {
            foreach (char s in numbers)
            {
                if (c == s)
                {
                    theNum += s;
                }
            }
        }

        if (theNum == "")
        {
            return 0;
        }

        if (theNum[0] == '0')
        {
            return 0;
        }

        return int.Parse(theNum);
    }

    public void AutoClick()
    {
        while (gunaButton1.Enabled)
        {
            try
            {
                if (siticoneRadioButton11.Checked)
                {
                    if (siticoneCheckBox1.Checked)
                    {
                        Thread.Sleep(siticoneRadioButton8.Checked ? random.GetRandomInt32(GetNumberByText(gunaLineTextBox1.Text), GetNumberByText(gunaLineTextBox2.Text)) : GetNumberByText(gunaLineTextBox14.Text));
                    }
                }
                else
                {
                    if (siticoneRadioButton1.Checked)
                    {
                        Thread.Sleep(1000 / GetNumberByText(gunaLineTextBox3.Text));
                    }
                    else
                    {
                        int minValue = (1000 / GetNumberByText(gunaLineTextBox4.Text)), maxValue = (1000 / GetNumberByText(gunaLineTextBox5.Text)), app = 0;

                        if (minValue > maxValue)
                        {
                            app = minValue;
                            minValue = maxValue;
                            maxValue = app;
                        }

                        Thread.Sleep(random.GetRandomInt32(minValue, maxValue));
                    }
                }

                MouseClick();
            }
            catch
            {

            }
        }
    }

    public void MouseClick()
    {
        if (siticoneRadioButton5.Checked)
        {
            LeftClick();
        }
        else if (siticoneRadioButton4.Checked)
        {
            RightClick();
        }
        else if (siticoneRadioButton3.Checked)
        {
            LeftClick();
            RightClick();
        }
        else if (siticoneRadioButton6.Checked)
        {
            RightClick();
            LeftClick();
        }
    }

    public void LeftClick()
    {
        input.SendLeftClick();
    }

    public void RightClick()
    {
        input.SendRightClick();
    }

    private void siticoneRadioButton11_CheckedChanged(object sender, EventArgs e)
    {
        checkStatus = 0;
    }

    private void siticoneRadioButton1_CheckedChanged(object sender, EventArgs e)
    {
        checkStatus = 1;
    }

    private void gunaButton3_Click(object sender, EventArgs e)
    {
        if (gunaButton3.Text == "Change your key for Start Clicking")
        {
            gunaButton3.Text = "Waiting for key";
            gunaButton2.Enabled = false;
        }
    }

    private void gunaButton2_Click(object sender, EventArgs e)
    {
        if (gunaButton2.Text == "Change your key for Stop Clicking")
        {
            gunaButton2.Text = "Waiting for key";
            gunaButton3.Enabled = false;
        }
    }

    private void siticoneRadioButton2_CheckedChanged(object sender, EventArgs e)
    {
        checkStatus = 2;
    }
}