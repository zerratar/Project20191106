using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameUI gameUI;

    private long money;
    private long shoes;

    public PlayerTeam Team;
    public bool IsPlayerControlled;

    public long Money
    {
        get => money; set
        {
            money = value;
            gameUI.SetMoney(value);
        }
    }
    public long Shoes
    {
        get => shoes; set
        {
            shoes = value;
            gameUI.SetShoes(value);
        }
    }

}

public enum PlayerTeam
{
    Orbit,
    Prism,
    Neutral
}