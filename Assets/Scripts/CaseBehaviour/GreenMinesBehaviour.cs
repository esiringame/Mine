﻿using UnityEngine;
using System.Collections.Generic;
using DesignPattern;

public class GreenMinesBehaviour : CaseBehaviour<GreenMinesBehaviour>
{
    public AudioClip boom;
    public AudioClip mineArmed;
    private float timer;
    PlayerController m_player = null;

    void Update()
    {
        if (m_player != null)
        {
            timer += Time.unscaledDeltaTime;
        }
        if (timer > 3.0f)
        {
            Fragmentation();
            if (m_player != null)
            {
                m_player.Die();
            }
        }
    }

    public override bool IsObstacle
    {
        get { return false; }
    }

    public override void OnEnter(PlayerController player)
    {
        GetComponent<AudioSource>().PlayOneShot(mineArmed, 1.0F);
        timer = 0.0f;
        m_player = player;
    }

    public override void OnLeave(PlayerController player)
    {
        GetComponent<AudioSource>().PlayOneShot(boom, 1.0F);
        Fragmentation();
        m_player = null;

        if (!player.IsJumping)
            player.Die();
    }

    public override void PutStone(PlayerController player)
    {
        GetComponent<AudioSource>().PlayOneShot(boom, 1.0F);
        Fragmentation();
    }

    public void Fragmentation()
    {
        List<ICaseBehaviour> adjacents = CasesAdjacentes();
   
        for (int i = 0; i < 3 && adjacents.Count > 0; i++)
        {
            // Choisit aléatoirement une des cases adjacentes
            
            ICaseBehaviour randomCase = adjacents[Random.Range(0,adjacents.Count)];
            int x= randomCase.PositionX;
            int y= randomCase.PositionY;
            // On la supprime de la liste pour ne pas retomber dessus
            adjacents.Remove(randomCase);

            // Fragmentation d'une mine verte ou range
            if (Random.value > 0.5f)
            {
                Grid.Instance.grid[y][x] = Grid.Instance.grid[y][x].ChangeBehaviour<RedMinesBehaviour>();
            }
            else
            {
                Grid.Instance.grid[y][x] = Grid.Instance.grid[y][x].ChangeBehaviour<GreenMinesBehaviour>();
            }
        }
        // Transforme cette case en case vide
        Grid.Instance.grid[PositionY][PositionX] = Grid.Instance.grid[PositionY][PositionX].ChangeBehaviour<EmptyCaseBehaviour>();
    }

    //Genère les cases adjacentes à la mine verte
    private List<ICaseBehaviour> CasesAdjacentes()
    {
        List<ICaseBehaviour> adjacents = new List<ICaseBehaviour>();
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                // Condition aux bords
                if (PositionY + i < Grid.Instance.Height && PositionX + j < Grid.Instance.Width && PositionY + i >= 0 && PositionX + j >= 0)
                {
                    ICaseBehaviour CurrentCase = Grid.Instance.grid[PositionY+i][PositionX+j];
                    // On ne prend pas en compte les mines et obstacles
                    if (CurrentCase is EmptyCaseBehaviour && !CurrentCase.HasStone)
                    {
                        adjacents.Add(CurrentCase);
                    }
                }
            }
        }
        return adjacents;
    }
}
