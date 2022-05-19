using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent_GA : Agent
{
    //public static Agent_GA _;

    public int numGenerations = 15;
    public int sizeGeneration = 20;
    public int sizeChromosome = 15;
    public float probCrossOver = 0.6f, probMutation = 0.1f;

    List<string>[] populations;
    string lastBestChromosome = "";
    int popIndex, eliteIndex;
    List<float> popFitness, popProbabilities;
    List<int> matingPoolIndices;

    static WorldState currentState;
    static List<Vector3> birdPosList;

    //TODO: Your code here (Q2): Set this flag to true in order to use the 2X site corssover operator of task 2 (you need to implement it below).
    const bool USE_2XSITE_CROSSOVER = false;

    private void Start()
    {
        //_ = this;
        if (populations == null)
        {
            populations = new List<string>[2];
            for (int i = 0; i < 2; i++)
                populations[i] = new List<string>();
        }
        if (popFitness == null)
            popFitness = new List<float>();
        if (popProbabilities == null)
            popProbabilities = new List<float>();
        if (matingPoolIndices == null)
            matingPoolIndices = new List<int>();
        if (currentState == null)
            currentState = new WorldState();
        if (birdPosList == null)
            birdPosList = new List<Vector3>();
    }

    public override Action GetAction()
    {
        MyLineRenderer.Init();
        //Initialize the population
        for (int i = 0; i < 2; i++)
            populations[i].Clear();
        popIndex = 0;
        for (int i = 0; i < sizeGeneration; i++)
        {
            populations[popIndex].Add(RandomChromosome());
        }
        if (lastBestChromosome != "")
        {
            //TODO: Your code here (Q1): Overwrite populations[popIndex][0] with the shifted version of last best chromosome.
        }

        for (int g = 1; g <= numGenerations; g++)
        {
            EvaluateCurrentPopulation();
            FillMatingPool();
            CrossOver();
            Mutation();
            Elitism();
            popIndex = 1 - popIndex;
        }
        //Go over the population one last time to find the best answer found by GA
        EvaluateCurrentPopulation();
        //Now extract first action from populations[popIndex][eliteIndex]
        lastBestChromosome = populations[popIndex][eliteIndex];
        int actionIdx = lastBestChromosome[0] == '0' ? 0 : 1;
        return (Action)actionIdx;
    }

    string RandomChromosome()
    {
        string result = "";
        for (int i = 0; i < sizeChromosome; i++)
            result += Random.Range(0, 2) == 0 ? "0" : "1";
        return result;
    }

    void EvaluateCurrentPopulation()
    {
        popFitness.Clear();
        popProbabilities.Clear();
        float sum = 0, max = float.MinValue;
        for (int i = 0; i < sizeGeneration; i++)
        {
            float f = ComputeFitness(populations[popIndex][i]);
            popFitness.Add(f);
            popProbabilities.Add(f);
            sum += popFitness[i];
            if (popFitness[i] > max)
            {
                max = popFitness[i];
                eliteIndex = i;
            }
        }
        if (sum > 0.1)
        {
            for (int i = 0; i < sizeGeneration; i++)
            {
                popProbabilities[i] /= sum;
            }
        }
        else
        {
            for (int i = 0; i < sizeGeneration; i++)
            {
                popProbabilities[i] = 1.0f / sizeGeneration;
            }
        }
    }

    //We assume that fitness is a positive reward (so it becomes a maximization problem)
    float ComputeFitness(string chromosome)
    {
        //Load the current state
        currentState.Load();
        //Inialize birdPos array to draw on screen
        birdPosList = new List<Vector3>();
        birdPosList.Add(currentState.birdPos);
        //Simulate the world as in chromosome
        int depth;
        for (depth = 0; depth < sizeChromosome && !currentState.gameOver; depth++)
        {
            currentState.SimulateForward(chromosome[depth] == '0' ? Action.None : Action.LeftClick);
            //Update birdPos array
            birdPosList.Add(currentState.birdPos);
        }
        //Now submit the lineStrip to line renderer
        MyLineRenderer.lineStrips.Add(birdPosList);
        //Now compute the actual fitness
        float value = currentState.score * 10 + depth * 5;
        value -= 1 * Mathf.Pow(currentState.birdPos.y - 1.5f, 2); //Try to stay in the middle of screen!
        if (currentState.gameOver)
            value -= 1000.0f;
        return value;
    }

    void FillMatingPool()
    {
        matingPoolIndices.Clear();
        for (int i = 0; i < sizeGeneration; i++)
        {
            float r = Random.Range(0.0f, 1.0f);
            float s = 0;
            for (int j = 0; j < sizeGeneration; j++)
            {
                s += popProbabilities[j];
                if (r <= s)
                {
                    matingPoolIndices.Add(j);
                    break;
                }
            }
        }
    }

    void CrossOver()
    {
        populations[1 - popIndex].Clear();
        for (int i = 0; i < sizeGeneration;)
        {
            string a, b;
            a = populations[popIndex][matingPoolIndices[i++]];
            b = populations[popIndex][matingPoolIndices[i++]];
            if (Random.Range(0.0f, 1.0f) <= probCrossOver)
            {
                string c = "", d = "";
                if (USE_2XSITE_CROSSOVER)
                {
                    //TODO: Your code here (Q2): Implement the 2x site crossover opperation shown in slide #9.
                }
                else
                {
                    int xsite = Random.Range(0, sizeChromosome - 1);
                    for (int j = 0; j < sizeChromosome; j++)
                    {
                        c += j <= xsite ? a[j] : b[j];
                        d += j <= xsite ? b[j] : a[j];
                    }
                }
                populations[1 - popIndex].Add(c);
                populations[1 - popIndex].Add(d);
            }
            else
            {
                populations[1 - popIndex].Add(a);
                populations[1 - popIndex].Add(b);
            }
        }
    }

    void Mutation()
    {
        if (Random.Range(0.0f, 1.0f) < probMutation)
        {
            int idx = Random.Range(0, sizeGeneration);
            int mutIdx = Random.Range(0, sizeChromosome);
            string s = populations[1 - popIndex][idx];
            populations[1 - popIndex][idx] = "";
            for (int i = 0; i < sizeChromosome; i++)
            {
                if (i == mutIdx)
                    populations[1 - popIndex][idx] += s[i] == '0' ? '1' : '0';
                else
                    populations[1 - popIndex][idx] += s[i];
            }
        }
    }

    void Elitism()
    {
        populations[1 - popIndex][Random.Range(0, sizeGeneration)] = populations[popIndex][eliteIndex];
    }
}