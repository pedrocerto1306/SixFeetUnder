using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Flags]
public enum EstadoParede
{
    // 0000 -> sem parede
    // 1111 -> esquerda, direita, cima, baixo
    LEFT = 1, //0001
    RIGHT = 2, //0010
    UP = 4, //0100
    DOWN = 8, //1000

    VISITADO = 128
}

public struct Position
{
    public int X;
    public int Y;
}

public struct Vizinho
{
    public Position posicao;
    public EstadoParede paredeCompartilhada;
}

public static class LevelGenerator
{
    private static EstadoParede GetParedeOposta(EstadoParede parede)
    {
        switch (parede)
        {
            case EstadoParede.LEFT:
                return EstadoParede.RIGHT;
            case EstadoParede.RIGHT:
                return EstadoParede.LEFT;
            case EstadoParede.UP:
                return EstadoParede.DOWN;
            case EstadoParede.DOWN:
                return EstadoParede.UP;
            default:
                return EstadoParede.LEFT;
        }
    }

    private static EstadoParede[,] RecursiveBacktracking(EstadoParede[,] mockupLab, int width, int height)
    {
        var rng = new System.Random();
        var positionStack = new Stack<Position>();
        var pos = new Position { X = rng.Next(0, width), Y = rng.Next(0, height) };

        positionStack.Push(pos);

        while(positionStack.Count > 0)
        {
            var current = positionStack.Pop();
            var vizinhos = GetVizinhosNaoVisitados(current, mockupLab, width, height);

            if(vizinhos.Count > 0)
            {
                positionStack.Push(current);

                var randIndex = rng.Next(0, vizinhos.Count);
                var randVizinho = vizinhos[randIndex];

                var vizPos = randVizinho.posicao;
                mockupLab[current.X, current.Y] &= ~randVizinho.paredeCompartilhada;
                mockupLab[vizPos.X, vizPos.Y] &= ~GetParedeOposta(randVizinho.paredeCompartilhada);

                mockupLab[vizPos.X, vizPos.Y] |= EstadoParede.VISITADO;

                positionStack.Push(vizPos);
            }
        }

        return mockupLab; //1000 1111

    }

    public static List<Vizinho> GetVizinhosNaoVisitados(Position p, EstadoParede[,] lab, int width, int height)
    {
        var list = new List<Vizinho>();

        if(p.X > 0) //esquerda
        {
            if (!lab[p.X-1,p.Y].HasFlag(EstadoParede.VISITADO))
            {
                list.Add(new Vizinho
                {
                    posicao = new Position
                    {
                        X = p.X - 1,
                        Y = p.Y
                    },
                    paredeCompartilhada = EstadoParede.LEFT
                });
            }
        }

        if (p.Y > 0) //abaixo
        {
            if (!lab[p.X, p.Y - 1].HasFlag(EstadoParede.VISITADO))
            {
                list.Add(new Vizinho
                {
                    posicao = new Position
                    {
                        X = p.X,
                        Y = p.Y - 1
                    },
                    paredeCompartilhada = EstadoParede.DOWN
                });
            }
        }

        if (p.Y < height - 1) //cima
        {
            if (!lab[p.X, p.Y + 1].HasFlag(EstadoParede.VISITADO))
            {
                list.Add(new Vizinho
                {
                    posicao = new Position
                    {
                        X = p.X,
                        Y = p.Y + 1
                    },
                    paredeCompartilhada = EstadoParede.UP
                });
            }
        }

        if (p.X < width - 1) //direita
        {
            if (!lab[p.X + 1, p.Y].HasFlag(EstadoParede.VISITADO))
            {
                list.Add(new Vizinho
                {
                    posicao = new Position
                    {
                        X = p.X + 1,
                        Y = p.Y
                    },
                    paredeCompartilhada = EstadoParede.RIGHT
                });
            }
        }

        return list;
    }

    public static EstadoParede[,] Gerar(int width, int height)
    {
        EstadoParede[,] labirinto = new EstadoParede[width, height];
        EstadoParede inicial = EstadoParede.RIGHT | EstadoParede.LEFT | EstadoParede.UP | EstadoParede.DOWN; //Chunk com todas as paredes

        //monta grid de paredes totalmente fechado
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                labirinto[i, j] = inicial; //1111 todas as paredes
            }
        }

        //Cava as paredes para gerar o labirinto
        return RecursiveBacktracking(labirinto, width, height);
    }
}
