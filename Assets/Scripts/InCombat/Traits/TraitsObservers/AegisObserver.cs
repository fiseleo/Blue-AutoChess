﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AegisObserver : CharacterObserverBase
{
    private CharacterCTRL character;
    public AegisObserver(int level, CharacterCTRL character)
    {
        this.character = character;
    }
    public override Dictionary<int, TraitLevelStats> GetTraitObserverLevel()
    {
        Dictionary<int, TraitLevelStats> statsByStarLevel = new Dictionary<int, TraitLevelStats>()
        {//秒數,阻擋次數,分攤到原主人的傷害
            {0, new TraitLevelStats(0,0,100)},
            {1, new TraitLevelStats(3,1,30)},
            {2, new TraitLevelStats(5,1,40)},
            {3, new TraitLevelStats(10,3,50)},
            {4, new TraitLevelStats(999,5,55)}
        };
        return statsByStarLevel;
    }
    public override void OnCastedSkill(CharacterCTRL character)
    {
        base.OnCastedSkill(character);
        CustomLogger.Log(this,$"character at {character.CurrentHex.Position} casted skill");
        SetCenterPoint(character);
    }

    public void SetCenterPoint(CharacterCTRL character)
    {
        HexNode centerNode = character.CurrentHex;
        bool isAlly = character.IsAlly;

        if (isAlly)
        {
            centerNode.AllyBlockingZonecenter = true;
            centerNode.TargetedAllyZone = true;
        }
        else
        {
            centerNode.EnemyBlockingZonecenter = true;
            centerNode.TargetedEnemyzone = true;
        }
        CustomLogger.Log(this, $"setting {character.name} as center , pos int = {centerNode.Position}");
        foreach (var neighbor in centerNode.Neighbors)
        {
            if (isAlly)
            {
                neighbor.TargetedAllyZone = true;
            }
            else
            {
                neighbor.TargetedEnemyzone = true;
            }
        }
        CoroutineController coroutineController = character.GetComponent<CoroutineController>();
        coroutineController.StartRemoveCenterPointCoroutine(centerNode,5f);
        SpawnGrid.Instance.UpdateBlockingZoneWalls();
    }
    
}
