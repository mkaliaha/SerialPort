using System;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;

namespace SerialPort
{
    public partial class Chat : Form
    {
        public Chat(Serial port)
        {
            SPort = port;
            SPort.ErrorReceived += SerialPort_ErrorReceived;
            SPort.DataReceived += SerialPort_DataReceived;
            InitializeComponent();
        }

        public Serial SPort { get; }

        public void SerialPort_ErrorReceived(object obj, SerialErrorReceivedEventArgs args)
        {
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            toolTip1.SetToolTip(sendButton, "Error data received");
        }

        public void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs args)
        {
            void Act()
            {
                var temp = Encoding.UTF8.GetString(SPort.ReadBytes());
                if (temp != "\n")
                    richTextBox1.AppendText(temp);
            }

            richTextBox1.Invoke((Action) Act);
        }

        private void Chat_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void Chat_Shown(object sender, EventArgs e)
        {
            textBox1.Focus();
        }


        private void SendButton_Click(object sender, EventArgs e)
        {
            var text = textBox1.Text + "\n";
            if (string.Equals(text, "\n", StringComparison.Ordinal))
                return;

            var data = Encoding.UTF8.GetBytes(text);
            SPort.WriteData(data);
            richTextBox1.AppendText("ME (" + SPort.PortName + ") " + DateTime.Now.ToString("T") + ": " + text);
            textBox1.Clear();
        }

        private void Chat_Load(object sender, EventArgs e)
        {
            Text = SPort.PortName;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SPort.Close();
            SPort.BaudRate = Convert.ToInt32(comboBox1.Text);
            SPort.Open();
        }
    }
}