using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C3
{
    public class ChiSquaredClassifier<T> : IClassifier<T>
    {
        Dictionary<T, int> categoryMarginals;
        Dictionary<string, int> termMarginals;
        Dictionary<T, Dictionary<string, int>> termsPerCategory;
        int totalTerms = 0;

        public ChiSquaredClassifier() 
        {
            categoryMarginals = new Dictionary<T, int>();
            termMarginals = new Dictionary<string, int>();
            termsPerCategory = new Dictionary<T, Dictionary<string, int>>();
        }

        public void Train(List<KeyValuePair<string, T>> data)
        {
            foreach (var pd in data)
            {
                if (!categoryMarginals.ContainsKey(pd.Value))
                {
                    categoryMarginals.Add(pd.Value, 1);
                    termsPerCategory.Add(pd.Value, new Dictionary<string, int>());
                }
                else
                    categoryMarginals[pd.Value]++;
                foreach (string term in pd.Key.Split(' '))
                {
                    totalTerms++;
                    if (!termMarginals.ContainsKey(term))
                        termMarginals.Add(term, 1);
                    else
                        termMarginals[term]++;
                    if (!termsPerCategory[pd.Value].ContainsKey(term))
                        termsPerCategory[pd.Value].Add(term, 1);
                    else
                        termsPerCategory[pd.Value][term]++;
                }
            }
        }

        private double CalcChiSquared(T category, string term)
        {
            if (!termMarginals.ContainsKey(term))
                return 0;
            double chiSquare = 0.0;
            double observed; double expected;
            if (termsPerCategory[category].ContainsKey(term))
                observed = termsPerCategory[category][term];
            else
                observed = 0;
            expected = termMarginals[term] / (double)totalTerms * categoryMarginals[category];
            chiSquare = Math.Pow((observed - expected), 2) / expected;
            return chiSquare;
        }

        public Classification<T> Categorize(string purchase)
        {
            string[] arPurchase = purchase.Split(' ');
            double bestChiSquare = 0.0;
            T bestCategory = default(T);
            double tempSum = 0.0;
            foreach (T category in categoryMarginals.Keys)
            {
                foreach (string term in arPurchase)
                {
                    tempSum += CalcChiSquared(category, term);
                }
                Console.WriteLine("{0}\t{1:0.00}", category, tempSum);
                if (tempSum > bestChiSquare)
                {
                    bestChiSquare = tempSum;
                    bestCategory = category;
                }
                tempSum = 0.0;
            }
            return new Classification<T>(bestChiSquare, bestCategory);
        }
    }
}