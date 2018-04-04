using System;

namespace C3.Readers
{
    public static class BankConfigFactory
    {
        private const string typeTemplate = "C3.Readers.{0}RecordConfig";

        // Creates and returns BankRecordConfig objects given the bank name.
        public static BankRecordConfig GetByName(string classMapName)
        {
            string typeFullName = string.Format(typeTemplate, classMapName);
            Type cType = Type.GetType(typeFullName);
            if (cType == null)
            {
                throw new ApplicationException($"Unable to create BankConfig of type {typeFullName}.");
            }
            return Activator.CreateInstance(cType) as BankRecordConfig;
        }
    }
}
