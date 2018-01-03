using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Inscription.Models;

namespace Inscription.Controllers
{
    [Route("[Controller]")]
    public class InscriptionController : Controller
    {
        private const string school = "Université de Sherbrooke";
        private const string schoolProgram = "Génie Informatique";
        private const Int64 gradEpoch = 1544961600000;

        //[HttpGet]
        //public InscriptionOutput Get(/*[FromBody]InscriptionInput input*/)
        //{
        //    //return Post(input);
        //    return Post(new InscriptionInput
        //    {
        //        Shapes = new List<List<int>>
        //        {
        //            new List<int> {1, 2, 3 },
        //            new List<int> {1, 3, 3 },
        //            new List<int> {3, 4, 5 }
        //        }
        //    });
        //}

            [HttpGet]
            public ActionResult Get()
        {
            return Ok("I'm alive");
        }

        [HttpPost]
        public InscriptionOutput Post([FromBody]InscriptionInput input)
        {
            return new InscriptionOutput
            {
                Participants = new List<Participant>
                {
                    new Participant
                    {
                        IsCaptain = true,
                        FullName = "Alexandre Benoit",
                        Email = "Alexandre.C.Benoit@usherbrooke.ca",
                        Phone = "8196902895",
                        School = school,
                        SchoolProgram = schoolProgram,
                        GraduationDate = gradEpoch
                    },
                    new Participant
                    {
                        IsCaptain = false,
                        FullName = "Marc-André Frenette",
                        Email = "frem2110@usherbrooke.ca",
                        Phone = "8199441181",
                        School = school,
                        SchoolProgram = schoolProgram,
                        GraduationDate = gradEpoch
                    },
                    new Participant
                    {
                        IsCaptain = false,
                        FullName = "Jérôme Godin",
                        Email = "jerome.godin@usherbrooke.ca",
                        Phone = "8196744523",
                        School = school,
                        SchoolProgram = schoolProgram,
                        GraduationDate = gradEpoch
                    },
                    new Participant
                    {
                        IsCaptain = false,
                        FullName = "Valentin Libioulle",
                        Email = "Valentin.libioulle@usherbrooke.ca",
                        Phone = "581995218p",
                        School = school,
                        SchoolProgram = schoolProgram,
                        GraduationDate = gradEpoch
                    }
                },
                Solution = input.Shapes.Where(l => l.Max() < (l.Sum() - l.Max())).Count(),
                TeamName = "Keep Summer Safe"
            };
        }
    }
}
