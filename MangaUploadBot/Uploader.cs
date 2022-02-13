using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;

namespace MangaUploadBot
{
    public class Uploader
    {
        IWebDriver driver;

        public Uploader(Driver d)
        {
            this.driver = d.driver;
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

        public string CoverUpload(String filepath)
        {
            // Resim Yükleme Sayfasını Aç
            this.driver.Navigate().GoToUrl("https://turktoon.com/wp-admin/media-new.php");

            // Siteye Yükle
            this.driver.FindElement(By.XPath("//input[starts-with(@id,'html5_')]")).SendKeys(filepath);

            // Yüklenen dosyayı bul
            IWebElement row = this.driver.FindElement(By.Id("media-items")).FindElement(By.ClassName("media-item"));

            // Yüklemesi bitti mi kontrol et.
            string imglink;
            while (true)
            {
                try
                {
                    IWebElement pinkynail = row.FindElement(By.ClassName("pinkynail"));
                    imglink = pinkynail.GetAttribute("src");
                    break;
                }
                catch
                {
                    System.Threading.Thread.Sleep(200);
                }
            }

            return "<img src=\"" + imglink + "\" alt = \"\" class=\"aligncenter size-full\" />";
        }

        public void Share(IList<Object> manga, IList<Object> cover, String filename, String filepath, System.ComponentModel.BackgroundWorker backgroundWorker2, int a, int total)
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

            //------- Yükleme Yüzdesi +0,05
            backgroundWorker2.ReportProgress((int)Math.Round((double)(100 * (a + 0.05)) / total));

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
                double temppercent = tempint / rows.Count / 2.5f;
                backgroundWorker2.ReportProgress((int)Math.Round((double)(100 * (a + 0.1 + temppercent) / total)));
            }

            // Yükleme Sayfasını Aç
            this.driver.Navigate().GoToUrl("https://turktoon.com/wp-admin/post-new.php?post_type=post&ts_add_chapter=" + manga[0]);

            //------- Yükleme Yüzdesi +0,6
            backgroundWorker2.ReportProgress((int)Math.Round((double)(100 * (a + 0.6) / total)));

            // Bölüm Bilgilerini Doldur
            this.driver.FindElement(By.CssSelector("#title")).SendKeys("#" + filename);
            this.driver.FindElement(By.Id("ero_chapter")).SendKeys(filename);
            try { this.driver.FindElement(By.ClassName("switch-html")).Click(); }
            catch { System.Threading.Thread.Sleep(10); }

            //------- Yükleme Yüzdesi +0,65
            backgroundWorker2.ReportProgress((int)Math.Round((double)(100 * (a + 0.65) / total)));

            // Bölüm Kapağını (Var ise) Ekle
            if (cover != null)
            {
                this.driver.FindElement(By.Id("content")).SendKeys(cover[3].ToString() + Keys.Enter + Keys.Enter);
            }

            // Bölüm Resimlerini Ekle
            int tempint2 = 0;
            foreach (String imglink in imglinks)
            {
                this.driver.FindElement(By.Id("content")).SendKeys("<img src=\"" + imglink + "\" alt = \"\" class=\"aligncenter size-full\" />" + Keys.Enter + Keys.Enter);
                tempint++;
                double temppercent2 = tempint2 / imglinks.Count / 5;
                backgroundWorker2.ReportProgress((int)Math.Round((double)(100 * (a + 0.7 + temppercent2) / total)));
            }

            //------- Yükleme Yüzdesi +0,9
            backgroundWorker2.ReportProgress((int)Math.Round((double)(100 * (a + 0.95) / total)));

            // Paylaş
            this.driver.FindElement(By.Name("publish")).Click();
            System.Threading.Thread.Sleep(200);
        }

    }
}
