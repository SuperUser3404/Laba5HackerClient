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

namespace AndriyHacker
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                if (textBox1.Text == string.Empty)
                {
                    textBox1.BackColor = Color.Red;
                    textBox1.Text = "Поле для логіну порожнє";
                }
                else
                {
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        List<string> passwords = GetPasswordsFromFile(openFileDialog1.FileName);
                        textBox2.Text = AttackByDictionary(passwords, textBox1.Text);
                        label2.Text = "Broken password:";
                    }
                }
            }
            checkBox1.Checked = false;
        }

        List<string> GetPasswordsFromFile(string filepath)
        {
            return File.ReadAllLines(filepath).ToList();
        }

        string AttackByDictionary(List<string> passwords, string username)
        {
            for (int i = 0; i < passwords.Count; i++)
            {
                if (Auth(username, passwords[i]) == "Success")
                {
                    return passwords[i];
                }
            }

            return "Пароль не знайдено";
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
    }
}
