using OpenQA.Selenium;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MangaUploadBot
{
    public partial class Login : Form
    {
        IWebDriver driver;
        User user;

        public Login(Driver d, User user)
        {
            this.driver = d.driver;
            this.user = user;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Start();
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)System.Windows.Forms.Keys.Enter) Start();
        }
        private void Start()
        {
            if (!textBox1.Text.Equals("") && !textBox2.Text.Equals(""))
            {
                label4.Text = "Giriş yapılıyor...";
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

            if (driver.Title == "Başlangıç ‹ TurkToon — WordPress") user.LoggedIn = true;
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (user.LoggedIn) this.Close();
            else label4.Text = "Kullanıcı adı veya şifre yanlış.";
        }
    }
}
