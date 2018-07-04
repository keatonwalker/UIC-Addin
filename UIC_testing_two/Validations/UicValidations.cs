using System;
using System.ComponentModel.DataAnnotations;

namespace UIC_Edit_Workflow
{
    public sealed class UicValidations : ValidationAttribute
    {
        // https://github.com/agrc/uic-etl/blob/master/domain.uic-etl/xml/ContactDetail.cs
        public override bool IsValid(object value)
        {
            return (string)value == "correct";
        }
        public override string FormatErrorMessage(string name)
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture,
              ErrorMessageString, name);
        }
    }

    public sealed class NameTest : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return (string)value == "tester";
        }
        public override string FormatErrorMessage(string name)
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture,
              ErrorMessageString, name);
        }
    }

}
