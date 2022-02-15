using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

// Some stuff I stole that helps with gamestates.
public enum GameState { START, PLAYERTURN, ENEMYTURN, WON, LOST }
public enum BattleState { MAP, BATTLE }

public enum Team { PLAYER, ENEMY }

public class GameManager : MonoBehaviour
{
    public GameState gameState;
    public BattleState battleState;

    private static bool gameIsPaused = false;

    private static GameManager instance;

    private int turnNumber;

    public static GameManager Instance { get { return instance; } }

    [System.Serializable]
    public struct CharacterList
    {
        public CharacterData character;
        public Vector2Int startPosition;
        public Team team;

        public CharacterList(CharacterData character, Vector2Int startPosition, Team team) { this.character = character; this.startPosition = startPosition; this.team = team; }
    }

    public List<CharacterList> characterList;

    private List<GameCharacter> characters;

    public PlayerController player;
    public AIController enemy;

    MapManager mapManager;
    BattleManager battleManager;

    [SerializeField]
    private MapUnit mapUnitPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        gameState = GameState.START;

        turnNumber = 0;

        Scene mapScene = SceneManager.GetSceneByName("MapScene");
        if (!mapScene.IsValid()) { 
            SceneManager.LoadScene("MapScene", LoadSceneMode.Additive);
        }
        Scene battleScene = SceneManager.GetSceneByName("SampleScene");
        if (!battleScene.IsValid())
        {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive);
        }

        mapManager = new MapManager();
        battleManager = new BattleManager();

        gameState = GameState.PLAYERTURN;
        battleState = BattleState.MAP;
    }

    public void SetupCharacters(Transform parent) 
    {
        characters = new List<GameCharacter>();
        foreach (CharacterList item in characterList)
        {
            characters.Add(new GameCharacter(item.character, mapUnitPrefab, parent, item.startPosition, item.team));
        }
    }

    public List<GameCharacter> GetCharacterList() { return characters; }

    /*public void GameOver()
    {
        Debug.Log("Game Over");
        uiManager.WinScreen();
    }

    public void WinGame()
    {
        uiManager.WinScreen();
        PauseGame();
    }

    public void LoseGame()
    {
        uiManager.LoseScreen();
        PauseGame();
    }*/

    private void NewTurn()
    {
        turnNumber += 1;
        gameState = GameState.PLAYERTURN;
        player.StartOfTurn();
        Debug.Log("Turn: " + turnNumber);
    }

    private void NextPlayer()
    {
        if (gameState == GameState.PLAYERTURN)
        { 
            gameState = GameState.ENEMYTURN;
            enemy.StartOfTurn();
        }
        else if (gameState == GameState.ENEMYTURN)
        { NewTurn(); }
    }

    public void EndTurn()
    {
        enemy.EndOfTurn();
        NextPlayer();
    }

    public bool IsPaused()
    {
        return gameIsPaused;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public MapManager GetMapManager()
    {
        return mapManager;
    }

    public BattleManager GetBattleManager()
    {
        return battleManager;
    }

    public void StartBattle(BattleData data)
    {
        battleState = BattleState.BATTLE;
        battleManager.NewBattle(data);
    }

    public void EndBattle(BattleData data, BattleResults results)
    {
        battleState = BattleState.MAP;
        mapManager.EndBattle(data, results);
    }

}
