using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.ComponentModel.DataAnnotations;

namespace Test2
{
    public class Movie
    {
        public string Code { get; set; }
        public string? Code2 { get; set; } = null!;
        public string Titles { get; set; }
        public string MainTitle { get; set; }
        public HashSet<Actor>? Director { get; set; }
        public string Rating { get; set; }
        public HashSet<Actor>? Actors { get; set; }
        public List<Tag>? Tags { get; set; }
        public string Top10Codes { get; set; } = "";
        [NotMapped]
        public LinkedList<Movie> Top10 { get; set; } = new LinkedList<Movie>();
        [NotMapped]
        public LinkedList<int> Top10Ratings { get; set; } = new LinkedList<int>();

        public Movie() { }
        public Movie(string code, string title)
        {
            MainTitle = title;
            Titles = title;
            Code = code;
            Rating = "0.0";
        }
        public void AddTitle(string title, string region, string lang)
        {
            Titles += " | " + title;
            if (lang == "ru" || region == "RU")
            {
                MainTitle = title;
            }
        }
        public void AddDirector(Actor director)
        {
            if (director != null)
            {
                if (Director != null)
                {
                    Director.Add(director);
                }
                else
                {
                    Director = new HashSet<Actor> { director };
                }
            }
        }
        public void AddRating(string rating) { Rating = rating; }
        public void AddActor(Actor actor)
        {
            if (actor != null)
            {
                if (Actors != null)
                {
                    Actors.Add(actor);
                }
                else
                {
                    Actors = new HashSet<Actor> { actor };
                }
            }
        }
        public void AddCode2(string code2) { Code2 = code2; }
        public void AddTag(Tag tag)
        {
            if (Tags != null)
            {
                Tags.Add(tag);
            }
            else
            {
                Tags = new List<Tag> { tag };
            }
        }
        public void Print(ApplicationContext db)
        {
            Console.WriteLine(Code + "  ");
            Console.Write("Название: ");
            Console.Write(Titles);
            Console.WriteLine();
            Console.WriteLine("Рейтинг: " + Rating);
            Console.Write("Режиссер:");
            int r = 0;
            foreach (Actor actor in Director)
            {
                if (r > 0) Console.Write(",");
                Console.Write(" " + actor.Name);
                r++;
            }
            Console.WriteLine();
            Console.Write("Актеры:");
            int p = 0;
            foreach (Actor actor in Actors)
            {
                if (p > 0) Console.Write(",");
                Console.Write(" " + actor.Name);
                p++;
            }
            Console.WriteLine();
            Console.Write("Теги: |");
            foreach (Tag tag in Tags)
            {
                Console.Write(" " + tag.Name + " |");
            }
            Console.WriteLine();
            Console.Write("Похожие фильмы: |");
            string[] SimilarMoviesCodes = Top10Codes.Split("|");
            foreach (string movieCode in SimilarMoviesCodes)
            {
                if (movieCode != "")
                {
                    Console.Write(" " + db.Movies.FirstOrDefault(m => m.Code == movieCode).MainTitle + " |");
                }
            }
            Console.WriteLine();
            Console.WriteLine();
        }
        public void FindSimilarMovies()
        {
            LinkedListNode<Movie> tMovie;
            LinkedListNode<int> tInt;
            HashSet<Movie> closeMovies = new HashSet<Movie>();


            if (Director != null)
            {
                foreach (Actor dir in Director)
                {
                    if (dir.Movies != null)
                    {
                        foreach (Movie movie in dir.Movies)
                        {
                            closeMovies.Add(movie);
                        }
                    }
                }
            }

            if (Actors != null)
            {
                foreach (Actor actor in Actors)
                {
                    if (actor.Movies != null)
                    {
                        foreach (Movie movie in actor.Movies)
                        {
                            closeMovies.Add(movie);
                        }
                    }
                }
            }

            if (Tags != null)//(Tags != null && Top10.Count < 10)
            {
                foreach (Tag tag in Tags)
                {
                    if (tag.Movies != null)
                    {
                        foreach (Movie movie in tag.Movies)
                        {
                            closeMovies.Add(movie);
                        }
                    }
                }
            }
            foreach (Movie movie in closeMovies)
            {
                if (movie != this)
                {
                    int r = this.CompareTo(movie);
                    if (Top10.Count == 0)
                    {
                        Top10.AddFirst(movie);
                        Top10Ratings.AddFirst(r);
                    }
                    else
                    {
                        tMovie = Top10.First;
                        tInt = Top10Ratings.First;
                        while (tInt.Next != null && tInt.Value > r)
                        {
                            tMovie = tMovie.Next;
                            tInt = tInt.Next;
                        }
                        if (tInt.Value < r)
                        {
                            Top10.AddBefore(tMovie, movie);
                            Top10Ratings.AddBefore(tInt, r);
                        }
                        else
                        {
                            Top10.AddLast(movie);
                            Top10Ratings.AddLast(r);
                        }
                        while (Top10.Count > 10)
                        {
                            Top10.RemoveLast();
                            Top10Ratings.RemoveLast();
                        }
                    }
                }
            }
            foreach (Movie movie in Top10)
            {
                if (Top10Codes == "")
                {
                    Top10Codes = movie.Code;
                }
                else
                {
                    Top10Codes += "|" + movie.Code;
                }
            }
        }

        public int CompareTo(Movie movie)
        {
            int answer = Convert.ToInt32(movie.Rating.Replace(".", ""));
            if (this.Actors != null && movie.Actors != null)
            {
                int k = 0;
                foreach (Actor actor in this.Actors)
                {
                    if (movie.Actors.Contains(actor))
                    {
                        k++;
                    }
                }
                answer += 10 * k;
            }
            if (this.Director != null && movie.Director != null)
            {
                int k = 0;
                foreach (Actor director in this.Director)
                {
                    if (movie.Director.Contains(director))
                    {
                        k++;
                    }
                }
                answer += 20 * k;
            }
            if (this.Tags != null && movie.Tags != null)
            {
                int k = 0;
                foreach (Tag tag in this.Tags)
                {
                    if (movie.Tags.Contains(tag))
                    {
                        k++;
                    }
                }
                answer += k;
            }
            return answer;
        }

    }
}
