using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inscription.Models
{
    public class InscriptionOutput
    {
        public List<Participant> Participants { get; set; }
        public int Solution { get; set; }
        public string TeamName { get; set; }
    }
}
