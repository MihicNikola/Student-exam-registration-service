using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq; // Za generisanje XML fajlova
using MySql.Data.MySqlClient; // Ovo dodajemo zbog konekcije
using ClosedXML.Excel; // Za generisanje XLSX fajlova
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace OOP // namespace projekta
{
    public class CQuery // klasa CQuery
    {

        private static string server = "localhost"; // adresa servera
        private static string db = "oop"; // naziv baze podataka
        private static string user = "root"; // username
        private static string pass = ""; // password
        private static MySqlConnection? connection; // konekcija - MySql.Data.MySqlClient

        public static void OpenConnection()
        {
            // Kreiranje stringa za konekciju koristeći parametre kao što su server, baza, korisničko ime i lozinka
            string connectionString = $"SERVER={server};DATABASE={db};UID={user};PASSWORD={pass}";

            try
            {
                // Kreiranje nove konekcije i pokušaj otvaranja
                connection = new MySqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception ex) // Ukoliko dođe do greške
            {
                // Ispisuje poruku o grešci u konzolu
                Console.WriteLine("Greška pri konektovanju: " + ex.Message);
            }
        }

        // Metoda za zatvaranje konekcije sa bazom podataka
        public static void CloseConnection()
        {
            try
            {
                // Proverava da li je konekcija već otvorena
                if (connection != null)
                {
                    // Zatvara konekciju
                    connection.Close();
                }
            }
            catch (Exception ex) // Ukoliko dođe do greške
            {
                // Ispisuje poruku o grešci u konzolu
                Console.WriteLine("Greška pri zatvaranju konekcije: " + ex.Message);
            }
        }

        static string Enkriptuj(string ulaz)
        {
            if (string.IsNullOrEmpty(ulaz)) // Proverava da li je ulazni string prazan?
            {
                throw new ArgumentException("Ulazni string ne sme biti prazan.");
            }

            // Obrtanje redosleda karaktera
            char[] niz = ulaz.ToCharArray(); // Prvo pretvara string u niz karaktera
            Array.Reverse(niz);

            // Modifikacija karaktera
            for (int i = 0; i < niz.Length; i++)
            {
                int pseudoRandom = (i % 3) + 1; // Pseudoslučajni broj zasnovan na položaju
                niz[i] = (char)(niz[i] + pseudoRandom); // Novi niz (string)
            }

            // Ponovno obrtanje redosleda
            Array.Reverse(niz);

            return new string(niz); // vraca niz
        }

        // Metoda za proveru korisničkog login-a
        public static bool Login(ref CStudent student, string email, string lozinka)
        {
            // Ovde bi se lozinka trebala enkriptovati pre upita u bazu
            string enkriptovana_lozinka = Enkriptuj(lozinka);

            // SQL upit za proveru korisnika
            string upit1 = $"SELECT dstudent.*, mkorisnik.* FROM dstudent JOIN mkorisnik ON dstudent.korisnik = mkorisnik.korisnik_id WHERE mkorisnik.email = '{email}' AND mkorisnik.lozinka = '{enkriptovana_lozinka}';";

            // Kreiranje i izvršavanje SQL komande
            MySqlCommand cmd = new MySqlCommand(upit1, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            // Proverava da li postoji rezultat upita
            bool indikator = reader.Read();
            if (indikator == true) // Ako postoji
            {
                // Kreira novi objekat klase CStudent sa podacima iz baze
                student = new CStudent((uint)reader["korisnik_id"], (string)reader["ime"], (string)reader["prezime"], (string)reader["email"], (string)reader["lozinka"], (uint)reader["student_id"], (string)reader["indeks"], (string)reader["status"], (int)reader["god_stud"], (double)reader["sredstva"]);
            }

            // Zatvara reader
            reader.Close();

            // Vraća indikator da li je korisnik uspešno ulogovan
            return indikator;
        }

        // Metoda za pronalaženje informacija o roku na osnovu njegovog ID-a
        public static CRok PronadjiRok(uint rokId)
        {
            // Kreiranje SQL upita za dohvatanje naziva roka i datuma prijava
            string upit = $"SELECT naziv, prijava_od, prijava_do FROM mrok WHERE id = '{rokId}'";

            // Kreiranje SQL komande sa upitom i konekcijom
            MySqlCommand cmd = new MySqlCommand(upit, connection);
            // Izvršavanje komande i dobijanje rezultata
            MySqlDataReader reader = cmd.ExecuteReader();

            // Čitanje prvog reda iz rezultata upita
            reader.Read();

            // Dobijanje vrednosti iz kolona i konverzija u odgovarajući tip
            DateTime datum1 = (DateTime)reader["prijava_od"];
            DateTime datum2 = (DateTime)reader["prijava_do"];
            string nazivRoka = (string)reader["naziv"];

            // Zatvaranje reader-a
            reader.Close();

            // Kreiranje i vraćanje novog objekta CRok sa dobijenim podacima
            return new CRok(rokId, nazivRoka, CDatum.IzDateTime(datum1), CDatum.IzDateTime(datum2));
        }

        // Metoda za pronalaženje ID-a ispita na osnovu ID-a predmeta i ID-a roka
        public static uint PronadjiIspit(uint predmetId, uint rokId)
        {
            // Kreiranje SQL upita za dohvatanje ID-a ispita
            string upit = $"SELECT id FROM dispit WHERE rok = '{rokId}' AND predmet = '{predmetId}'";

            // Kreiranje SQL komande sa upitom i konekcijom
            MySqlCommand cmd = new MySqlCommand(upit, connection);
            // Izvršavanje komande i dobijanje rezultata
            MySqlDataReader reader = cmd.ExecuteReader();

            // Čitanje prvog reda iz rezultata upita
            reader.Read();

            // Dobijanje vrednosti iz kolone id i konverzija u uint
            uint idIspita = (uint)reader["id"];

            // Zatvaranje reader-a
            reader.Close();

            // Vraćanje dobijenog ID-a ispita
            return idIspita;
        }

        // Metoda za pronalaženje datuma ispita na osnovu ID-a predmeta i ID-a roka
        public static CDatum PronadjiDatumIspita(uint predmetId, uint rokId)
        {
            // Kreiranje SQL upita za dohvatanje datuma ispita
            string upit = $"SELECT dispit.datum FROM dispit WHERE dispit.rok = '{rokId}' AND dispit.predmet = '{predmetId}'";

            // Kreiranje SQL komande sa upitom i konekcijom
            MySqlCommand cmd = new MySqlCommand(upit, connection);
            // Izvršavanje komande i dobijanje rezultata
            MySqlDataReader reader = cmd.ExecuteReader();

            // Čitanje prvog reda iz rezultata upita
            reader.Read();

            // Dobijanje vrednosti iz kolone datum i konverzija u DateTime
            DateTime datum = (DateTime)reader["datum"];
            // Konverzija DateTime u CDatum
            CDatum datumIspita = CDatum.IzDateTime(datum);

            // Zatvaranje reader-a
            reader.Close();

            // Vraćanje dobijenog datuma ispita
            return datumIspita;
        }

        // Metoda za dohvatanje osnovne cene na osnovu statusa studenta
        public static double OsnovnaCena(CStudent student)
        {
            // Kreiranje SQL upita za dohvatanje cene na osnovu statusa studenta
            string upit = $"SELECT cena FROM mstatus WHERE status = '{student.Status}'";

            // Kreiranje SQL komande sa upitom i konekcijom
            MySqlCommand cmd = new MySqlCommand(upit, connection);
            // Izvršavanje komande i dobijanje rezultata
            MySqlDataReader reader = cmd.ExecuteReader();

            // Čitanje prvog reda iz rezultata upita
            reader.Read();

            // Dobijanje vrednosti iz kolone cena i konverzija u tip double
            double osnovnaCena = (double)reader["cena"];

            // Zatvaranje reader-a
            reader.Close();

            // Vraćanje dobijene osnovne cene
            return osnovnaCena;
        }

        // Metoda za ubacivanje informacija o prijavi ispita u bazu
        public static void Ubaci(CStudent student, uint predmet, uint rok, double cena)
        {
            // Pronalaženje ID-a ispita na osnovu predmeta i roka
            uint ispit = PronadjiIspit(predmet, rok);

            // Kreiranje SQL upita za ubacivanje podataka o prijavi ispita
            string upit = $"INSERT INTO gprijava (student, ispit, cena) VALUES ('{student.StudentId}', '{ispit}', '{cena}')";

            // Kreiranje SQL komande sa upitom i konekcijom
            MySqlCommand cmd = new MySqlCommand(upit, connection);
            // Izvršavanje komande
            MySqlDataReader reader = cmd.ExecuteReader();

            // Zatvaranje reader-a
            reader.Close();
        }

        // Metoda za ažuriranje računa studenta u bazi podataka
        public static void UpdateRacun(CStudent student, double novac)
        {
            // Kreiranje SQL upita za ažuriranje stanja sredstava studenta
            string query = $"UPDATE dstudent SET sredstva = '{novac}' WHERE student_id = '{student.StudentId}'";

            // Kreiranje SQL komande sa upitom i konekcijom
            MySqlCommand cmd = new MySqlCommand(query, connection);
            // Izvršavanje komande
            MySqlDataReader reader = cmd.ExecuteReader();

            // Zatvaranje reader-a
            reader.Close();

            // Ažuriranje sredstava na računu studenta u aplikaciji
            student.SredstvaNaRacunu = novac;
        }

        // Metoda za popunjavanje ComboBox-a sa spiskom ispita koje student nije prijavio
        public static void NePrijavljeniIspiti(CStudent student, ComboBox box, uint rokId)
        {
            // Kreiranje SQL upita za dohvatanje neprijavljenih ispita za studenta za određeni rok
            string query = $"SELECT mpredmet.id, mpredmet.naziv FROM mpredmet JOIN dispit ON mpredmet.id = dispit.predmet WHERE dispit.rok = '{rokId}' AND dispit.id NOT IN (SELECT gprijava.ispit FROM gprijava WHERE gprijava.student = '{student.StudentId}')";

            // Kreiranje SQL komande sa upitom i konekcijom
            MySqlCommand cmd = new MySqlCommand(query, connection);
            // Izvršavanje komande i dobijanje rezultata
            MySqlDataReader reader = cmd.ExecuteReader();

            // Čitanje svakog reda iz rezultata upita
            while (reader.Read())
            {
                // Kreiranje stringa koji sadrži id i naziv ispita
                string item = $"{reader["id"]}-{reader["naziv"]}"; // Format "id-naziv"
                                                                   // Dodavanje ovog stringa u ComboBox
                box.Items.Add(item);
            };

            // Zatvaranje reader-a
            reader.Close();
        }

        // Metoda za popunjavanje ListBox-a sa spiskom ispita koje je student prijavio
        public static void PrijavljeniIspiti(CStudent student, ListBox box)
        {
            // Kreiranje SQL upita za dohvatanje prijavljenih ispita za studenta
            string query = $"SELECT mpredmet.naziv FROM mpredmet INNER JOIN dispit ON mpredmet.id = dispit.predmet INNER JOIN gprijava ON gprijava.ispit = dispit.id WHERE gprijava.student = '{student.StudentId}'";

            // Kreiranje SQL komande sa upitom i konekcijom
            MySqlCommand cmd = new MySqlCommand(query, connection);
            // Izvršavanje komande i dobijanje rezultata
            MySqlDataReader reader = cmd.ExecuteReader();

            // Čitanje svakog reda iz rezultata upita
            while (reader.Read())
            {
                // Dodavanje naziva ispita u ListBox
                box.Items.Add(reader["naziv"]);
            };

            // Zatvaranje reader-a
            reader.Close();
        }

        public static void GenerisiCSV(CStudent student, string putanjaFajla)
        {
            var sb = new StringBuilder(); // Kreira StringBuilder objekat za dinamičko dodavanje stringova.
            sb.AppendLine("Naziv Ispita,Datum Prijave,Cena"); // Dodaje zaglavlje CSV fajla.

            // Priprema SQL upita za dohvatanje podataka o prijavljenim ispitima.
            string query = $"SELECT mpredmet.naziv, gprijava.datum_prijave, gprijava.cena FROM mpredmet INNER JOIN dispit ON mpredmet.id = dispit.predmet INNER JOIN gprijava ON gprijava.ispit = dispit.id WHERE gprijava.student = '{student.StudentId}'";

            MySqlCommand cmd = new MySqlCommand(query, connection); // Kreira SQL komandu sa upitom i konekcijom.
            MySqlDataReader reader = cmd.ExecuteReader(); // Izvršava upit i dobija rezultate.

            while (reader.Read()) // Iterira kroz svaki red dobijenih rezultata.
            {
                string naziv = (string)reader["naziv"]; // Dohvata naziv ispita.
                DateTime datumPrijave = (DateTime)reader["datum_prijave"]; // Dohvata datum prijave.
                double cena = (double)(reader["cena"]); // Dohvata cenu ispita.

                sb.AppendLine($"{naziv},{datumPrijave.ToShortDateString()},{cena}"); // Dodaje red u CSV fajl za svaki ispit.
            }

            reader.Close(); // Zatvara reader objekat.
            File.WriteAllText("CSV-" + putanjaFajla, sb.ToString()); // Snima CSV fajl na disk.
        }

        public static void GenerisiXML(CStudent student, string putanjaFajla)
        {
            XElement root = new XElement("PrijavljeniIspiti"); // Kreira root element za XML dokument.

            string query = $"SELECT mpredmet.naziv, gprijava.datum_prijave, gprijava.cena FROM mpredmet INNER JOIN dispit ON mpredmet.id = dispit.predmet INNER JOIN gprijava ON gprijava.ispit = dispit.id WHERE gprijava.student = '{student.StudentId}'";

            MySqlCommand cmd = new MySqlCommand(query, connection); // Priprema SQL komandu.
            MySqlDataReader reader = cmd.ExecuteReader(); // Izvršava upit i dobija rezultate.

            while (reader.Read()) // Iterira kroz svaki red dobijenih rezultata.
            {
                string naziv = (string)reader["naziv"];
                DateTime datumPrijave = (DateTime)reader["datum_prijave"];
                double cena = (double)reader["cena"];

                XElement ispitElement = new XElement("Ispit", // Kreira XML element za svaki ispit.
                    new XElement("Naziv", naziv),
                    new XElement("DatumPrijave", datumPrijave.ToShortDateString()),
                    new XElement("Cena", cena)
                );

                root.Add(ispitElement); // Dodaje XML element ispita u root element.
            }

            reader.Close(); // Zatvara reader.

            XDocument xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root); // Kreira XML dokument.
            xDoc.Save("XML-" + putanjaFajla); // Snima XML dokument na disk.
        }

        public static void GenerisiXLSX(CStudent student, string putanjaFajla)
        {
            using (var workbook = new XLWorkbook()) // Kreira novi Excel radni list.
            {
                var worksheet = workbook.Worksheets.Add("Prijavljeni Ispiti"); // Dodaje novi radni list u radnu svesku.
                worksheet.Cell(1, 1).Value = "Naziv Ispita"; // Postavlja vrednost ćelije u prvom redu i prvoj koloni.
                worksheet.Cell(1, 2).Value = "Datum Prijave"; // Postavlja vrednost ćelije u prvom redu i drugoj koloni.
                worksheet.Cell(1, 3).Value = "Cena"; // Postavlja vrednost ćelije u prvom redu i trećoj koloni.

                string query = $"SELECT mpredmet.naziv, gprijava.datum_prijave, gprijava.cena FROM mpredmet INNER JOIN dispit ON mpredmet.id = dispit.predmet INNER JOIN gprijava ON gprijava.ispit = dispit.id WHERE gprijava.student = '{student.StudentId}'";

                MySqlCommand cmd = new MySqlCommand(query, connection); // Priprema SQL komandu.
                MySqlDataReader reader = cmd.ExecuteReader(); // Izvršava upit i dobija rezultate.

                int row = 2; // Početak od drugog reda, jer je prvi red zaglavlje.
                while (reader.Read()) // Iterira kroz svaki red dobijenih rezultata.
                {
                    worksheet.Cell(row, 1).Value = reader["naziv"].ToString(); // Postavlja vrednost ćelije u redu 'row' i prvoj koloni.
                    worksheet.Cell(row, 2).Value = Convert.ToDateTime(reader["datum_prijave"]).ToShortDateString(); // Postavlja vrednost ćelije u redu 'row' i drugoj koloni.
                    worksheet.Cell(row, 3).Value = Convert.ToDouble(reader["cena"]); // Postavlja vrednost ćelije u redu 'row' i trećoj koloni.
                    row++; // Povećava brojač redova.
                }

                reader.Close(); // Zatvara reader.
                workbook.SaveAs("XLSX-" + putanjaFajla + ".xlsx"); // Snima XLSX fajl na disk.
            }
        }
    }
}
