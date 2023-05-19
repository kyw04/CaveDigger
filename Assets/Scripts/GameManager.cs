using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Player player;
    public float playerTimeScale;
    public Inventory inventory;

    private void Awake()
    {
        if (instance == null) { instance = this; }
    }
}
