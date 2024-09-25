using System;

[Serializable]
public class Nft
{
    public string id;
    public string name;
    public string image;
}

[Serializable]
public class Nfts
{
    public Nft[] nfts;
}

[Serializable]
public class GameStart
{
    public string walletAddress;
    public bool canPlay;
}

[Serializable]
public class Deposited
{
    public string walletAddress;
    public int depositTime;
    public int win;
    public string session = "";
    public string tx = null;
}

[Serializable]
public class LeadboardRow
{
    public string walletAddress;
    public int won;
    public int rank;
}

[Serializable]
public class Leaderboard
{
    public LeadboardRow top1;
    public LeadboardRow top2;
    public LeadboardRow top3;
    public LeadboardRow top4;
    public LeadboardRow top5;
    public LeadboardRow player;
}

[Serializable]
public class PrizePoolResponse
{
    public string total;
}

public static class NetworkParams
{
    public static string networkId = "59144";

    public static string AraActUrl = "http://localhost:3000";
    //public static string AraActUrl = "https://ara-act-dc51162b3a11.herokuapp.com";
}

