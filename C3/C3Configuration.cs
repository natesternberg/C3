using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace C3
{
    public class C3PredictedColumn
    {
        public string columnName;
        public HashSet<string> validValues;
        public string defaultValue;
        public string classifierName;
    }

    public class C3Configuration
    {
        public C3PredictedColumn[] columns;

        public static C3Configuration LoadFromConfigurationManager()
        {
            var results = ConfigurationManager.GetSection("columns") as C3PredictedColumn[];
            return new C3Configuration() { columns = results };
        }
    }

    // Parse custom section from app.config and return set of C3PredictedColumn
    public class C3ColumnSection : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            List<C3PredictedColumn> columnDefinition = new List<C3PredictedColumn>();
            foreach (XmlNode childNode in section.ChildNodes)
            {
                C3PredictedColumn column = new C3PredictedColumn();
                List<string> values = new List<string>();
                if (childNode.Attributes == null || childNode.Attributes["name"].Value == null)
                {
                    throw new ConfigurationErrorsException("'name' attribte required for each column.", childNode);
                }
                column.columnName = childNode.Attributes["name"].Value;
                if (childNode.Attributes == null || childNode.Attributes["classifier"].Value == null)
                {
                    throw new ConfigurationErrorsException("'classifier' attribte required for each column.", childNode);
                }
                column.classifierName = childNode.Attributes["classifier"].Value;
                
                if (childNode.ChildNodes.Count == 0)
                {
                    throw new ConfigurationErrorsException("Expected at least one child element for each column.", childNode);
                }
                foreach (XmlNode valueNode in childNode.ChildNodes)
                {
                    values.Add(valueNode.ChildNodes[0].Value);
                }
                if (values.ContainsDuplicates())
                {
                    throw new ConfigurationErrorsException("Column values may not contain duplicates.", childNode);
                }
                column.validValues = values.ToHashSet(StringComparer.InvariantCultureIgnoreCase);

                if (childNode.Attributes["default"].Value == null)
                {
                    throw new ConfigurationErrorsException($"Column {childNode.Attributes["name"].Value} does not have an associated default value.", childNode);
                }
                if (!column.validValues.Contains(childNode.Attributes["default"].Value))
                {
                    throw new ConfigurationErrorsException($"Column {childNode.Attributes["name"].Value} specifies a default value of '{childNode.Attributes["default"].Value}', but that value is not present in the list of valid values.", childNode);
                }
                column.defaultValue = childNode.Attributes["default"].Value;
                columnDefinition.Add(column);
            }
            return columnDefinition.ToArray();
        }
    }
}
