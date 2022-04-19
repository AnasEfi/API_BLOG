using System.Text;    
namespace API_BLOG
{
    public class Utils
    {
        public static string GetCurrentDateAsString()
        => $"{DateTime.Now.Day}/{DateTime.Now.Month}/{DateTime.Now.Year}";
        public static int GetDaysFromCreationDate(string date)
            =>
            (int)DateTime.Now.Subtract(
            new DateTime(Convert.ToInt32(date.Split('/')[2]), Convert.ToInt32(date.Split('/')[1]), Convert.ToInt32(date.Split('/')[0]))
            ).TotalDays;
    }
}
