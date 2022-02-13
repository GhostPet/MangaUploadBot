using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;

namespace MangaUploadBot
{
    public class User
    {
        public String UserName;
        public String Password;

        public User(String u, String p)
        {
            this.UserName = u;
            this.Password = p;
        }
    }

    public class Sharer
    {
        IWebDriver driver;

        public Sharer (IWebDriver d)
        {
            this.driver = d;
        }

        public void CheckImageNames(String filepath)
        {
            String[] imgdirs = Directory.GetFiles(filepath);

            // İsimde sayı harici karakter var mı kontrol et
            List<int> chapvals = new List<int>();
            foreach (String img in imgdirs)
            {
                String[] temp = img.Split("\\".ToCharArray());
                String[] filename = temp[temp.Length - 1].Split(".".ToCharArray());
                if (filename.Length > 2)
                {
                    return;
                }
                try
                {
                    chapvals.Add(int.Parse(filename[0]));
                }
                catch
                {
                    return;
                }
            }

            // En büyük sayıyı bul
            int maxval = chapvals[0];
            foreach (int chap in chapvals)
            {
                if (maxval < chap)
                {
                    maxval = chap;
                }
            }

            // Düzelt
            foreach (String img in imgdirs)
            {
                String[] temp = img.Split("\\".ToCharArray());
                String filename = temp[temp.Length - 1];
                String basepath = "";
                foreach (String temp2 in temp)
                {
                    if (temp2 != filename)
                    {
                        basepath += temp2 + "\\";
                    }
                }

                String[] temp3 = filename.Split(".".ToCharArray());
                String filetype = temp3[1];
                int filenum = int.Parse(temp3[0]);

                File.Move(basepath + filename, basepath + filenum.ToString("D" + maxval.ToString().Length) + "." + filetype);
            }
            return;
        }

        public void Share (IList<Object> manga, IList<Object> cover, String filename, String filepath, System.ComponentModel.BackgroundWorker backgroundWorker2, int a, int total)
        {
            // Resim Yükleme Sayfasını Aç
            this.driver.Navigate().GoToUrl("https://turktoon.com/wp-admin/media-new.php");

            // Resimleri Kontrol Et ve Düzelt
            CheckImageNames(filepath);

            // Resimlerin Konumlarını Listele
            String[] imgdirs = Directory.GetFiles(filepath);

            // Hepsini Sırasıyla Siteye Yükle
            foreach (String imgdir in imgdirs)
            {
                this.driver.FindElement(By.XPath("//input[starts-with(@id,'html5_')]")).SendKeys(imgdir);
            }

            //------- Yükleme Yüzdesi +0,4
            backgroundWorker2.ReportProgress((int)Math.Round((double)(100 * (a + 0.1)) / total));

            // Resimlerin yüklendiği tabloda sıraları bul
            IReadOnlyCollection<IWebElement> rows = this.driver.FindElement(By.Id("media-items")).FindElements(By.ClassName("media-item"));

            // Tek tek yüklemesi bitti mi kontrol et ve linkini al.
            int tempint = 0;
            List<string> imglinks = new List<string>();
            foreach (IWebElement row in rows)
            {
                while (true)
                {
                    try
                    {
                        IWebElement pinkynail = row.FindElement(By.ClassName("pinkynail"));
                        imglinks.Add(pinkynail.GetAttribute("src"));
                        break;
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep(200);
                    }
                }
                tempint++;
                double temppercent = tempint / rows.Count * 3 / 10;
                backgroundWorker2.ReportProgress((int)Math.Round((double)(100 * (a + 0.6 + temppercent) / total)));
            }

            //------- Yükleme Yüzdesi +0,7
            backgroundWorker2.ReportProgress((int)Math.Round((double)(100 * (a + 0.4) / total)));

            // Yükleme Sayfasını Aç
            this.driver.Navigate().GoToUrl("https://turktoon.com/wp-admin/post-new.php?post_type=post&ts_add_chapter=" + manga[0]);

            // Bölüm Bilgilerini Doldur
            this.driver.FindElement(By.CssSelector("#title")).SendKeys("#" + filename);
            this.driver.FindElement(By.Id("ero_chapter")).SendKeys(filename);
            try { this.driver.FindElement(By.ClassName("switch-html")).Click(); }
            catch { System.Threading.Thread.Sleep(10); }

            backgroundWorker2.ReportProgress((int)Math.Round((double)(100 * (a + 0.45) / total)));

            // Bölüm Kapağını (Var ise) Ekle
            if (cover != null)
            {
                this.driver.FindElement(By.Id("content")).SendKeys(cover[3].ToString() + Keys.Enter + Keys.Enter);
            }

            backgroundWorker2.ReportProgress((int)Math.Round((double)(100 * (a + 0.5) / total)));

            // Bölüm Resimlerini Ekle
            int tempint2 = 0;
            foreach (String imglink in imglinks)
            {
                this.driver.FindElement(By.Id("content")).SendKeys("<img src=\"" + imglink + "\" alt = \"\" class=\"aligncenter size-full\" />" + Keys.Enter + Keys.Enter);
                tempint++;
                double temppercent2 = tempint2 / imglinks.Count / 5;
                backgroundWorker2.ReportProgress((int)Math.Round((double)(100 * (a + 0.6 + temppercent2) / total)));
            }

            //------- Yükleme Yüzdesi +0,9
            backgroundWorker2.ReportProgress((int)Math.Round((double)(100 * (a + 0.9) / total)));

            // Paylaş
            this.driver.FindElement(By.Name("publish")).Click();
            System.Threading.Thread.Sleep(200);
        }

    }
}
