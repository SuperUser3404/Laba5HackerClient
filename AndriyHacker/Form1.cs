using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace HackerClient
{
    public partial class Form1 : Form
    {
        private string pathToDictionary;
        Stream stream;
        TcpClient tcpClient;
        public Form1()
        {
            InitializeComponent();
            tcpClient = new TcpClient();
            tcpClient.Connect("127.0.0.1", 8888);
            stream = tcpClient.GetStream();
            label4.Text = $"Адреса клієнта: 127.0.0.1 : {((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port}";
        }

        string Auth(string username, string password)
        {
            List<byte> buffer = new List<byte>();
            int bytesRead = 10;
            byte[] requestData = Encoding.UTF8.GetBytes(username + " " + password + "\n");
            stream.Write(requestData, 0 , requestData.Length);

            while ((bytesRead = stream.ReadByte()) != '\n')
            {
                buffer.Add((byte)bytesRead);
            }

            string result = Encoding.UTF8.GetString(buffer.ToArray());
            return result;
        }

        string AttackByFormation(string username, List<string> formations)
        {
            for (int i = 0; i < formations.Count; i++)
            {
                string temp = formations[i];
                for (int j = 0; j < formations.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    temp += formations[j];

                    if (Auth(username, temp) == "Success")
                    {
                        return temp;
                    }
                }
            }
            return "Password is not found";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            byte[] request = Encoding.UTF8.GetBytes("Close" + "\n");
            stream.Write(request,0,request.Length);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Auth(textBox1.Text, textBox2.Text) == "Success")
            {
                button1.BackColor = Color.Green;
            }
            else
            {
                button1.BackColor = Color.Red;
            }
        }
        private void textBox1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "Поле для логіну порожнє")
            {
                textBox1.Text = string.Empty;
            }
            textBox1.BackColor = Color.White;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty)
            {
                textBox1.BackColor = Color.Red;
                textBox1.Text = "Поле для логіну порожнє";
            }
            else
            {
                List<string> formations = new List<string>() { "pa", "ss", "wo", "rd" };
                textBox2.Text = AttackByFormation(textBox1.Text , formations);
                label2.Text = "Broken password:";
            }
        }
    }
}
