using System;
using System.Reflection;

namespace C3.Classifiers
{
    public static class ClassifierFactory
    {
        private const string typeTemplate = "C3.{0}`1, {1}";

        // Creates and returns classifiers given the classifier's name and type argument.
        // Needed since the user specifies the classifier type in app.config
        public static IClassifier<T> GetClassifierByName<T>(string classifierName)
        {
            string c3assembly = Assembly.GetExecutingAssembly().FullName;
            string typeFullName = string.Format(typeTemplate, classifierName, c3assembly);
            Type cType = Type.GetType(typeFullName);
            if (cType == null)
            {
                throw new ApplicationException($"Unable to create classifier of type {typeFullName}.");
            }
            Type gType = cType.MakeGenericType(typeof(T));
            return Activator.CreateInstance(gType) as IClassifier<T>;
        }
    }
}