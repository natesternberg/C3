using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C3
{
    /// <summary>
    /// Builds a Naive Bayes classifier from a List<> of Tuple<string, T>.  The string is a description of the item,
    /// and T is the category, usually an string, but could be anything.  Probabilities are calculated when 
    /// Train() is called.  By default the description is tokenized with String.Split() but you can override this
    /// by passing in the optional tokenizer parameter in the constructor.
    /// 
    /// Note that it does no case-normalization / stemming / etc. on the string.  Do that ahead of time, or 
    /// pass in a function to the tokenizer.
    /// </summary>
    /// <typeparam name="T">The category that the classifier will return.</typeparam>
    public class NaiveBayesClassifier<T> : IClassifier<T>
    {
        private Dictionary<T, double> categoryProbability;
        private Dictionary<string, double> termProbability;
        private Dictionary<T, Dictionary<string, double>> conditionalProbability;
        private Func<string, string[]> tokenizer = x => x.Split(' ');

        public NaiveBayesClassifier()
        {
            categoryProbability = new Dictionary<T, double>();
            termProbability = new Dictionary<string, double>();
            conditionalProbability = new Dictionary<T, Dictionary<string, double>>();
        }

        public NaiveBayesClassifier(Func<string, string[]> tokenizer) : this()
        {
            this.tokenizer = tokenizer;
        }

        private double GetPosterior(string input, T category)
        {
            double termEpsilon = 1;
            double conditionalEpsilon = 0.0001;
            double totalPosteriorProbability = 0.0;
            foreach (string term in this.tokenizer(input))
            {
                double termPosteriorProbability = 0.0;
                double catProb = categoryProbability[category];
                double termProb = termEpsilon;
                if (termProbability.ContainsKey(term))
                    termProb = termProbability[term];
                double termGivenCatProb = conditionalEpsilon;
                if (conditionalProbability[category].ContainsKey(term))
                    termGivenCatProb = conditionalProbability[category][term];
                termPosteriorProbability = Math.Log10(catProb * termGivenCatProb / termProb);  
                totalPosteriorProbability += termPosteriorProbability;
            }
            return totalPosteriorProbability;
        }


        public void Train(List<KeyValuePair<string, T>> data)
        {
            var catCount = new Dictionary<T, int>();
            var termCount = new Dictionary<string, int>();
            var termPerCatCount = new Dictionary<T, Dictionary<string, int>>();
            int totalTermCount = 0;
            foreach (var pd in data)
            {
                T category = pd.Value;
                if (catCount.ContainsKey(category))
                    catCount[category]++;
                else
                    catCount.Add(category, 1);
                if (!termPerCatCount.ContainsKey(category))
                    termPerCatCount.Add(category, new Dictionary<string, int>());
                foreach (string token in this.tokenizer(pd.Key))
                {
                    totalTermCount++;
                    if (termCount.ContainsKey(token))
                        termCount[token]++;
                    else
                        termCount.Add(token, 1);
                    if (!termPerCatCount[category].ContainsKey(token))
                        termPerCatCount[category].Add(token, 1);
                    else
                        termPerCatCount[category][token]++;
                }
            }

            foreach (T category in catCount.Keys)
            {
                double catProb = (double)catCount[category] / (double)data.Count;
                categoryProbability.Add(category, catProb);
            }
            foreach (string term in termCount.Keys)
            {
                double termProb = (double)termCount[term] / (double)totalTermCount;
                termProbability.Add(term, termProb);
            }
            foreach (T category in termPerCatCount.Keys)
            {
                conditionalProbability.Add(category, new Dictionary<string, double>());
                int totalTermsPerCategory = 0;
                foreach (int tempCount in termPerCatCount[category].Values)
                    totalTermsPerCategory += tempCount;
                foreach (string term in termPerCatCount[category].Keys)
                {
                    double prob = (double)termPerCatCount[category][term] / (double)totalTermsPerCategory;
                    conditionalProbability[category].Add(term, prob);
                }
            }
        }

        public Classification<T> Categorize(string input)
        {
            if (this.termProbability.Count == 0 || this.categoryProbability.Count == 0 || this.conditionalProbability.Count == 0)
                throw new InvalidOperationException("Classifier must be initialized with Train() before calling Categorize().");
            double highestProbSoFar = double.MinValue;
            T bestCategorySoFar = default(T);
            foreach (T category in categoryProbability.Keys)
            {
                double tempProb = GetPosterior(input, category);
                if (tempProb > highestProbSoFar)
                {
                    highestProbSoFar = tempProb;
                    bestCategorySoFar = category;
                }
            }
            return new Classification<T>(highestProbSoFar, bestCategorySoFar);
        }
    }
}