using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP
{
    public class CRok // Klasa Rok, preuzima informacije iz baze
    {
        public uint Id { get; set; } // jedinstveni Id roka
        public string? Naziv { get; set; } // naziv roka, moze biti null
        public CDatum Prijava_od { get; set; } = new CDatum(); // od kog datuma traje prijava
        public CDatum Prijava_do { get; set; } = new CDatum(); // do kog datuma traje prijava

        public CRok() { } // default konstruktor
        public CRok(uint id, string naziv, CDatum prijava_od, CDatum prijava_do) // konstruktor
        {
            Id = id;
            Naziv = naziv;
            Prijava_od = prijava_od;
            Prijava_do = prijava_do;
        }
    }
}
