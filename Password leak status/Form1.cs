using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Password_leak_status
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox1.PasswordChar = default(char);
            }
            else
            {
                textBox1.PasswordChar = '*';
            }
        }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {

        }
        static string Hash(string input)
        {
            var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("비밀번호를 입력해주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string sha1 = Hash(textBox1.Text);
            string sha_pper = sha1.ToUpper();
            string Str_SHA1 = sha_pper.Substring(0, 8);
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create("https://exts.kr/api.hackedpw.json?prefix=" + Str_SHA1 + "");
            //HttpWebResponse 객체 받아옴
            HttpWebResponse wRes = (HttpWebResponse)myReq.GetResponse();
            // Response의 결과를 스트림을 생성합니다.
            Stream respGetStream = wRes.GetResponseStream();
            StreamReader readerGet = new StreamReader(respGetStream, Encoding.UTF8);
            string resultGet = readerGet.ReadToEnd();
            var jo = JObject.Parse(resultGet);

            /*
            {
  "success": true,
  "data": {
    "hacked": true,
    "count": 1296186
  }
}
             */
            var data = (JObject)jo["data"];
            foreach (var Str_data in data)
            {
                string Strdata = Str_data.Key;

                string adata = jo["data"]["hacked"].ToString();

                if(adata == "True")
                {
                    string count = jo["data"]["count"].ToString();
                    MessageBox.Show("당신의 패스워드가 유출 되었습니다. \r\n패스워드 유출횟수: " + count + " 번 입니다.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    MessageBox.Show("당신의 패스워드 유출되지 않았습니다.", "유출되지 않음", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }        
}