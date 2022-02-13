using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MangaUploadBot
{
    public partial class MainUi : Form
    {
        Uploader uploader;
        User user;
        GoogleApi GoogleApi;
        Driver driver;
        string version;
        IList<IList<Object>> mangas;
        IList<Object> selectedmanga;
        IList<IList<Object>> covers;
        IList<Object> selectedcover;
        string[] files;
        string label4text;

        public MainUi(User user, GoogleApi api, Driver driver, string version)
        {
            this.user = user;
            this.GoogleApi = api;
            this.driver = driver;
            this.uploader = new Uploader(driver);
            this.version = version;
            InitializeComponent();
            GoogleApi.Checkforupdates(version, false);
            refreshgoogledata();
        }

        void refreshgoogledata()
        {
            this.selectedcover = null;
            this.selectedmanga = null;
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox2.Items.Add("-");

            this.mangas = GoogleApi.GetData("series!A2:B");
            this.covers = GoogleApi.GetData("covers!A2:D");

            foreach (var value in mangas)
            {
                comboBox1.Items.Add(value[1]);
            }
        }
        void reset()
        {
            refreshgoogledata();
            this.files = null;
            label3.Text = "Bir dosya seçin ya da sürükleyip bırakın.";
            button1.Text = "Dosya Seç";
            button2.Text = "Yükle";
            yeniToolStripMenuItem.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.files == null)
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

                button1.Text = "Temizle";
                button2.Enabled = true;
            }
            else
            {
                this.files = null;
                label3.Text = "Bir dosya seçin ya da sürükleyip bırakın.";
                button1.Text = "Bölüm Seç";
                button2.Enabled = false;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            yeniToolStripMenuItem.Enabled = false;
            button2.Text = "Yükleniyor...";
            backgroundWorker.RunWorkerAsync();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            refreshgoogledata();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.selectedcover = null;
            comboBox2.Items.Clear();
            comboBox2.Items.Add("-");

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

        private void label3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Seçeceğiniz dosya tüm bölümleri içeren bir klasör olmalıdır. Tüm bölümleri içeren klasörlerin adları bölüm numaraları şeklinde olmalıdır.");
        }

        // Menüler
        private void çıkışYapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void kullanıcıDeğiştirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Restart();
        }
        private void güncelleştirmeleriDenetleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoogleApi.Checkforupdates(version, true);
        }
        private void hakkındaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Turktoon Upload Bot Sürüm " + this.version + "\n\nGhostPet tarafından yapılmıştır. Tüm hakları saklıdır.");
        }
        private void mangaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            this.label4text = "Ayarların kapatılması bekleniyor...";
            AddManga newMDIChild = new AddManga(this.driver, this.GoogleApi);
            newMDIChild.Show();
            reset();
        }
        private void kapakToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            this.label4text = "Ayarların kapatılması bekleniyor...";
            AddCover newMDIChild = new AddCover(this.uploader, this.GoogleApi, this.mangas);
            newMDIChild.Show();
            reset();
        }

        // Sürükle Bırak Dosya Seç
        private void MainUi_DragEnter(object sender, DragEventArgs e)
        {
            label3.Text = "Klasörü buraya bırakın.";
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
        private void MainUi_DragDrop(object sender, DragEventArgs e)
        {
            if (this.files != null)
            {
                DialogResult dialogResult = MessageBox.Show("Zaten başka bir bölüm seçilmiş, değiştirmek istiyor musun?", "Uyarı Kutusu", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }
            string[] dropfiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            
            if (dropfiles.Length == 1 && Directory.Exists(dropfiles[0]))
            {
                foreach (string item in Directory.GetDirectories(dropfiles[0]))
                {
                    if (Directory.Exists(item))
                    {
                        this.files = Directory.GetDirectories(dropfiles[0]);
                        break;
                    }
                }
                this.files = dropfiles;
            }
            else if (dropfiles.Length == 1)
            {
                label3.Text = "Canım benim, buraya resim değil, dosya atacaksın.";
                return;
            }
            else
            {
                foreach (string filename in dropfiles)
                {
                    if (!Directory.Exists(filename))
                    {
                        label3.Text = "Canım benim, buraya SADECE klasör atacaksın.";
                        return;
                    }
                }

                this.files = dropfiles;
            }

            label3.Text = this.files.Length + " adet bölüm paylaşılacak.";
            button1.Text = "Temizle";
            button2.Enabled = true;
        }
        private void MainUi_DragLeave(object sender, EventArgs e)
        {
            label3.Text = "Bir dosya seçin ya da sürükleyip bırakın.";
        }

        // Bölüm Paylaşma
        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int a = 0;

            foreach (String file in this.files)
            {
                String[] temp = file.Split("\\".ToCharArray());
                String filename = temp[temp.Length - 1];

                this.label4text = a + 1 + "/" + this.files.Length + ": " + filename + ". bölüm paylaşılıyor...";
                backgroundWorker.ReportProgress((int)Math.Round((double)(100 * a) / this.files.Length));
                this.uploader.Share(this.selectedmanga, this.selectedcover, filename, file, backgroundWorker, a, this.files.Length);
                a += 1;
            }

            this.label4text = "Tamamlandı.";
            backgroundWorker.ReportProgress(100);
        }
        private void backgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            label4.Text = this.label4text;
        }
        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            reset();
        }
    }
}
