
using MySql.Data.MySqlClient;
using OOP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace OOP // namespace projekta
{

    public class CKorisnik // bazna klasa korisnik
    {
        // Osnovne informacije o korisniku
        public uint korisnikId { get; set; } // jedinstveni id korisnika
        public string? Ime { get; set; } // ime korisnika
        public string? Prezime { get; set; } // prezime korisnika
        public string? Email { get; set; } // email korisnika - za login
        public string? Lozinka { get; set; } // lozinka korisnika - za login

        public CKorisnik() { } // default konstruktor za baznu klasu

        // Konstruktor klase CKorisnik
        public CKorisnik(uint id, string ime, string prezime, string email, string lozinka) // konstruktor
        {
            korisnikId = id;
            Ime = ime;
            Prezime = prezime;
            Email = email;
            Lozinka = lozinka;
        }

    }

    public class CStudent : CKorisnik // klasa student nasledjuje CKorisnik
    {
        // Dodatne informacije o studentu
        public uint StudentId { get; set; } // jedinstveni id studenta
        public string? BrojIndeksa { get; set; } // broj indeksa
        public string? Status { get; set; } // status: budzet ili samofinansiranje
        public int GodinaStudiranja { get; set; } // godina studiranja: 1,2,3,4,5
        public double SredstvaNaRacunu { get; set; } // novcana sredstva na racunu

        public CStudent() : base() // default konstruktor - base poziva konstruktor roditeljske klase
        { }
        // Konstruktor klase CStudent koji poziva konstruktor roditeljske klase (CKorisnik)
        public CStudent(uint korisnikId, string ime, string prezime, string email, string lozinka,
                        uint studentId, string brojIndeksa, string status, int godinaStudiranja, double sredstvaNaRacunu)
            : base(korisnikId, ime, prezime, email, lozinka) // konstruktor sa podacima za unos
        {
            StudentId = studentId;
            BrojIndeksa = brojIndeksa;
            Status = status;
            GodinaStudiranja = godinaStudiranja;
            SredstvaNaRacunu = sredstvaNaRacunu;
        }

        public bool PrijavaIspita(uint predmet, uint trenutniRok) // "glavna" metoda za prijavu ispita
        {

            int kasnjenje = 0; // broj dana kasnjenja, stavljamo na nulu na pocetku

            CRok rok = CQuery.PronadjiRok(trenutniRok); // vraca objekat klase CRok za uneti id broj roka

            //CDatum danasnjiDan = CDatum.IzDateTime(DateTime.Now); // za danasnji dan
            CDatum danasnjiDan = new CDatum(16, 1, 2024); // Unosimo dan koji zelimo - radi provere

            CDatum datumIspita = CQuery.PronadjiDatumIspita(predmet, trenutniRok); // Pronalazi datum ispita i pravi objekat klase CDatum

            kasnjenje = CDatum.RazlikaUDanima(danasnjiDan, rok.Prijava_do); // Razlika u danima
            if (kasnjenje <= 0) kasnjenje = 0; // Ne postoji kasnjenje (ili su isti tani ili je danasnjiDan pre kraja prijave)

            double osnovnaCena = CQuery.OsnovnaCena(this); // Preuzimamo osnovnu cenu iz baze u zavisnosti od statusa studenta
            // Vremenski uslov: kasnjenje je manje od 10 dana, nije ustupio dan ispita i pocetka prijave je pre danasnjegDana ili su isti dani
            if ( kasnjenje <= 10 && CDatum.RazlikaUDanima(datumIspita,danasnjiDan) > 0 && CDatum.RazlikaUDanima(rok.Prijava_od,danasnjiDan) <= 0 )
            {
                // Ukupna cena ispita se formira kao suma osnovne cene i prozivoda dana kasnjenja sa 150dinara
                double cenaIspita = osnovnaCena + kasnjenje * 150.00;

                if (cenaIspita <= this.SredstvaNaRacunu) // Finansijski uslov: da li student ima novca za prijavu na racunu
                {

                    CQuery.Ubaci(this, predmet, trenutniRok, cenaIspita); // ubacuje prijavu u tabelu gprijava
                    CQuery.UpdateRacun(this, this.SredstvaNaRacunu - cenaIspita); // azurira sredstva u bazi podataka i objektu za logovanog studenta

                    MessageBox.Show($"Usprešno ste prijavili ispit!"); // pop-up poruka

                    return true; // uspesno prijavljen ispita

                }
                else MessageBox.Show($"Nemate dovoljno sredstava na računu!"); // pop-up poruka

            }
            else MessageBox.Show($"Nije moguće prijaviti ispit."); // pop-up poruka

            return false; // neuspesno prijavljen ispit

        }

        public void DisplayInfo(TextBox box) // prikazuje informacije u vidu stringa za odabrani text box
        {
            // Postavljanje teksta za ime i prezime
            box.Text = $"Ime i prezime: {this.Ime} {this.Prezime}\r\n";

            // Postavlja broj indeksa
            box.Text += $"Broj indeksa: {this.BrojIndeksa}\r\n";

            // Dodavanje informacija o statusu
            string statusText = (this.Status == "S") ? "Status: Samofinansiranje" : "Status: Budzet";
            box.Text += $"{statusText}\r\n";

            // Dodavanje informacija o sredstvima na računu
            box.Text += $"Sredstva na racunu: {this.SredstvaNaRacunu}";
        }

    }

}
