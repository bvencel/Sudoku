﻿using Sudoku.Entities;
using Sudoku.Helpers;
using System;
using System.Collections.Generic;

namespace Sudoku
{
    internal class Program
    {
        private static readonly SudokuWithLegalValues originalSudoku = new SimpleSudoku(new byte[,] {
                {5, 3, 0,  0, 7, 0,  0, 0, 0},
                {6, 0, 0,  1, 9, 5,  0, 0, 0},
                {0, 9, 8,  0, 0, 0,  0, 6, 0},

                {8, 0, 0,  0, 6, 0,  0, 0, 3},
                {4, 0, 0,  8, 0, 3,  0, 0, 1},
                {7, 0, 0,  0, 2, 0,  0, 0, 6},

                {0, 6, 0,  0, 0, 0,  2, 8, 0},
                {0, 0, 0,  4, 1, 9,  0, 0, 5},
                {0, 0, 0,  0, 8, 0,  0, 7, 9}}).ToSudokuWithLegalValues();

        private static readonly SudokuWithLegalValues originalSudoku16 = new SimpleSudoku(new byte[,] {
            { 0, 0, 3, 0,   5, 0,13, 0,  16, 0,14, 0,   0, 0,15, 0},
            { 0,10,15, 0,   0, 0, 0, 0,   0, 0, 0, 6,   0, 0, 0,11},
            { 6, 0, 0, 8,   0,10,15, 0,   0, 9, 0, 0,   4, 2, 0, 5},
            { 0, 5, 0, 0,   0, 0, 0, 0,   2,10, 8,15,   0, 0,16, 0},

            { 2, 9, 8,13,   6, 0, 0, 1,   0, 0, 0, 7,  14, 0, 0,16},
            {15, 4, 0, 5,   0, 8,16, 0,   6,14, 1,11,   0, 0, 0, 9},
            { 0, 0, 7,11,   0,13, 0,15,   5, 0, 0,16,   0, 0, 0, 0},
            { 3,14, 0,16,   4, 0, 0, 0,   0, 2, 0, 0,  15, 0,11, 0},

            { 0, 3, 0, 4,   0, 0, 9, 0,   0, 0, 0,13,   8, 0, 6, 2},
            { 0, 0, 0, 0,  13, 0, 0, 6,  10, 0, 2, 0,  11, 7, 0, 0},
            {11, 0, 0, 0,  15,16, 1, 4,   0,12, 9, 0,   3, 0,10,14},
            {14, 0, 0,15,   8, 0, 0, 0,   7, 0, 0, 3,   1, 9,12, 4},

            { 0,15, 0, 0,   7, 1, 6,12,   0, 0, 0, 0,   0, 0, 2, 0},
            { 7, 0,13, 6,   0, 0, 5, 0,   0,15, 4, 0,  12, 0, 0, 8},
            { 8, 0, 0, 0,  14, 0, 0, 0,  13, 0, 0, 0,   0, 3, 1, 0},
            { 0,12, 0, 0,   0, 2, 0,16,   0, 5, 0,10,   0,15, 4, 0}}).ToSudokuWithLegalValues();

