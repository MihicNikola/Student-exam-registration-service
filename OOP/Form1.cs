using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OOP // isti namespace kao i ostatak projekta
{
    public partial class Form1 : Form // generisao VS
    {

        CStudent student = new CStudent(); // kreiramo "prazan" objekat klase CStudent
        
        public Form1() // Konstruktor forme1
        {
            InitializeComponent(); // Inicijalizacija svih komponenti - generisao VS
            CQuery.OpenConnection(); // Otvaranje konekcije sa bazom
            this.FormClosing += Zatvaranje; // Dodavanje mehanizma za zatvaranje aplikacije
        }

        private void button1_Click(object sender, EventArgs e) // Dugme1 - Prijavi se!
        {
            if (CQuery.Login(ref student, textBox1.Text, textBox2.Text)) // Ukoliko Login vrati TRUE
            {

                Form2 infoForma = new Form2(student); // Kreira se nova forma
                infoForma.Show(); // Nova forma se prikazuje
                this.Hide(); // Trenutna forma se sakriva

            }
            else // Ukoliko Login vrati FALSE
            {
                textBox1.Text = null; // Brise se uneti tekst
                textBox2.Text = null; // -||-
                MessageBox.Show("Greska: Pogresan unos korisnickog imena i/ili sifre!"); // Pop-up poruka
            }
        }

        private void Zatvaranje(object sender, FormClosingEventArgs e) // Metoda zatvaranja aplikacije
        {
            // Prikazace pop-up message box sa pitanjem i detektuje sta korisnik odgovori. Ukoliko dogovor bude potvrdan:
            if (MessageBox.Show("Da li ste sigurni da želite da zatvorite aplikaciju?", "Potvrda", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit(); // Gasi aplikaciju
            }
            else // Ukoliko odgovor bude negativan:
            {
                e.Cancel = true; // Gasi message box i nista se ne desava
            };

        }


    }
}
