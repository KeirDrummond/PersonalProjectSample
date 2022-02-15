using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BattleScene : MonoBehaviour
{
    public Tilemap collision;
    public Tilemap platforms;
    public BattlePlayerController playerController;
    public Canvas canvas;
    public HealthBar playerHealthBar;
    public GameObject root;
    public GameCamera cam;

    private void Awake()
    {
        GameManager.Instance.GetBattleManager().SetupBattleScene(collision, platforms, canvas, playerHealthBar, cam, root);
    }
}
