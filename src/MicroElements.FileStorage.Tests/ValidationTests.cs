using System.Collections.Generic;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.Validation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MicroElements.FileStorage.Tests
{
    public class ValidationTests
    {
        [Fact]
        public void SimpleValidationSample()
        {
            Customer customer = new Customer();
            CustomerValidator validator = new CustomerValidator();
            ValidationResult results = validator.Validate(customer);

            results.IsValid.Should().BeFalse();
            results.Errors.Should().NotBeEmpty();
        }

        [Fact]
        public void ValidationProvider()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IValidator<Customer>, CustomerValidator>();
            services.AddSingleton<IValidatorFactory, ServiceProviderValidationFactory>();
            var serviceProvider = services.BuildServiceProvider();
            var validatorFactory = serviceProvider.GetRequiredService<IValidatorFactory>();
            validatorFactory.GetValidator<Customer>().Should().NotBeNull();
        }
    }

    public class Customer
    {
        public string Surname { get; set; }
        public string Forename { get; set; }
        public int Discount { get; set; }
        public bool HasDiscount { get; set; }
        public string Address { get; set; }
        public string Postcode { get; set; }
    }

    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(customer => customer.Surname).NotEmpty();
            RuleFor(customer => customer.Forename).NotEmpty().WithMessage("Please specify a first name");
            RuleFor(customer => customer.Discount).NotEqual(0).When(customer => customer.HasDiscount);
            RuleFor(customer => customer.Address).Length(20, 250);
            RuleFor(customer => customer.Postcode).Must(BeAValidPostcode).WithMessage("Please specify a valid postcode");
        }

        private bool BeAValidPostcode(string postcode)
        {
            // custom postcode validating logic goes here
            return postcode != null;
        }
    }
}
