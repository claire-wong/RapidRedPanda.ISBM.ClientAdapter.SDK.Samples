using System.Reflection;
using RapidRedPanda.ISBM.ClientAdapter;
using RapidRedPanda.ISBM.ClientAdapter.EndpointOptions;

namespace ISBM20ClientAdapterValidationRunner;

internal static class Program
{
    private const string PackageName = "RapidRedPanda.ISBM.ClientAdapter";
    private const string ExpectedPackageVersion = "2.1.1";
    private const string ExpectedAssemblyVersion = "2.1.0.0";

    private static async Task<int> Main(string[] args)
    {
        RunnerOptions options = RunnerOptions.Parse(args);

        try
        {
            ValidatePackageSurface();

            if (!string.IsNullOrWhiteSpace(options.HostAddress))
            {
                await ValidateLiveChannelManagementAsync(options);
            }
            else
            {
                Console.WriteLine("Live validation skipped. Pass --host <url> to create and clean up temporary channels and security tokens.");
            }

            Console.WriteLine("Validation complete.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static void ValidatePackageSurface()
    {
        Assembly assembly = typeof(ProviderPublicationService).Assembly;
        AssemblyName assemblyName = assembly.GetName();
        string? informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        Require(assemblyName.Name == PackageName, $"Expected {PackageName}, found {assemblyName.Name}.");
        Require(assemblyName.Version?.ToString() == ExpectedAssemblyVersion, $"Expected assembly version {ExpectedAssemblyVersion}, found {assemblyName.Version}.");
        Require(informationalVersion != null && informationalVersion.StartsWith(ExpectedPackageVersion, StringComparison.Ordinal), $"Expected package informational version {ExpectedPackageVersion}, found {informationalVersion}.");

        ValidateCredentialProperty<ProviderPublicationService>();
        ValidateCredentialProperty<ConsumerPublicationService>();
        ValidateCredentialProperty<ConsumerRequestService>();
        ValidateCredentialProperty<ProviderRequestService>();
        ValidateCredentialProperty<ChannelManagementService>();

        ValidateAsyncMethod<ProviderPublicationService>("OpenPublicationSessionAsync", typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ProviderPublicationService>("PostPublicationAsync", typeof(string), typeof(string), typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ProviderPublicationService>("PostPublicationAsync", typeof(string), typeof(string), typeof(string), typeof(string), typeof(PostPublicationOptions), typeof(CancellationToken));
        ValidateAsyncMethod<ProviderPublicationService>("ClosePublicationSessionAsync", typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ProviderPublicationService>("ExpirePublicationAsync", typeof(string), typeof(string), typeof(string), typeof(CancellationToken));

        ValidateAsyncMethod<ConsumerPublicationService>("OpenSubscriptionSessionAsync", typeof(string), typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ConsumerPublicationService>("OpenSubscriptionSessionAsync", typeof(string), typeof(string), typeof(string), typeof(OpenSubscriptionSessionOptions), typeof(CancellationToken));
        ValidateAsyncMethod<ConsumerPublicationService>("ReadPublicationAsync", typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ConsumerPublicationService>("RemovePublicationAsync", typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ConsumerPublicationService>("CloseSubscriptionSessionAsync", typeof(string), typeof(string), typeof(CancellationToken));

        ValidateAsyncMethod<ConsumerRequestService>("OpenConsumerRequestSessionAsync", typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ConsumerRequestService>("OpenConsumerRequestSessionAsync", typeof(string), typeof(string), typeof(OpenConsumerRequestSessionOptions), typeof(CancellationToken));
        ValidateAsyncMethod<ConsumerRequestService>("PostRequestAsync", typeof(string), typeof(string), typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ConsumerRequestService>("ReadResponseAsync", typeof(string), typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ConsumerRequestService>("RemoveResponseAsync", typeof(string), typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ConsumerRequestService>("ExpireRequestAsync", typeof(string), typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ConsumerRequestService>("CloseConsumerRequestSessionAsync", typeof(string), typeof(string), typeof(CancellationToken));

        ValidateAsyncMethod<ProviderRequestService>("OpenProviderRequestSessionAsync", typeof(string), typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ProviderRequestService>("OpenProviderRequestSessionAsync", typeof(string), typeof(string), typeof(string), typeof(OpenProviderRequestSessionOptions), typeof(CancellationToken));
        ValidateAsyncMethod<ProviderRequestService>("ReadRequestAsync", typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ProviderRequestService>("PostResponseAsync", typeof(string), typeof(string), typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ProviderRequestService>("RemoveRequestAsync", typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ProviderRequestService>("CloseProviderRequestSessionAsync", typeof(string), typeof(string), typeof(CancellationToken));

        ValidateAsyncMethod<ChannelManagementService>("GetChannelsAsync", typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ChannelManagementService>("GetChannelAsync", typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ChannelManagementService>("CreateChannelAsync", typeof(string), typeof(string), typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ChannelManagementService>("CreateChannelAsync", typeof(string), typeof(string), typeof(string), typeof(string), typeof(CreateChannelOptions), typeof(CancellationToken));
        ValidateAsyncMethod<ChannelManagementService>("AddSecurityTokensAsync", typeof(string), typeof(string), typeof(AddSecurityTokensOptions), typeof(CancellationToken));
        ValidateAsyncMethod<ChannelManagementService>("RemoveSecurityTokensAsync", typeof(string), typeof(string), typeof(RemoveSecurityTokensOptions), typeof(CancellationToken));
        ValidateAsyncMethod<ChannelManagementService>("DeleteChannelAsync", typeof(string), typeof(string), typeof(CancellationToken));

        ValidateOptions();

        Console.WriteLine($"{PackageName} package {informationalVersion} assembly version {assemblyName.Version} surface validated.");
    }

    private static async Task ValidateLiveChannelManagementAsync(RunnerOptions options)
    {
        using CancellationTokenSource timeout = new CancellationTokenSource(options.Timeout);
        ChannelManagementService service = new ChannelManagementService();
        service.Credential.Username = options.Username;
        service.Credential.Password = options.Password;

        string suffix = Guid.NewGuid().ToString("N")[..12];
        string publicationChannelId = $"validation-publication-{suffix}";
        string requestChannelId = $"validation-request-{suffix}";
        string userToken = $"validation-user-{suffix}";
        string passwordToken = Guid.NewGuid().ToString("N");
        List<string> createdChannels = new List<string>();

        try
        {
            CreateChannelOptions createOptions = new CreateChannelOptions();
            createOptions.SecurityTokens.Add(new CreateChannelOptions.SecurityToken
            {
                type = "UsernameToken",
                username = userToken,
                password = passwordToken
            });

            dynamic publicationCreate = await service.CreateChannelAsync(options.HostAddress!, publicationChannelId, "Publication", "ClientAdapter 2.1.1 validation publication channel", createOptions, timeout.Token);
            RequireSuccess(publicationCreate.StatusCode, $"create publication channel {publicationChannelId}");
            createdChannels.Add(publicationChannelId);

            dynamic requestCreate = await service.CreateChannelAsync(options.HostAddress!, requestChannelId, "Request", "ClientAdapter 2.1.1 validation request channel", timeout.Token);
            RequireSuccess(requestCreate.StatusCode, $"create request channel {requestChannelId}");
            createdChannels.Add(requestChannelId);

            AddSecurityTokensOptions addTokens = new AddSecurityTokensOptions();
            addTokens.SecurityTokens.Add(new AddSecurityTokensOptions.SecurityToken
            {
                type = "UsernameToken",
                username = $"{userToken}-added",
                password = Guid.NewGuid().ToString("N")
            });

            dynamic addResponse = await service.AddSecurityTokensAsync(options.HostAddress!, requestChannelId, addTokens, timeout.Token);
            RequireSuccess(addResponse.StatusCode, $"add security token to {requestChannelId}");

            RemoveSecurityTokensOptions removeTokens = new RemoveSecurityTokensOptions();
            removeTokens.SecurityTokens.Add(new RemoveSecurityTokensOptions.SecurityToken
            {
                type = "UsernameToken",
                username = $"{userToken}-added",
                password = string.Empty
            });

            dynamic removeResponse = await service.RemoveSecurityTokensAsync(options.HostAddress!, requestChannelId, removeTokens, timeout.Token);
            RequireSuccess(removeResponse.StatusCode, $"remove security token from {requestChannelId}");

            Console.WriteLine("Live channel and token validation passed.");
        }
        finally
        {
            foreach (string channelId in createdChannels)
            {
                dynamic deleteResponse = await service.DeleteChannelAsync(options.HostAddress!, channelId, CancellationToken.None);
                Console.WriteLine($"Cleanup delete {channelId}: HTTP {deleteResponse.StatusCode}");
            }
        }
    }

    private static void ValidateCredentialProperty<TService>()
    {
        Require(typeof(TService).GetMember("Credential", BindingFlags.Public | BindingFlags.Instance).Length == 1, $"{typeof(TService).Name} does not expose Credential.");
        Require(typeof(TService).GetMember("Credentials", BindingFlags.Public | BindingFlags.Instance).Length == 0, $"{typeof(TService).Name} still exposes obsolete Credentials.");
    }

    private static void ValidateAsyncMethod<TService>(string name, params Type[] parameterTypes)
    {
        MethodInfo? method = typeof(TService).GetMethod(name, parameterTypes);
        if (method == null)
        {
            throw new InvalidOperationException($"{typeof(TService).Name}.{name}({string.Join(", ", parameterTypes.Select(t => t.Name))}) was not found.");
        }

        Require(method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>), $"{typeof(TService).Name}.{name} does not return Task<T>.");
    }

    private static void ValidateOptions()
    {
        Require(typeof(OpenSubscriptionSessionOptions).GetField("ListenerURL") != null, "OpenSubscriptionSessionOptions.ListenerURL was not found.");
        Require(typeof(OpenProviderRequestSessionOptions).GetField("ListenerURL") != null, "OpenProviderRequestSessionOptions.ListenerURL was not found.");
        Require(typeof(OpenConsumerRequestSessionOptions).GetField("ListenerURL") != null, "OpenConsumerRequestSessionOptions.ListenerURL was not found.");
        Require(typeof(PostPublicationOptions).GetField("Expiry") != null, "PostPublicationOptions.Expiry was not found.");
        Require(typeof(PostPublicationOptions).GetField("MediaType") != null, "PostPublicationOptions.MediaType was not found.");
        Require(typeof(PostRequestOptions).GetField("Expiry") != null, "PostRequestOptions.Expiry was not found.");
        Require(typeof(PostRequestOptions).GetField("MediaType") != null, "PostRequestOptions.MediaType was not found.");
        Require(typeof(PostResponseOptions).GetField("MediaType") != null, "PostResponseOptions.MediaType was not found.");
        Require(typeof(CreateChannelOptions).GetField("SecurityTokens") != null, "CreateChannelOptions.SecurityTokens was not found.");
        Require(typeof(AddSecurityTokensOptions).GetField("SecurityTokens") != null, "AddSecurityTokensOptions.SecurityTokens was not found.");
        Require(typeof(RemoveSecurityTokensOptions).GetField("SecurityTokens") != null, "RemoveSecurityTokensOptions.SecurityTokens was not found.");
    }

    private static void RequireSuccess(int statusCode, string operation)
    {
        Require(statusCode >= 200 && statusCode < 300, $"Expected success while trying to {operation}; received HTTP {statusCode}.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private sealed class RunnerOptions
    {
        public string? HostAddress { get; private set; }
        public string Username { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public TimeSpan Timeout { get; private set; } = TimeSpan.FromSeconds(30);

        public static RunnerOptions Parse(string[] args)
        {
            RunnerOptions options = new RunnerOptions();

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                string ReadValue()
                {
                    if (i + 1 >= args.Length)
                    {
                        throw new ArgumentException($"{arg} requires a value.");
                    }

                    i++;
                    return args[i];
                }

                switch (arg)
                {
                    case "--host":
                        options.HostAddress = ReadValue();
                        break;
                    case "--username":
                        options.Username = ReadValue();
                        break;
                    case "--password":
                        options.Password = ReadValue();
                        break;
                    case "--timeout-seconds":
                        options.Timeout = TimeSpan.FromSeconds(int.Parse(ReadValue()));
                        break;
                    default:
                        throw new ArgumentException($"Unknown argument: {arg}");
                }
            }

            return options;
        }
    }
}
