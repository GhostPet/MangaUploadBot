using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MangaUploadBot
{
    public partial class MainUi : Form
    {
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };

        User user;
        IWebDriver driver;
        SheetsService service;

        IList<IList<Object>> mangas;
        IList<Object> selectedmanga;
        IList<IList<Object>> covers;
        IList<Object> selectedcover;

        string[] files;
        string label4text;

        string version = "1.0.1";

        public MainUi(IWebDriver d, User u)
        {
            driver = d;
            user = u;

            InitializeComponent();

            GoogleCredential credential;
            using (var stream = new FileStream("turktoon-bot-55d1a7b67f2f.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }

            this.service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Turktoon Upload Bot by GhostPet",
            });

            refreshgoogledata();
        }

        private void refreshgoogledata()
        {
            this.selectedcover = null;
            this.selectedmanga = null;

            String spreadsheetId = "1-71OojtQ3941aO203ZIYUMFAtAxsYoXSSPCxrvDsRpY";

            String range = "series!A2:B";
            SpreadsheetsResource.ValuesResource.GetRequest request = this.service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();
            this.mangas = response.Values;

            String range2 = "covers!A2:D";
            SpreadsheetsResource.ValuesResource.GetRequest request2 = this.service.Spreadsheets.Values.Get(spreadsheetId, range2);
            ValueRange response2 = request2.Execute();
            this.covers = response2.Values;

            foreach (var value in mangas)
            {
                comboBox1.Items.Add(value[1]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Bir Dosya Seçin";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    this.files = Directory.GetDirectories(dlg.SelectedPath);
                    label3.Text = this.files.Length + " adet bölüm paylaşılacak.";
                }
            }

            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            backgroundWorker2.RunWorkerAsync();
        }

        private void çıkışYapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void kullanıcıDeğiştirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Restart();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.selectedcover = null;
            comboBox2.Items.Clear();
            comboBox2.Enabled = false;

            foreach (var value in mangas)
            {
                if (value[1].Equals(comboBox1.SelectedItem.ToString()))
                {
                    this.selectedmanga = value;
                    break;
                }
            }

            foreach (var value in covers)
            {
                if (value[0].Equals(comboBox1.SelectedItem.ToString()))
                {
                    comboBox2.Enabled = true;
                    comboBox2.Items.Add(value[1]);
                    if (this.selectedcover == null)
                    {
                        this.selectedcover = value;
                    }
                }
            }

            if (comboBox2.Items.Count > 0)
            {
                comboBox2.SelectedIndex = 0;

            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.selectedcover = null;
            foreach (var value in covers)
            {
                if (value[0].Equals(comboBox1.SelectedItem.ToString()) && value[1].Equals(comboBox2.SelectedItem.ToString()))
                {
                    this.selectedcover = value;
                    break;
                }
            }
        }

        private void backgroundWorker2_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            label4.Text = this.label4text;
        }

        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Sharer sharer = new Sharer(this.driver);
            int a = 0;

            foreach (String file in this.files)
            {
                String[] temp = file.Split("\\".ToCharArray());
                String filename = temp[temp.Length - 1];

                this.label4text = a + "/" + this.files.Length + ": " + filename + ". bölüm paylaşılıyor...";
                backgroundWorker2.ReportProgress((int)Math.Round((double)(100 * a) / this.files.Length));
                sharer.Share(this.selectedmanga, this.selectedcover, filename, file, backgroundWorker2, a, this.files.Length);
                a += 1;
            }

            this.label4text = "Tamamlandı.";
            backgroundWorker2.ReportProgress(100);
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            button2.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            refreshgoogledata();
        }

        private void güncelleştirmeleriDenetleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String spreadsheetId = "1-71OojtQ3941aO203ZIYUMFAtAxsYoXSSPCxrvDsRpY";

            String range = "usage!C2:C2";
            SpreadsheetsResource.ValuesResource.GetRequest request = this.service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();
            String latestversion = response.Values[0][0].ToString();

            if (latestversion != version)
            {
                MessageBox.Show("Şu anda eski bir sürümü kullanıyorsunuz. Güncel sürüm:" + latestversion);
            }
            else
            {
                MessageBox.Show("Şu anda en son sürümü kullanmaktasınız.");
            }
        }

        private void hakkındaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Turktoon Upload Bot Sürüm " + version + "\n\nTurktoon Upload Bot'u GhostPet tarafından yapılmıştır. Tüm hakları saklıdır.");
        }
    }
}
