
namespace Battleships.Models
{
    //game model
    public class GameModel
    {
        // current HIT count
        public int CurrentHitCount { get; set; }

        // previous HIT count
        public int PreviousHitCount { get; set; }

        //the battleship
        public List<Coordinates> Battleship { get; set; } = new List<Coordinates>();

        //destroyer1
        public List<Coordinates> Destroyer1 { get; set; } = new List<Coordinates>();

        //destroyer2
        public List<Coordinates> Destroyer2 { get; set; } = new List<Coordinates>();

        //ship coordinates - combination of the position of both the battlship and the destroyer
        public List<Coordinates> ShipCoordinates { get; set; } = new List<Coordinates>();
        
        //boolean value to determine if the battleship is sunk
        public bool IsBattleshipSunk { get; set; } = false;
        
        //boolean value used to set if the destroyer is sunk
        public bool IsDestroyerSunk1 { get; set; } = false;

        public bool IsDestroyerSunk2 { get; set; } = false;

    }
}
