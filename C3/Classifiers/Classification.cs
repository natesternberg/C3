namespace C3
{
    public class Classification<T>
    {
        public double Probability { get; set; }
        public T Category { get; set; }
        public Classification(double p, T c)
        {
            Probability = p;
            Category = c;
        }

        public override string ToString()
        {
            return $"{Category}: {Probability:0.00000}";
        }
    }
}