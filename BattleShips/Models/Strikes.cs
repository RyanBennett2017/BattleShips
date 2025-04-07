namespace Battleships.Models
{
    public class Strikes
    {
        //running total of strikes made when playing
        public List<Coordinates> StrikeCoordinates { get; set; } = new List<Coordinates>();
    }
}
