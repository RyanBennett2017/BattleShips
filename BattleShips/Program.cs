using Battleships.Management;
using Battleships.Models;
using Battleships.Startup;
using Microsoft.Extensions.DependencyInjection;
using static System.Console;

namespace Battleships
{
    public class Program
    {
        //set to true to actually see position of the "enemy" - good for testing. Set to false for actual game play
        const bool _showShips = false;

        private readonly IBoardManagement _gameManager;

        public Program(IBoardManagement gameManager)
        {
            _gameManager = gameManager;
        }

        public static void Main()
        {
            //registering service (management of the business logic)
            IServiceProvider serviceProvider = DependencyResolver.RegisterServices();
            Program? program = serviceProvider.GetService<Program>();

            //fire up a game
            if (program != null)
                program.StartGame();

            DependencyResolver.DisposeServices(serviceProvider);
        }

        #region Game Logic

        public void StartGame()
        {
            Strikes _strikeCoordinates = new Strikes();
            GameModel _gameModel = new GameModel();

            _gameModel.Battleship = _gameManager.CreateGame(5, _gameModel.ShipCoordinates); //5 grid positions for the battleship
            _gameModel.Destroyer1 = _gameManager.CreateGame(4, _gameModel.ShipCoordinates); //4 grid positions for the destroyer
            _gameModel.Destroyer2 = _gameManager.CreateGame(4, _gameModel.ShipCoordinates); //4 grid positions for the destroyer

            //set the rows A to J (1 to 10)
            Dictionary<char, int> _coordinates = SetRows();

            //output to the console the layout
            DisplayGrid(_strikeCoordinates, _gameModel, _showShips);

            //create the game with 100 available coordinates to "strike"
            int _game;
            for (_game = 1; _game < 101; _game++)
            {
                Coordinates _coordinate = new Coordinates();

                //input to fire
                ForegroundColor = ConsoleColor.White;
                WriteLine("\nEnter your firing coordinates (e.g. A1):");
                string? _strike = ReadLine();

                //submit the entered coordinates
                _coordinate = _gameManager.InputValidation(_strike, _coordinates);

                //error: incorrect coordinates entered
                if (_coordinate.X == -1 || _coordinate.Y == -1)
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("\nSorry but you've entered a set of invalid firing coordinates!!!");
                    continue;
                }

                //error: coordinates already entered
                if (_strikeCoordinates.StrikeCoordinates.Any(s => s.X == _coordinate.X && s.Y == _coordinate.Y))
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine("\nYou've already hit that coordinate before!!!");
                    continue;
                }

                //add valid strike to array of strikes
                var index = _strikeCoordinates.StrikeCoordinates.FindIndex(p => p.X == _coordinate.X && p.Y == _coordinate.Y);
                if (index == -1)
                    _strikeCoordinates.StrikeCoordinates.Add(_coordinate);

                //clear the console so we can refresh with updated coordinates (strike)
                Clear();

                //get updated model data - coordinate positioning, if any ship is sunk or even if the game is over
                _gameModel.ShipCoordinates.OrderBy(o => o.X).ThenBy(n => n.Y).ToList();
                _gameModel = _gameManager.GameStatus(_strikeCoordinates.StrikeCoordinates, _gameModel);

                //output the game (refresh) afdter every strike, updating the layout
                DisplayGrid(_strikeCoordinates, _gameModel, _showShips);

                //this exists to add spacing on the grid only
                if ((!_gameModel.IsBattleshipSunk && _gameModel.IsDestroyerSunk1 && _gameModel.IsDestroyerSunk2) || (_gameModel.IsBattleshipSunk && !_gameModel.IsDestroyerSunk1 && !_gameModel.IsDestroyerSunk2) || (_gameModel.IsBattleshipSunk && _gameModel.IsDestroyerSunk1 && _gameModel.IsDestroyerSunk2))
                    WriteLine();


                //alert: if the battleship is sunk write out a message
                if (_gameModel.IsBattleshipSunk)
                {
                    ForegroundColor = ConsoleColor.DarkRed;
                    WriteLine("Well Done Battleship SUNK!!!");
                }

                //alert: if the destroyer is sunk write out a message
                if (_gameModel.IsDestroyerSunk1)
                {
                    ForegroundColor = ConsoleColor.DarkRed;
                    WriteLine("Well Done Destroyer 1 SUNK!!!");
                }

                //alert: if the destroyer is sunk write out a message
                if (_gameModel.IsDestroyerSunk2)
                {
                    ForegroundColor = ConsoleColor.DarkRed;
                    WriteLine("Well Done Destroyer 2 SUNK!!!");
                }

                //if the game is over (both battleship and destroyer "sunk", then the game ends)
                if (_gameModel.IsDestroyerSunk1 && _gameModel.IsDestroyerSunk2 && _gameModel.IsBattleshipSunk) { break; }
            }

            //alert: when the game is won!
            ForegroundColor = ConsoleColor.White;
            if (_gameModel.IsDestroyerSunk1 && _gameModel.IsDestroyerSunk2 && _gameModel.IsBattleshipSunk)
                WriteLine("\nGAME OVER!!!");

