﻿using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class MapGenerator
{
    private const int SectorWidth = 5;

    private const int StonesNumberBySector = 6;
    private const int ObstaclesNumberBySector = 7;
    private const int MinesNumberBySector = 12;

    public static CaseData[][] GenerateMap(int sizeX, int sizeY, int seed)
    {
        Random.seed = seed;

        CaseData[][] map = EmptyMap(sizeX, sizeY);

        // Begin

        var sectorGenerator = new SectorGenerator {
            Height = sizeY - 2,
            Width = 2,
            NumberByType =
                {
                    {CaseData.Obstacle, 4}
                }
        };

        sectorGenerator.GenerateSector(map, 2, 1);

        // Center

        sectorGenerator = new SectorGenerator {
            Height = sizeY - 2,
            Width = SectorWidth,
            NumberByType =
                {
                    {CaseData.Obstacle, ObstaclesNumberBySector},
                    {CaseData.Stone, StonesNumberBySector}
                }
        };

        for (int i = 0; i < sizeX / SectorWidth - 1; i++)
        {
            int redMinesNumber = Random.Range(MinesNumberBySector / 2 - MinesNumberBySector / 3,
                MinesNumberBySector / 2 + MinesNumberBySector / 3);

            sectorGenerator.NumberByType[CaseData.RedMines] = redMinesNumber;
            sectorGenerator.NumberByType[CaseData.GreenMines] = MinesNumberBySector - redMinesNumber;
            sectorGenerator.GenerateSector(map, 4 + i * SectorWidth, 1);
        }

        // End
        
        sectorGenerator = new SectorGenerator {
            Height = sizeY - 2,
            Width = 1,
            NumberByType =
                {
                    {CaseData.Obstacle, 2}
                }
        };

        sectorGenerator.GenerateSector(map, 4 + (sizeX / SectorWidth - 1) * SectorWidth, 1);
        
        return map;
    }

    public static CaseData[][] EmptyMap(int sizeX, int sizeY)
    {
        var map = new CaseData[sizeY][];
        for (int i = 0; i < sizeY; i++)
            map[i] = new CaseData[sizeX];

        // Borders
        for (int i = 0; i < sizeY; i++)
            map[i][0] = CaseData.BorderLeft;
        for (int i = 0; i < sizeY; i++)
            map[i][sizeX - 1] = CaseData.BorderRight;
        for (int i = 0; i < sizeX; i++)
            map[0][i] = CaseData.BorderBottom;
        for (int i = 0; i < sizeX; i++)
            map[sizeY - 1][i] = CaseData.BorderTop;

        // Start
        map[sizeY / 2][0] = CaseData.Start;

        // Well
        map[sizeY / 2][sizeX - 2] = CaseData.Well;

        return map;
    }

    private class SectorGenerator
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Dictionary<CaseData, int> NumberByType { get; set; }

        public SectorGenerator()
        {
            NumberByType = new Dictionary<CaseData, int>();
        }

        public void GenerateSector(CaseData[][] map, int originSectorX, int originSectorY)
        {
            foreach (KeyValuePair<CaseData, int> pair in NumberByType)
            {
                CaseData type = pair.Key;
                int count = pair.Value;

                if (type == CaseData.EmptyCase)
                    continue;

                for (int i = 0; i < count; i++)
                {
                    int x, y;
                    int visitedCount = 0;

                    do
                    {
                        y = Random.Range(0, Height);
                        x = Random.Range(0, Width);
                        visitedCount++;

                        if (visitedCount >= Width * Height)
                            break;
                    }
                    while (map[originSectorY + y][originSectorX + x] != CaseData.EmptyCase || !ConditionsByCaseType(map, type, originSectorX + x, originSectorY + y));

                    map[originSectorY + y][originSectorX + x] = type;
                }
            }
        }

        private bool ConditionsByCaseType(CaseData[][] map, CaseData type, int x, int y)
        {
            switch (type)
            {
                case CaseData.Obstacle:

                    for (int i = -1; i <= 1 ; i++)
                        for (int j = -1; j <= 1; j++)
                            if (y + i >= 0 && x + j >= 0 && y + i < map.Length && x + j < map[0].Length)
                                if (map[y + i][x + j] == CaseData.Obstacle)
                                    return false;

                    break;
            }

            return true;
        }
    }
}
