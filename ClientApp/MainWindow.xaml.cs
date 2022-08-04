using ClientApp.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;



namespace ClientApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient client = new HttpClient();
        string imagePath;
        public MainWindow()
        {
            InitializeComponent();
            client.BaseAddress = new Uri("https://localhost:7101/api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            GetBooks();
        }

        private async Task GetBooks()
        {
            try 
            { 
                var response = await client.GetStringAsync("books");
   
                var Books = JsonSerializer.Deserialize<List<Book>>(response);
                dgBooks.DataContext = Books;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error, something went wrong");
                Console.WriteLine(ex.Message);
            }

        }
        private async void CreateBook(Book book)
        {
            try
            {
                await client.PostAsJsonAsync("books", book);
                await GetBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error, something went wrong");
                Console.WriteLine(ex.Message);
            }
        }

        private async void UpdateBook(Book book)
        {
            try
            {
                await client.PutAsJsonAsync("books/", book);
                await GetBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error, something went wrong");
                Console.WriteLine(ex.Message);
            }
        }

        private async void DeleteBooks()
        {
            try
            {
                var books = ((FrameworkElement)dgBooks).DataContext as List<Book>;
                foreach (Book book in books)
                {
                    if (book.toDelete == true)
                    {
                        await client.DeleteAsync("books/" + book.Id);
                    }
                }
                await GetBooks();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error, something went wrong");
                Console.WriteLine(ex.Message);
            }
            
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var books = ((FrameworkElement)dgBooks).DataContext as List<Book>;
            
            if (books.Where(x => x.toDelete == true).Count() == 0)
                MessageBox.Show("Select items to delete");
            else 
                DeleteBooks();
        }   

        private void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            if (openFileDialog.ShowDialog() == true)
            {
                imagePath = openFileDialog.FileName;
                Uri fileUri = new Uri(imagePath);
                imgDynamic.Source = new BitmapImage(fileUri);
            }
        } 

        private void BtnEditBook(object sender, RoutedEventArgs e)
        {
            Book book = ((FrameworkElement)sender).DataContext as Book;
            IdBox.Text = book.Id.ToString();
            TitleBox.Text = book.Title;
            imgDynamic.Source = Base64ToImg(book.Image);

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {

            if (IdBox.Text == "")
            {
                if(imagePath != null)
                {
                    if (TitleBox.Text != "")
                    {
                        Book book = new Book()
                        {
                            Id = Math.Abs(Guid.NewGuid().GetHashCode()),
                            Title = TitleBox.Text,
                            Image = ImgToBase64(imagePath)
                        };
                        CreateBook(book);
                        CleaningBox();
                    }
                    else
                        MessageBox.Show("Fill in all fields");
                }
                else
                MessageBox.Show("Fill in all fields");
            }
            else
            {
                if(imagePath != null)
                {
                    Book book = new Book()
                    {
                        Id = Convert.ToInt32(IdBox.Text),
                        Title = TitleBox.Text,
                        Image = ImgToBase64(imagePath)
                    };
                    UpdateBook(book);
                    CleaningBox();
                }
                else
                {
                    var eldBook = (((FrameworkElement)dgBooks).DataContext as List<Book>).FirstOrDefault(x => x.Id == Convert.ToInt32(IdBox.Text));
                    Book book = new Book()
                    {
                        Id = Convert.ToInt32(IdBox.Text),
                        Title = TitleBox.Text,
                        Image = eldBook.Image
                    };
                    UpdateBook(book);
                    CleaningBox();
                }
            }         
        }

        private string ImgToBase64(string image)
        {

            byte[] imageBytes = File.ReadAllBytes(image);
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }

        private BitmapImage Base64ToImg(string base64)
        {
            byte[] binaryData = Convert.FromBase64String(base64);
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(binaryData);
            bi.EndInit();
            return bi;
        }

        private void CleaningBox()
        {
            IdBox.Text = null;
            TitleBox.Text = null;
            imgDynamic.Source = null;
        }
    }
}
