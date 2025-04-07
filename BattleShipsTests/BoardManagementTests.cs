using Battleships.Models;
using System.Collections.Generic;
using Board = Battleships.Management.BoardManagement;

namespace BattleShipsTests
{
    public class BoardManagementTests
    {

        public List<Coordinates> _coordinates { get; set; } = new List<Coordinates>();

        GameModel _gameModel = new GameModel();
        Board myBoard = new Board();
        Coordinates coordinate = new Coordinates();

        public List<Coordinates> Battleship { get; set; } = new List<Coordinates>();
        public List<Coordinates> Destroyer { get; set; } = new List<Coordinates>();
        public List<Coordinates> Strikes { get; set; } = new List<Coordinates>();


        Dictionary<char, int> coordinatesDict = new Dictionary<char, int> {
                                                          { 'A', 1 },
                                                          { 'B', 2 },
                                                          { 'C', 3 },
                                                          { 'D', 4 },
                                                          { 'E', 5 },
                                                          { 'F', 6 },
                                                          { 'G', 7 },
                                                          { 'H', 8 },
                                                          { 'I', 9 },
                                                          { 'J', 10 }
                                                        };

        [Fact]
        public void GameCreatedWithBattleshipTest()
        {
            // Arrange
            int BattleshipSize = 5;

            // Act, expected returns coordinates 
            _coordinates = myBoard.CreateGame(BattleshipSize, _gameModel.ShipCoordinates); //5 grid positions for the battleship
            var result = _coordinates.Count;

            // Assert
            Assert.Equal(5, result);
        }

        [Fact]
        public void GameCreatedWithDestroyerTest()
        {
            // Arrange
            int DestroyerSize = 4;

            // Act, expected returns coordinates 
            _coordinates = myBoard.CreateGame(DestroyerSize, _gameModel.ShipCoordinates); //4 grid positions for the battleship
            var result = _coordinates.Count;

            // Assert
            Assert.Equal(4, result);
        }

        [Fact]
        public void InputValidationIncorrectCoordinateTest()
        {
            // Arrange
            string strike = "A11";

            // Act
            //submit the entered coordinates
            coordinate = myBoard.InputValidation(strike, coordinatesDict);

            // Assert
            // incorrect coordinate given
            Assert.Equal(-1, coordinate.Y);
        }


        [Fact]
        public void InputValidationCorrectCoordinateTest()
        {
            // Arrange
            string strike = "A1";

            // Act
            //submit the entered coordinates
            coordinate = myBoard.InputValidation(strike, coordinatesDict);

            // Assert
            // correct coordinate given
            Assert.Equal(1, coordinate.X);
        }

        [Fact]
        public void BattleShipIsSunkTest()
        {
            // Arrange
            Battleship.Add(new Coordinates() { X = 1, Y = 3 });
            Battleship.Add(new Coordinates() { X = 1, Y = 4 });
            Battleship.Add(new Coordinates() { X = 1, Y = 5 });
            Battleship.Add(new Coordinates() { X = 1, Y = 6 });
            Battleship.Add(new Coordinates() { X = 1, Y = 7 });

            // populate the game model with the Battleship coordinates
            _gameModel.Battleship = Battleship; 

            Strikes.Add(new Coordinates() { X = 1, Y = 3 });
            Strikes.Add(new Coordinates() { X = 1, Y = 4 });
            Strikes.Add(new Coordinates() { X = 1, Y = 5 });
            Strikes.Add(new Coordinates() { X = 1, Y = 6 });
            Strikes.Add(new Coordinates() { X = 1, Y = 7 });

            // Act
            _gameModel = myBoard.GameStatus(Strikes,_gameModel);

            // Assert
            Assert.True(_gameModel.IsBattleshipSunk);
        }

        [Fact]
        public void BattleShipIsNotSunkTest()
        {
            // Arrange
            Battleship.Add(new Coordinates() { X = 1, Y = 3 });
            Battleship.Add(new Coordinates() { X = 1, Y = 4 });
            Battleship.Add(new Coordinates() { X = 1, Y = 5 });
            Battleship.Add(new Coordinates() { X = 1, Y = 6 });
            Battleship.Add(new Coordinates() { X = 1, Y = 7 });

            // populate the game model with the Battleship coordinates
            _gameModel.Battleship = Battleship;

            Strikes.Add(new Coordinates() { X = 1, Y = 3 });
            Strikes.Add(new Coordinates() { X = 1, Y = 4 });
            Strikes.Add(new Coordinates() { X = 1, Y = 5 });
            Strikes.Add(new Coordinates() { X = 1, Y = 6 });
            Strikes.Add(new Coordinates() { X = 1, Y = 8 });

            // Act
            _gameModel = myBoard.GameStatus(Strikes, _gameModel);

            // Assert
            Assert.False(_gameModel.IsBattleshipSunk);
        }

        [Fact]
        public void DestroyerIsSunkTest()
        {
            // Arrange
            Destroyer.Add(new Coordinates() { X = 1, Y = 3 });
            Destroyer.Add(new Coordinates() { X = 1, Y = 4 });
            Destroyer.Add(new Coordinates() { X = 1, Y = 5 });
            Destroyer.Add(new Coordinates() { X = 1, Y = 6 });

            // populate the game model with the Battleship coordinates
            _gameModel.Destroyer1 = Destroyer;

            Strikes.Add(new Coordinates() { X = 1, Y = 3 });
            Strikes.Add(new Coordinates() { X = 1, Y = 4 });
            Strikes.Add(new Coordinates() { X = 1, Y = 5 });
            Strikes.Add(new Coordinates() { X = 1, Y = 6 });

            // Act
            _gameModel = myBoard.GameStatus(Strikes, _gameModel);

            // Assert
            Assert.True(_gameModel.IsDestroyerSunk1);
        }

        [Fact]
        public void DestroyerIsNotSunkTest()
        {
            // Arrange
            Destroyer.Add(new Coordinates() { X = 1, Y = 3 });
            Destroyer.Add(new Coordinates() { X = 1, Y = 4 });
            Destroyer.Add(new Coordinates() { X = 1, Y = 5 });
            Destroyer.Add(new Coordinates() { X = 1, Y = 6 });

            // populate the game model with the Battleship coordinates
            _gameModel.Destroyer1 = Destroyer;

            Strikes.Add(new Coordinates() { X = 1, Y = 3 });
            Strikes.Add(new Coordinates() { X = 1, Y = 4 });
            Strikes.Add(new Coordinates() { X = 1, Y = 5 });
            Strikes.Add(new Coordinates() { X = 1, Y = 9 });

            // Act
            _gameModel = myBoard.GameStatus(Strikes, _gameModel);

            // Assert
            Assert.False(_gameModel.IsDestroyerSunk1);
        }
    }
}