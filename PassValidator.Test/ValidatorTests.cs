using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PassValidator.Web.Validation;

namespace PassValidator.Test
{
    [TestClass]
    public class ValidatorTestsC:\Development\pkpassvalidator\PassValidator.Test\ValidatorTests.cs
    {
        [TestMethod]
        public void TestMethod1()
        {
            Validator validator = new Validator();
            var result = validator.Validate(new byte[0]);
        }
    }
}