        private static readonly SudokuWithLegalValues originalSudoku16Empty = new SimpleSudoku(new byte[,] {
            {5, 3, 0, 0,  0, 0, 0, 0,  7, 0, 0, 0,  0, 0, 0, 0},
            {6, 0, 0, 1,  5, 0, 0, 0,  9, 0, 0, 0,  0, 0, 0, 0},
            {0, 9, 8, 0,  0, 0, 0, 0,  0, 0, 0, 0,  6, 0, 0, 0},
            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},

            {0, 0, 0, 0,  0, 0, 0, 0,  6, 0, 0, 0,  0, 3, 0, 0},
            {4, 0, 0, 8,  3, 0, 0, 0,  0, 0, 0, 0,  0, 1, 0, 0},
            {7, 0, 0, 0,  0, 0, 0, 0,  2, 0, 0, 0,  0, 6, 0, 0},
            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},

            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},

            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            {0, 6, 0, 0,  0, 0, 0, 0,  0, 0, 0, 2,  8, 0, 0, 0},
            {0, 0, 0, 4,  9, 0, 0, 0,  1, 0, 0, 0,  0, 5, 0, 0},
            {0, 0, 0, 0,  0, 0, 0, 0,  8, 0, 0, 0,  7, 9, 0, 0}}).ToSudokuWithLegalValues();

        private static readonly SudokuWithLegalValues originalSudoku16Empty2 = new SimpleSudoku(new byte[,] {
            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},

            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},

            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},

            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0},
            {0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0,  0, 0, 0, 0}}).ToSudokuWithLegalValues();

        private static readonly SudokuWithLegalValues originalSudoku16Solved = new SimpleSudoku(new byte[,] {
                { 4, 2, 3, 7,   5, 6,13, 9,  16,11,14, 1,  10, 8,15,12},
                {16,10,15,14,   2, 3,12, 8,   4,13, 5, 6,   7, 1, 9,11},
                { 6, 1,11, 8,  16,10,15,14,   3, 9, 7,12,   4, 2,13, 5},
                {13, 5, 9,12,   1, 4,11, 7,   2,10, 8,15,   6,14,16, 3},

                { 2, 9, 8,13,   6,11,10, 1,  15, 4,12, 7,  14, 5, 3,16},
                {15, 4,10, 5,   3, 8,16, 2,   6,14, 1,11,  13,12, 7, 9},
                {12, 6, 7,11,   9,13,14,15,   5, 3,10,16,   2, 4, 8, 1},
                { 3,14, 1,16,   4,12, 7, 5,   9, 2,13, 8,  15, 6,11,10},

                {10, 3, 5, 4,  12, 7, 9,11,  14, 1,15,13,   8,16, 6, 2},
                { 9, 8,12, 1,  13,14, 3, 6,  10,16, 2, 4,  11, 7, 5,15},
                {11, 7, 6, 2,  15,16, 1, 4,   8,12, 9, 5,   3,13,10,14},
                {14,13,16,15,   8, 5, 2,10,   7, 6,11, 3,   1, 9,12, 4},

                { 5,15, 4, 9,   7, 1, 6,12,  11, 8, 3,14,  16,10, 2,13},
                { 7,16,13, 6,  10, 9, 5, 3,   1,15, 4, 2,  12,11,14, 8},
                { 8,11, 2,10,  14,15, 4,13,  12, 7,16, 9,   5, 3, 1, 6},
                { 1,12,14, 3,  11, 2, 8,16,  13, 5, 6,10,   9,15, 4, 7}}).ToSudokuWithLegalValues();

        private static readonly SudokuWithLegalValues originalSudokuEmpty = new SimpleSudoku(new byte[,] {
                {0, 0, 0,  0, 0, 0,  0, 0, 0},
                {0, 0, 0,  0, 0, 0,  0, 0, 0},
                {0, 0, 0,  0, 0, 0,  0, 0, 0},

                {0, 0, 0,  0, 0, 0,  0, 0, 0},
                {0, 0, 0,  0, 0, 0,  0, 0, 0},
                {0, 0, 0,  0, 0, 0,  0, 0, 0},

                {0, 0, 0,  0, 0, 0,  0, 0, 0},
                {0, 0, 0,  0, 0, 0,  0, 0, 0},
                {0, 0, 0,  0, 0, 0,  0, 0, 0}}).ToSudokuWithLegalValues();

        private static readonly SudokuWithLegalValues originalSudokuSolved = new SimpleSudoku(new byte[,] {
                {5, 3, 4,  6, 7, 8,  9, 1, 2},
                {6, 7, 2,  1, 9, 5,  3, 4, 8},
                {1, 9, 8,  3, 4, 2,  5, 6, 7},

                {8, 5, 9,  7, 6, 1,  4, 2, 3},
                {4, 2, 6,  8, 5, 3,  7, 9, 1},
                {7, 1, 3,  9, 2, 4,  8, 5, 6},

                {9, 6, 1,  5, 3, 7,  2, 8, 4},
                {2, 8, 7,  4, 1, 9,  6, 3, 5},
                {3, 4, 5,  2, 8, 6,  1, 7, 9}}).ToSudokuWithLegalValues();

        private static void Main()
        {
            List<SudokuWithLegalValues> results = SudokuHelper.Solve(originalSudoku16, 2, null, out _);

            int counter = 1;

            if (results == null)
            {
                return;
            }

            foreach (var sudokuSolution in results)
            {
                Console.WriteLine($"Solution {counter}/{results.Count}:");
                SudokuHelper.LogSudoku(sudokuSolution, true);
                Console.WriteLine();
                Console.WriteLine();

                string compact = SudokuHelper.GenerateDisplayStringCompact(sudokuSolution);
                Console.WriteLine(compact);
                counter++;
            }
        }
    }
}