using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Jayrowe.Configuration.AwsSecretsManager.Tests
{
    [TestClass]
    public class AwsSecretsManagerConfigurationSourceTests
    {
        [TestMethod]
        public void PathNotExists_NotOptional()
        {
            var secretsManager = new Mock<IAmazonSecretsManager>();

            var secretName = "PathNotExists_NotOptional";

            secretsManager.Setup(
                m => m.GetSecretValueAsync(
                    It.Is<GetSecretValueRequest>(r => r.VersionStage == "AWSCURRENT" && r.VersionId == null && r.SecretId == secretName),
                    It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new ResourceNotFoundException(""));

            var source = new AwsSecretsManagerConfigurationSource(
                secretName,
                false,
                secretsManager.Object);

            var builder = new ConfigurationBuilder();
            builder.Add(source);

            Assert.ThrowsException<FileNotFoundException>(() => builder.Build());
        }

        [TestMethod]
        public void PathNotExists_Optional()
        {
            var secretsManager = new Mock<IAmazonSecretsManager>();

            var secretName = "PathNotExists_Optional";

            secretsManager.Setup(
                m => m.GetSecretValueAsync(
                    It.Is<GetSecretValueRequest>(r => r.VersionStage == "AWSCURRENT" && r.VersionId == null && r.SecretId == secretName),
                    It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new ResourceNotFoundException(""));

            var source = new AwsSecretsManagerConfigurationSource(
                secretName,
                true,
                secretsManager.Object);

            var builder = new ConfigurationBuilder();
            builder.Add(source);

            var configuration = builder.Build();

            Assert.AreEqual(0, configuration.AsEnumerable().Count());
            Assert.AreEqual(0, configuration.GetChildren().Count());
        }

        [TestMethod]
        public void PathExists_NotOptional()
        {
            var secretsManager = new Mock<IAmazonSecretsManager>();

            var secretName = "PathExists_NotOptional";
            var result = @"{""key"":""value"",""section"":{""subkey"":""subvalue""}}";

            secretsManager.Setup(
                m => m.GetSecretValueAsync(
                    It.Is<GetSecretValueRequest>(r => r.VersionStage == "AWSCURRENT" && r.VersionId == null && r.SecretId == secretName),
                    It.IsAny<CancellationToken>()))
                    .ReturnsAsync(
                    new GetSecretValueResponse
                    {
                        SecretString = result
                    });

            var source = new AwsSecretsManagerConfigurationSource(
                secretName,
                false,
                secretsManager.Object);

            var builder = new ConfigurationBuilder();
            builder.Add(source);

            var configuration = builder.Build();

            Assert.AreEqual(3, configuration.AsEnumerable().Count());
            Assert.AreEqual(2, configuration.GetChildren().Count());

            Assert.AreEqual("value", configuration["key"]);
            Assert.AreEqual("subvalue", configuration.GetSection("section")["subkey"]);
        }

        [TestMethod]
        public void PathExists_Optional()
        {
            var secretsManager = new Mock<IAmazonSecretsManager>();

            var secretName = "PathExists_Optional";
            var result = @"{""key"":""value"",""section"":{""subkey"":""subvalue""}}";

            secretsManager.Setup(
                m => m.GetSecretValueAsync(
                    It.Is<GetSecretValueRequest>(r => r.VersionStage == "AWSCURRENT" && r.VersionId == null && r.SecretId == secretName),
                    It.IsAny<CancellationToken>()))
                    .ReturnsAsync(
                    new GetSecretValueResponse
                    {
                        SecretString = result
                    });

            var source = new AwsSecretsManagerConfigurationSource(
                secretName,
                true,
                secretsManager.Object);

            var builder = new ConfigurationBuilder();
            builder.Add(source);

            var configuration = builder.Build();

            Assert.AreEqual(3, configuration.AsEnumerable().Count());
            Assert.AreEqual(2, configuration.GetChildren().Count());

            Assert.AreEqual("value", configuration["key"]);
            Assert.AreEqual("subvalue", configuration.GetSection("section")["subkey"]);
        }

        [TestMethod]
        public void PathExists_BinaryContent()
        {
            var secretsManager = new Mock<IAmazonSecretsManager>();

            var secretName = "PathExists_Optional";
            var result = Encoding.UTF8.GetBytes(@"{""key"":""value"",""section"":{""subkey"":""subvalue""}}");

            secretsManager.Setup(
                m => m.GetSecretValueAsync(
                    It.Is<GetSecretValueRequest>(r => r.VersionStage == "AWSCURRENT" && r.VersionId == null && r.SecretId == secretName),
                    It.IsAny<CancellationToken>()))
                    .ReturnsAsync(
                    new GetSecretValueResponse
                    {
                        SecretBinary = new MemoryStream(result)
                    });

            var source = new AwsSecretsManagerConfigurationSource(
                secretName,
                true,
                secretsManager.Object);

            var builder = new ConfigurationBuilder();
            builder.Add(source);

            var configuration = builder.Build();

            Assert.AreEqual(3, configuration.AsEnumerable().Count());
            Assert.AreEqual(2, configuration.GetChildren().Count());

            Assert.AreEqual("value", configuration["key"]);
            Assert.AreEqual("subvalue", configuration.GetSection("section")["subkey"]);
        }

        [TestMethod]
        public void ThrowsException_NotOptional()
        {
            var secretsManager = new Mock<IAmazonSecretsManager>();

            var secretName = "ThrowsException_NotOptional";

            secretsManager.Setup(
                m => m.GetSecretValueAsync(
                    It.Is<GetSecretValueRequest>(r => r.VersionStage == "AWSCURRENT" && r.VersionId == null && r.SecretId == secretName),
                    It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new Exception(""));

            var source = new AwsSecretsManagerConfigurationSource(
                secretName,
                false,
                secretsManager.Object);

            var builder = new ConfigurationBuilder();
            builder.Add(source);

            Assert.ThrowsException<Exception>(() => builder.Build());
        }

        [TestMethod]
        public void ThrowsException_Optional()
        {
            var secretsManager = new Mock<IAmazonSecretsManager>();

            var secretName = "ThrowsException_Optional";

            secretsManager.Setup(
                m => m.GetSecretValueAsync(
                    It.Is<GetSecretValueRequest>(r => r.VersionStage == "AWSCURRENT" && r.VersionId == null && r.SecretId == secretName),
                    It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new Exception(""));

            var source = new AwsSecretsManagerConfigurationSource(
                secretName,
                true,
                secretsManager.Object);

            var builder = new ConfigurationBuilder();
            builder.Add(source);

            Assert.ThrowsException<Exception>(() => builder.Build());
        }
    }
}
