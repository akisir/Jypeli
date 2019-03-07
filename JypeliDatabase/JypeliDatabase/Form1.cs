//Form1.cs 7.3.2019
//by Aki Sirviö
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;

//Form includes users sign up and login features.
//Form1 class has all database commands.
namespace JypeliDatabase
{
    public partial class Form1 : Form
    {
        private JypeliDatabase game;
        private string player = "";

        // not in use
        public Form1()
        {
            InitializeComponent();
        }

        // constructor needed to store game class
        public Form1(JypeliDatabase game)
        {
            InitializeComponent();
            this.game = game;
        }

        // returns player's image in bitmap format. Called from JypeliDatabase class (public).
        public Bitmap CharacterImage
        {
            get
            {
                return GetImage();
            }
        }

        // opens database and return database connection with the return command
        private SqlConnection OpenDatabase()
        {
            SqlConnection cnn = new SqlConnection();
            cnn.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;
                AttachDbFilename=F:\USERS\HAM\SOURCE\REPOS\JYPELIDATABASE\JYPELIDATABASE\USERSDATABASE.MDF;
                Integrated Security=True;Connect Timeout=30";

            try
            {
                cnn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection not working ! \n" + ex.ToString());
                cnn.Close();
            }

            return cnn;
        }

        // retrieves image from database and converts it into bitmap format.
        private Bitmap GetImage()
        {
            using (SqlConnection cnn = OpenDatabase())
            {
                try
                {
                    string sql = "SELECT Image FROM Users WHERE Name = '" + player + "'";
                    SqlCommand myCommand = new SqlCommand(sql, cnn);
                    byte[] result = (byte[])myCommand.ExecuteScalar();
                    Bitmap myImage = null;
                    if (result[0] != 0x0)
                    {
                        myImage = (Bitmap)new ImageConverter().ConvertFrom(result);
                    }

                    return myImage;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
                return null;
            }
        }

        // save to database
        private void button1_Click(object sender, EventArgs e)
        {
            SaveDatabase();
        }

        // verifies that name is not in use and stores user name, password and image. Starts game
        private void SaveDatabase()
        {
            string name = textBox1.Text;
            string password = textBox2.Text;
            Image image = pictureBox1.Image;
            byte[] bImage = new byte[] { 0x0 };

            // virifies name
            bool uusi = CheckName(name);

            if (uusi && name != "" && password != "")
            {
                // converts image into bitmap
                if (image != null)
                {
                    bImage = (byte[])new ImageConverter().ConvertTo(image, typeof(byte[]));
                }

                // save to database
                using (SqlConnection cnn = OpenDatabase())
                {
                    try
                    {
                        string sql = "INSERT INTO [Users] (Name,Password,Image) values(@Name,@Password,@bImage)";
                        SqlCommand myCommand = new SqlCommand(sql, cnn);
                        myCommand.Parameters.Add("@Name", name);
                        myCommand.Parameters.Add("@Password", password);
                        myCommand.Parameters.Add("@bImage", bImage);
                        myCommand.ExecuteNonQuery();
                        MessageBox.Show("New user added!");
                        player = name;
                        Close();
                        game.Play();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("Name is already in use or the text field is empty.");
            }
        }

        // retrieves string from database and returns true if it is not in use
        private bool CheckName(string name)
        {
            using (SqlConnection cnn = OpenDatabase())
            {
                try
                {
                    string sql = "SELECT * FROM Users WHERE Name = '" + name + "'";
                    SqlDataReader myReader = null;
                    SqlCommand myCommand = new SqlCommand(sql, cnn);
                    myReader = myCommand.ExecuteReader();
                    if (!myReader.HasRows)
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }

            return false;
        }

        // if login is successful start game
        private void button2_Click(object sender, EventArgs e)
        {
            if (CheckLogin())
            {
                game.Play();
            }
        }

        // verifies that user password and name are found in database and return true value.
        bool CheckLogin()
        {
            string name = textBox1.Text;
            string password = textBox2.Text;

            using (SqlConnection cnn = OpenDatabase())
            {
                try
                {
                    string sql = "SELECT * FROM Users WHERE Name = @Name AND Password = @Password;";
                    SqlCommand myCommand = new SqlCommand(sql, cnn);
                    myCommand.Parameters.Add("@Name", name);
                    myCommand.Parameters.Add("@Password", password);

                    if (myCommand.ExecuteScalar() != null)
                    {
                        player = name;
                        Close();
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Name or password is incorrect!");
                    }

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }

                return false;
            }
        }

        // open file search and upload image to picturebox
        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open Image";
            dlg.Filter = "Image Files (*.bmp;*.jpg;*.jpeg,*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(dlg.FileName);
            }
            dlg.Dispose();
        }

        // clears Users table
        void TyhjennaTaulu()
        {
            using (SqlConnection cnn = OpenDatabase())
            {
                try
                {
                    string sqlTrunc = "TRUNCATE TABLE Users";
                    SqlCommand cmd = new SqlCommand(sqlTrunc, cnn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
        }
    }
}
