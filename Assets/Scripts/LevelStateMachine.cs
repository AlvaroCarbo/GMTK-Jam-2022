using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelStateMachine : MonoBehaviour
{
    public int actualTurn;
    public GameObject player, enemy;
    public TextMeshProUGUI turnText, turnTextBG, dmgPlayerText, dmgEnemyText;
    public GameObject diceHolderPlayer, diceHolderEnemy;
    public DiceRoller[] dicesOnUse;
    public Sprite spriteDefaultDice;
    public SpriteRenderer[] spriteRendererDices;
    public bool isPlayerTurn = true;
    public int totalAttack;
    public GameState State = GameState.PlayerMoveTurn;

    public enum GameState {
        PlayerMoveTurn,
        ChangePlayerMoveToAttackTurn,
        PlayerAttackTurn,
        ChangePlayerTurnToEnemyTurn,
        EnemyMoveTurn,
        ChangeEnemyMoveToAttackTurn,
        EnemyAttackTurn,
        ChangeEnemyTurnToPlayerTurn,
        LevelFinished
    }
    private void Awake()
    {
        actualTurn = 1;
        isPlayerTurn = true;
    }
    public void AddAttackToStack(int value) 
    {
        totalAttack += value;
        if (isPlayerTurn)
        {
            dmgPlayerText.text = "Your Attack: " + totalAttack; 
        }
        else 
        {
            dmgEnemyText.text = "Enemy Attack: " + totalAttack;
        }
    }

    public void ResetAttackValue()
    {
        totalAttack = 0;
    }

    public void EnableAttackGUI()
    {
        if (isPlayerTurn)
        {
            diceHolderPlayer.SetActive(true);
           
        }
        else 
        {
            diceHolderEnemy.SetActive(true);
        }
    }

   

    public void DisableAttackGUI() 
    {
        if (isPlayerTurn)
        {
            diceHolderPlayer.SetActive(false);
            spriteRendererDices = diceHolderPlayer.FindComponentsInChildrenWithTag<SpriteRenderer>("Dice");
            dmgPlayerText.text = "Your Attack: ";
            totalAttack = 0;
            for (int i = 0; i < spriteRendererDices.Length; i++)
            {
                dicesOnUse[i].ableToRoll = true;
                spriteRendererDices[i].sprite = spriteDefaultDice;
            }
        }
        else
        {
            diceHolderEnemy.SetActive(false);
            spriteRendererDices = diceHolderEnemy.FindComponentsInChildrenWithTag<SpriteRenderer>("Dice");
            dmgEnemyText.text = "Enemy Attack: ";
            totalAttack = 0;
            for (int i = 0; i < spriteRendererDices.Length; i++)
            {
                dicesOnUse[i].ableToRoll = true;
                spriteRendererDices[i].sprite = spriteDefaultDice;
            }
        }
    }
    public void FindDices()
    {
        if (isPlayerTurn) { dicesOnUse = diceHolderPlayer.FindComponentsInChildrenWithTag<DiceRoller>("Dice"); }
        else { dicesOnUse = diceHolderEnemy.FindComponentsInChildrenWithTag<DiceRoller>("Dice"); }
    }
    public void EnemyDiceRoll() 
    {
        for (int i = 0; i < dicesOnUse.Length; i++)
        {
            if (dicesOnUse[i].value != 0 && dicesOnUse[i].hasRolled)
            {
                AddAttackToStack(dicesOnUse[i].value);
                dicesOnUse[i].hasRolled = false;
            }
        }
    }
    public void FinishTurn() 
    {
        DisableAttackGUI();
        actualTurn++;
        turnText.text = "TURN " + actualTurn;
        turnTextBG.text = "TURN " + actualTurn;
        if (isPlayerTurn) { State = GameState.EnemyMoveTurn; }
        else { State = GameState.PlayerMoveTurn; }
        isPlayerTurn = !isPlayerTurn;
        
    }
    public void PlayerDiceRoll()
    {
        for (int i = 0; i < dicesOnUse.Length; i++) {
            if (dicesOnUse[i].value != 0 && dicesOnUse[i].hasRolled) {
                AddAttackToStack(dicesOnUse[i].value);
                dicesOnUse[i].hasRolled = false;
            }
        }
    }

    public void PlayerTurnMove() 
    { 
    
    }

    public void EnemyTurnMove() 
    {
    
    }
    public void FinishLevel() 
    { 
    
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (State) 
        {
            case GameState.PlayerMoveTurn:
                PlayerTurnMove();
                break;
            case GameState.ChangePlayerMoveToAttackTurn:
                EnableAttackGUI();
                FindDices();
                State = GameState.PlayerAttackTurn;
                break;
            case GameState.PlayerAttackTurn:
                PlayerDiceRoll();
                break;
            case GameState.ChangePlayerTurnToEnemyTurn:
                FinishTurn();
                break;
            case GameState.EnemyMoveTurn:
                EnemyTurnMove();
                break;
            case GameState.ChangeEnemyMoveToAttackTurn:
                EnableAttackGUI();
                FindDices();
                State = GameState.EnemyAttackTurn;
                break;
            case GameState.EnemyAttackTurn:
                EnemyDiceRoll();
                break;
            case GameState.ChangeEnemyTurnToPlayerTurn:
                FinishTurn();
                break;
            case GameState.LevelFinished:
                //Player dead or finished Level
                FinishLevel();
                break;

        }
    }
}
