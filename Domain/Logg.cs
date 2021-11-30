using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    [Serializable]
    public class Logg
    {
        public string Game { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
        public string Action { get; set; }
        public Logg() { }
    }
}
