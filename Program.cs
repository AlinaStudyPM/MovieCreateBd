using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;

namespace Test2
{
    public class Programm
    {
        //public string SearchedLine { get; set; }

        public static ConcurrentDictionary<string, Movie> MovieByCode = null!;
        public static ConcurrentDictionary<string, string> MovieCodeByTitle = null!; //TODO: Titles...
        public static ConcurrentDictionary<string, Actor> ActorByCode = null!;
        public static ConcurrentDictionary<string, HashSet<string>> ActorsCodesByName = null!;
        public static ConcurrentDictionary<string, HashSet<string>> MoviesCodesByCode2 = null!;
        public static ConcurrentDictionary<string, Tag> TagByCode = null!;
        public static ConcurrentDictionary<string, string> TagCodeByName = null!;

        public static string path1 = "TESTMovieByCode.txt";
        public static string path2 = "TESTRatings.txt";
        public static string path3 = "TESTCode2.txt";
        public static string path4 = "TESTTagsCodes.txt";
        public static string path5 = "TESTActorsDirectorsNames_IMDB.txt";
        public static string path6 = "TESTActorsDirectorsCodes_IMDB.txt";
        public static string path7 = "TESTTagAndMovie.txt";

        //public static string path1 = "MovieCodes_IMDB.tsv";
        //public static string path2 = "Ratings_IMDB.tsv";
        //public static string path3 = "links_IMDB_MovieLens.csv";
        //public static string path4 = "TagCodes_MovieLens.csv";
        //public static string path5 = "ActorsDirectorsNames_IMDB.txt";
        //public static string path6 = "ActorsDirectorsCodes_IMDB.tsv";
        //public static string path7 = "TagScores_MovieLens.csv";
        public static void Main(string[] args)
        {
            Stopwatch StopWatch = new Stopwatch();
            StopWatch.Start();
            UpdateDictionaries(StopWatch);
            UpdateDataBase(StopWatch);


            Console.WriteLine("movie: title  -  найти фильм по названию");
            Console.WriteLine("actor: name  -  найти фильмы с участием данного актера");
            Console.WriteLine("tag: title  -  найти фильмы по тегу");
            Console.WriteLine("update - обновить базы данных");
            Console.WriteLine();
            Console.WriteLine("Введите запрос:");
            string SearchedLine = Console.ReadLine();
            while (SearchedLine != null)
            {
                if (SearchedLine == "update")
                {
                    Update();
                }
                if (SearchedLine.Contains(":"))
                {
                    string type = SearchedLine.Substring(0, SearchedLine.IndexOf(":"));
                    if (type == "movie")
                    {
                        string title = SearchedLine.Substring(SearchedLine.IndexOf(":") + 2);
                        SearchMovie(title);
                    }
                    if(type == "actor")
                    {
                        string name = SearchedLine.Substring(SearchedLine.IndexOf(":") + 2);
                        
                    }
                    if(type == "tag")
                    {

                    }
                }
            }




                        //using (ApplicationContext db = new ApplicationContext(false))
                        //{
                        //    var searchedMovies = db.Movies.Where(movie => movie.Titles.Contains(SearchedLine))
                        //        .Include(m => m.Actors).Include(m => m.Director).Include(m => m.Tags);
                        //    foreach (var movie in searchedMovies)
                        //    {
                        //        movie.Print(db);
                        //        Console.WriteLine();
                        //    }

                        //}



                        //Search();
                        Console.ReadLine();
        }
        public static void SearchMovie(string line)
        {
            if (line != "")
            {
                using (ApplicationContext db = new ApplicationContext(false)) //TODO: Загрузить правильную базу данных
                {
                    var searchedMovies = db.Movies.Include(m => m.Actors).Include(m => m.Director).Include(m => m.Tags)
                        .Where(movie => movie.Titles.Contains(line));
                    if (searchedMovies.Count() > 0)
                    {
                        foreach (var movie in searchedMovies)
                        {
                            movie.Print(db);
                            Console.WriteLine();
                        }
                    }
                }
            }
        }
        public static void SearchMovieCode(string code)
        {
            if (code != "")
            {
                using (ApplicationContext db = new ApplicationContext(false)) //TODO: Загрузить правильную базу данных
                {
                    var searchedMovies = db.Movies.Include(m => m.Actors).Include(m => m.Director).Include(m => m.Tags)
                        .Where(movie => movie.Code == code);
                    if (searchedMovies.Count() > 0)
                    {
                        foreach (var movie in searchedMovies)
                        {
                            movie.Print(db);
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Такой фильм не найден");
                    }
                }
            }
        }
        public static void SearchActor(string name)
        {
            if (name != "")
            {
                using (ApplicationContext db = new ApplicationContext(false)) //TODO: Загрузить правильную базу данных
                {
                    var searchedActors = db.Actors.Where(a => a.Name == name);

                    if (searchedActors != null)
                    {
                        foreach (Actor sactor in searchedActors)
                        {
                            var movies = db.Movies.Include(m => m.Actors)
                                .Where(m => m.Actors.Any(a => a.Code == sactor.Code));
                            sactor.Movies = new HashSet<Movie>(movies);

                            var directors = db.Movies.Include(m => m.Director).AsParallel()
                                .Where(m => m.Director.Any(a => a.Code == sactor.Code));
                            sactor.DirectedMovies = new HashSet<Movie>(directors);

                            sactor.Print();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Такой актёр не найден");
                    }
                }
            }
        }

        public static void SearchTag(string name)
        {
            if (name != "")
            {
                using (ApplicationContext db = new ApplicationContext(false))
                {

                    Tag searchedTag = db.Tags.FirstOrDefault(tag => tag.Name == name);

                    if (searchedTag != null)
                    {
                        var movies = db.Movies.Include(m => m.Tags)
                            .Where(m => m.Tags.Any(t => t.Name == searchedTag.Name));
                        Console.WriteLine(movies.Count());
                        searchedTag.Movies = new HashSet<Movie>(movies);

                        searchedTag.Print();
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Такой тег не найден");
                    }
                }
            }
        }
        public static void Search()
        {
            Console.WriteLine("movie: title  -  найти фильм по названию");
            Console.WriteLine("actor: name  -  найти фильмы с участием данного актера");
            Console.WriteLine("tag: title  -  найти фильмы по тегу");
            Console.WriteLine("update - обновить базы данных");
            Console.WriteLine();
            Console.WriteLine("Введите запрос:");
            string SearchedLine = Console.ReadLine();
            while (SearchedLine != null)
            {
                if (SearchedLine == "update")
                {
                    Update();
                }
                if (SearchedLine.Contains(":"))
                {
                    string type = SearchedLine.Substring(0, SearchedLine.IndexOf(":"));
                    if (type == "movie")
                    {
                        string title = SearchedLine.Substring(SearchedLine.IndexOf(":") + 2);
                        if (MovieCodeByTitle.TryGetValue(title, out string code))
                        {
                            Console.WriteLine();
                            //MovieByCode[code].Print(db);
                            Console.WriteLine("Введите запрос:");
                            SearchedLine = Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine("Такой фильм не найден. Попробуйте ещё раз:");
                            SearchedLine = Console.ReadLine();
                        }
                    }
                    else if (type == "actor")
                    {
                        string name = SearchedLine.Substring(SearchedLine.IndexOf(":") + 2);
                        if (ActorsCodesByName.TryGetValue(name, out HashSet<string> codesOfActors))
                        {
                            if (codesOfActors.Count > 1)
                            {
                                Console.WriteLine();
                                Console.WriteLine("По вашему запросу найдено несколько актёров");
                            }
                            Console.WriteLine();
                            foreach(string code in codesOfActors)
                            {
                                ActorByCode[code].Print();
                            }
                            Console.WriteLine("Введите запрос:");
                            SearchedLine = Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine("Такой актёр не найден. Попробуйте ещё раз:");
                            SearchedLine = Console.ReadLine();
                        }
                    }
                    else if (type == "tag")
                    {
                        string title = SearchedLine.Substring(SearchedLine.IndexOf(":") + 2);
                        int p = 0;
                        if (TagCodeByName.TryGetValue(title, out string tagCode))
                        {
                            Console.WriteLine();
                            foreach (Movie movie in TagByCode[tagCode].Movies)
                            {
                                if (p < 100)
                                {
                                    Console.WriteLine(movie.Titles.First());
                                    p++;
                                }
                                else
                                {
                                    Console.WriteLine("Вывести ещё фильмы да/нет: ");
                                    string answer = Console.ReadLine();
                                    if (answer == "да") { p = 0; continue; }
                                    else { break; }
                                }
                            }
                            Console.WriteLine();
                            Console.WriteLine("Введите запрос:");
                            SearchedLine = Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine("\nТакой тэг не найден. Попробуйте ещё раз:");
                            SearchedLine = Console.ReadLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nНеизвестная команда. Попробуйте ещё раз:");
                        SearchedLine = Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("\nНеизвестная строка. Попробуйте ещё раз:");
                    SearchedLine = Console.ReadLine();
                }
            }
        }
        public static void DownloadData()
        {
            using (ApplicationContext db = new ApplicationContext(false))
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                //Dictionary<string, Movie> test = db.Movies.Include(m => m.Actors).ToDictionary(m => m.Code, m => new Movie(m.Code, "NotFinded"));
                //TODO: разобраться с Titles
                MovieByCode = new ConcurrentDictionary<string, Movie>(db.Movies
                    .Include(m => m.Actors).Include(m => m.Director).Include(m => m.Tags)
                    .ToDictionary(m => m.Code, m =>
                {
                    Movie newMovie = new Movie(m.Code, m.MainTitle);
                    newMovie.AddRating(m.Rating);
                    newMovie.Director = m.Director;
                    newMovie.AddCode2(m.Code2);
                    newMovie.Tags = m.Tags;
                    if (m.Actors != null) { newMovie.Actors = m.Actors; }
                    return newMovie;
                }));
                MovieCodeByTitle = new ConcurrentDictionary<string, string>(db.Movies.ToDictionary(m => m.MainTitle, m => m.Code));
                MoviesCodesByCode2 = new ConcurrentDictionary<string, HashSet<string>>();
                foreach(var movie in db.Movies)
                {
                    MoviesCodesByCode2.AddOrUpdate(movie.Code2, new HashSet<string> { movie.Code }, (oldKey, oldValue) =>
                    {
                        oldValue.Add(movie.Code);
                        return oldValue;
                    });
                }
                ActorByCode = new ConcurrentDictionary<string, Actor>(db.Actors.Include(a => a.Movies).Include(a => a.DirectedMovies).ToDictionary(a => a.Code, a =>
                {
                    Actor newActor = new Actor(a.Code, a.Name);
                    newActor.Movies = a.Movies;
                    newActor.DirectedMovies = a.DirectedMovies;
                    return newActor;
                }));
                ActorsCodesByName = new ConcurrentDictionary<string, HashSet<string>>();
                foreach (var actor in db.Actors)
                {
                    ActorsCodesByName.AddOrUpdate(actor.Name, new HashSet<string> { actor.Code }, (oldKey, oldValue) =>
                    { 
                        oldValue.Add(actor.Code);
                        return oldValue;
                    });
                }
                TagByCode = new ConcurrentDictionary<string, Tag>(db.Tags.Include(t => t.Movies).ToDictionary(t => t.Code, t =>
                {
                    Tag newTag = new Tag(t.Code, t.Name);
                    newTag.Movies = t.Movies;
                    return newTag;
                }));
                TagCodeByName = new ConcurrentDictionary<string, string>(db.Tags.ToDictionary(t => t.Name, t => t.Code));
                Console.WriteLine("Загрузка данных из БД завершена");

                //foreach(var tag in TagByCode)
                //{
                //    Console.WriteLine(tag.Key + ": " + tag.Value.Name);
                //    if(tag.Value.Movies != null)
                //    {
                //        foreach(var movie in tag.Value.Movies)
                //        {
                //            Console.Write(movie.Code + " ");
                //        }
                //    }
                //    Console.WriteLine();
                //}

                //foreach(var actors in ActorsByName)
                //{
                //    Console.Write(actors.Key + ": ");
                //    foreach (var actor in actors.Value)
                //    {
                //        Console.Write(actor.Code + " ");
                //    }
                //    Console.WriteLine();
                //}

                //foreach (var actor in ActorByCode)
                //{
                //    Console.WriteLine(actor.Key + " " + actor.Value.Name);
                //    if (actor.Value.Movies != null)
                //    {
                //        foreach (var movie in actor.Value.Movies)
                //        {
                //            Console.Write(movie.Title + " ");
                //        }
                //        Console.WriteLine();
                //    }
                //    if (actor.Value.DirectedMovies != null)
                //    {
                //        foreach (var movie in actor.Value.DirectedMovies)
                //        {
                //            Console.Write(movie.Title + " ");
                //        }
                //        Console.WriteLine();
                //    }
                //}

                //foreach(var movie in MovieCodeByTitle)
                //{
                //    Console.WriteLine(movie.Key + " " +  movie.Value);
                //}

                //foreach(var movie in MovieByCode)
                //{
                //    Console.Write(movie.Key + " " + movie.Value.Code2 + " " + movie.Value.Rating + " " + movie.Value.Title + " ");

                //    if (movie.Value.Director != null)
                //    {
                //        foreach (Actor dir in movie.Value.Director)
                //        {
                //            Console.Write("*" + dir.Name + "*");
                //        }
                //    }

                //    if (movie.Value.Actors != null)
                //    {
                //        foreach (Actor actor in movie.Value.Actors)
                //        {
                //            Console.Write("|" + actor.Name);
                //        }
                //    }

                //    if (movie.Value.Tags != null)
                //    {
                //        foreach (Tag tag in movie.Value.Tags)
                //        {
                //            Console.Write("*" + tag.Name);
                //        }
                //    }
                //    Console.WriteLine();
                //}
            }
        }
        public static void Update()
        {
            Stopwatch StopWatch = new Stopwatch();
            StopWatch.Start();
            UpdateDictionaries(StopWatch);
            UpdateDataBase(StopWatch);
        }
        public static void UpdateDictionaries(Stopwatch StopWatch)
        {
            GC.Collect();
            UpdateMovieAndCodes(StopWatch);
            UpdateTagsAndCodes(StopWatch);
            UpdateActorsAndCodes(StopWatch);
            UpdateMoviesVSActors(StopWatch);
            UpdateMoviesVSTags(StopWatch);
        }
        public static void UpdateMovieAndCodes(Stopwatch StopWatch)
        {
            MovieByCode = new ConcurrentDictionary<string, Movie>();
            MovieCodeByTitle = new ConcurrentDictionary<string, string>();
            Parallel.ForEach(File.ReadAllLines(path1), s =>
            {
                int index0 = s.IndexOf('\t');
                int index1 = s.IndexOf('\t', index0 + 1);
                int index2 = s.IndexOf('\t', index1 + 1);
                int index3 = s.IndexOf('\t', index2 + 1);
                int index4 = s.IndexOf('\t', index3 + 1);

                ReadOnlySpan<char> line = s.AsSpan();
                ReadOnlySpan<char> region = line.Slice(index2 + 1, index3 - index2 - 1);
                ReadOnlySpan<char> lang = line.Slice(index3 + 1, index4 - index3 - 1);
                if (region.Equals("RU".AsSpan(), StringComparison.Ordinal)
                    || region.Equals("GB".AsSpan(), StringComparison.Ordinal)
                    || region.Equals("US".AsSpan(), StringComparison.Ordinal)
                    || lang.Equals("ru".AsSpan(), StringComparison.Ordinal)
                    || lang.Equals("en".AsSpan(), StringComparison.Ordinal))
                {
                    string code = line.Slice(0, index0).ToString();
                    string title = line.Slice(index1 + 1, index2 - index1 - 1).ToString();
                    string R = region.ToString();
                    string L = lang.ToString();
                    MovieCodeByTitle.TryAdd(title, code);
                    MovieByCode.AddOrUpdate(code, new Movie(code, title), (oldKey, oldValue) =>
                    {
                        lock (oldValue)
                        {
                            if (!oldValue.Titles.Contains(title))
                            {
                                oldValue.AddTitle(title, R, L);                            
                            }
                        }
                        return oldValue;
                    });
                }
            });
            Console.WriteLine("Создание словарей MovieByCode и MovieCodeByTitle завершено " + StopWatch.Elapsed);
            Parallel.ForEach(File.ReadLines(path2), s =>
            {
                int index0 = s.IndexOf('\t');
                int index1 = s.IndexOf('\t', index0 + 1);
                string codeMovie = s.Substring(0, index0);
                if (MovieByCode.TryGetValue(codeMovie, out Movie movie))
                {
                    lock (movie)
                    {
                        string rating = s.Substring(index0 + 1, index1 - index0 - 1);
                        movie.AddRating(rating);
                    }
                }
            });
            Console.WriteLine("Добавление рейтинга к фильмам завершено " + StopWatch.Elapsed);
            MoviesCodesByCode2 = new ConcurrentDictionary<string, HashSet<string>>();
            Parallel.ForEach(File.ReadLines(path3), s =>
            {
                int index0 = s.IndexOf(',');
                int index1 = s.IndexOf(',', index0 + 1);
                string code2 = s.Substring(0, index0);
                string codeMovie = "tt" + s.Substring(index0 + 1, index1 - index0 - 1);
                if (code2 != "")
                {
                    if (MovieByCode.TryGetValue(codeMovie, out Movie movie))
                    {
                        lock (movie)
                        {
                            movie.AddCode2(code2);
                        }
                        MoviesCodesByCode2.AddOrUpdate(code2, new HashSet<string> { codeMovie }, (oldKey, oldValue) =>
                        {
                            lock (oldValue)
                            {
                                oldValue.Add(codeMovie);
                            }
                            return oldValue;
                        });
                    }
                }

            });
            Console.WriteLine("Сопоставление кодов фильмов завершено " + StopWatch.Elapsed);
        }
        public static void UpdateTagsAndCodes(Stopwatch StopWatch)
        {
            TagByCode = new ConcurrentDictionary<string, Tag>();
            TagCodeByName = new ConcurrentDictionary<string,string>();
            Parallel.ForEach(File.ReadAllLines(path4), s =>
            {
                int index0 = s.IndexOf(',');
                int index1 = s.Length;
                string codeTag = s.Substring(0, index0);
                string tagName = s.Substring(index0 + 1, index1 - index0 - 1);
                TagByCode.TryAdd(codeTag, new Tag(codeTag, tagName));
                TagCodeByName.TryAdd(tagName, codeTag);
            });
            Console.WriteLine("Создание словарей TagByCode и TagByName завершено " + StopWatch.Elapsed);
        }
        public static void UpdateActorsAndCodes(Stopwatch StopWatch)
        {
            ActorByCode = new ConcurrentDictionary<string, Actor>();
            ActorsCodesByName = new ConcurrentDictionary<string, HashSet<string>>();
            Parallel.ForEach(File.ReadAllLines(path5), s =>
            {
                int index0 = s.IndexOf('\t');
                int index1 = s.IndexOf('\t', index0 + 1);
                string code = s.Substring(0, index0);
                string name = s.Substring(index0 + 1, index1 - index0 - 1);
                ActorByCode.TryAdd(code, new Actor(code, name));
                ActorsCodesByName.AddOrUpdate(name, new HashSet<string> { code }, (oldKey, oldValue) =>
                {
                    lock (oldValue)
                    {
                        oldValue.Add(code);
                    }
                    return oldValue;
                });
            });
            Console.WriteLine("Создание словаря ActorByCode завершено " + StopWatch.Elapsed);
        }
        public static void UpdateMoviesVSActors(Stopwatch StopWatch)
        {
            Parallel.ForEach(File.ReadAllLines(path6), s =>
            {

                int index0 = s.IndexOf('\t');
                int index1 = s.IndexOf('\t', index0 + 1);
                int index2 = s.IndexOf('\t', index1 + 1);
                int index3 = s.IndexOf('\t', index2 + 1);
                ReadOnlySpan<char> line = s.AsSpan();
                ReadOnlySpan<char> category = line.Slice(index2 + 1, index3 - index2 - 1);
                if (category.Equals("actor".AsSpan(), StringComparison.Ordinal))
                {
                    string movieCode = s.Substring(0, index0);
                    string actorCode = s.Substring(index1 + 1, index2 - index1 - 1);
                    if (MovieByCode.TryGetValue(movieCode, out Movie movie) && ActorByCode.TryGetValue(actorCode, out Actor actor))
                    {
                        lock (movie)
                        {
                            lock (actor)
                            {
                                movie.AddActor(actor);
                                actor.AddMovie(movie);
                            }
                        }
                    }
                }
                if (category.Equals("director".AsSpan(), StringComparison.Ordinal))
                {
                    string movieCode = s.Substring(0, index0);
                    string actorCode = s.Substring(index1 + 1, index2 - index1 - 1);
                    if (MovieByCode.TryGetValue(movieCode, out Movie movie) && ActorByCode.TryGetValue(actorCode, out Actor actor))
                    {
                        lock (movie)
                        {
                            lock (actor)
                            {
                                movie.AddDirector(actor);
                                actor.AddDirectedMovie(movie);
                            }
                        }
                    }
                }
            });
            Console.WriteLine("Добавление актеров к фильмам завершено " + StopWatch.Elapsed);

        }
        public static void UpdateMoviesVSTags(Stopwatch StopWatch)
        {
            Parallel.ForEach(File.ReadLines(path7), s =>
            {
                int index0 = s.IndexOf(',');
                int index1 = s.IndexOf(',', index0 + 1);
                int index2 = s.IndexOf(',', index1 + 1);

                //Посмотреть параллельное добавление
                ReadOnlySpan<char> line = s.AsSpan();
                ReadOnlySpan<char> relevance = line.Slice(index1 + 1, 3);
                if (Convert.ToInt32(relevance[2]) >= Convert.ToInt32('5'))
                {
                    string code2Movie = s.Substring(0, index0);
                    string codeTag = s.Substring(index0 + 1, index1 - index0 - 1);
                    if (MoviesCodesByCode2.TryGetValue(code2Movie, out HashSet<string> codesOfMovies) && TagByCode.TryGetValue(codeTag, out Tag tag))
                    {
                        foreach (string movieCode in codesOfMovies)
                        {
                            Movie movie = MovieByCode[movieCode];
                            lock (movie)
                            {
                                movie.AddTag(tag);
                            }
                            lock (tag)
                            {
                                tag.AddMovie(movie);
                            }
                        }
                    }
                }
            });
            Console.WriteLine("Добавление тэгов к фильмам завершено " + StopWatch.Elapsed);
        }
        public static void UpdateSimilarMovies(Stopwatch StopWatch)
        {
            Parallel.ForEach(MovieByCode.Values, movie =>
            {
                movie.FindSimilarMovies();
            });
            Console.WriteLine("Рекомендации похожих фильмов добавлены " + StopWatch.Elapsed);
        }
        public static void UpdateDataBase(Stopwatch StopWatch)
        {
            using(ApplicationContext db = new ApplicationContext(true))
            {
                List<Movie> ListOfMovies = new List<Movie>();
                List<Actor> ListOfActors = new List<Actor>();
                List<Tag> ListOfTags = new List<Tag>();
                foreach (var movie in MovieByCode)
                {
                    db.Movies.Add(movie.Value);
                }
                db.SaveChanges();
                Console.WriteLine("Добавление фильмов в БД завершено" + StopWatch.Elapsed);
            }
            
        }
    }
}