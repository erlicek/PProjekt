namespace PProjekt;

using Microsoft.Maui.Controls.Shapes;
using PProjekt.Views;
using System.IO;
using System.Reflection;
using System.Text;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void LoginTop(object sender, EventArgs e)
    {
        BottomBtn.Text = "LogIn";
        LoginPage.IsEnabled = false;
        RegisterPage.IsEnabled = true;
        LoginPage.FontAttributes = FontAttributes.Bold;
        RegisterPage.FontAttributes = FontAttributes.None;
        LoginPage.BackgroundColor = Color.Parse("#00cbfe");
        RegisterPage.BackgroundColor = Color.Parse("#b0efff");
        clearInputs();
        setInputsToDefalt();
    }

    private void RegisterTop(object sender, EventArgs e)
    {
        BottomBtn.Text = "Register";
        RegisterPage.IsEnabled = false;
        LoginPage.IsEnabled = true;
        LoginPage.FontAttributes = FontAttributes.None;
        RegisterPage.FontAttributes = FontAttributes.Bold;
        LoginPage.BackgroundColor = Color.Parse("#b0efff");
        RegisterPage.BackgroundColor = Color.Parse("#00cbfe");
        clearInputs();
        setInputsToDefalt();
    }

    private void Submit(object sender, EventArgs e)
    {

        string filePath = System.IO.Path.Combine(FileSystem.AppDataDirectory, "UsersData.txt");
        //Path.Combine veme stringy a naformátuje jako cestu, FileSystem.AppDataDirectory najde cestu AppData pro danou aplikaci
        try
        {
            if (BottomBtn.Text == "Register")
            {
                if (username.Text == string.Empty || password.Text == string.Empty)
                {
                    throw new FormatException("Not all fields were filled in!");
                }
                else
                {
                    register(filePath);
                }

            }
        }
        catch (FormatException FE)
        {
            DisplayAlert("Error", FE.Message, "OK");
        }

        if (BottomBtn.Text == "LogIn")
        {
            login(filePath);
        }

    }


    public void register(string filePath)
    {
        int canRegister = 1;
        if (password.Text.Length < 8)
        {
            password.Text = string.Empty;
            password.Placeholder = "Less than 8 characters";
            password.PlaceholderColor = Color.Parse("Red");
        }
        else if (username.Text == "AdminDel" && password.Text == "admin123")
        {
            DisplayAlert("Error", "You cant use this username and password", "OK");
            clearInputs();
            setInputsToDefalt();
        }
        else
        {
            if(!File.Exists(filePath))
            {
                registerLogic(filePath);
            }
            else
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string allLines = sr.ReadToEnd();
                    string[] lines = allLines.Split('\n');
                    foreach (string line in lines)
                    {
                        string[] data = line.Split(new string[] { "##" }, StringSplitOptions.None);
                        if (data[0] == username.Text.ToString())
                        {
                            clearInputs();
                            username.Placeholder = "This username is alredy used";
                            username.PlaceholderColor = Color.Parse("Red");
                            password.Placeholder = " Password (min 8 character)";
                            password.PlaceholderColor = Color.Parse("#8597ab");
                            canRegister = 0;
                            break;
                        }
                        else
                        {
                            canRegister = 1;
                        }
                    }
                }
                if(canRegister == 1)
                registerLogic(filePath);
            }
        }
    }


    public void login(string filePath)
    {
        bool IsHere = false;
        bool LogDone = false;
        try
        {
            if (File.Exists(filePath))
            {

                    
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string allLines = sr.ReadToEnd();
                    string[] lines = allLines.Split('\n');
                    foreach (string line in lines)
                    {
                        string[] data = line.Split(new string[] { "##" }, StringSplitOptions.None); //StringSplitOptions.None zahrnuje všechny prvku včetně "" do pole
                        if(username.Text == null && password.Text != string.Empty)
                        {
                            throw new FormatException("Some fields were not filled in!");
                        }
                        if (data[0] == username.Text.Trim() && data[1].Trim() == password.Text) //Trim odebere ze stringu neviditelné znaky ze začátku a konce
                        {
                            //DisplayAlert("nvm", "prihlaseno", "ok");
                            UserFile userFile = new UserFile(data[2], data[0]);
                            Navigation.PushAsync(userFile);
                            clearInputs();
                            setInputsToDefalt();
                            LogDone = true;
                            break;

                        }
                        else if (data[0] == username.Text && data[1].Trim() != password.Text)
                        {
                            password.Placeholder = "Wrong password";
                            password.PlaceholderColor = Color.Parse("Red");
                            password.Text = string.Empty;
                        }
                        else if (username.Text == string.Empty)
                        {
                            throw new FormatException("Some fields were not filled in!");
                        }
                        else if(password.Text == null)
                        {
                            IsHere = false;
                        }
                        else if (data[0] != username.Text && password.Text != null)
                        {
                            IsHere = false;
                        }
                        else
                        {
                            IsHere = true;
                        }
                    }
                    if(!IsHere && !LogDone)
                    {
                        throw new FormatException("Please register first!");
                    }
                }
            }
            else
            {
                throw new FormatException("Please register first!");
            }
        }
        catch (FormatException FE)
        {
            DisplayAlert("Error", FE.Message, "OK");
        }
        if (username.Text == "AdminDel" && password.Text == "admin123")
        {
            string[] files = Directory.GetFiles(FileSystem.AppDataDirectory);
            foreach (string file in files)
            {
                File.Delete(file);
            }
            clearInputs();
            setInputsToDefalt();
        }
    }


    public string generateRandomCharatcers()
    {
        Random random = new Random();
        const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string randomString = new string(Enumerable.Repeat(characters, 12) //Enumerable je použito pro opakování písmen
                                    .Select(s => s[random.Next(s.Length)]).ToArray());
        return randomString;
    }
    public void clearInputs()
    {
        username.Text = string.Empty;
        password.Text = string.Empty;
    }
    public void setInputsToDefalt()
    {
        username.PlaceholderColor = Color.Parse("#8597ab");
        username.Placeholder = "Username";
        password.PlaceholderColor = Color.Parse("#8597ab");
        password.Placeholder = "Password (min 8 character)";
    }
    public void registerLogic(string filePath)
    {
        string rndString = generateRandomCharatcers();

        using (StreamWriter sw = new StreamWriter(filePath, true)) //vytvoříme StreamWriter object a parametr true = pokud není soubor nalezen, tak ho vytvoří
        {
            sw.WriteLine(username.Text + "##" + password.Text + "##" + rndString);
        }

        string userFolderPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, rndString + ".txt");

        using (StreamWriter sw = new StreamWriter(userFolderPath, true)) { }

        clearInputs();
        setInputsToDefalt();
    }
}