            WriteLine("\nYou fired a total of {0} shots!!!", _strikeCoordinates.StrikeCoordinates.Count);
            ReadLine();
        }

        #endregion

        #region Console Output

        //output: display the game on the console
        static void DisplayGrid(Strikes _strikeCoordinates, GameModel _gameModel, bool _showShips)
        {
            DisplayHeader();
            WriteLine();

            List<Coordinates> _strikes = _strikeCoordinates.StrikeCoordinates.OrderBy(s => s.X).ThenBy(s => s.Y).ToList();
            List<Coordinates> _shipCoordinates = _gameModel.ShipCoordinates.OrderBy(s => s.X).ThenBy(s => s.Y).ToList();

            _shipCoordinates = _shipCoordinates.Where(s => !_strikes.Exists(p => p.X == s.X && p.Y == s.Y)).ToList();

            int _hitCounter = 0;
            int _shipCounter = 0;
            _gameModel.CurrentHitCount = 0;

            char row = 'A';
            try
            {
                for (int x = 1; x < 11; x++)
                {
                    for (int y = 1; y < 11; y++)
                    {
                        bool _continue = true;

                        if (y == 1)
                        {
                            ForegroundColor = ConsoleColor.White;
                            Write(" " + row + " ");
                            row++;
                        }

                        if (_strikes.Count != 0 && _strikes[_hitCounter].X == x && _strikes[_hitCounter].Y == y)
                        {
                            if (_strikes.Count - 1 > _hitCounter)
                                _hitCounter++;

                            //if any strike matches the position of any ship change display to a red "X" indicating a "hit"
                            if (_gameModel.ShipCoordinates.Exists(s => s.X == x && s.Y == y))
                            {
                                ForegroundColor = ConsoleColor.Red;
                                Write("[X]");
                                _continue = false;
                                _gameModel.CurrentHitCount++;
                            }
                            //else display a "miss"
                            else
                            {
                                ForegroundColor = ConsoleColor.White;
                                Write("[X]");
                                _continue = false;
                            }
                        }

                        //display ships if bool "showShip" is set to true - useful for testing
                        if (_continue && _showShips && _shipCoordinates.Count != 0 && _shipCoordinates[_shipCounter].X == x && _shipCoordinates[_shipCounter].Y == y)
                        {
                            if (_shipCoordinates.Count - 1 > _shipCounter)
                                _shipCounter++;

                            ForegroundColor = ConsoleColor.DarkGreen;
                            Write("[#]");
                            _continue = false;
                        }

                        if (_continue)
                        {
                            ForegroundColor = ConsoleColor.DarkGray;
                            Write("[ ]");
                        }

                        DisplayLegend(x, y, _gameModel, _strikes);
                        
                    }
                    WriteLine();
                }    
            }
            catch (Exception ex)
            {
                string error = ex.Message.ToString();
            }

            DisplayResult(_gameModel, _strikes);
            _gameModel.PreviousHitCount = _gameModel.CurrentHitCount;
        }

        //output: displaying the "legend" as in number of ships you need to sink and their "length"
        static void DisplayLegend(int _x, int _y, GameModel _gameModel, List<Coordinates> _strikes)
        {
            //when the grid is rendered upon every "strike" we re-write the legend to the correct position from the x and y coordinates incremented during the render (amending colour from green to red as each ship is sunk)

            if (_x == 1 && _y == 10)
            {
                ForegroundColor = ConsoleColor.White;
                Write("    Enemy Ships:");
            }

            if (_x == 3 && _y == 10)
            {
                if (_gameModel.IsBattleshipSunk)
                {
                    ForegroundColor = ConsoleColor.Red;
                    Write("    Battleship [5]");
                }
                else
                {
                    ForegroundColor = ConsoleColor.DarkGreen;
                    Write("    Battleship [5]");
                }
            }

            if (_x == 4 && _y == 10)
            {
                if (_gameModel.IsDestroyerSunk1)
                {
                    ForegroundColor = ConsoleColor.Red;
                    Write("    Destroyer 1 [4] ");
                }
                else
                {
                    ForegroundColor = ConsoleColor.DarkGreen;
                    Write("    Destroyer 1 [4] ");
                }
            }

            if (_x == 5 && _y == 10)
            {
                if (_gameModel.IsDestroyerSunk2)
                {
                    ForegroundColor = ConsoleColor.Red;
                    Write("    Destroyer 2 [4] ");
                }
                else
                {
                    ForegroundColor = ConsoleColor.DarkGreen;
                    Write("    Destroyer 2 [4] ");
                }
            }
        }

        static void DisplayResult(GameModel _gameModel, List<Coordinates> _strikes)
        {
                if (_gameModel.CurrentHitCount > _gameModel.PreviousHitCount)
                {
                    ForegroundColor = ConsoleColor.Green;
                    WriteLine("\n\tGreat Shot!!!");
                }
                else
                {
                    if (_strikes.Count > 0)
                    {
                        ForegroundColor = ConsoleColor.White;
                        WriteLine("\n\tUnlucky...Try again!!!");
                    }
                }

        }

        //output: display the header as in number 1 to 10
        static void DisplayHeader()
        {
            ForegroundColor = ConsoleColor.White;
            Write("   ");
            for (int i = 1; i < 11; i++)
            {
                Write(" " + i + " ");
            }
        }

        //output: sets up to display the rows from A to J (1-10)
        static Dictionary<char, int> SetRows()
        {
            Dictionary<char, int> _rows = new Dictionary<char, int> { { 'A', 1 }, { 'B', 2 }, { 'C', 3 }, { 'D', 4 }, { 'E', 5 }, { 'F', 6 }, { 'G', 7 }, { 'H', 8 }, { 'I', 9 }, { 'J', 10 } };
            return _rows;
        }

        #endregion
    }
}