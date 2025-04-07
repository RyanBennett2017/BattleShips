using Battleships.Models;

namespace Battleships.Management
{
    public interface IBoardManagement
    {
        List<Coordinates> CreateGame(int _shipSize, List<Coordinates> _layout);
        GameModel GameStatus(List<Coordinates> _strikes, GameModel _gameModel);
        Coordinates InputValidation(string? _strike, Dictionary<char, int> _coordinates);
    }
}