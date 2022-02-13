using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MangaUploadBot
{
    public partial class AddCover : Form
    {
        Uploader uploader;
        GoogleApi GoogleApi;
        IList<IList<object>> mangas;

        public AddCover(Uploader u, GoogleApi api, IList<IList<Object>> mangas)
        {
            this.uploader = u;
            this.GoogleApi = api;
            this.mangas = mangas;
            InitializeComponent();
            foreach (IList<object> manga in mangas)
            {
                comboBox1.Items.Add(manga[1]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string uploadfile = openFileDialog1.FileName;
                textBox3.Text = this.uploader.CoverUpload(uploadfile);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IList<IList<object>> data = new List<IList<object>>();
            data.Add(new List<object>() { comboBox1.SelectedItem, textBox1.Text, "", textBox3.Text });
            GoogleApi.SetData(data, "covers!A1");
            this.Close();
        }
    }
}
