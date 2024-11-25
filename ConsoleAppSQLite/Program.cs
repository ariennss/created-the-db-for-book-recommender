using Newtonsoft.Json.Linq;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.SQLite;
using System.Text;
using System.Text.Json;

internal class Program
{
    // inserimento reviews funziona fino alla sb ma poi dice che non soddisfa il requisito di chiave.

    private static void Main(string[] args)
    {
        //Console.WriteLine("Hello, World!");
        string connectionString = "Data Source=bookRecommender.db;Version=3";

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            SQLiteCommand command = connection.CreateCommand();

            // Create table for books
            //command.CommandText = @"
            //    CREATE TABLE IF NOT EXISTS Books (
            //        book_id INTEGER PRIMARY KEY AUTOINCREMENT,
            //        title INTEGER,
            //        author_id INTEGER,
            //        description TEXT,
            //        img_url TEXT,
            //        ratings_count INTEGER,
            //        lcv INTEGER DEFAULT 0
            //    )
            //";


            //Create table for authors
            //command.CommandText = @" CREATE TABLE IF NOT EXISTS Authors (
            //        author_id INTEGER PRIMARY KEY AUTOINCREMENT,
            //        full_name TEXT,
            //        lcv INTEGER DEFAULT 0
            //    )";

            //Create table for reviews
            command.CommandText = @" CREATE TABLE IF NOT EXISTS Reviews (
                    review_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    book_id INTEGER,
                    user_id TEXT,
                    rating INTEGER,
                    lcv INTEGER DEFAULT 0
                )";
            // Insert data

            command.ExecuteNonQuery();

            string filePath = "C:\\Downloads\\reviews.json";

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;


                    int batchSize = 1000000;
                    int counter = 0;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(@"INSERT INTO Reviews(book_id, user_id, rating) VALUES ");
                    while ((line = sr.ReadLine()) != null)
                    {
                        
                        //sb.AppendLine(@"
                        //    INSERT INTO Books(book_id, title, author_id, description, img_url, ratings_count)
                        //    VALUES ");

                        
                        if (line.StartsWith("["))
                        {
                            line = line.Substring(1);
                        }
                        if (line.EndsWith("]"))
                        {
                            line = line.Substring(0, line.Length - 1);
                        }
                        if (line.EndsWith(","))
                        {
                            line = line.Substring(0, (line.Length - 1));
                        }

                        JObject jsonLine = JObject.Parse(line);
                        //int book_id = (int)jsonLine["book_id"]!;
                        //string title = jsonLine["title"]!.ToString();
                        //title = title.Replace("'", " ");
                        //var authors = jsonLine["authors"]!;
                        //if (authors == null || authors.Count() == 0)
                        //{
                        //    continue;
                        //}
                        //var firstAuthor = authors[0];
                        //var author_id = (int)firstAuthor["author_id"];
                        //string description = jsonLine["description"]!.ToString();
                        //if (string.IsNullOrWhiteSpace(description))
                        //{
                        //    continue;
                        //}
                        //description = description.Replace("'", " ");
                        //description = description.Replace("\n", " ");
                        //string image_url = jsonLine["image_url"]!.ToString();
                        //int ratings_count = (int)jsonLine["ratings_count"]!;
                        //if (ratings_count <= 199)
                        //{
                        //    continue;
                        //}

                        //for authors:
                        //int author_id = (int)jsonLine["author_id"];
                        //string name = jsonLine["name"]!.ToString();
                        //name = name.Replace("'", " ");

                        //FOR REVIEWS
                        int book_id = (int)jsonLine["book_id"]!;
                        int rating = (int)jsonLine["rating"]!;
                        string user_id = jsonLine["user_id"]!.ToString();

                        //string values = $"({book_id}, '{title}', {author_id}, '{description}', '{image_url}', {ratings_count})";
                        string values = $"({book_id}, '{user_id}', {rating})";
                        counter++;
                        sb.Append(values + ",");
                        if (counter % batchSize == 0 || line == null)
                        {
                            // Remove the trailing comma
                            sb.Length--;

                            // Execute the batch
                            command.CommandText = sb.ToString();
                            command.ExecuteNonQuery();

                            // Clear the StringBuilder for the next batch
                            sb.Clear();
                            sb.AppendLine(@"INSERT INTO Reviews(book_id, user_id, rating) VALUES ");

                            Console.WriteLine($"{counter} rows inserted.");
                        }
                        //command.CommandText = sb.ToString().Substring(0, sb.ToString().Length - 1);

                        //command.ExecuteNonQuery();

                      
                        if (counter % 1000 == 0)
                        {
                            Console.WriteLine(counter);
                        }
                    }

                  

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }

    public class Book
    {
        public int Book_id { get; set; }
        public string Title { get; set; }
        public int Author_id { get; set; }
        public string Description { get; set; }
        public string Img_url { get; set; }
        public int Ratings_count { get; set; }
        public bool Lcv { get; set; }
    }
}