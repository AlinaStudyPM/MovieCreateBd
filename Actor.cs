using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2
{
    public class Actor
    {
        public string Code { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public HashSet<Movie> Movies { get; set; }
        [NotMapped]
        public HashSet<Movie> DirectedMovies { get; set; }


        public Actor() { }
        public Actor(string code, string name)
        {
            Code = code;
            Name = name;
        }
        public void AddMovie(Movie movie)
        {
            if (Movies != null)
            {
                Movies.Add(movie);
            }
            else
            {
                Movies = new HashSet<Movie> { movie };
            }
        }
        public void AddDirectedMovie(Movie movie)
        {
            if (DirectedMovies != null)
            {
                DirectedMovies.Add(movie);
            }
            else
            {
                DirectedMovies = new HashSet<Movie> { movie };
            }
        }

        public void Print()
        {
            Console.WriteLine(Code + "  " + Name);
            Console.Write("Участвовал в фильмах: ");
            int q = 0;
            foreach (var movie in Movies)
            {
                if (q > 0) Console.Write(" | ");
                Console.Write(movie.MainTitle);
                q++;
            }
            Console.WriteLine();
            Console.Write("Прдюссировал фильмы: ");
            int p = 0;
            foreach (var movie in DirectedMovies)
            {
                if (p > 0) Console.Write(" | ");
                Console.Write(movie.MainTitle);
                p++;
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
