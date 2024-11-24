using Newtonsoft.Json.Linq;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.SQLite;
using System.Text;
using System.Text.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        //Console.WriteLine("Hello, World!");
        string connectionString = "Data Source=bookRecommender.db;Version=3";

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            SQLiteCommand command = connection.CreateCommand();

            // Create table
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Books (
                    book_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    title INTEGER,
                    author_id INTEGER,
                    description TEXT,
                    img_url TEXT,
                    ratings_count INTEGER,
                    lcv INTEGER DEFAULT 0
                )
            ";
            command.ExecuteNonQuery();

            // Insert data
          

            string filePath = "C:\\Users\\s.prandi\\Downloads\\goodreads_books_romance.json";

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(@"
                        INSERT INTO Books(book_id, title, author_id, description, img_url, ratings_count)
                        VALUES ");

                    int counter = 0;
                    while ((line = sr.ReadLine()) != null)
                    {

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
                        int book_id = (int)jsonLine["book_id"]!;
                        string title = jsonLine["title"]!.ToString();
                        title = title.Replace("'", " ");
                        var authors = jsonLine["authors"]!;
                        var firstAuthor = authors[0];
                        var author_id = (int)firstAuthor["author_id"];
                        string description = jsonLine["description"]!.ToString();
                        if (string.IsNullOrWhiteSpace(description))
                        {
                            continue;
                        }
                        description = description.Replace("'", " ");
                        description = description.Replace("\n", " ");
                        string image_url = jsonLine["image_url"]!.ToString();
                        int ratings_count = (int)jsonLine["ratings_count"]!;
                        if (ratings_count <= 199)
                        {
                            continue;
                        }
                        string values = $"({book_id}, '{title}', {author_id}, '{description}', '{image_url}', {ratings_count})";

                        sb.Append(values + ",");

                        counter++;
                        if (counter % 1000 == 0)
                        {
                            Console.WriteLine(counter);
                        }
                    }

                    command.CommandText = sb.ToString().Substring(0, sb.ToString().Length - 1);

                    command.ExecuteNonQuery();

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