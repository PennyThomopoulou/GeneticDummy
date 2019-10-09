﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace GeneticDummy
{
    class Program
    {
        const int POPULATION_SIZE = 100;
        const double MUTATION_CHANCE = 0.1;
        const int AVAILABLE_COLOURS = 4;
        Random rand = new Random(); 

        static void Main(string[] args)
        {
            string blocksPath = args[0];
            ConnectedBlocksGraph connectedBlocks = new ConnectedBlocksGraph(blocksPath);
            //int[,] population = new int[POPULATION_SIZE,]

        }

        int GetMaxFitness(ConnectedBlocksGraph connectedBlocks)
        {
            int maxFitness = 0;
            foreach (var a in connectedBlocks)
            {
                foreach (var i in a)
                {
                    maxFitness++;
                }
            }
            return maxFitness;
        }

        int RouletteWheelSelection(int maxSize, int[] weights, Random rand)
        {
            int weightSum = weights.Sum();
            int randomInt = rand.Next(weightSum);
            for (int i = 0; i < maxSize; i++)
            {
                if (randomInt <weights[i])
                {
                    return i;
                }
                randomInt -= weights[i];
            }
            return 0;
        }
        

        bool CanEnd(int[] fitness, int maxFitness)
        {
            return maxFitness == fitness.Max();
        }



        int[] ParentRecombination(int[] parentA, int[] parentB, Random rand)
        {
            int splitPosition = rand.Next(parentA.Length);
            int[] child = new int[parentA.Length];
            for (int i = 0; i < parentA.Length; i++)
            {
                if (i <= splitPosition)
                {
                    child[i] = parentA[i];
                }
                else
                {
                    child[i] = parentB[i];
                }
            }
            return child;
        }

        int[,] ApplyMutation(int[,] population, Random random, double mutationChance, int availableColours)
        {
            for (int i = 0; i < population.GetLength(0); i++)
            {
                if(random.NextDouble() < mutationChance)
                {
                    int[] mutatedChild = MutateChild(population.GetRow(i), random, availableColours);
                    for (int j = 0; j < population.GetLength(1); j++)
                    {
                        population[i, j] = mutatedChild[j];
                    }
                }
            }
            return population;
        }

        int[,] ParentSelection(int[,] population, int[] fitness, Random random)
        {
            //Select two parents with roulette wheel selection
            int[,] selectedParents = new int[2, population.GetLength(1)];
            int firstParentPosition = RouletteWheelSelection(population.GetLength(0), fitness, random);
            int secondParentPosition = RouletteWheelSelection(population.GetLength(0), fitness, random);
            //Ensure that the two parents are different
            while (firstParentPosition == secondParentPosition)
            {
                secondParentPosition = RouletteWheelSelection(population.GetLength(0), fitness, random);
            }
            //Add parents in list to return
            for (int i = 0; i < population.GetLength(1); i++)
            {
                selectedParents[0, i] = population[firstParentPosition, i];
                selectedParents[1, i] = population[secondParentPosition, i];
            }

            return selectedParents;
        }

        int[] GetFitness(int[,] population, List<int[]> connectedBlocks, int positiveFitnessPoints, int negativeFitnessPoints)
        {
            int populationSize = population.GetLength(0);
            int numberOfBlocks = population.GetLength(1);
            int[] fitness = new int[populationSize];
            for (int i = 0; i < populationSize; i++)
            {
                for (int j = 0; j < numberOfBlocks; j++)
                {
                    for (int k = 0; k < connectedBlocks[j].GetLength(0); k++)
                    {
                        if(population[i,j] == population[i, connectedBlocks[j][k]-1])
                        {
                            fitness[i] -= negativeFitnessPoints;
                        }
                        else
                        {
                            fitness[i] += positiveFitnessPoints;
                        }
                    }
                }

            }
            return fitness;
        }
    }
}