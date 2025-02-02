using BLL.Interfaces;
using BLL.Models;
using DAL.Entities;
using PlanBoard.ViewModels;
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
using System.Windows.Markup;
using BCrypt.Net;

namespace PlanBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BoardViewModel _BVM;

        // Activated when Add note button is clicked
        bool AddNoteMode = false;

        // Connected Account
        UserModel _User = null;

        // For guest
        string LocalBoardFolder = "SavedBoards";

        public MainWindow(BoardViewModel BVM)
        {
            InitializeComponent();

            this.DataContext = _BVM = BVM;

            // Load Board menu updating
            FillLoadMenu();
        }

        private void SaveBoard_Click(object sender, RoutedEventArgs e)
        {
            // xaml string representation of the Board
            string mystrXAML = XamlWriter.Save(BoardContainer.Content);

            if (_User == null) // Guest save: Just dowload the xamlstring into a local .txt document
            {
                string NewFilePath = $"{LocalBoardFolder}/{SaveFileNameInput.Text}.txt";
                SaveFileNameInput.Text = "";
                Directory.CreateDirectory(Path.GetDirectoryName(NewFilePath));
                File.WriteAllText(NewFilePath, mystrXAML);
            }
            else // Accoutn Save: Loads the xaml string into a database on a web server.
            {
                if (!_User.Boards.Exists(X=>X.Name==SaveFileNameInput.Text)) // Creates a new board
                {
                    _User.Boards.Add(new BoardModel() { Content = mystrXAML, Name = SaveFileNameInput.Text });
                    _BVM.UserService.Update(_User);
                    MessageBox.Show("Board Saved");
                }
                else // Updates an existing board
                {
                    _User.Boards.Where(X => X.Name == SaveFileNameInput.Text).First().Content = mystrXAML;
                    _BVM.UserService.Update(_User);
                    MessageBox.Show("Board Updated");
                }
                ShareCodeView.Text = $"{Convert.ToBase64String(Encoding.UTF8.GetBytes(_User.Username))}//Board:{SaveFileNameInput.Text}";
            }
            FillLoadMenu();
        }

        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            AddNoteMode = true;
        }

        private void FillLoadMenu()
        {
            // Remove Previous boards (to refresh)
            LoadBoardMenu.Items.Clear();

            if (_User == null)// Guest boards
            {
                if (Directory.Exists(LocalBoardFolder)) // Gets boards from folder
                {
                    List<string> files = Directory.GetFiles(LocalBoardFolder).ToList();
                    foreach (string file in files)
                    {
                        string BoardName = Path.GetFileName(file).Replace(Path.GetExtension(file), "");
                        MenuItem BItem = new MenuItem();
                        BItem.Header = BoardName;
                        BItem.Click += BItem_Click;

                        LoadBoardMenu.Items.Add(BItem);
                    }
                }
                else // Creates folder if it doesn't exist
                {
                    Directory.CreateDirectory(LocalBoardFolder);
                }
            }
            else // Account Boards
            {
                foreach (BoardModel model in _User.Boards)
                {
                    MenuItem BItem = new MenuItem();
                    BItem.Header = model.Name;
                    BItem.Click += BItem_Click;

                    LoadBoardMenu.Items.Add(BItem);
                }
            }
        }

        private void BItem_Click(object sender, RoutedEventArgs e) // Is activated upon clicking on a board in the load board menu. Which just loads the board onto the workspace
        {
            if(_User == null) // Guest
            {
                string BoardPath = $"{LocalBoardFolder}/{(sender as MenuItem).Header}.txt";
                string xamlString = File.ReadAllText(BoardPath);

                using (var stringReader = new StringReader(xamlString))
                {
                    using (var xmlReader = System.Xml.XmlReader.Create(stringReader))
                    {
                        Canvas obj = (Canvas)XamlReader.Load(xmlReader);
                        BoardContainer.Content = obj;
                    }
                }
            }
            else // Account
            {
                string Board = $"{(sender as MenuItem).Header}";
                var user = _BVM.UserService.GetAll(X => X.ID == _User.ID);
                string xamlString = (_User = user.First()).Boards.Find(X => X.Name == Board).Content;

                using (var stringReader = new StringReader(xamlString))
                {
                    using (var xmlReader = System.Xml.XmlReader.Create(stringReader))
                    {
                        Canvas obj = (Canvas)XamlReader.Load(xmlReader);
                        BoardContainer.Content = obj;
                    }
                }

                // Creating the share link...
                ShareCodeView.Text = $"{Convert.ToBase64String(Encoding.UTF8.GetBytes(_User.Username))}//Board:{Board}";
            }
        }

        private void ProjectView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Makes sure the canvas is scaled to the size of the screen, (may be off, not tested).
            Canvas can = BoardContainer.Content as Canvas;
            can.Width = this.Width;
            can.Height = this.Height - 100;

            if (AddNoteMode) // Adding a note onto the board
            {
                // Mouse Position
                Point mc = e.GetPosition((Canvas)BoardContainer.Content);

                // Making sure the note is completely inside the board.
                if(((Canvas)BoardContainer.Content).Width-200 < mc.X)
                {
                    mc.X = ((Canvas)BoardContainer.Content).Width - 200;
                }
                if (((Canvas)BoardContainer.Content).Height - 200 < mc.Y)
                {
                    mc.Y = ((Canvas)BoardContainer.Content).Height - 200;
                }

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

                // Adding it to the board
                ((Canvas)BoardContainer.Content).Children.Add(NotePaper);
                Canvas.SetTop(NotePaper, mc.Y);
                Canvas.SetLeft(NotePaper, mc.X);

                AddNoteMode = false;
            }
        }

        private void NewBoard_Click(object sender, RoutedEventArgs e) // Just an empty board, the name is not neccessary and nothing depends on it...
        {
            BoardContainer.Content = new Canvas() { Name="ProjectView" };
            ShareCodeView.Text = "N/A";
        }

        private async void SignIn_Click(object sender, RoutedEventArgs e)
        {
            // Tries to find the user with the username
            var user = _BVM.UserService.GetAll(X => X.Username == ExUsernameInput.Text).FirstOrDefault();

            if (user != null && BCrypt.Net.BCrypt.Verify(ExPasswordInput.Text, user.Password)) // If it found the user with the username and that the password matches.
            {
                _User = user; 
                MessageBox.Show("✅ Account Connected!");
                
                // Update the boards
                FillLoadMenu();

                // Same as NewBoard_Click, should probably make it a seperate function...
                BoardContainer.Content = new Canvas() { Name = "ProjectView" };
                ShareCodeView.Text = "N/A";
            }
            else // User Authentication failed
            {
                MessageBox.Show("❌ Invalid Username or Password!", "Incorrect Information", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Clears input fields
            ExUsernameInput.Text = "";
            ExPasswordInput.Text = "";
        }

        private async void SignUp_Click(object sender, RoutedEventArgs e)
        {
            // Searches for an existing user with the inputed username.
            var existingUser = _BVM.UserService.GetAll(X => X.Username == UsernameInput.Text).FirstOrDefault();

            if (existingUser == null) // If there is no user with that usrname yet, Account creation proceeds.
            {
                // Password Encryption
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(PasswordInput.Text);
                // Adds the new user
                _BVM.UserService.Update(new UserModel() { Username = UsernameInput.Text, Password = hashedPassword });

                MessageBox.Show("✅ Account Created!");
            }
            else
            {
                MessageBox.Show($"❌ The user '{UsernameInput.Text}' already exists!", "Creation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //Clears input fields
            PasswordInput.Text = "";
            UsernameInput.Text = "";
        }

        private void ShareCodeButton_Click(object sender, RoutedEventArgs e) // When the user enters an invitation link.
        {
            if (_User != null) // If not guest
            {
                // Gets the board name from the link
                string Boardname = ShareCodeInput.Text.Substring(ShareCodeInput.Text.IndexOf("//Board:") + 8);

                // Gets the original users name.
                string Username = ShareCodeInput.Text.Substring(0, ShareCodeInput.Text.IndexOf("//Board"));
                Username = Encoding.UTF8.GetString(Convert.FromBase64String(Username));

                // Gets the board owner
                UserModel user = _BVM.UserService.GetAll(X=>X.Username==Username).First();

                try
                {
                    if (user != null) // If the board owner from the link does not exist
                    {
                        //Gets the specific board from the user
                        var board = user.Boards.Where(X => X.Name == Boardname).FirstOrDefault();
                        
                        if (board != null) // If the user actually has a board with that name.
                        {
                            //board.Users.Add(_User);

                            // Gives the current user access to the board.
                            _User.Boards.Add(board);
                            _BVM.UserService.Update(_User);
                        }
                        else
                        {
                            throw new Exception("Share Code was invalid!");
                        }
                    }
                    else
                    {
                        throw new Exception("Share Code was invalid!");
                    }
                    // Update board list
                    FillLoadMenu();
                    MessageBox.Show("You gained access to a Canvas");
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Account needed!");
            }
            // Clears input field
            ShareCodeInput.Text = "";
        }
    }
}