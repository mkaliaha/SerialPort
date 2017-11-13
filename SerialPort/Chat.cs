using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SerialPort
{
    public partial class Chat : Form
    {
        private readonly List<byte[]> _buffer = new List<byte[]>();
        private byte[] _message;
        private int _sender;
        public PackageComposer Composer = new PackageComposer();
        public bool RingInited;

        public Chat(Serial input, Serial output, int num)
        {
            InputSerial = input;
            InputSerial.DataReceived += SerialPort_DataReceived;
            OutputSerial = output;
            MachineNumber = num;

            InitializeComponent();
            if (MachineNumber != 1) InitButton.Enabled = false;
        }

        public Serial InputSerial { get; }
        public Serial OutputSerial { get; }
        public int MachineNumber { get; }

        public void SerialPort_ErrorReceived(object obj, SerialErrorReceivedEventArgs args)
        {
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            toolTip1.SetToolTip(sendButton, "Error data received");
        }

        public void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs args)
        {
            var data = InputSerial.ReadBytes();
            if (Composer.IsToken(data))
            {
                if (!RingInited)
                {
                    RingInited = true;
                }
                else if (_buffer.Count > 0)
                {
                    OutputSerial.WriteData(_buffer.First());
                    _buffer.RemoveAt(0);
                    return;
                }
                OutputSerial.WriteData(data);
                return;
            }
            if (!Composer.IsMessage(data)) return;
            if (Composer.GetSourceAddress(data) == MachineNumber)
            {
                OutputSerial.WriteData(Composer.Token);
            }
            else if (Composer.GetDestAddress(data) == MachineNumber)
            {
                _message = Composer.GetDataFromMessage(data);
                _sender = Composer.GetSourceAddress(data);
                OutputSerial.WriteData(data);
                richTextBox1.Invoke((Action) ShowIncoming);
            }
            else
            {
                OutputSerial.WriteData(data);
            }
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
            if (!RingInited)
            {
                richTextBox1.AppendText("Init ring first!\n");
                return;
            }
            var text = textBox1.Text + "\n";
            if (string.Equals(text, "\n", StringComparison.Ordinal))
                return;
            if (Convert.ToByte(DesCombobox.Text) == MachineNumber)
            {
                richTextBox1.AppendText("Can't send to yourself");
                return;
            }
            var data = Encoding.UTF8.GetBytes(text);
            _buffer.Add(Composer.ComposeMessage(data, Convert.ToByte(DesCombobox.Text), (byte) MachineNumber));
            richTextBox1.AppendText("SEND to " + DesCombobox.Text + ": " + DateTime.Now.ToString("T") + ": " + text);
            textBox1.Clear();
        }

        private void ShowIncoming()
        {
            richTextBox1.AppendText("GOT FROM " + _sender + ": " + DateTime.Now.ToString("T") + ": " +
                                    Encoding.UTF8.GetString(_message));
        }

        private void Chat_Load(object sender, EventArgs e)
        {
            Text = MachineNumber.ToString();
        }

        private void InitButton_Click(object sender, EventArgs e)
        {
            OutputSerial.WriteData(Composer.Token);
            RingInited = true;
            InitButton.Enabled = false;
        }
    }
}