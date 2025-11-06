using UnityEngine;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    protected override bool isDestroy => false;

    private Player player;

    public Player Player
    {
        get { return player; }
        set { player = value; }
    }
}
