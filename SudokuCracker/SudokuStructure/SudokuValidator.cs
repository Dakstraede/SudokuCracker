﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuCracker.SudokuStructure
{
    public class SudokuValidator
    {
        public  List<string> ErrorMessagesList = new List<string>();
        private readonly IEnumerable<char> _characterSet;
        private readonly IEnumerable<string> _lines;
        private Grille _grid;

        public SudokuValidator(IEnumerable<char> characterSet, IEnumerable<string> lines, ref Grille grid)
        {
            _characterSet = characterSet;
            _lines = lines;
            _grid = grid;
        }

        public bool ExecuteTests()
        {
            try
            {
                if (!_ValidateCharacterSet()) return false;
                if (!_ValidateLines()) return false;
                if (!_ValidateColumns()) return false;
                return _ValidateRegions();
            }
            finally
            {
                _grid.InitializeCases(_lines);
                _grid.ErrorMessagesList = ErrorMessagesList;
                if (ErrorMessagesList.Any())
                {
                    _grid.IsValid = false;
                }
            }
        }

        /// <summary>
        /// Checks if the set of given characters does not contain any double
        /// and will constitute a perfect square
        /// </summary>
        /// <returns></returns>
        private bool _ValidateCharacterSet()
        {
            var result = true;

            if (_characterSet.Distinct().Count() != _characterSet.Count())
            {
                ErrorMessagesList.Add("Il y a des doublons dans le jeu de caractères");
                result = false;
            }

            if (Math.Sqrt(_characterSet.Count()) % 1 != 0)
            {
                ErrorMessagesList.Add("Le jeu de caractères ne permet pas de constituer un carré parfait");
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Checks if the lines' length is correct, and if there is no double or unknow character
        /// </summary>
        /// <returns></returns>
        private bool _ValidateLines()
        {
            var result = true;
            if (Math.Sqrt(_lines.Count())%1 != 0)
            {
                ErrorMessagesList.Add("La grille fournie ne forme pas un carré parfait");
                result = false;
            }

            var i = 1;
            foreach (var line in _lines)
            {
                if (line.Replace(Case.EmptyCase, "").Distinct().Count() != line.Replace(Case.EmptyCase, "").Length)
                {
                    ErrorMessagesList.Add("Doublons de caractères à la ligne "+i);
                    result = false;
                }
                if (line.Length != _characterSet.Count())
                {
                    ErrorMessagesList.Add("La ligne "+i+" est de taille incorrecte");
                }
                foreach (char c in line)
                {
                    if (!_characterSet.Contains(c) && c.ToString() != Case.EmptyCase)
                    {
                        ErrorMessagesList.Add("Symbole absent du jeu de caractères à la ligne "+i);
                        result = false;
                    }
                }
                i++;
            }
            return result;
        }

        /// <summary>
        /// Checks if no doubles are present in columns
        /// </summary>
        /// <returns></returns>
        private bool _ValidateColumns()
        {
            bool result = true;
            int count = _characterSet.Count();

            var columnTab = new char[count][];
            for (var i = 0; i < count; i++)
                columnTab[i] = new char[count];

            var rowTab = new char[count][];
            for (int i = 0; i < count; i++)
                rowTab[i] = _lines.ElementAt(i).ToCharArray();

            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    columnTab[i][j] = rowTab[j][i];
                }
            }
            
            for (int idx = 0; idx < count; idx++)
            {
                string currentLine = new string(columnTab[idx]);
                if (currentLine.Replace(Case.EmptyCase, "").Distinct().Count() != currentLine.Replace(Case.EmptyCase, "").Length)
                {
                    ErrorMessagesList.Add("Doublon de symbole à la colonne " + (idx + 1));
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// Checks if no doubles are present in regions
        /// </summary>
        /// <returns></returns>
        private bool _ValidateRegions()
        {
            bool result = true;

            var regionSize = Math.Sqrt(_characterSet.Count());
            int count = _characterSet.Count();
            var rowTab = new char[count][];
            for (int i = 0; i < count; i++)
                rowTab[i] = _lines.ElementAt(i).ToCharArray();

            string[] regionTab = new string[count];

            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                    regionTab[((i / (int)regionSize) * (int)regionSize) + (j / (int)regionSize)] += rowTab[i][j];
            }

            int idx = 1;
            foreach (var region in regionTab)
            {
                if (region.Replace(Case.EmptyCase, "").Distinct().Count() != region.Replace(Case.EmptyCase, "").Length)
                {
                    ErrorMessagesList.Add("Doublon de symboles dans la région "+idx);
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Displays errors in console
        /// </summary>
        public void PrintError()
        {
            foreach (var error in ErrorMessagesList)
            {
                Console.WriteLine(error);
            }
        }
        
    }
}
