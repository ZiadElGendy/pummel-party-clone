using EpicToonFX;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PummelPartyClone;

public class DiceManager : Singleton<DiceManager>
{
    public GameObject Dice;
    public GameObject[] Dices;
    [SerializeField] private int _numPlayers = 3; //Number of players playing //Can be assigned a different value from game manager later on
    [SerializeField] private Vector3 _position = Vector3.zero;
    private int _diceSpawned = 0;
    private int _diceFinished = 0;
    private int _winningPlayerFirstTurn = 1;
    private int _maxValueOfDicePerPlayer = 0;
    //private string _isTied = "(true";
    private bool _isTied = false;

    private float _diceSpacing; //The spacing between each dice depending on how many players are playing
    private float _displacement;

    public Vector3 InitialCameraPosition;
    public override void Awake()
    {
        base.Awake();
        InitialCameraPosition = Camera.main.transform.position;
        _diceSpacing = 6f / _numPlayers;
        if (_numPlayers % 2 != 0)
        {
            _displacement = ((_numPlayers - 1) / 2) * _diceSpacing;
        }
        else
        {
            _displacement = ((_numPlayers - 1) / 2) * _diceSpacing + _diceSpacing / 2;
        }

        Dices = new GameObject[_numPlayers];
    }

    public void SpawnDice()
    {
        if (_diceSpawned >= _numPlayers)
        {
            Debug.Log("Dice spawn limit exceeded");
        }
        else
        {
            for (int i = 0; i < _numPlayers; i++)
            {
                Dices[i] = Instantiate(Dice, (new Vector3(_position.x - _displacement + _diceSpacing * _diceSpawned, _position.y, _position.z)), Quaternion.identity);
                _diceSpawned++;
                Dices[i].GetComponent<ETFXRotation>().enabled = false;
            }

            ActivateDice();

            Debug.Log("Dice Spawned");
        }
    }

    #region First Turn Manager
    private void ActivateDice() //A method that is called everytime a dice is finished so that we move on to the next one
    {
        Dices[_diceFinished].GetComponent<ETFXRotation>().enabled = true;
    }

    public void SaveValueOfDice(int value)
    {
        _diceFinished++;

        if (_diceFinished == 1) // If this is the first 
        {
            _maxValueOfDicePerPlayer = value;
        }
        else if (value > _maxValueOfDicePerPlayer)
        {
            _maxValueOfDicePerPlayer = value;
            _isTied = false;
            _winningPlayerFirstTurn = _diceFinished;
        }
        else if (_maxValueOfDicePerPlayer == value)
        {
            _isTied = true;
        }

        if (_diceFinished == _numPlayers)
        {
            ExecuteFirstTurn();
        }
        else
        {
            ActivateDice();
        }
    }

    private void ExecuteFirstTurn()
    {
        if (_isTied && _numPlayers > 1)
        {
            Debug.Log("It's a tie! \n Time to reroll.");
            InitializeDiceManagerProperties();
            SpawnDice();
        }
        else
        {
            Debug.Log("The winner is player number " + _winningPlayerFirstTurn + "!");
            InitializeDiceManagerProperties();
        }
    }
    #endregion

    private void InitializeDiceManagerProperties() //Initializing everything again
    {
        _isTied = false;
        _diceFinished = 0;
        _diceSpawned = 0;
        _maxValueOfDicePerPlayer = 0;
        _winningPlayerFirstTurn = 1;
    }
}
