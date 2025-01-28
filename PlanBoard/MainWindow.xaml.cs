using PlanBoard.Tools;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace PlanBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool AddNoteMode = false;
        string BoardFolder = "SavedBoards";

        public MainWindow()
        {
            InitializeComponent();

            FillLoadMenu();
        }

        private void SaveBoard_Click(object sender, RoutedEventArgs e)
        {
            string NewFilePath = $"{BoardFolder}/{SaveFileNameInput.Text}.txt";
            SaveFileNameInput.Text = "";
            FileManager.Save(NewFilePath,BoardContainer.Content);
            FillLoadMenu();
        }

        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            AddNoteMode = true;
        }

        private void FillLoadMenu()
        {
            LoadBoardMenu.Items.Clear();
            List<string> files = Directory.GetFiles(BoardFolder).ToList();
            foreach (string file in files) {
                string BoardName = Path.GetFileName(file).Replace(Path.GetExtension(file),"");
                MenuItem BItem = new MenuItem();
                BItem.Header = BoardName;
                BItem.Click += BItem_Click;

                LoadBoardMenu.Items.Add(BItem);
            }
        }

        private void BItem_Click(object sender, RoutedEventArgs e)
        {
            string BoardPath = $"{BoardFolder}/{(sender as MenuItem).Header}.txt";
            BoardContainer.Content = FileManager.Load<Canvas>(BoardPath);
        }

        private void ProjectView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (AddNoteMode)
            {
                Point mc = e.GetPosition((Canvas)BoardContainer.Content);

                // creating the note
                Border NotePaper = new Border();
                NotePaper.CornerRadius = new CornerRadius(20);
                NotePaper.Background = new SolidColorBrush(Colors.Black);
                NotePaper.BorderThickness = new Thickness(0);
                NotePaper.Width = 200;
                NotePaper.Height = 200;

                Grid NoteFormat = new Grid();
                NoteFormat.RowDefinitions.Add(new RowDefinition() {
                    Height = new GridLength(0.2,GridUnitType.Star) 
                });
                NoteFormat.RowDefinitions.Add(new RowDefinition());
                NoteFormat.ColumnDefinitions.Add(new ColumnDefinition());
                NoteFormat.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(0.2, GridUnitType.Star)
                });
                NoteFormat.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(0.2, GridUnitType.Star)
                });
                NotePaper.Child = NoteFormat;

                TextBox NoteTitle = new TextBox();
                NoteTitle.Background = new SolidColorBrush(Colors.Transparent);
                NoteTitle.Text = "Title";
                NoteTitle.BorderBrush = new SolidColorBrush(Colors.Transparent);
                NoteTitle.Foreground = new SolidColorBrush(Colors.LightSlateGray);
                NoteTitle.FontWeight = FontWeights.Bold;
                NoteTitle.Margin = new Thickness(10,5,0,0);
                NoteTitle.FontSize = 20;
                NoteFormat.Children.Add(NoteTitle);
                Grid.SetRow(NoteTitle, 0);
                Grid.SetColumn(NoteTitle, 0);

                TextBox NotePassage = new TextBox();
                NotePassage.Background = new SolidColorBrush(Colors.Transparent);
                NotePassage.Text = "Enter Text...";
                NotePassage.BorderBrush = new SolidColorBrush(Colors.Transparent);
                NotePassage.Foreground = new SolidColorBrush(Colors.GhostWhite);
                //NotePassage.FontWeight = FontWeights.Bold;
                NotePassage.Margin = new Thickness(10);
                NotePassage.FontSize = 15;
                NotePassage.TextWrapping = TextWrapping.Wrap;
                NoteFormat.Children.Add(NotePassage);
                Grid.SetRow(NotePassage, 1);
                Grid.SetColumnSpan(NotePassage, 3);
                Grid.SetColumn(NotePassage, 0);

                ((Canvas)BoardContainer.Content).Children.Add(NotePaper);
                Canvas.SetTop(NotePaper, mc.Y);
                Canvas.SetLeft(NotePaper, mc.X);

                AddNoteMode = false;
            }
        }
    }
}