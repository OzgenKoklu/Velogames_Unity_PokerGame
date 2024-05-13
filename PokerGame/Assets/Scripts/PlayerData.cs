using System;

[Serializable]
public class PlayerData
{
    public string Id;
    public string Name; 

    //PLAYER STATS
    public int CurrentBalance;

    public int TournamentsWon;
    public int TournamentsAttended;

    public int AllHandsWon;
    public int AllHandsAttended;

    public int AllInShowdownsWon;
    public int AllInShowdownsAttended;

    public int ShowDownsWon;
    public int ShowDownsAttended;

    public PlayerData()
    {
        // should generate random something but unity engine random does not work for constructor
        Id = "";

        // Generate random name
        Name = "Player" + Id.ToString(); // Combine "Player" with ID

        // Initialize all integer stats to 0
        CurrentBalance = 0;
        TournamentsWon = 0;
        TournamentsAttended = 0;
        AllHandsWon = 0;
        AllHandsAttended = 0;
        AllInShowdownsWon = 0;
        AllInShowdownsAttended = 0;
        ShowDownsWon = 0;
        ShowDownsAttended = 0;
    }
}
