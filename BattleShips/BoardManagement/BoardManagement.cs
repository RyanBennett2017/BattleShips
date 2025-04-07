using Battleships.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;

namespace Battleships.Management
{
    public class BoardManagement : IBoardManagement
    {
        #region Public
        
        //create the game
        public List<Coordinates> CreateGame(int _shipSize, List<Coordinates> _layout)
        {
            List<Coordinates> _coordinates = new List<Coordinates>();
            bool _gameCreated = false;

            //this loop will creates a set of random coordinates, ensuring that the generated ones don't overlap with any existing ones in "_layout".
            //if any duplication occurs, it continues to randomise until a valid set of coordinates is produced.
            do
            {
                //set position of the battleship and destroyer
                _coordinates = Randomize(_shipSize);
                _gameCreated = _coordinates.Any(c => _layout.Exists(e => e.X == c.X && e.Y == c.Y));
            }
            while (_gameCreated);

            _layout.AddRange(_coordinates);
            return _coordinates;
        }

        //check to keep tabs on game progress
        public GameModel GameStatus(List<Coordinates> _strikes, GameModel _gameModel)
        {
            _gameModel.IsBattleshipSunk = _gameModel.Battleship.Where(b => !_strikes.Any(a => b.X == a.X && b.Y == a.Y)).ToList().Count == 0;
            _gameModel.IsDestroyerSunk1 = _gameModel.Destroyer1.Where(d => !_strikes.Any(a => d.X == a.X && d.Y == a.Y)).ToList().Count == 0;
            _gameModel.IsDestroyerSunk2 = _gameModel.Destroyer2.Where(d => !_strikes.Any(a => d.X == a.X && d.Y == a.Y)).ToList().Count == 0;
            return _gameModel;
        }

        //receiving coordinates (a strike) onto the grid, check for errors e.g. invalid data etc, return the "strike" coordinates
        public Coordinates InputValidation(string? _strike, Dictionary<char, int> _coordinates)
        {
            Coordinates _coordinate = new Coordinates();

            if (_strike != null)
            {
                char[] _strikeArray = _strike.ToUpper().ToCharArray();

                if (_strikeArray.Length < 2 || _strikeArray.Length > 4)
                    return _coordinate;

                if (_coordinates.TryGetValue(_strikeArray[0], out int value))
                {
                    _coordinate.X = value;
                }
                else
                {
                    return _coordinate;
                }

                if (_strikeArray.Length == 3)
                {
                    if (_strikeArray[1] == '1' && _strikeArray[2] == '0')
                    {
                        _coordinate.Y = 10;
                        return _coordinate;
                    }
                    else
                    {
                        return _coordinate;
                    }
                }

                if (_strikeArray[1] - '0' > 9)
                {
                    return _coordinate;
                }
                else
                {
                    _coordinate.Y = _strikeArray[1] - '0';
                }
            }

            return _coordinate;
        }

        #endregion

        #region Private

        //randomize ship positions
        private List<Coordinates> Randomize(int _shipSize)
        {
            List<Coordinates> _coordinates = new List<Coordinates>();

            //set a random direction - so we'll use odd numbers to represent horizontal poitioning and we'll use even numbers to represent vertical positioning
            int _direction = new Random().Next(1, _shipSize);

            //set a random row number between 1 and 10
            int _row = new Random().Next(1, 11);

            //set a random column number between 1 and 10
            int _col = new Random().Next(1, 11);

            //if vertical positioning
            if (_direction % 2 != 0)
            {
                //if the column selected minus the ship size is greater than zero, we can place that ship within that row
                if (_row - _shipSize > 0)
                {
                    //will set from row right to left
                    for (int i = 0; i < _shipSize; i++)
                    {
                        Coordinates _coordinate = new Coordinates();
                        _coordinate.X = _row - i;
                        _coordinate.Y = _col;
                        _coordinates.Add(_coordinate);
                    }
                }
                else
                {
                    //will set row left to right
                    for (int i = 0; i < _shipSize; i++)
                    {
                        Coordinates _coordinate = new Coordinates();
                        _coordinate.X = _row + i;
                        _coordinate.Y = _col;
                        _coordinates.Add(_coordinate);
                    }
                }
            }
            //else set horizontal positioning
            else
            {
                //if the column selected minus the ship size is greater than zero, we can place that ship within that row
                if (_col - _shipSize > 0)
                {
                    //set column bottom to top
                    for (int i = 0; i < _shipSize; i++)
                    {
                        Coordinates _coordinate = new Coordinates();
                        _coordinate.X = _row;
                        _coordinate.Y = _col - i;
                        _coordinates.Add(_coordinate);
                    }
                }
                else
                {
                    //else set column top to bottom
                    for (int i = 0; i < _shipSize; i++)
                    {
                        Coordinates _coordinate = new Coordinates();
                        _coordinate.X = _row;
                        _coordinate.Y = _col + i;
                        _coordinates.Add(_coordinate);
                    }
                }
            }
            return _coordinates;
        }

        #endregion
    }
}
