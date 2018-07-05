using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace UIC_Edit_Workflow.Models
{
    internal class ValidatableBindableBase : BindableBase, INotifyDataErrorInfo
    {
        protected ValidatableBindableBase()
        {
            LoadHash = CalculateFieldHash();
        }
        private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                return _errors[propertyName];
            }

            return null;
        }

        public bool HasErrors => _errors.Count > 0;

        protected override void SetProperty<T>(ref T member, T val, [CallerMemberName] string propertyName = null)
        {
            ValidateProperty(propertyName, val);
            base.SetProperty(ref member, val, propertyName);
        }

        private void ValidateProperty<T>(string propertyName, T value)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(this)
            {
                MemberName = propertyName
            };
            Validator.TryValidateProperty(value, context, results);

            if (results.Any())
            {
                _errors[propertyName] = results.Select(c => c.ErrorMessage).ToList();
            }
            else
            {
                _errors.Remove(propertyName);
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public string LoadHash { get; set; }

        protected virtual string FieldValueString()
        {
            throw new NotImplementedException("not yet implemented");
        }

        protected string CalculateFieldHash()
        {
            var fieldString = FieldValueString();
            // step 1, calculate MD5 hash from input

            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(fieldString);
            var hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();

            foreach (var t in hash)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }

        public bool HasModelChanged()
        {
            return !LoadHash.Equals(CalculateFieldHash());
        }
    }
}
