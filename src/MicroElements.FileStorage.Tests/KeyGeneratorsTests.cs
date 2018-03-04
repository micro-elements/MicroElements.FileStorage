using System;
using System.Threading.Tasks;
using FluentAssertions;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.KeyGenerators;
using MicroElements.FileStorage.Serializers;
using MicroElements.FileStorage.Tests.Models;
using Xunit;

namespace MicroElements.FileStorage.Tests
{
    public class KeyGeneratorsTests
    {
        [Fact]
        public void GuidKeyGeneratorTest()
        {
            var keyGenerator = new GuidKeyGenerator<Person>();
            keyGenerator.KeyStrategy.Should().Be(KeyType.UniqId);
            var key1 = keyGenerator.GetNextKey(null, null);
            key1.KeyType.Should().Be(KeyType.UniqId);
            Guid.Parse(key1.Value).Should().NotBe(Guid.Empty);

            var key2 = keyGenerator.GetNextKey(null, null);
            key2.Value.Should().NotBe(key1.Value);
        }

        [Fact]
        public void HashKeyGeneratorTest()
        {
            var keyGenerator = new HashKeyGenerator<Person>(new JsonSerializer());
            keyGenerator.KeyStrategy.Should().Be(KeyType.Hash);

            Action testCode = () => keyGenerator.GetNextKey(null, null);
            testCode.Should().Throw<ArgumentNullException>();

            var key1 = keyGenerator.GetNextKey(null, TestData.Bill);
            key1.Value.Should().Be("069D064D627D599C8EC69F9B4D843B2A");

            var key2 = keyGenerator.GetNextKey(null, TestData.Bill);
            key2.Value.Should().Be(key1.Value);
        }

        [Fact]
        public async Task IdentityKeyGeneratorTest()
        {
            var keyGenerator = new IdentityKeyGenerator<Person>();
            keyGenerator.KeyStrategy.Should().Be(KeyType.Identity);

            Action testCode = () => keyGenerator.GetNextKey(null, TestData.Bill);
            testCode.Should().Throw<ArgumentNullException>();

            var dataStore = await TestData.CreatePersonsDataStore();
            var key1 = keyGenerator.GetNextKey(dataStore, TestData.Bill);
            key1.Value.Should().Be("1");

            var key2 = keyGenerator.GetNextKey(dataStore, TestData.Bill);
            key2.Value.Should().Be(key1.Value);

            Action ctorWithNegative = () => new IdentityKeyGenerator<Person>(-1);
            ctorWithNegative.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void SemanticKeyGeneratorTest()
        {
            var keyGenerator = new SemanticKeyGenerator<Person>(person => $"{person.FirstName}_{person.LastName}");
            keyGenerator.KeyStrategy.Should().Be(KeyType.Semantic);

            Action testCode = () => keyGenerator.GetNextKey(null, TestData.Bill);
            testCode.Should().NotThrow<ArgumentNullException>();

            var key1 = keyGenerator.GetNextKey(null, TestData.Bill);
            key1.Value.Should().Be("Bill_Gates");

            var key2 = keyGenerator.GetNextKey(null, TestData.Bill);
            key2.Value.Should().Be(key1.Value);
        }

        [Fact]
        public void KeyGeneratorTest1()
        {
            var keyGenerator = new KeyGenerator<Person>(person => $"{person.FirstName}_{person.LastName}", KeyType.Semantic);
            keyGenerator.KeyStrategy.Should().Be(KeyType.Semantic);

            Action testCode = () => keyGenerator.GetNextKey(null, TestData.Bill);
            testCode.Should().NotThrow<ArgumentNullException>();

            var key1 = keyGenerator.GetNextKey(null, TestData.Bill);
            key1.Value.Should().Be("Bill_Gates");

            var key2 = keyGenerator.GetNextKey(null, TestData.Bill);
            key2.Value.Should().Be(key1.Value);
        }

        [Fact]
        public void KeyGeneratorTest2()
        {
            var keyGenerator = new KeyGenerator<Person>(person => person.FirstName + "_" + Guid.NewGuid().ToString() + "_" + person.LastName, KeyType.UniqId);
            keyGenerator.KeyStrategy.Should().Be(KeyType.UniqId);

            var key1 = keyGenerator.GetNextKey(null, TestData.Bill);
            key1.Value.Should().StartWith("Bill_");
            key1.Value.Should().EndWith("_Gates");
        }
    }
}
