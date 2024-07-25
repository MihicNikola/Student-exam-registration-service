namespace OOP
{
    // Definicija klase CDatum
    public class CDatum
    {
        // Privatna polja koja predstavljaju dan, mesec i godinu
        private int _dan;
        private int _mesec;
        private int _godina;

        // Staticki niz koji sadrzi broj dana u mesecima (ne racunajuci prestupnu godinu)
        private static readonly int[] daniUMesecu = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        // Svojstvo koje omogucava pristup i postavljanje vrednosti dana
        public int Dan
        {

            get { return _dan; } // getter za dan
            set
            {
                // Setter za dan, provera da li je uneti datum validan pre postavljanja
                if (JeValidanDatum(value, Mesec, Godina))
                {
                    _dan = value; // postavlja dan ako je datum validan
                }
                else
                {
                    // Bacanje izuzetka ako je uneti dan nevalidan
                    throw new ArgumentException("Nevalidan dan.");
                }
            }
        }

        // Slicno kao za Dan, definisu se svojstva za Mesec i Godinu
        public int Mesec
        {
            get { return _mesec; } // getter za mesec
            set
            {
                // Setter za mesec, proverava da li je mesec u validnom opsegu
                if (value < 1 || value > 12)
                {
                    throw new ArgumentException("Nevalidan mesec."); // Bacanje izuzetka ako mesec nije u opsegu
                }
                else if (!JeValidanDatum(Dan, value, Godina))
                {
                    throw new ArgumentException("Nevalidan dan u datom mesecu"); // Bacanje izuzetka ako kombinacija dana i meseca nije validna 
                }
                _mesec = value; // Postavlja mesec ako je validan
            }
        }

        public int Godina
        {
            get { return _godina; } // Getter za godinu
            set
            {
                // Setter za godinu; proverava da li je kombinacija dana, meseca i godine validna
                if (!JeValidanDatum(Dan, Mesec, value))
                {
                    throw new ArgumentException("Nevalidan dan u datoj godini"); // Bacanje izuzetka datum nije validan 
                }
                _godina = value; // Postavlja godinu ako je datum validan
            }
        }

        public CDatum() { } // default konstruktor

        public CDatum(int d, int m, int g) // konstruktor koji dodeljuje vrednosti
        {
            if (JeValidanDatum(d, m, g)) // da li je datum d.m.g. validan?
            {
                _dan = d;
                _mesec = m;
                _godina = g;
            }
            else
            {
                throw new ArgumentException("Nevalidan datum."); // "Baca" izuzetak ako datum nije validan
            }
        }

        public void Ispisi() // ispis u konzolnoj liniji
        {
            Console.WriteLine($"{Dan}-{Mesec}-{Godina}");
        }

        public void Dodaj(int n) // mora se napraviti objekat, pa se metoda poziva objekat.Dodaj(n)
        {
            for (int i = 0; i < n; i++) // for petlja do n dodatih dana
            {
                if (++_dan > DaniUMesecu(_mesec, _godina)) // ako dan prekoraci broj dana u mesecu
                {
                    _dan = 1; // postavi dan na 1. u mesecu
                    if (++_mesec > 12) // ako mesec prekoraci 12. mesec
                    {
                        _mesec = 1; // postavi mesec na januar
                        _godina++; // povecaj godinu za 1
                    }
                }
            }
        }

        public void Oduzmi(int n) // kao funkcija dodaj, samo oduzima dane
        {
            for (int i = 0; i < n; i++) // brojac do n oduzetih dana
            {
                if (--_dan < 1) // ukoliko je umanjeni dan manji od 1
                {
                    if (--_mesec < 1) // ukoliko je umanjeni mesec manji od januara
                    {
                        _mesec = 12; // postavi mesec decembar
                        _godina--; // smanji za jednu godinu
                    }
                    _dan = DaniUMesecu(_mesec, _godina); // dan postavi na broj dana u datom mesecu u godini
                }
            }
        }

        private static bool JeValidanDatum(int d, int m, int g) // da li je datum validan
        {   // vraca true ukoliko je mesec >= 1 ili <=12, a dan >=1, a <= od dana u tom mesecu u toj godini
            return m >= 1 && m <= 12 && d >= 1 && d <= DaniUMesecu(m, g);
        }

        private static int DaniUMesecu(int m, int g)// vraca broj dana u mesecu
        {
            if (m == 2 && DaLiJePrestupna(g)) // ukoliko je febriar i godina je prestupna
            {
                return 29; // vrati 29
            }
            return daniUMesecu[m - 1]; // za svaki drugi slucaj vrati iz niza daniUMesecu, -1 jer indeksi u nizu krecu od 0, a meseci od 1
        }

        private static bool DaLiJePrestupna(int g) // da li je prestupna godina
        {
            return ((g % 4 == 0 && g % 100 != 0) || (g % 400 == 0));
        }

        public static CDatum IzDateTime(DateTime dateTime) // Konvertuje iz DateTime formata u nas CDatum
        {   // vraca novi objekat klase CDatum
            return new CDatum(dateTime.Day, dateTime.Month, dateTime.Year);
        }

        private static int PretvoriUDane(CDatum datum) // fiksni datum je 1.1.2000.
        {
            int dani = datum.Dan - 1; // - 1 jer je dan u fiksnom datumu 1
            for (int m = 1; m < datum.Mesec; m++) // m krece od 1, jer je mesec u fiksnom datumu 1
            {
                dani += DaniUMesecu(m, datum.Godina); // suma svih dana u svim mesecima do datum.Mesec
            }

            for (int g = 2000; g < datum.Godina; g++) // g = 2000 jer je godina u fiksnom datumu 2000
            {
                dani += DaLiJePrestupna(g) ? 366 : 365; // suma dana u svim godinama do datum.Godina
            }

            return dani; // vraca broj dana
        }

        public static int RazlikaUDanima(CDatum datum1, CDatum datum2) // Vraca broj dana izmedju dva uneta datuma
        {
            int ukupnoDanaDatum1 = PretvoriUDane(datum1); // pretvara datum1 u broj dana
            int ukupnoDanaDatum2 = PretvoriUDane(datum2); // pretvara datum2 u broj dana
                                                          // vraca razliku u danima izmedju dva datuma
            return ukupnoDanaDatum1 - ukupnoDanaDatum2; // moze biti i pozitivno i negativno i nula
        }

        public static bool SuIsti(CDatum datum1, CDatum datum2) // Da li su datumi isti
        {   // Ukoliko su dani isti, meseci isti i godine isti, onda su datumi isti
            return datum1.Dan == datum2.Dan && datum1.Mesec == datum2.Mesec && datum1.Godina == datum2.Godina;
        }

        public static bool JePre(CDatum datum1, CDatum datum2) // Da li je datum1 pre datuma2
        {   // ako je razlika u danima manja od 0, to znaci da je datum1 pre datum2
            if (RazlikaUDanima(datum1, datum2) < 0) return true;
            else return false;
        }

        public static bool JePosle(CDatum datum1, CDatum datum2) // Da li je datum1 posle datum2
        {   // ako je razlika u danima veca od 0, to znaci da je datum1 posle datum2
            if (RazlikaUDanima(datum1, datum2) > 0) return true;
            else return false;
        }

        public static bool JeIzmedju(CDatum datum1, CDatum datum2, CDatum datum3) // Da li je datum1 izmedju datum2 i datum3
        {   // Ukoliko je datum 1 posle datuma 2 i pre datuma 3, to znaci da je datum 1 izmedju ta dva datuma
            if (JePosle(datum1, datum2) && JePre(datum1, datum3)) return true;
            else return false;
        }

    }

}
