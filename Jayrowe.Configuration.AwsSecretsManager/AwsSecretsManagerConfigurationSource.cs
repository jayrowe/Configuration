using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Text;

namespace Jayrowe.Configuration.AwsSecretsManager
{
    /// <summary>
    /// Configuration source that uses json stored in AWS Secrets Manager
    /// </summary>
    public class AwsSecretsManagerConfigurationSource : IConfigurationSource
    {
        private readonly string _secretName;
        private readonly bool _optional;
        private IAmazonSecretsManager _secretsManager;

        /// <summary>
        /// Creates an instance that attempts to read from the given secret name, using a provided instance of <see cref="IAmazonSecretsManager"/> or
        /// a default instance if none is provided
        /// </summary>
        /// <param name="secretName">
        /// Name of the secret to read
        /// </param>
        /// <param name="optional">
        /// Whether this configuration is optional
        /// </param>
        /// <param name="secretsManager">
        /// <see cref="IAmazonSecretsManager"/> instance to use
        /// </param>
        public AwsSecretsManagerConfigurationSource(string secretName, bool optional, IAmazonSecretsManager secretsManager)
        {
            _secretName = secretName ?? throw new ArgumentNullException(nameof(secretName), "secretName cannot be null");
            _optional = optional;
            _secretsManager = secretsManager ?? new AmazonSecretsManagerClient();
        }

        /// <summary>
        /// Builds the <see cref="IConfigurationProvider" /> instance for this source
        /// </summary>
        /// <param name="builder">
        /// <see cref="IConfigurationBuilder"/> instance
        /// </param>
        /// <returns>
        /// An <see cref="IConfigurationProvider"/> instance
        /// </returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            var json = new JsonConfigurationSource
            {
                FileProvider = new AwsSecretsManagerFileProvider(_secretsManager),
                Optional = _optional,
                Path = _secretName
            };

            return json.Build(builder);
        }

        private class AwsSecretsManagerFileProvider : IFileProvider
        {
            private readonly IAmazonSecretsManager _secretsManager;

            public AwsSecretsManagerFileProvider(IAmazonSecretsManager secretsManager)
            {
                _secretsManager = secretsManager;
            }

            public IDirectoryContents GetDirectoryContents(string subpath)
            {
                return new NotFoundDirectoryContents();
            }

            public IFileInfo GetFileInfo(string subpath)
            {
                var request = new GetSecretValueRequest();
                request.SecretId = subpath;
                request.VersionStage = "AWSCURRENT";

                try
                {
                    return new AwsSecretsManagerFileInfo(_secretsManager.GetSecretValueAsync(request).GetAwaiter().GetResult());
                }
                catch (ResourceNotFoundException)
                {
                    return new NotFoundFileInfo(subpath);
                }
            }

            public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
        }

        private class AwsSecretsManagerFileInfo : IFileInfo
        {
            private readonly GetSecretValueResponse _response;
            private byte[] _bytes;

            public AwsSecretsManagerFileInfo(GetSecretValueResponse response)
            {
                _response = response;
            }

            public bool Exists => true;

            public long Length
            {
                get
                {
                    EnsureBytes();
                    return _bytes.Length;
                }
            }

            public string PhysicalPath => null;

            public string Name => _response.Name;

            public DateTimeOffset LastModified => _response.CreatedDate;

            public bool IsDirectory => false;

            public Stream CreateReadStream()
            {
                EnsureBytes();
                return new MemoryStream(_bytes);
            }

            private void EnsureBytes()
            {
                if (_response.SecretString != null)
                {
                    _bytes = Encoding.UTF8.GetBytes(_response.SecretString);
                }
                else
                {
                    _bytes = _response.SecretBinary.ToArray();
                }
            }
        }
    }


}