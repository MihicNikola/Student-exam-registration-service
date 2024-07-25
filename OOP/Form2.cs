using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace OOP // namespace projekta
{
    public partial class Form2 : Form
    {
        // Deklaracija instance klase CStudent koja će biti korišćena u ovoj formi.
        CStudent student = new CStudent();

        // Promenljiva koja drži ID trenutnog roka, postavljena na 1 kao default.
        uint rok = 1;

        // Konstruktor klase Form2 koji prima objekat klase CStudent.
        public Form2(CStudent student1)
        {
            // Inicijalizacija komponenti forme.
            InitializeComponent();

            // Postavljanje prosleđenog studenta kao trenutnog studenta u formi.
            student = student1;

            // Prikazuje informacije o studentu u textBox1.
            student.DisplayInfo(textBox1);

            // Popunjava comboBox1 sa ne prijavljenim ispitima studenta.
            CQuery.NePrijavljeniIspiti(student, comboBox1, rok);

            // Popunjava listBox1 sa prijavljenim ispitima studenta.
            CQuery.PrijavljeniIspiti(student, listBox1);

            // Dodaje događaj za zatvaranje forme.
            this.FormClosing += Zatvaranje;
        }

        // Metoda koja se poziva prilikom pokušaja zatvaranja forme.
        private void Zatvaranje(object sender, FormClosingEventArgs e)
        {
            // Postavlja dijalog koji traži potvrdu za izlaz iz aplikacije.
            if (MessageBox.Show("Da li ste sigurni da želite da zatvorite aplikaciju?", "Potvrda", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Zatvara celu aplikaciju.
                Application.Exit();
            }
            else
            {
                // Prekida zatvaranje forme.
                e.Cancel = true;
            };
        }

        // Metoda za izvlačenje ID-a iz stringa koji sadrži ID i naziv ispita.
        public static uint IzvuciIdIzStringa(string kombinovaniString)
        {
            // Razdvaja string na delove koristeći '-' kao separator.
            string[] delovi = kombinovaniString.Split('-');
            if (delovi.Length > 0)
            {
                // Pokušava da konvertuje prvi deo (pretpostavljeni ID) u uint.
                if (uint.TryParse(delovi[0].Trim(), out uint id))
                {
                    // Vraća ID ako je konverzija uspešna.
                    return id;
                }
            }
            // Vraća 0 kao signal greške u slučaju neuspešne konverzije.
            return 0;
        }

        // Metoda koja se poziva kada se klikne na button1.
        private void button1_Click(object sender, EventArgs e)
        {
            // Uzima tekst iz comboBox1.
            string predmet = comboBox1.Text;
            if (!string.IsNullOrEmpty(predmet))
            {
                // Izvlači ID ispita iz odabranog teksta u comboBox1.
                uint predmetId = IzvuciIdIzStringa(predmet);

                // Student prijavljuje ispit sa izvučenim ID-om.
                student.PrijavaIspita(predmetId, rok);

                // Čisti comboBox1, listBox1 i textBox1 i ponovno ih popunjava sa ažuriranim podacima.
                comboBox1.Items.Clear();
                listBox1.Items.Clear();
                textBox1.Clear();
                CQuery.NePrijavljeniIspiti(student, comboBox1, rok);
                CQuery.PrijavljeniIspiti(student, listBox1);
                student.DisplayInfo(textBox1);

                // Resetuje odabrani indeks comboBox1.
                comboBox1.SelectedIndex = -1;
            }
            else
            {
                // Ako ništa nije odabrano u comboBox1, resetuje odabrani indeks i prikazuje poruku.
                comboBox1.SelectedIndex = -1;
                MessageBox.Show("Niste odabrali nijedan predmet.");
            }
        }

        // Metoda koja se poziva kada se klikne na button2.
        private void button2_Click(object sender, EventArgs e)
        {
            // Kreira objekat CDatum za trenutni datum i vreme.
            CDatum danasnjiDan = CDatum.IzDateTime(DateTime.Now);

            // Generiše putanju fajla na osnovu trenutnog datuma i ID-a studenta.
            string putanjaFajla = $"{danasnjiDan.Godina}-{danasnjiDan.Mesec}-{danasnjiDan.Dan}-{student.StudentId}";

            // Poziva metode za generisanje CSV, XML i XLSX fajlova.
            CQuery.GenerisiCSV(student, putanjaFajla);
            CQuery.GenerisiXML(student, putanjaFajla);
            CQuery.GenerisiXLSX(student, putanjaFajla);

            // Prikazuje poruku o uspešnom generisanju fajlova.
            MessageBox.Show($"Fajlovi generisani: {putanjaFajla}");
        }
    }
}

