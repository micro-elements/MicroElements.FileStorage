// Copyright (c) MicroElements. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// todo: now FluentValidation is used for simplicity. remove or use own abstraction.
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace MicroElements.FileStorage.Abstractions
//{
//    public interface IValidator
//    {
//        /// <summary>
//        /// Validates the specified instance
//        /// </summary>
//        /// <param name="instance"></param>
//        /// <returns>A ValidationResult containing any validation failures</returns>
//        ValidationResult Validate(object instance);
//    }

//    /// <summary>
//    /// Defines a validator for a particular type.
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    public interface IValidator<in T> : IValidator
//    {
//        /// <summary>
//        /// Validates the specified instance.
//        /// </summary>
//        /// <param name="instance">The instance to validate</param>
//        /// <returns>A ValidationResult object containing any validation failures.</returns>
//        ValidationResult Validate(T instance);
//    }

//    public class ValidationResult
//    {
//        private readonly IList<ValidationError> _errors;

//        /// <summary>
//        /// Whether validation succeeded
//        /// </summary>
//        public virtual bool IsValid => Errors.Count == 0;

//        /// <summary>
//        /// A collection of errors
//        /// </summary>
//        public IList<ValidationError> Errors => _errors;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
//        /// </summary>
//        public ValidationResult()
//        {
//            _errors = new List<ValidationError>();
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
//        /// </summary>
//        /// <param name="failures">List of <see cref="ValidationError"/> which is later available through <see cref="Errors"/>. This list get's copied.</param>
//        /// <remarks>
//        /// Every caller is responsible for not adding <c>null</c> to the list.
//        /// </remarks>
//        public ValidationResult(IEnumerable<ValidationError> failures)
//        {
//            _errors = failures.Where(failure => failure != null).ToList();
//        }
//    }

//    public class ValidationError
//    {
//        private ValidationError()
//        {

//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="ValidationError"/> class.
//        /// </summary>
//        public ValidationError(string propertyName, string errorMessage) : this(propertyName, errorMessage, null)
//        {
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="ValidationError"/> class.
//        /// </summary>
//        public ValidationError(string propertyName, string errorMessage, object attemptedValue)
//        {
//            PropertyName = propertyName;
//            ErrorMessage = errorMessage;
//            AttemptedValue = attemptedValue;
//        }

//        /// <summary>
//        /// The name of the property.
//        /// </summary>
//        public string PropertyName { get; private set; }

//        /// <summary>
//        /// The error message
//        /// </summary>
//        public string ErrorMessage { get; private set; }

//        /// <summary>
//        /// The property value that caused the failure.
//        /// </summary>
//        public object AttemptedValue { get; private set; }

//        /// <summary>
//        /// Custom state associated with the failure.
//        /// </summary>
//        public object CustomState { get; set; }

//        /// <summary>
//        /// Custom severity level associated with the failure.
//        /// </summary>
//        public Severity Severity { get; set; }

//        /// <summary>
//        /// Gets or sets the error code.
//        /// </summary>
//        public string ErrorCode { get; set; }

//        /// <summary>
//        /// Gets or sets the formatted message arguments.
//        /// These are values for custom formatted message in validator resource files
//        /// Same formatted message can be reused in UI and with same number of format placeholders
//        /// Like "Value {0} that you entered should be {1}"
//        /// </summary>
//        public object[] FormattedMessageArguments { get; set; }

//        /// <summary>
//        /// Gets or sets the formatted message placeholder values.
//        /// </summary>
//        public Dictionary<string, object> FormattedMessagePlaceholderValues { get; set; }

//        /// <summary>
//        /// The resource name used for building the message
//        /// </summary>
//        public string ResourceName { get; set; }

//        /// <summary>
//        /// Creates a textual representation of the failure.
//        /// </summary>
//        public override string ToString()
//        {
//            return ErrorMessage;
//        }
//    }

//    /// <summary>
//    /// Specifies the severity of a rule.
//    /// </summary>
//    public enum Severity
//    {
//        /// <summary>
//        /// Error
//        /// </summary>
//        Error,

//        /// <summary>
//        /// Warning
//        /// </summary>
//        Warning,

//        /// <summary>
//        /// Info
//        /// </summary>
//        Info
//    }

//    /// <summary>An exception that represents failed validation</summary>
//    [Serializable]
//    public class ValidationException : Exception
//    {
//        /// <summary>Validation errors</summary>
//        public IEnumerable<ValidationError> Errors { get; private set; }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="ValidationException"/> class.
//        /// </summary>
//        /// <param name="message"></param>
//        public ValidationException(string message)
//            : this(message, Enumerable.Empty<ValidationError>())
//        {
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="ValidationException"/> class.</summary>
//        /// <param name="message"></param>
//        /// <param name="errors"></param>
//        public ValidationException(string message, IEnumerable<ValidationError> errors)
//            : base(message)
//        {
//            this.Errors = errors;
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="ValidationException"/> class.</summary>
//        /// <param name="errors"></param>
//        public ValidationException(IEnumerable<ValidationError> errors)
//            : base(ValidationException.BuildErrorMesage(errors))
//        {
//            this.Errors = errors;
//        }

//        private static string BuildErrorMesage(IEnumerable<ValidationError> errors)
//        {
//            return "Validation failed: " + string.Join(string.Empty, errors.Select<ValidationError, string>((Func<ValidationError, string>)(x => Environment.NewLine + " -- " + x.ErrorMessage)).ToArray<string>());
//        }
//    }
//}
