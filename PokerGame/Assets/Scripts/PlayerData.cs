using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public string Id; // { get; set; } // Set From FireBase Maybe. 
    public string Name; // { get; set; }

    //PLAYER STATS
    public int CurrentBalance;// { get; set; }

    public int TournamentsWon;// { get; set; }
    public int TournamentsAttended;// { get; set; }

    public int AllHandsWon;// { get; set; }
    public int AllHandsAttended;// { get; set; }

    public int AllInShowdownsWon;// { get; set; }
    public int AllInShowdownsAttended;// { get; set; }

    public int ShowDownsWon;// { get; set; }
    public int ShowDownsAttended;// { get; set; }

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
