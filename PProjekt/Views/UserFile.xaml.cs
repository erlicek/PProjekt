namespace PProjekt.Views;
using System.IO;

public partial class UserFile : ContentPage
{
    public string UserID;
    public string PathToFile;
    public UserFile(string ID, string Name)
    {
        InitializeComponent();
        UserID = ID;
        PathToFile = System.IO.Path.Combine(FileSystem.AppDataDirectory, UserID.ToString().Trim() + ".txt");
        GetDataFromUserFile(PathToFile);
        

        NameOfUser.Text = "Notepad for: " + Name;
    }

    private async void LogOut_Clicked(object sender, EventArgs e)
    {

        SaveUserData(PathToFile);
        await Navigation.PopAsync();
    }



    public void GetDataFromUserFile(string PathToFile)
    {
        try
        {
            if (File.Exists(PathToFile))
            {
                //string filePath = System.IO.Path.Combine(FileSystem.AppDataDirectory, ID.ToString().Trim() + ".txt");
                using (StreamReader sr = new StreamReader(PathToFile))
                {
                    string allLines = sr.ReadToEnd();
                    UserText.Text = allLines;
                }
            }
            else
            {
                throw new FormatException("File does not exists!");
            }
        }
        catch (FormatException FE)
        {
            DisplayAlert("Error", FE.Message, "OK");
        }
    }

    public void SaveUserData(string PathToFile)
    {
        try
        {
            if (File.Exists(PathToFile))
            {
                using (StreamWriter sw = new StreamWriter(PathToFile))
                {
                    sw.Write(UserText.Text);
                }
            }
            else
            {
                throw new FormatException("File does not exists!");
            }
        }
        catch (FormatException FE)
        {
            DisplayAlert("Error", FE.Message, "OK");
        }
    }
}