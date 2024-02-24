using GymHubApp.Helpers;
using GymHubApp.Models;
using SQLite;

namespace GymHubApp
{
    public class LocalDB
    {

        private static string DB_NAME = "gymhub_local_db.db3";
        private static SQLiteConnection connection;

        public async static Task<bool> CheckUser()
        {
            try
            {
                connection = new SQLiteConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
                connection.CreateTable<User>();
                var count = connection.Table<User>().Count();
                if (count == 0)
                    return false;
                else
                {
                    User user = connection.Table<User>().FirstOrDefault();
                    if (await JWTVerifier.IsJwtExpired(user.token))
                    {
                        Console.WriteLine(user.token);
                        return true;
                    }
                    return false; 
                }
              
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error storing user: {ex.Message}");
                return false;
            }
            finally
            {
                connection.Close();
            }
        }


        public static bool StoreUser(User user)
        {
            try
            {
                dropTable();
                connection = new SQLiteConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
                connection.CreateTable<User>();
                connection.Insert(user);
                connection.Table<User>().ToList();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error storing user: {ex.Message}");
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        public static User GetUser()
        {
            try
            {
                connection = new SQLiteConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
                connection.CreateTable<User>();
                User user = connection.Table<User>().FirstOrDefault();
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving first user: {ex.Message}");
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        public static void dropTable()
        {
            connection = new SQLiteConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
            connection.CreateTable<User>();
            connection.DropTable<User>();
        }

    }
}

