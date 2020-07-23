using System;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Net;
using System.IO;

namespace Otomasyon
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Veritabani veritababni = new Veritabani();

        OleDbConnection baglan;
        OleDbCommand komut;
        OleDbDataReader oku;

        string kategori;
        public string state;

        bool isimFarkli = true;
        bool kategoriFarkli = true;
        bool isimFarkli2 = true;
        bool kategoriFarkli2 = true;
        string seciliKotegoriGuncelle;

        void _Veritabani(ListView listView, ComboBox filtreComboBox, bool sayiFiltresiVar)
        {
            listView.Items.Clear();
            baglan.Open();
            if (filtreComboBox == null) komut = new OleDbCommand("Select * from UrunTakip", baglan);
            else if (filtreComboBox.SelectedIndex == -1 || filtreComboBox.Text == "Tümü") komut = new OleDbCommand("Select * from UrunTakip", baglan);
            else komut = new OleDbCommand("Select * from UrunTakip where UrunKategori = '" + filtreComboBox.Text + "'", baglan);
            oku = komut.ExecuteReader();
            while (oku.Read())
            {
                try
                {
                    if (Convert.ToInt32(oku["UrunStok"].ToString()) > 0 || !sayiFiltresiVar)
                    {
                        ListViewItem ekle = new ListViewItem();
                        ekle.Text = oku["UrunKategori"].ToString();
                        ekle.SubItems.Add(oku["UrunIsmi"].ToString());
                        ekle.SubItems.Add(oku["UrunStok"].ToString());
                        ekle.SubItems.Add(oku["UrunFiyat"].ToString() + " TL");
                        listView.Items.Add(ekle);
                    }
                }
                catch (Exception ex) { }
            }
            baglan.Close();
        }

        void _ComboBoxEkle(ComboBox comboBox, bool tumuVar)
        {
            comboBox.Items.Clear();
            if (tumuVar) comboBox.Items.Add("Tümü");
            baglan.Open();
            komut = new OleDbCommand("Select * from UrunTakip", baglan);
            oku = komut.ExecuteReader();
            while (oku.Read())
            {
                comboBox.Items.Remove(oku["UrunKategori"].ToString());
                comboBox.Items.Add(oku["UrunKategori"].ToString());
            }
            baglan.Close();
            comboBox.SelectedIndex = -1;
        }

        void LogEkle(string kategori, string isim, string fiyat, string alimFiyat, string tarih, string islem)
        {
            baglan.Open();
            komut.CommandText = "Insert into Log (UrunKategori, UrunIsmi, UrunFiyat, AlimFiyat, Tarih, Islem) values ('" + kategori + "', '" + isim + "', '" + fiyat + "', '" + alimFiyat + "', '" + tarih + "', '" + islem + "')";
            komut.Connection = baglan;
            komut.ExecuteNonQuery();
            baglan.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            baglan = new OleDbConnection(veritababni.konum);
            StokSil_panel.Visible = false;
            Guncelle_panel.Visible = false;
            UrunAlim_panel.Visible = false;
            UrunSatis_panel.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel5.Visible = false;
            if (state == "user")
            {
                UrunAlim_button.Visible = false;
                button4.Visible = false;
                button2.Visible = false;
                button6.Visible = false;
                button7.Visible = false;
                button9.Visible = false;
                button11.Visible = false;
            }
        }

        private void UrunSatis_button_Click(object sender, EventArgs e)
        {
            label3.Text = "-";
            label4.Text = "-";
            label8.Text = "-";
            label9.Text = "-";
            textBox9.Text = "";
            _ComboBoxEkle(comboBox1, true);
            _Veritabani(UrunSatis_listView, comboBox1, true);
            StokSil_panel.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel5.Visible = false;
            panel4.Visible = false;
            Guncelle_panel.Visible = false;
            UrunAlim_panel.Visible = false;
            UrunSatis_panel.Visible = true;
            numericUpDown1.Maximum = 1;
            numericUpDown1.Value = 1;
        }

        private void UrunAlim_button_Click(object sender, EventArgs e)
        {
            _ComboBoxEkle(comboBox2, false);
            panel4.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel5.Visible = false;
            StokSil_panel.Visible = false;
            Guncelle_panel.Visible = false;
            UrunSatis_panel.Visible = false;
            UrunAlim_panel.Visible = true;
        }

        int satilmaMiktari;

        string alimFiyat;

        private void UrunuSat_button_Click(object sender, EventArgs e)
        {
            textBox9.Text = "";
            int yeniStok = Convert.ToInt32(label8.Text.Replace(" ", "")) - Convert.ToInt32(numericUpDown1.Value);
            baglan.Open();
            komut.CommandText = "Update UrunTakip set UrunStok = '" + yeniStok + "' where UrunIsmi = '" + label4.Text + "'";
            komut.Connection = baglan;
            komut.ExecuteNonQuery();

            komut = new OleDbCommand("Select * from UrunSatilan where UrunIsmi = '" + label4.Text + "'", baglan);
            oku = komut.ExecuteReader();
            while (oku.Read())
            {
                satilmaMiktari = Convert.ToInt32(oku["UrunSatilan"].ToString());
                alimFiyat = oku["AlisFiyat"].ToString();
            }
            baglan.Close();

            baglan.Open();
            komut.CommandText = "Update UrunSatilan set UrunSatilan = '" + (satilmaMiktari + Convert.ToInt32(numericUpDown1.Value)) + "' where UrunIsmi = '" + label4.Text + "'";
            komut.Connection = baglan;
            komut.ExecuteNonQuery();
            baglan.Close();

            for(int i = 0; i < numericUpDown1.Value; i++)
            {
                date = DateTime.Now.ToString("hh.mm-dd.MM.yy");
                LogEkle(label3.Text, label4.Text, label9.Text.Replace(" TL", ""), alimFiyat.Replace(" TL", ""), date, "Satış");
            }
            

            MessageBox.Show("'" + label4.Text + "' Adlı Ürün '" + numericUpDown1.Value + "' Adet Satıldı.\nToplam Fiyat = " + (float.Parse(label9.Text.Replace(" TL", "")) * Convert.ToInt32(numericUpDown1.Value.ToString())).ToString() + " TL", "Ürün Satıldı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _Veritabani(UrunSatis_listView, comboBox1, true);
            label3.Text = "-";
            label4.Text = "-";
            label8.Text = "-";
            label9.Text = "-";
            UrunSat_button.Enabled = false;
            numericUpDown1.Maximum = 1;
            numericUpDown1.Value = 1;
        }

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {
            comboBox2.Visible = radioButton1.Checked;
            textBox1.Visible = !radioButton1.Checked;
        }

        private void radioButton2_CheckedChanged_1(object sender, EventArgs e)
        {
            comboBox2.Visible = !radioButton2.Checked;
            textBox1.Visible = radioButton2.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            kategori = "";
            if (radioButton1.Checked) kategori = comboBox2.Text;
            if (radioButton2.Checked) kategori = textBox1.Text;
            if (kategori.Replace(" ", "") != "" && textBox2.Text.Replace(" ", "") != "" && textBox3.Text.Replace(" ", "") != "" && textBox4.Text.Replace(" ", "") != "")
            {
                bool gir = true;
                try
                {
                    Convert.ToInt32(textBox3.Text);
                    float.Parse(textBox4.Text.Replace(".", ","));
                    float.Parse(textBox10.Text.Replace(".", ","));
                }
                catch (Exception ex)
                {
                    gir = false;
                    MessageBox.Show("Stok ve Fiyat Bölümüne Sadece Sayı Girin.\nStok Bölümüne Yalnızca Tam Sayılar Girilebilir.", "Sadece Sayı Girin", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (gir)
                {
                    baglan.Open();
                    komut = new OleDbCommand("Select * from UrunTakip", baglan);
                    oku = komut.ExecuteReader();
                    while (oku.Read())
                    {
                        if (oku["UrunIsmi"].ToString() != textBox2.Text)
                        {
                            isimFarkli = true;
                        }
                        else
                        {
                            isimFarkli = false;
                            break;
                        }
                        if (oku["UrunKategori"].ToString() != textBox1.Text || radioButton1.Checked)
                        {
                            kategoriFarkli = true;
                        }
                        else
                        {
                            kategoriFarkli = false;
                            break;
                        }
                    }
                    baglan.Close();
                    if (kategoriFarkli)
                    {
                        if (isimFarkli)
                        {
                            if (kategori.Replace(" ", "") != "Tümü")
                            {
                                baglan.Open();
                                komut = new OleDbCommand("Insert into UrunTakip (UrunIsmi, UrunKategori, UrunFiyat, UrunStok, AlisFiyat) values ('" + textBox2.Text + "', '" + kategori + "', '" + float.Parse(textBox4.Text.Replace(".", ",")) + "', '" + float.Parse(textBox3.Text) + "', '" + float.Parse(textBox10.Text.Replace(".", ",")) + "')", baglan);
                                oku = komut.ExecuteReader();
                                baglan.Close();
                                baglan.Open();
                                komut = new OleDbCommand("Insert into UrunSatilan (UrunIsmi, UrunKategori, UrunFiyat, UrunSatilan, AlisFiyat) values ('" + textBox2.Text + "', '" + kategori + "', '" + float.Parse(textBox4.Text.Replace(".", ",")) + "', '" + 0 + "', '" + float.Parse(textBox10.Text.Replace(".", ",")) + "')", baglan);
                                oku = komut.ExecuteReader();
                                baglan.Close();
                                baglan.Open();
                                komut = new OleDbCommand("Insert into IadeTakip (UrunIsmi, Kategori, Fiyat, Stok) values ('" + textBox2.Text + "', '" + kategori + "', '" + float.Parse(textBox4.Text.Replace(".", ",")) + "', '" + 0 + " ')", baglan);
                                oku = komut.ExecuteReader();
                                baglan.Close();
                                textBox1.Clear();
                                textBox2.Clear();
                                textBox3.Clear();
                                textBox4.Clear();
                                textBox10.Clear();
                                comboBox2.SelectedIndex = -1;
                            }
                            else
                            {
                                MessageBox.Show("'Tümü' Kategori Adı Olarak Kullanılamaz.", "Bu Kategori Kullanılamaz", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            UrunAlim_panel.Visible = true;
                            UrunSatis_panel.Visible = false;
                            Guncelle_panel.Visible = false;
                            StokSil_panel.Visible = false;
                            _ComboBoxEkle(comboBox2, false);
                            MessageBox.Show("Ürün Başarıyla Eklendi", "Ürün Başarıyla Eklendi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("'" + textBox2.Text + "' İsimli Ürün Zaten Mevcut.\nAynı Ürün İsmi Birden Fazla Kez Kullanılamaz.", "Ürün İsmi Zaten Mevcut", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("'" + textBox1.Text + "' İsimli Kategori Zaten Mevcut\nLütfen Mevcut Kategoriler Arasından Seçim Yapın.", "Kategori Zaten Mevcut", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Bu Alanlar Boş Bırakılamaz", "Alanlar Boş Bırakılamaz", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _Veritabani(UrunSatis_listView, comboBox1, true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            groupBox2.Enabled = false;
            _ComboBoxEkle(comboBox3, false);
            _ComboBoxEkle(comboBox5, true);
            listView1.Items.Clear();
            baglan.Open();
            if (comboBox5 == null) komut = new OleDbCommand("Select * from UrunTakip", baglan);
            else if (comboBox5.SelectedIndex == -1 || comboBox5.Text == "Tümü") komut = new OleDbCommand("Select * from UrunTakip", baglan);
            else komut = new OleDbCommand("Select * from UrunTakip where UrunKategori = '" + comboBox5.Text + "'", baglan);
            oku = komut.ExecuteReader();
            while (oku.Read())
            {
                try
                {
                    ListViewItem ekle = new ListViewItem();
                    ekle.Text = oku["UrunKategori"].ToString();
                    ekle.SubItems.Add(oku["UrunIsmi"].ToString());
                    ekle.SubItems.Add(oku["UrunStok"].ToString());
                    ekle.SubItems.Add(oku["UrunFiyat"].ToString() + " TL");
                    ekle.SubItems.Add(oku["AlisFiyat"].ToString() + " TL");
                    listView1.Items.Add(ekle);
                }
                catch (Exception ex) { }
            }
            baglan.Close();
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel5.Visible = false;
            StokSil_panel.Visible = false;
            UrunAlim_panel.Visible = false;
            UrunSatis_panel.Visible = false;
            Guncelle_panel.Visible = true;
        }

        bool gir = true;

        private void button3_Click(object sender, EventArgs e)
        {
            seciliKotegoriGuncelle = "";
            if (radioButton4.Checked) seciliKotegoriGuncelle = comboBox3.Text;
            else if (radioButton3.Checked) seciliKotegoriGuncelle = textBox5.Text;

            gir = true;
            try
            {
                Convert.ToInt32(textBox7.Text);
                float.Parse(textBox8.Text.Replace(".", ","));
            }
            catch (Exception ex)
            {
                gir = false;
                MessageBox.Show("Stok ve Fiyat Bölümüne Sadece Sayı Girin.\nStok Bölümüne Yalnızca Tam Sayılar Girilebilir.", "Sadece Sayı Girin", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (gir)
            {
                if (seciliKotegoriGuncelle.Replace(" ", "") != "" && textBox6.Text.Replace(" ", "") != "" && textBox7.Text.Replace(" ", "") != "" && textBox8.Text.Replace(" ", "") != "")
                {
                    baglan.Open();
                    komut = new OleDbCommand("Select * from UrunTakip", baglan);
                    oku = komut.ExecuteReader();
                    while (oku.Read())
                    {
                        if (oku["UrunIsmi"].ToString() != textBox6.Text || oku["UrunIsmi"].ToString() == listView1.SelectedItems[0].SubItems[1].Text)
                        {
                            isimFarkli2 = true;
                        }
                        else
                        {
                            isimFarkli2 = false;
                            break;
                        }
                        if (oku["UrunKategori"].ToString() != textBox5.Text)
                        {
                            kategoriFarkli2 = true;
                        }
                        else if (radioButton3.Checked)
                        {
                            kategoriFarkli2 = false;
                            break;
                        }
                    }
                    baglan.Close();
                    if (kategoriFarkli2)
                    {
                        if (isimFarkli2)
                        {
                            baglan.Open();
                            komut = new OleDbCommand("Update UrunTakip set UrunKategori = '" + seciliKotegoriGuncelle + "', UrunIsmi = '" + textBox6.Text + "', UrunFiyat = '" + float.Parse(textBox8.Text.Replace(".", ",")) + "', UrunStok = '" + float.Parse(textBox7.Text) + "', AlisFiyat = '" + float.Parse(textBox11.Text) + "' where UrunIsmi = '" + listView1.SelectedItems[0].SubItems[1].Text + "'", baglan);
                            oku = komut.ExecuteReader();

                            komut = new OleDbCommand("Update IadeTakip set Kategori = '" + seciliKotegoriGuncelle + "', UrunIsmi = '" + textBox6.Text + "', Fiyat = '" + float.Parse(textBox8.Text.Replace(".", ",")) + "' where UrunIsmi = '" + listView1.SelectedItems[0].SubItems[1].Text + "'", baglan);
                            oku = komut.ExecuteReader();

                            komut = new OleDbCommand("Update UrunSatilan set UrunKategori = '" + seciliKotegoriGuncelle + "', UrunIsmi = '" + textBox6.Text + "', UrunFiyat = '" + float.Parse(textBox8.Text.Replace(".", ",")) + "', AlisFiyat = '" + float.Parse(textBox11.Text) + "' where UrunIsmi = '" + listView1.SelectedItems[0].SubItems[1].Text + "'", baglan);
                            oku = komut.ExecuteReader();
                            baglan.Close();
                            textBox5.Text = "";
                            textBox6.Text = "";
                            textBox7.Text = "";
                            textBox8.Text = "";
                            textBox11.Text = "";
                            groupBox2.Enabled = false;
                            button3.Enabled = false;

                            _Veritabani(listView1, null, false);
                            _ComboBoxEkle(comboBox3, false);
                            _ComboBoxEkle(comboBox5, true);
                        }
                        else
                        {
                            MessageBox.Show("'" + textBox6.Text + "' İsimli Ürün Zaten Mevcut.\nAynı Ürün İsmi Birden Fazla Kez Kullanılamaz.", "Ürün İsmi Zaten Mevcut", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("'" + textBox5.Text + "' İsimli Kategori Zaten Mevcut\nLütfen Mevcut Kategoriler Arasından Seçim Yapın.", "Kategori Zaten Mevcut", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Bu Alanlar Boş Bırakılamaz", "Alanlar Boş Bırakılamaz", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            textBox5.Visible = !radioButton4.Checked;
            comboBox3.Visible = radioButton4.Checked;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            textBox5.Visible = radioButton3.Checked;
            comboBox3.Visible = !radioButton3.Checked;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _ComboBoxEkle(comboBox4, true);
            _Veritabani(listView2, null, false);
            panel5.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            UrunAlim_panel.Visible = false;
            UrunSatis_panel.Visible = false;
            Guncelle_panel.Visible = false;
            StokSil_panel.Visible = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button5.Enabled = false;
            baglan.Open();
            komut = new OleDbCommand("Delete from UrunTakip where UrunIsmi = '" + label25.Text + "'", baglan);
            komut.ExecuteNonQuery();

            komut = new OleDbCommand("Delete from IadeTakip where UrunIsmi = '" + label25.Text + "'", baglan);
            komut.ExecuteNonQuery();

            komut = new OleDbCommand("Delete from UrunSatilan where UrunIsmi = '" + label25.Text + "'", baglan);
            komut.ExecuteNonQuery();
            baglan.Close();
            MessageBox.Show("'" + label25.Text + "' Adlı Ürün Başarıyla Silindi.", "Ürün Silindi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _ComboBoxEkle(comboBox4, true);
            _Veritabani(listView2, comboBox4, false);
            label19.Text = "-";
            label25.Text = "-";
            label22.Text = "-";
            label24.Text = "-";
        }

        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            label19.Text = listView2.SelectedItems[0].SubItems[0].Text.ToString();
            label25.Text = listView2.SelectedItems[0].SubItems[1].Text.ToString();
            label22.Text = listView2.SelectedItems[0].SubItems[2].Text.ToString();
            label24.Text = listView2.SelectedItems[0].SubItems[3].Text.ToString();
            button5.Enabled = true;
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            _Veritabani(listView2, comboBox4, false);
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.SelectedIndex = -1;
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            textBox11.Text = "";
            groupBox2.Enabled = false;
            listView1.Items.Clear();
            baglan.Open();
            if (comboBox5 == null) komut = new OleDbCommand("Select * from UrunTakip", baglan);
            else if (comboBox5.SelectedIndex == -1 || comboBox5.Text == "Tümü") komut = new OleDbCommand("Select * from UrunTakip", baglan);
            else komut = new OleDbCommand("Select * from UrunTakip where UrunKategori = '" + comboBox5.Text + "'", baglan);
            oku = komut.ExecuteReader();
            while (oku.Read())
            {
                try
                {
                    ListViewItem ekle = new ListViewItem();
                    ekle.Text = oku["UrunKategori"].ToString();
                    ekle.SubItems.Add(oku["UrunIsmi"].ToString());
                    ekle.SubItems.Add(oku["UrunStok"].ToString());
                    ekle.SubItems.Add(oku["UrunFiyat"].ToString() + " TL");
                    ekle.SubItems.Add(oku["AlisFiyat"].ToString() + " TL");
                    listView1.Items.Add(ekle);
                }
                catch (Exception ex) { }
            }
            baglan.Close();
        }

        private void label28_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(readPage("Iletisim"), "İletişim Bilgileri", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lütfen İnternet Bağlantınızı Kontrol Edin.", "İnternet Bağlantı Sorunu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listView1_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
            button3.Enabled = true;
            groupBox2.Enabled = true;
            radioButton4.Checked = true;
            comboBox3.SelectedItem = listView1.SelectedItems[0].SubItems[0].Text;
            textBox5.Text = listView1.SelectedItems[0].SubItems[0].Text;
            textBox6.Text = listView1.SelectedItems[0].SubItems[1].Text;
            textBox7.Text = listView1.SelectedItems[0].SubItems[2].Text.Replace(" ", "");
            textBox8.Text = listView1.SelectedItems[0].SubItems[3].Text.Replace(" TL", "");
            textBox11.Text = listView1.SelectedItems[0].SubItems[4].Text.Replace(" TL", "");
        }

        private void UrunSatis_listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            label3.Text = UrunSatis_listView.SelectedItems[0].SubItems[0].Text.ToString();
            label4.Text = UrunSatis_listView.SelectedItems[0].SubItems[1].Text.ToString();
            label8.Text = UrunSatis_listView.SelectedItems[0].SubItems[2].Text.ToString();
            label9.Text = UrunSatis_listView.SelectedItems[0].SubItems[3].Text.ToString();
            numericUpDown1.Maximum = Convert.ToInt32(label8.Text);
            numericUpDown1.Value = 1;
            UrunSat_button.Enabled = true;
        }

        string okunan = null;
        char sonOkunacak = '-';
        string gelen = "";
        int sonI = 0;
        bool eslesti = false;
        string url = "http://www.ticariotomasyon.c1.biz/Iletisim.html";
        string date;

        public string readPage(string ara)
        {
            try
            {
                ara = "i." + ara;
                okunan = null;
                WebRequest istek = WebRequest.Create(url);
                WebResponse cevap = istek.GetResponse();
                StreamReader donenBilgiler = new StreamReader(cevap.GetResponseStream());
                gelen = donenBilgiler.ReadToEnd();

                for (int i = 0; i < gelen.Length; i++)
                {
                    if (gelen[i] == ara[0])
                    {
                        for (int j = 0; j < ara.Length; j++)
                        {
                            if (gelen[i + j] == ara[j])
                            {
                                eslesti = true;
                                sonI = i + j;
                            }
                            else
                            {
                                eslesti = false;
                                break;
                            }
                        }
                        if (eslesti) break;
                    }
                }
                if (eslesti)
                {
                    for (int i = 2; i < gelen.Length; i++)
                    {
                        if (gelen[i + sonI] == sonOkunacak)
                        {
                            break;
                        }
                        okunan += gelen[i + sonI];
                    }
                    return okunan.Replace("\\n", "\n");
                }
                else return "0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lütfen İnternet Bağlantınızı Kontrol Edin.", "İnternet Bağlantı Sorunu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            panel3.Visible = false;
            panel4.Visible = false;
            UrunAlim_panel.Visible = false;
            panel5.Visible = false;
            UrunSatis_panel.Visible = false;
            Guncelle_panel.Visible = false;
            StokSil_panel.Visible = false;
            panel2.Visible = true;
            _ComboBoxEkle(comboBox6, true);
            listView3.Items.Clear();
            float toplam = 0;
            float kar = 0;
            baglan.Open();
            if (comboBox6 == null) komut = new OleDbCommand("Select * from UrunSatilan", baglan);
            else if (comboBox6.SelectedIndex == -1 || comboBox6.Text == "Tümü") komut = new OleDbCommand("Select * from UrunSatilan", baglan);
            else komut = new OleDbCommand("Select * from UrunSatilan where UrunKategori = '" + comboBox6.Text + "'", baglan);
            oku = komut.ExecuteReader();
            while (oku.Read())
            {
                if (Convert.ToInt32(oku["UrunSatilan"].ToString()) > 0)
                {
                    ListViewItem ekle = new ListViewItem();
                    ekle.Text = oku["UrunKategori"].ToString();
                    ekle.SubItems.Add(oku["UrunIsmi"].ToString());
                    ekle.SubItems.Add(oku["UrunSatilan"].ToString());
                    ekle.SubItems.Add(oku["UrunFiyat"].ToString() + " TL");
                    toplam += Convert.ToInt32(oku["UrunSatilan"].ToString()) * float.Parse(oku["UrunFiyat"].ToString());
                    kar += (float.Parse(oku["UrunFiyat"].ToString()) - Convert.ToInt32(oku["AlisFiyat"].ToString())) * Convert.ToInt32(oku["UrunSatilan"].ToString());
                    listView3.Items.Add(ekle);
                }
            }
            label42.Text = toplam + " TL";
            label53.Text = kar + " TL";
            baglan.Close();
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkBox1.Checked = false;
            checkBox2.Checked = false;
            SatilanVeriGetir(listView3, comboBox6, true);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;
                Sirala();
            }
            else if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
                TersSirala();
            }
            else
            {
                SatilanVeriGetir(listView3, comboBox6, true);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
                TersSirala();
            }
            else if (checkBox1.Checked)
            {
                checkBox2.Checked = false;
                Sirala();
            }
            else
            {
                SatilanVeriGetir(listView3, comboBox6, true);
            }
        }

        int yer = 0;

        void SatilanVeriGetir(ListView listView, ComboBox filtre, bool sayiFiltresiVar)
        {
            listView.Items.Clear();
            baglan.Open();
            if (filtre == null) komut = new OleDbCommand("Select * from UrunSatilan", baglan);
            else if (filtre.SelectedIndex == -1 || filtre.Text == "Tümü") komut = new OleDbCommand("Select * from UrunSatilan", baglan);
            else komut = new OleDbCommand("Select * from UrunSatilan where UrunKategori = '" + filtre.Text + "'", baglan);
            oku = komut.ExecuteReader();
            while (oku.Read())
            {
                if (Convert.ToInt32(oku["UrunSatilan"].ToString()) > 0 || !sayiFiltresiVar)
                {
                    ListViewItem ekle = new ListViewItem();
                    ekle.Text = oku["UrunKategori"].ToString();
                    ekle.SubItems.Add(oku["UrunIsmi"].ToString());
                    ekle.SubItems.Add(oku["UrunSatilan"].ToString());
                    ekle.SubItems.Add(oku["UrunFiyat"].ToString() + " TL");
                    listView.Items.Add(ekle);
                }
            }
            baglan.Close();

        }

        void Sirala()
        {
            yer = 0;
            if (listView3.Items.Count > 1)
            {
                while (true)
                {
                    if (Convert.ToInt32(listView3.Items[yer].SubItems[2].Text) < Convert.ToInt32(listView3.Items[yer + 1].SubItems[2].Text))
                    {
                        string[] a = new string[4];
                        a[0] = listView3.Items[yer].SubItems[0].Text;
                        a[1] = listView3.Items[yer].SubItems[1].Text;
                        a[2] = listView3.Items[yer].SubItems[2].Text;
                        a[3] = listView3.Items[yer].SubItems[3].Text;
                        listView3.Items[yer].SubItems[0].Text = listView3.Items[yer + 1].SubItems[0].Text;
                        listView3.Items[yer].SubItems[1].Text = listView3.Items[yer + 1].SubItems[1].Text;
                        listView3.Items[yer].SubItems[2].Text = listView3.Items[yer + 1].SubItems[2].Text;
                        listView3.Items[yer].SubItems[3].Text = listView3.Items[yer + 1].SubItems[3].Text;
                        listView3.Items[yer + 1].SubItems[0].Text = a[0];
                        listView3.Items[yer + 1].SubItems[1].Text = a[1];
                        listView3.Items[yer + 1].SubItems[2].Text = a[2];
                        listView3.Items[yer + 1].SubItems[3].Text = a[3];
                        yer = 0;
                    }
                    else
                    {
                        if (yer + 2 == listView3.Items.Count)
                        {
                            break;
                        }
                        else
                        {
                            yer++;
                        }
                    }
                }
            }
        }

        void TersSirala()
        {
            yer = 0;
            if (listView3.Items.Count > 1)
            {
                while (true)
                {
                    if (Convert.ToInt32(listView3.Items[yer].SubItems[2].Text) > Convert.ToInt32(listView3.Items[yer + 1].SubItems[2].Text))
                    {
                        string[] a = new string[4];
                        a[0] = listView3.Items[yer].SubItems[0].Text;
                        a[1] = listView3.Items[yer].SubItems[1].Text;
                        a[2] = listView3.Items[yer].SubItems[2].Text;
                        a[3] = listView3.Items[yer].SubItems[3].Text;
                        listView3.Items[yer].SubItems[0].Text = listView3.Items[yer + 1].SubItems[0].Text;
                        listView3.Items[yer].SubItems[1].Text = listView3.Items[yer + 1].SubItems[1].Text;
                        listView3.Items[yer].SubItems[2].Text = listView3.Items[yer + 1].SubItems[2].Text;
                        listView3.Items[yer].SubItems[3].Text = listView3.Items[yer + 1].SubItems[3].Text;
                        listView3.Items[yer + 1].SubItems[0].Text = a[0];
                        listView3.Items[yer + 1].SubItems[1].Text = a[1];
                        listView3.Items[yer + 1].SubItems[2].Text = a[2];
                        listView3.Items[yer + 1].SubItems[3].Text = a[3];
                        yer = 0;
                    }
                    else
                    {
                        if (yer + 2 == listView3.Items.Count)
                        {
                            break;
                        }
                        else
                        {
                            yer++;
                        }
                    }
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            label26.Text = "-";
            label34.Text = "-";
            label27.Text = "-";
            label32.Text = "-";
            numericUpDown2.Maximum = 1;
            panel5.Visible = false;
            button8.Enabled = false;
            StokSil_panel.Visible = false;
            panel4.Visible = false;
            Guncelle_panel.Visible = false;
            UrunAlim_panel.Visible = false;
            UrunSatis_panel.Visible = false;
            panel2.Visible = false;
            panel3.Visible = true;
            _ComboBoxEkle(comboBox7, true);
            SatilanVeriGetir(listView4, comboBox7, true);
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            SatilanVeriGetir(listView4, comboBox7, true);
        }

        private void listView4_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            label26.Text = listView4.SelectedItems[0].SubItems[0].Text.ToString();
            label34.Text = listView4.SelectedItems[0].SubItems[1].Text.ToString();
            label32.Text = listView4.SelectedItems[0].SubItems[3].Text.ToString();
            baglan.Open();
            komut = new OleDbCommand("Select * from UrunSatilan where UrunIsmi = '" + label34.Text + "'", baglan);
            oku = komut.ExecuteReader();
            while (oku.Read())
            {
                label27.Text = oku["UrunSatilan"].ToString() + " Tane";
            }
            baglan.Close();
            numericUpDown2.Maximum = Convert.ToInt32(label27.Text.Replace(" Tane", ""));
            numericUpDown2.Value = 1;
            button8.Enabled = true;
        }

        int stok = 0;
        int stok2 = 0;
        string alimF;

        private void button8_Click(object sender, EventArgs e)
        {
            button8.Enabled = false;
            if (numericUpDown2.Value != 0)
            {
                baglan.Open();
                komut = new OleDbCommand("Select * from IadeTakip where UrunIsmi = '" + label34.Text + "'", baglan);
                oku = komut.ExecuteReader();
                while (oku.Read())
                {
                    stok2 = Convert.ToInt32(oku["Stok"].ToString());
                }
                komut = new OleDbCommand("Update IadeTakip set Stok = '" + (stok2 + numericUpDown2.Value) + "' where UrunIsmi = '" + label34.Text + "'", baglan);
                komut.ExecuteNonQuery();
                baglan.Close();
                baglan.Open();
                komut = new OleDbCommand("Select * from UrunTakip where UrunIsmi = '" + label34.Text + "'", baglan);
                oku = komut.ExecuteReader();
                while (oku.Read())
                {
                    stok = Convert.ToInt32(oku["UrunStok"].ToString());
                    alimF = oku["AlisFiyat"].ToString();
                }
                komut = new OleDbCommand("Update UrunTakip set UrunStok = '" + (stok + numericUpDown2.Value) + "' where UrunIsmi = '" + label34.Text + "'", baglan);
                komut.ExecuteNonQuery();
                komut = new OleDbCommand("Update UrunSatilan set UrunSatilan = '" + (Convert.ToInt32(label27.Text.Replace(" Tane", "")) - numericUpDown2.Value) + "' where UrunIsmi = '" + label34.Text + "'", baglan);
                komut.ExecuteNonQuery();
                baglan.Close();

                MessageBox.Show("'" + label34.Text + "' Adlı Ürün '" + numericUpDown2.Value + "' Tane İade Edildi\nAlınacak Ücret: '" + (Convert.ToInt32(numericUpDown2.Value) * float.Parse(label32.Text.Replace(" TL", ""))) + "' TL", "İade Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SatilanVeriGetir(listView4, comboBox7, true);
                date = DateTime.Now.ToString("hh.mm-dd.MM.yy");
                LogEkle(label26.Text, label34.Text, label32.Text.Replace(" TL", ""), alimF, date, "İade");
                label26.Text = "-";
                label34.Text = "-";
                label27.Text = "-";
                label32.Text = "-";
                numericUpDown2.Maximum = 1;

            }
            else
            {
                MessageBox.Show("Lütfen İade Edilecek Miktarı Seçiniz.", "İade Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            if (textBox9.Text.Replace(" ", "") == "")
            {
                _Veritabani(UrunSatis_listView, comboBox1, true);
            }
            else
            {
                UrunSatis_listView.Items.Clear();
                baglan.Open();
                if (UrunSatis_listView == null) komut = new OleDbCommand("Select * from UrunTakip", baglan);
                else if (comboBox1.SelectedIndex == -1 || comboBox1.Text == "Tümü") komut = new OleDbCommand("Select * from UrunTakip", baglan);
                else komut = new OleDbCommand("Select * from UrunTakip where UrunKategori = '" + comboBox1.Text + "'", baglan);
                oku = komut.ExecuteReader();
                while (oku.Read())
                {
                    if (textBox9.TextLength >= oku["UrunIsmi"].ToString().Length)
                    {
                        if (textBox9.Text.ToLower().Replace("ı", "i") == oku["UrunIsmi"].ToString().ToLower().Replace("ı", "i"))
                        {
                            ListViewItem ekle = new ListViewItem();
                            ekle.Text = oku["UrunKategori"].ToString();
                            ekle.SubItems.Add(oku["UrunIsmi"].ToString());
                            ekle.SubItems.Add(oku["UrunStok"].ToString());
                            ekle.SubItems.Add(oku["UrunFiyat"].ToString() + " TL");
                            UrunSatis_listView.Items.Add(ekle);
                        }
                    }
                    else if (textBox9.Text.ToLower().Replace("ı", "i") == oku["UrunIsmi"].ToString().Remove(textBox9.TextLength).ToLower().Replace("ı", "i"))
                    {
                        ListViewItem ekle = new ListViewItem();
                        ekle.Text = oku["UrunKategori"].ToString();
                        ekle.SubItems.Add(oku["UrunIsmi"].ToString());
                        ekle.SubItems.Add(oku["UrunStok"].ToString());
                        ekle.SubItems.Add(oku["UrunFiyat"].ToString() + " TL");
                        UrunSatis_listView.Items.Add(ekle);
                    }
                }
                baglan.Close();
            }
        }

        private void UrunSatis_listView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                label3.Text = UrunSatis_listView.SelectedItems[0].SubItems[0].Text.ToString();
                label4.Text = UrunSatis_listView.SelectedItems[0].SubItems[1].Text.ToString();
                label8.Text = UrunSatis_listView.SelectedItems[0].SubItems[2].Text.ToString();
                label9.Text = UrunSatis_listView.SelectedItems[0].SubItems[3].Text.ToString();
                numericUpDown1.Maximum = Convert.ToInt32(label8.Text);
                numericUpDown1.Value = 1;
                UrunSat_button.Enabled = true;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            _ComboBoxEkle(comboBox8, true);
            panel5.Visible = false;
            StokSil_panel.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            Guncelle_panel.Visible = false;
            UrunAlim_panel.Visible = false;
            UrunSatis_panel.Visible = false;
            panel4.Visible = true;
            listView5.Items.Clear();
            baglan.Open();
            if (UrunSatis_listView == null) komut = new OleDbCommand("Select * from IadeTakip", baglan);
            else if (comboBox8.SelectedIndex == -1 || comboBox8.Text == "Tümü") komut = new OleDbCommand("Select * from IadeTakip", baglan);
            else komut = new OleDbCommand("Select * from IadeTakip where Kategori = '" + comboBox8.Text + "'", baglan);
            oku = komut.ExecuteReader();
            while (oku.Read())
            {
                if (Convert.ToInt32(oku["Stok"]) > 0)
                {
                    ListViewItem ekle = new ListViewItem();
                    ekle.Text = oku["Kategori"].ToString();
                    ekle.SubItems.Add(oku["UrunIsmi"].ToString());
                    ekle.SubItems.Add(oku["Stok"].ToString());
                    ekle.SubItems.Add(oku["Fiyat"].ToString() + " TL");
                    listView5.Items.Add(ekle);
                }
            }
            baglan.Close();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            button10.Enabled = false;
            baglan.Open();
            komut = new OleDbCommand("Select * from IadeTakip where UrunIsmi = '" + label47.Text + "'", baglan);
            oku = komut.ExecuteReader();
            while (oku.Read())
            {
                stok2 = Convert.ToInt32(oku["Stok"].ToString());
            }
            komut = new OleDbCommand("Update IadeTakip set Stok = '" + (stok2 - numericUpDown3.Value) + "' where UrunIsmi = '" + label47.Text + "'", baglan);
            komut.ExecuteNonQuery();

            listView5.Items.Clear();
            if (UrunSatis_listView == null) komut = new OleDbCommand("Select * from IadeTakip", baglan);
            else if (comboBox8.SelectedIndex == -1 || comboBox8.Text == "Tümü") komut = new OleDbCommand("Select * from IadeTakip", baglan);
            else komut = new OleDbCommand("Select * from IadeTakip where Kategori = '" + comboBox8.Text + "'", baglan);
            oku = komut.ExecuteReader();
            while (oku.Read())
            {
                if (Convert.ToInt32(oku["Stok"]) > 0)
                {
                    ListViewItem ekle = new ListViewItem();
                    ekle.Text = oku["Kategori"].ToString();
                    ekle.SubItems.Add(oku["UrunIsmi"].ToString());
                    ekle.SubItems.Add(oku["Stok"].ToString());
                    ekle.SubItems.Add(oku["Fiyat"].ToString() + " TL");
                    listView5.Items.Add(ekle);
                }
            }
            baglan.Close();
            label45.Text = "";
            label47.Text = "";
            label49.Text = "";
            label50.Text = "";
            numericUpDown3.Maximum = 1;
            numericUpDown3.Value = 1;
        }

        private void comboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView5.Items.Clear();
            baglan.Open();
            if (UrunSatis_listView == null) komut = new OleDbCommand("Select * from IadeTakip", baglan);
            else if (comboBox8.SelectedIndex == -1 || comboBox8.Text == "Tümü") komut = new OleDbCommand("Select * from IadeTakip", baglan);
            else komut = new OleDbCommand("Select * from IadeTakip where Kategori = '" + comboBox8.Text + "'", baglan);
            oku = komut.ExecuteReader();
            while (oku.Read())
            {
                if (Convert.ToInt32(oku["Stok"]) > 0)
                {
                    ListViewItem ekle = new ListViewItem();
                    ekle.Text = oku["Kategori"].ToString();
                    ekle.SubItems.Add(oku["UrunIsmi"].ToString());
                    ekle.SubItems.Add(oku["Stok"].ToString());
                    ekle.SubItems.Add(oku["Fiyat"].ToString() + " TL");
                    listView5.Items.Add(ekle);
                }
            }
            baglan.Close();
        }

        private void listView5_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            label45.Text = listView5.SelectedItems[0].SubItems[0].Text;
            label47.Text = listView5.SelectedItems[0].SubItems[1].Text;
            label49.Text = listView5.SelectedItems[0].SubItems[2].Text;
            numericUpDown3.Maximum = Convert.ToInt32(label49.Text);
            label50.Text = listView5.SelectedItems[0].SubItems[3].Text;
            button10.Enabled = true;
        }

        private void label56_Click(object sender, EventArgs e)
        {
            GirisEkrani giris = new GirisEkrani();
            giris.Show();
            this.Hide();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            comboBox10.Items.Clear();
            comboBox11.Items.Clear();
            comboBox12.Items.Clear();
            for (int i = 1; i < 32; i++)
            {
                comboBox10.Items.Add(i.ToString());
            }
            for (int i = 1; i < 13; i++)
            {
                comboBox11.Items.Add(i.ToString());
            }
            for (int i = 20; i < 30; i++)
            {
                comboBox12.Items.Add(i.ToString());
            }

            comboBox10.SelectedIndex = -1;
            comboBox11.SelectedIndex = -1;
            comboBox12.SelectedIndex = -1;
            
            comboBox9.Items.Clear();
            comboBox9.Items.Add("Tümü");
            baglan.Open();
            komut = new OleDbCommand("Select * from UrunTakip", baglan);
            oku = komut.ExecuteReader();
            while (oku.Read())
            {
                comboBox9.Items.Remove(oku["UrunKategori"].ToString());
                comboBox9.Items.Add(oku["UrunKategori"].ToString());
            }
            baglan.Close();
            comboBox9.SelectedIndex = -1;
            StokSil_panel.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            Guncelle_panel.Visible = false;
            UrunAlim_panel.Visible = false;
            UrunSatis_panel.Visible = false;
            panel4.Visible = false;
            panel5.Visible = true;
            listView6.Items.Clear();
            baglan.Open();
            if (comboBox9 == null) komut = new OleDbCommand("Select * from Log", baglan);
            else if (comboBox9.SelectedIndex == -1 || comboBox9.Text == "Tümü") komut = new OleDbCommand("Select * from Log", baglan);
            else komut = new OleDbCommand("Select * from Log where UrunKategori = '" + comboBox9.Text + "'", baglan);
            oku = komut.ExecuteReader();
            while (oku.Read())
            {
                try
                {
                    ListViewItem ekle = new ListViewItem();
                    ekle.Text = oku["UrunKategori"].ToString();
                    ekle.SubItems.Add(oku["UrunIsmi"].ToString());
                    ekle.SubItems.Add(oku["UrunFiyat"].ToString() + " TL");
                    ekle.SubItems.Add(oku["AlimFiyat"].ToString() + " TL");
                    ekle.SubItems.Add(oku["Tarih"].ToString());
                    ekle.SubItems.Add(oku["Islem"].ToString());
                    listView6.Items.Add(ekle);
                }
                catch (Exception ex) { }
            }
            baglan.Close();
            karHesapla();
        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView6.Items.Clear();
            baglan.Open();
            if (comboBox9 == null) komut = new OleDbCommand("Select * from Log", baglan);
            else if (comboBox9.SelectedIndex == -1 || comboBox9.Text == "Tümü") komut = new OleDbCommand("Select * from Log", baglan);
            else komut = new OleDbCommand("Select * from Log where UrunKategori = '" + comboBox9.Text + "'", baglan);
            oku = komut.ExecuteReader();
            while (oku.Read())
            {
                try
                {
                    ListViewItem ekle = new ListViewItem();
                    ekle.Text = oku["UrunKategori"].ToString();
                    ekle.SubItems.Add(oku["UrunIsmi"].ToString());
                    ekle.SubItems.Add(oku["UrunFiyat"].ToString() + " TL");
                    ekle.SubItems.Add(oku["AlimFiyat"].ToString() + " TL");
                    ekle.SubItems.Add(oku["Tarih"].ToString());
                    ekle.SubItems.Add(oku["Islem"].ToString());
                    listView6.Items.Add(ekle);
                }
                catch (Exception ex) { }
            }
            baglan.Close();
            karHesapla();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            listView6.Items.Clear();
            baglan.Open();
            if (comboBox9 == null) komut = new OleDbCommand("Select * from Log", baglan);
            else if (comboBox9.SelectedIndex == -1 || comboBox9.Text == "Tümü") komut = new OleDbCommand("Select * from Log", baglan);
            else komut = new OleDbCommand("Select * from Log where UrunKategori = '" + comboBox9.Text + "'", baglan);
            oku = komut.ExecuteReader();
            while (oku.Read())
            {
                try
                {
                    ListViewItem ekle = new ListViewItem();
                    ekle.Text = oku["UrunKategori"].ToString();
                    ekle.SubItems.Add(oku["UrunIsmi"].ToString());
                    ekle.SubItems.Add(oku["UrunFiyat"].ToString() + " TL");
                    ekle.SubItems.Add(oku["AlimFiyat"].ToString() + " TL");
                    ekle.SubItems.Add(oku["Tarih"].ToString());
                    ekle.SubItems.Add(oku["Islem"].ToString());
                    listView6.Items.Add(ekle);
                }
                catch (Exception ex) { }
            }
            baglan.Close();
            for (int j = 0; j < listView6.Items.Count; j++)
            {
                bool _eslesti = false;
                string gun = listView6.Items[j].SubItems[4].Text[6].ToString() + listView6.Items[j].SubItems[4].Text[7].ToString();
                string ay = listView6.Items[j].SubItems[4].Text[9].ToString() + listView6.Items[j].SubItems[4].Text[10].ToString();
                string yil = listView6.Items[j].SubItems[4].Text[12].ToString() + listView6.Items[j].SubItems[4].Text[13].ToString();
                _eslesti = true;

                if (comboBox10.SelectedIndex != -1)
                {
                    if (Convert.ToInt32(gun) != Convert.ToInt32(comboBox10.Text))
                    {
                        _eslesti = false;
                    }
                }
                if(comboBox11.SelectedIndex != -1 && _eslesti)
                {
                    if (Convert.ToInt32(ay) != Convert.ToInt32(comboBox11.Text))
                    {
                        _eslesti = false;
                    }
                }
                if(comboBox12.SelectedIndex != -1 && _eslesti)
                {
                    if (Convert.ToInt32(yil) != Convert.ToInt32(comboBox12.Text))
                    {
                        _eslesti = false;
                    }
                }

                if (!_eslesti)
                {
                    listView6.Items[j].Remove();
                    j--;
                }
                karHesapla();
            }
        }

        void karHesapla()
        {
            int _kar = 0;
            int _toplam = 0;

            for (int i = 0; i < listView6.Items.Count; i++)
            {
                if(listView6.Items[i].SubItems[5].Text == "Satış")
                {
                    _toplam += Convert.ToInt32(listView6.Items[i].SubItems[2].Text.Replace(" TL", ""));
                    _kar += Convert.ToInt32(listView6.Items[i].SubItems[2].Text.Replace(" TL", "")) - Convert.ToInt32(listView6.Items[i].SubItems[3].Text.Replace(" TL", ""));
                }
                else if(listView6.Items[i].SubItems[5].Text == "İade")
                {
                    _toplam -= Convert.ToInt32(listView6.Items[i].SubItems[2].Text.Replace(" TL", ""));
                    _kar -= Convert.ToInt32(listView6.Items[i].SubItems[2].Text.Replace(" TL", "")) - Convert.ToInt32(listView6.Items[i].SubItems[3].Text.Replace(" TL", ""));
                }
                
            }
            label62.Text = _kar + " TL";
            label64.Text = _toplam + " TL";
        }
    }
}
