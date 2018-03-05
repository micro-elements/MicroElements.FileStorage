using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using Classifiers;
using FluentAssertions;
using MicroElements.FileStorage.Abstractions;
using MicroElements.FileStorage.KeyAccessors;
using MicroElements.FileStorage.NuGetEngine;
using Microsoft.Extensions.Logging;
using Xunit;

namespace MicroElements.FileStorage.Tests
{
    public class NuGetStorageTests
    {
        [Fact]
        public async Task LoadNugetPackage()
        {
            var storageConfiguration = new NuGetStorageConfiguration
            {
                PackageSource = "https://www.myget.org/F/micro-elements/api/v3/index.json",
                PackageId = "Classifiers.Currency",
                PackageVersion = "0.0.1"
            };
            var nuGetStorageEngine = new NuGetStorageProvider(storageConfiguration, new LoggerFactory().AddConsole());
            var readDirectory = nuGetStorageEngine.ReadDirectory("Classifiers/Currency").Select(task => task.Result).ToList();
            readDirectory.Should().HaveCount(2);
            var readFile = await nuGetStorageEngine.ReadFile(@"Classifiers/Currency/ISO_4217.xml");
            readFile.Should().NotBeNull();
            readFile.Location.Should().Be(@"Classifiers/Currency/ISO_4217.xml");
            readFile.Content.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task LoadCurrenciesFromNuGet()
        {
            var storageConfiguration = new NuGetStorageConfiguration
            {
                PackageSource = "https://www.myget.org/F/micro-elements/api/v3/index.json",
                PackageId = "Classifiers.Currency",
                PackageVersion = "0.0.1",
            };

            var nuGetStorageEngine = new NuGetStorageProvider(storageConfiguration, new LoggerFactory().AddConsole());

            var dataStoreConfiguration = new DataStoreConfiguration
            {
                Storages = new[]
                {
                    new DataStorageConfiguration
                    {
                        StorageProvider = nuGetStorageEngine,
                        Collections = new[]
                        {
                            new CollectionConfigurationTyped<Currency>()
                            {
                                SourceFile = @"Classifiers\Currency\ISO_4217.xml",
                                Serializer = new CurrencyXmlSerializer(),
                                KeyGetter = new KeyAccessor<Currency>(currency1 => $"{currency1.Ccy}_{currency1.CtryNm}",
                                    (currency1, s) => { })
                            }
                        }
                    }
                }
            };

            var dataStore = new DataStore(dataStoreConfiguration);
            await dataStore.Initialize();

            var collection = dataStore.GetCollection<Currency>();
            var currency = collection.Get("EUR_SPAIN");
            currency.Should().NotBeNull();
        }
    }
}

namespace Classifiers
{
    /*
    <ISO_4217 Pblshd="2018-01-01">
	<CcyTbl>

     *<CcyNtry>
			<CtryNm>AFGHANISTAN</CtryNm>
			<CcyNm>Afghani</CcyNm>
			<Ccy>AFN</Ccy>
			<CcyNbr>971</CcyNbr>
			<CcyMnrUnts>2</CcyMnrUnts>
		</CcyNtry>
     */

    [XmlRoot("CcyNtry")]
    public class Currency
    {
        public string CtryNm { get; set; }
        public string CcyNm { get; set; }
        public string Ccy { get; set; }
        public int CcyNbr { get; set; }
        public string CcyMnrUnts { get; set; }
    }

    public class CurrencyXmlSerializer : ISerializer
    {
        /// <inheritdoc />
        public IEnumerable<object> Deserialize(FileContent content, Type type)
        {
            var xDocument = XDocument.Parse(content.Content);
            var curElements = xDocument.XPathSelectElements("//CcyNtry");
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(type);
            foreach (var curElement in curElements)
            {
                object entity;
                try
                {
                    entity = xmlSerializer.Deserialize(new StringReader(curElement.ToString()));
                }
                catch (Exception e)
                {
                    // ignore wrong currencies
                    continue;
                }
                yield return entity;
            }
        }

        /// <inheritdoc />
        public IEnumerable<T> Deserialize<T>(FileContent content)
        {
            return Deserialize(content, typeof(T)).Cast<T>();
        }

        /// <inheritdoc />
        public FileContent Serialize(IReadOnlyCollection<object> items, Type type)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public SerializerInfo GetInfo()
        {
            return new SerializerInfo
            {
                Extension = "xml"
            };
        }
    }
}
