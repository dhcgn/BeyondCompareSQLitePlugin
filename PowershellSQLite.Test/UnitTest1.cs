using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Management.Automation;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PowershellSQLite.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            
            
            
            var tableDynamicParameters = new TableDynamicParameters();
            var typeDescriptionProvider = TypeDescriptor.AddAttributes(tableDynamicParameters, new ValidateSetAttribute("Table1_New", "Table2_New"));
            var tableDynamicParameters_new = (TableDynamicParameters)typeDescriptionProvider.CreateInstance(new ServiceContainer(), tableDynamicParameters.GetType(), null, null);

        
           
            // var prop = TypeDescriptor.CreateProperty(tableDynamicParameters.GetType(), "Tables", tableDynamicParameters.GetType(), new ValidateSetAttribute("Auto", "Vogel"));
                 
       
           
        }
    }
}
