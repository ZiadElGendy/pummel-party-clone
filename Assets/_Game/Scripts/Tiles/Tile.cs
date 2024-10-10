using PummelPartyClone;
using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    public int Id;
    public TileType type;
    private Renderer _renderer;
    private bool _isHighlighted = false;
    public IntGameEvent OnBranchSelect;

    public List<Tile> LinkedTiles;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        _renderer.material.color = type.Color;
    }

    public bool IsEnterable()
    {
        return type.EntryLocation != "";
    }

    public void Highlight()
    {
        _renderer.material.color = Color.yellow;
        _isHighlighted = true;
    }

    public void Unhighlight()
    {
        _renderer.material.color = type.Color;
        _isHighlighted = false;
    }

    private void OnMouseDown()
    {
        if (_isHighlighted)
        {
            OnBranchSelect.Raise(Id);
        }
    }

    public void OnStep(PlayerController player)
    {
        player.Inventory.AddSubCoins(type.CoinValue);
        if (IsEnterable())
        {
            //String GameEvent that will be listened to by the TurnManager
            //But I guess this will need a switch case on the other side?
            //OnEnterableTileStep.Raise(type.GetEntryLocation());
        }
    }
}