public class PlayerData
{
    public int Id { get; set; }
    public string Name { get; set; }

    //PLAYER STATS
    public int CurrentBalance { get; set; }
    public int TournamentsWon { get; set; }
    public int TournamentsAttended { get; set; }
    public float PercentageOfHandsWon { get; set; }
    public float PercentageOfAllInsWon { get; set; }
    public float ShowDownsWon { get; set; }
    public float ShowDownsAttended { get; set; }
}
