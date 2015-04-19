﻿using UnityEngine;
using System.Collections.Generic;
using DesignPattern;

public class GreenMinesBehaviour : CaseBehaviour<GreenMinesBehaviour>
{
    public AudioClip boom;
    public AudioClip mineArmed;

    public override bool IsObstacle
    {
        get { return false; }
    }

    public override void onEnter(GameObject player)
    {
        GetComponent<AudioSource>().PlayOneShot(mineArmed, 1.0F);
        //player.mode = mode.GreenMine;
    }

    public override void onLeave(GameObject player)
    {
        GetComponent<AudioSource>().PlayOneShot(boom, 1.0F);
        fragmentation();
        //player.die
    }

    public override void putStone()
    {
        GetComponent<AudioSource>().PlayOneShot(boom, 1.0F);
        fragmentation();
    }

    //Genère les cases adjacentes à la mine verte
    private List<ICaseBehaviour> casesAdjacentes()
    {
        List<ICaseBehaviour> adjacents = new List<ICaseBehaviour>();
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                // Condition aux bords
                if (PositionX + i < 10 && PositionY + j < 30 && PositionX + i > 0 && PositionY + j > 0)
                {
                    ICaseBehaviour CurrentCase = Grid.Instance.grid[j][i];
                    // On ne prend pas en compte les mines et obstacles
                    if (CurrentCase is EmptyCaseBehaviour)
                    {
                        adjacents.Add(CurrentCase);
                    }
                }
            }
        }
        return adjacents;     
    }

    public void fragmentation()
    {
        List<ICaseBehaviour> adjacents = casesAdjacentes();
   
        for (int i = 0; i < 3 && adjacents.Count >0; i++)
        {
            // Choisit aléatoirement une des cases adjacentes
            
            ICaseBehaviour randomCase = adjacents[Random.Range(0,adjacents.Count-1)];
            int x= randomCase.PositionX;
            int y= randomCase.PositionY;
            // On la supprime de la liste pour ne pas retomber dessus
            adjacents.Remove(randomCase);

            // Fragmentation d'une mine verte ou range
            if (Random.value > 0.5f)
            {
                Grid.Instance.grid[y][x] = Factory<RedMinesBehaviour>.New("Case/RedMines");
            }

            else
            {
                Grid.Instance.grid[y][x] = Factory<GreenMinesBehaviour>.New("Case/GreenMines");
            }
        }
        // Transforme cette case en case vide
        Grid.Instance.grid[PositionY][PositionX] = Factory<EmptyCaseBehaviour>.New("Case/EmptyCase"); 
    }
}
