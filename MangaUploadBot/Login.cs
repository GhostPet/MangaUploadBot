using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MangaUploadBot
{
    public partial class Login : Form
    {
        bool loggedin = false;
        IWebDriver driver;
        User user;

        public Login(ChromeDriver driver, User user)
        {
            this.driver = driver;
            this.user = user;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!textBox1.Text.Equals("") && !textBox2.Text.Equals(""))
            {
                label4.Text = "Durum: Giriş yapılıyor...";
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            this.user.UserName = textBox1.Text;
            this.user.Password = textBox2.Text;

            driver.Navigate().GoToUrl("https://turktoon.com/wp-login.php");

            driver.FindElement(By.Id("user_login")).SendKeys(this.user.UserName);
            driver.FindElement(By.Id("user_pass")).SendKeys(this.user.Password);
            driver.FindElement(By.Id("wp-submit")).Click();

            if (driver.Title == "Başlangıç ‹ TurkToon — WordPress")
            {
                this.loggedin = true;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (loggedin)
            {
                this.Close();
            }
            else
            {
                label4.Text = "Kullanıcı adı veya şifre yanlış.";
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)System.Windows.Forms.Keys.Enter && !textBox1.Text.Equals("") && !textBox2.Text.Equals(""))
            {
                label4.Text = "Giriş yapılıyor...";
                backgroundWorker1.RunWorkerAsync();
            }
        }
    }
}
