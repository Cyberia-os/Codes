using System;
using System.ComponentModel;
using System.IO;

namespace PuzzleGame
{
    public class Tile : INotifyPropertyChanged
    {
        private int _row;
        private int _column;
        public int Number { get; }

        public int Row
        {
            get => _row;
            set { _row = value; OnPropertyChanged(nameof(Row)); }
        }

        public int Column
        {
            get => _column;
            set { _column = value; OnPropertyChanged(nameof(Column)); }
        }

        public bool IsEmpty => Number == 0;

        public string ImagePath => Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Images", $"{Number}.jpg");

        public Tile(int number, int row, int column)
        {
            Number = number;
            _row = row;
            _column = column;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}