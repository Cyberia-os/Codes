using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace PuzzleGame
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private const int Size = 4;
        private const int Total = Size * Size;
        private const int EasyShuffleSteps = 8;
        private const int NormalShuffleSteps = 1000;

        private const string RulesText =
            "1. Картинка разрезана на кусочки и перемешана на поле 4×4.\n\n" +
            "2. Одна ячейка пустая. Кликните по кусочку рядом с ней (сверху, снизу, слева или справа), и он переместится на её место.\n\n" +
            "3. Двигать кусочки по диагонали или через другие кусочки нельзя.\n\n" +
            "4. Цель: расставить кусочки по порядку (слева направо, сверху вниз). Пустая ячейка должна оказаться в правом нижнем углу.\n\n" +
            "5. Старайтесь собрать картинку за как можно меньшее число ходов.";

        private int _shuffleSteps = EasyShuffleSteps;

        public ObservableCollection<Tile> Tiles { get; } = new ObservableCollection<Tile>();

        private int _moveCount;
        public int MoveCount
        {
            get => _moveCount;
            set { _moveCount = value; OnPropertyChanged(nameof(MoveCount)); }
        }

        private string _statusMessage = "Соберите картинку по кусочкам!";
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(nameof(StatusMessage)); }
        }

        private enum Screen { Menu, Rules, Game }

        private Screen _currentScreen = Screen.Menu;
        private Screen CurrentScreen
        {
            get => _currentScreen;
            set
            {
                _currentScreen = value;
                OnPropertyChanged(nameof(IsMenuVisible));
                OnPropertyChanged(nameof(IsRulesVisible));
                OnPropertyChanged(nameof(IsGameVisible));
            }
        }

        public bool IsMenuVisible => CurrentScreen == Screen.Menu;
        public bool IsRulesVisible => CurrentScreen == Screen.Rules;
        public bool IsGameVisible => CurrentScreen == Screen.Game;

        public ICommand StartEasyGameCommand { get; }
        public ICommand StartNormalGameCommand { get; }
        public ICommand NewGameCommand { get; }
        public ICommand TileClickCommand { get; }
        public ICommand ShowRulesCommand { get; }
        public ICommand ShowRulesDialogCommand { get; }
        public ICommand BackToMenuCommand { get; }
        public ICommand ExitCommand { get; }

        private readonly Random _random = new Random();

        public GameViewModel()
        {
            StartEasyGameCommand = new RelayCommand(_ => StartGameFromMenu(EasyShuffleSteps));
            StartNormalGameCommand = new RelayCommand(_ => StartGameFromMenu(NormalShuffleSteps));
            NewGameCommand = new RelayCommand(_ => StartNewGame());
            TileClickCommand = new RelayCommand(p => OnTileClicked(p as Tile));
            ShowRulesCommand = new RelayCommand(_ => CurrentScreen = Screen.Rules);
            ShowRulesDialogCommand = new RelayCommand(_ => ShowRulesDialog());
            BackToMenuCommand = new RelayCommand(_ => CurrentScreen = Screen.Menu);
            ExitCommand = new RelayCommand(_ => Application.Current.Shutdown());

            StartNewGame();
        }

        private void ShowRulesDialog()
        {
            MessageBox.Show(RulesText, "Правила игры",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void StartGameFromMenu(int shuffleSteps)
        {
            _shuffleSteps = shuffleSteps;
            StartNewGame();
            CurrentScreen = Screen.Game;
        }

        private void StartNewGame()
        {
            int[] numbers;
            do { numbers = GenerateSolvablePermutation(); }
            while (IsSolvedArray(numbers));

            Tiles.Clear();
            for (int i = 0; i < numbers.Length; i++)
                Tiles.Add(new Tile(numbers[i], i / Size, i % Size));

            MoveCount = 0;
            StatusMessage = "Соберите картинку по кусочкам!";
        }

        private void OnTileClicked(Tile clickedTile)
        {
            if (clickedTile == null || clickedTile.IsEmpty) 
                return;

            Tile emptyTile = FindEmptyTile();
            if (!AreAdjacent(clickedTile, emptyTile))
                return;

            SwapPositions(clickedTile, emptyTile);
            MoveCount++;

            if (IsSolved())
                StatusMessage = $"Поздравляем! Вы собрали картинку за {MoveCount} ходов!";
        }

        private Tile FindEmptyTile()
        {
            foreach (var tile in Tiles)
                if (tile.IsEmpty) 
                    return tile;
            return null;
        }

        private bool AreAdjacent(Tile a, Tile b)
        {
            bool sameRow = a.Row == b.Row && Math.Abs(a.Column - b.Column) == 1;
            bool sameCol = a.Column == b.Column && Math.Abs(a.Row - b.Row) == 1;
            return sameRow || sameCol;
        }

        private void SwapPositions(Tile a, Tile b)
        {
            int tr = a.Row, tc = a.Column;
            a.Row = b.Row; a.Column = b.Column;
            b.Row = tr; b.Column = tc;
        }

        private bool IsSolved()
        {
            foreach (var tile in Tiles)
            {
                int expected = tile.Row * Size + tile.Column;
                if (tile.IsEmpty) { if (expected != Total - 1) return false; }
                else { if (tile.Number != expected + 1) return false; }
            }
            return true;
        }

        private bool IsSolvedArray(int[] numbers)
        {
            for (int i = 0; i < numbers.Length - 1; i++)
                if (numbers[i] != i + 1) return false;
            return numbers[numbers.Length - 1] == 0;
        }

        private int[] GenerateSolvablePermutation()
        {
            int[] numbers = new int[Total];
            for (int i = 0; i < Total - 1; i++) numbers[i] = i + 1;
            numbers[Total - 1] = 0;

            int emptyIndex = Total - 1;

            for (int step = 0; step < _shuffleSteps; step++)
            {
                var neighbors = GetNeighborIndices(emptyIndex);
                int swapWith = neighbors[_random.Next(neighbors.Count)];

                (numbers[emptyIndex], numbers[swapWith]) =
                    (numbers[swapWith], numbers[emptyIndex]);

                emptyIndex = swapWith;
            }
            return numbers;
        }

        private List<int> GetNeighborIndices(int index)
        {
            var result = new List<int>();
            int row = index / Size, col = index % Size;

            if (row > 0) result.Add(index - Size);
            if (row < Size - 1) result.Add(index + Size);
            if (col > 0) result.Add(index - 1);
            if (col < Size - 1) result.Add(index + 1);

            return result;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}