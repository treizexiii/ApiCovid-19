using System;

namespace ApiConnect.Models
{
    public class Data
    {
        public string Nom { get; set; }
        public string Code { get; set; }
        public DateTime Date { get; set; }
        public string Gueris { get; set; }
        public string NouvellesHospitalisations { get; set; }
        public string NouvellesReanimation { get; set; }
        public string Hospitalises { get; set; }
        public string Reanimation { get; set; }
        public string Deces { get; set; }
        public string DecesEhpad { get; set; }
    }
}
