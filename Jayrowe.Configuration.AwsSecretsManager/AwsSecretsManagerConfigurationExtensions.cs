using Amazon.SecretsManager;
using Microsoft.Extensions.Configuration;

namespace Jayrowe.Configuration.AwsSecretsManager
{
    /// <summary>
    /// Extension methods for adding <see cref="AwsSecretsManagerConfigurationSource"/>
    /// </summary>
    public static class AwsSecretsManagerConfigurationExtensions
    {
        /// <summary>
        /// Adds the <see cref="AwsSecretsManagerConfigurationSource"/> for the secret named <paramref name="secretName"/> to <paramref name="builder"/>
        /// </summary>
        /// <param name="builder">
        /// The <see cref="IConfigurationBuilder" /> to which the source is added
        /// </param>
        /// <param name="secretName">
        /// The name of the secret to include
        /// </param>
        /// <returns>
        /// The <see cref="IConfigurationBuilder"/>
        /// </returns>
        public static IConfigurationBuilder AddAwsSecretsManager(this IConfigurationBuilder builder, string secretName)
        {
            return AddAwsSecretsManager(builder, secretName, false, null);
        }

        /// <summary>
        /// Adds the <see cref="AwsSecretsManagerConfigurationSource"/> for the secret named <paramref name="secretName"/> to <paramref name="builder"/>
        /// </summary>
        /// <param name="builder">
        /// The <see cref="IConfigurationBuilder" /> to which the source is added
        /// </param>
        /// <param name="secretName">
        /// The name of the secret to include
        /// </param>
        /// <param name="optional">
        /// Whether this configuration is optional
        /// </param>
        /// <returns>
        /// The <see cref="IConfigurationBuilder"/>
        /// </returns>
        public static IConfigurationBuilder AddAwsSecretsManager(this IConfigurationBuilder builder, string secretName, bool optional)
        {
            return AddAwsSecretsManager(builder, secretName, optional, null);
        }

        /// <summary>
        /// Adds the <see cref="AwsSecretsManagerConfigurationSource"/> for the secret named <paramref name="secretName"/> to <paramref name="builder"/>
        /// </summary>
        /// <param name="builder">
        /// The <see cref="IConfigurationBuilder" /> to which the source is added
        /// </param>
        /// <param name="secretName">
        /// The name of the secret to include
        /// </param>
        /// <param name="optional">
        /// Whether this configuration is optional
        /// </param>
        /// <param name="secretsManager">
        /// <see cref="IAmazonSecretsManager"/> instance to use
        /// </param>
        /// <returns>
        /// The <see cref="IConfigurationBuilder"/>
        /// </returns>
        public static IConfigurationBuilder AddAwsSecretsManager(this IConfigurationBuilder builder, string secretName, bool optional, IAmazonSecretsManager secretsManager)
        {
            return builder.Add(new AwsSecretsManagerConfigurationSource(secretName, optional, secretsManager));
        }
    }
}
