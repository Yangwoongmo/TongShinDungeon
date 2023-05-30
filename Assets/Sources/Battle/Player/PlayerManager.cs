public class PlayerManager
{
    private static PlayerManager instance;

    private PlayerInfoRepository repository = PlayerInfoRepository.GetInstance();
    private Player player;

    private PlayerManager()
    {
    }

    public static PlayerManager GetInstance()
    {
        if (instance == null)
        {
            instance = new PlayerManager();
        }
        return instance;
    }

    public Player GetPlayer()
    {
        if (player == null)
        {
            player = repository.LoadPlayerData();
        }
        return player;
    }

    public void SavePlayerData()
    {
        repository.SavePlayerData(player);
    }
}
