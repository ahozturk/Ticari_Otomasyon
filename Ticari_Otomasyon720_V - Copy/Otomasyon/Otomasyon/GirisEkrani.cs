using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Otomasyon
{
    public partial class GirisEkrani : Form
    {
        public GirisEkrani()
        {
            InitializeComponent();
        }

        Veritabani veritababni = new Veritabani();
        Form1 form1 = new Form1();
        OleDbConnection baglan;
        OleDbCommand komut;
        OleDbDataReader oku;

        string kullaniciAdi, sifre;

        private void GirisEkrani_Load(object sender, EventArgs e)
        {
            panel2.Visible = false;
            baglan = new OleDbConnection(veritababni.konum);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel2.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                _VeriCek("user");
                if (textBox3.Text.Replace(" ", "") != "" && textBox4.Text.Replace(" ", "") != "" && textBox5.Text.Replace(" ", "") != "" && textBox6.Text.Replace(" ", "") != "")
                {
                    if (kullaniciAdi == textBox3.Text && sifre == textBox4.Text)
                    {
                        baglan.Open();
                        komut = new OleDbCommand("Update Giris set KullaniciAdi = '" + textBox5.Text + "', sifre = '" + textBox6.Text + "' where state = 'user'", baglan);
                        komut.ExecuteNonQuery();
                        baglan.Close();
                        textBox3.Text = "";
                        textBox4.Text = "";
                        textBox5.Text = "";
                        textBox6.Text = "";
                        MessageBox.Show("Kullanıcı Adı / Şifre Değiştirildi.", "İşlem Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı Adı / Şifre Yanlış", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Bu Alanlar Boş Bırakılamaz", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (radioButton4.Checked)
            {
                _VeriCek("admin");
                if (textBox3.Text.Replace(" ", "") != "" && textBox4.Text.Replace(" ", "") != "" && textBox5.Text.Replace(" ", "") != "" && textBox6.Text.Replace(" ", "") != "")
                {
                    if (kullaniciAdi == textBox3.Text && sifre == textBox4.Text)
                    {
                        baglan.Open();
                        komut = new OleDbCommand("Update Giris set KullaniciAdi = '" + textBox5.Text + "', sifre = '" + textBox6.Text + "' where state = 'admin'", baglan);
                        komut.ExecuteNonQuery();
                        baglan.Close();
                        textBox3.Text = "";
                        textBox4.Text = "";
                        textBox5.Text = "";
                        textBox6.Text = "";
                        MessageBox.Show("Kullanıcı Adı / Şifre Değiştirildi.", "İşlem Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        panel1.Visible = true;
                        panel2.Visible = false;
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı Adı / Şifre Yanlış", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Bu Alanlar Boş Bırakılamaz", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                _VeriCek("user");
                if (textBox1.Text.Replace(" ", "") != "" && textBox2.Text.Replace(" ", "") != "")
                {
                    if (kullaniciAdi == textBox1.Text && sifre == textBox2.Text)
                    {
                        form1.state = "user";
                        form1.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı Adı / Şifre Yanlış", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Bu Alanlar Boş Bırakılamaz", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (radioButton2.Checked)
            {
                _VeriCek("admin");
                if (textBox1.Text.Replace(" ", "") != "" && textBox2.Text.Replace(" ", "") != "")
                {
                    if (kullaniciAdi == textBox1.Text && sifre == textBox2.Text)
                    {
                        form1.state = "admin";
                        form1.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı Adı / Şifre Yanlış", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Bu Alanlar Boş Bırakılamaz", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        void _VeriCek(string state)
        {
            baglan.Open();
            komut = new OleDbCommand("Select * from Giris where state = '" + state + "'", baglan);
            oku = komut.ExecuteReader();
            while (oku.Read())
            {
                kullaniciAdi = oku["KullaniciAdi"].ToString();
                sifre = oku["sifre"].ToString();
            }
            baglan.Close();
        }
    }
}
