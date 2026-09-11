using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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
        ValidationReport report = new ValidationReport(options.Verbose);

        try
        {
            ValidatePackageSurface();

            if (!string.IsNullOrWhiteSpace(options.HostAddress))
            {
                await ValidateLiveAsync(options, report);
            }
            else
            {
                report.Skip("live", "host", "Live validation skipped. Pass --host <url>.");
            }
        }
        catch (Exception ex)
        {
            report.Fail("runner", "unhandled exception", ex);
        }

        report.PrintSummary();
        return report.HasFailures ? 1 : 0;
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

        ValidateCredentialMember<ProviderPublicationService>();
        ValidateCredentialMember<ConsumerPublicationService>();
        ValidateCredentialMember<ConsumerRequestService>();
        ValidateCredentialMember<ProviderRequestService>();
        ValidateCredentialMember<ChannelManagementService>();

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
        ValidateAsyncMethod<ConsumerRequestService>("PostRequestAsync", typeof(string), typeof(string), typeof(string), typeof(string), typeof(PostRequestOptions), typeof(CancellationToken));
        ValidateAsyncMethod<ConsumerRequestService>("ReadResponseAsync", typeof(string), typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ConsumerRequestService>("RemoveResponseAsync", typeof(string), typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ConsumerRequestService>("ExpireRequestAsync", typeof(string), typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ConsumerRequestService>("CloseConsumerRequestSessionAsync", typeof(string), typeof(string), typeof(CancellationToken));

        ValidateAsyncMethod<ProviderRequestService>("OpenProviderRequestSessionAsync", typeof(string), typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ProviderRequestService>("OpenProviderRequestSessionAsync", typeof(string), typeof(string), typeof(string), typeof(OpenProviderRequestSessionOptions), typeof(CancellationToken));
        ValidateAsyncMethod<ProviderRequestService>("ReadRequestAsync", typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ProviderRequestService>("PostResponseAsync", typeof(string), typeof(string), typeof(string), typeof(string), typeof(CancellationToken));
        ValidateAsyncMethod<ProviderRequestService>("PostResponseAsync", typeof(string), typeof(string), typeof(string), typeof(string), typeof(PostResponseOptions), typeof(CancellationToken));
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

    private static async Task ValidateLiveAsync(RunnerOptions options, ValidationReport report)
    {
        bool channelOnly = string.Equals(options.Scenario, "channel", StringComparison.OrdinalIgnoreCase);
        if (!channelOnly && !string.Equals(options.Scenario, "all", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"Unsupported scenario '{options.Scenario}'. Supported values: all, channel.");
        }

        using CancellationTokenSource timeout = new CancellationTokenSource(options.Timeout);
        string host = options.HostAddress!;
        string suffix = Guid.NewGuid().ToString("N").Substring(0, 12);
        string publicationChannelId = $"validation-publication-{suffix}";
        string requestChannelId = $"validation-request-{suffix}";
        string topic = $"validation.topic.{suffix}";
        string username = $"validation-user-{suffix}";
        string password = Guid.NewGuid().ToString("N");
        string addedUsername = $"validation-added-{suffix}";
        string addedPassword = Guid.NewGuid().ToString("N");
        List<string> createdChannels = new List<string>();
        List<Func<Task>> cleanupActions = new List<Func<Task>>();

        ChannelManagementService management = new ChannelManagementService();
        SetCredential(management, options.Username, options.Password);

        ProviderPublicationService providerPublication = new ProviderPublicationService();
        ConsumerPublicationService consumerPublication = new ConsumerPublicationService();
        ConsumerRequestService consumerRequest = new ConsumerRequestService();
        ProviderRequestService providerRequest = new ProviderRequestService();
        SetCredential(providerPublication, username, password);
        SetCredential(consumerPublication, username, password);
        SetCredential(consumerRequest, username, password);
        SetCredential(providerRequest, username, password);

        string? publicationSessionId = null;
        string? subscriptionSessionId = null;
        string? consumerRequestSessionId = null;
        string? providerRequestSessionId = null;

        try
        {
            CreateChannelOptions protectedOptions = new CreateChannelOptions();
            protectedOptions.SecurityTokens.Add(new CreateChannelOptions.SecurityToken
            {
                type = "UsernameToken",
                username = username,
                password = password
            });

            object getChannelsBeforeCreate = await management.GetChannelsAsync(host, timeout.Token);
            ExpectSuccess(report, "channel retrieval/listing", "list channels before create", getChannelsBeforeCreate, 200);

            object publicationCreate = await management.CreateChannelAsync(host, publicationChannelId, "Publication", "ClientAdapter 2.1.1 validation publication channel", protectedOptions, timeout.Token);
            ExpectSuccess(report, "channel creation", "create protected publication channel", publicationCreate, 201);
            if (IsSuccess(publicationCreate))
            {
                createdChannels.Add(publicationChannelId);
            }

            object requestCreate = await management.CreateChannelAsync(host, requestChannelId, "Request", "ClientAdapter 2.1.1 validation request channel", protectedOptions, timeout.Token);
            ExpectSuccess(report, "channel creation", "create protected request channel", requestCreate, 201);
            if (IsSuccess(requestCreate))
            {
                createdChannels.Add(requestChannelId);
            }

            if (createdChannels.Count != 2)
            {
                report.Block("dependent workflows", "create temporary channels", "Channel creation did not complete; skipping dependent live workflows.");
                return;
            }

            object getPublication = await management.GetChannelAsync(host, publicationChannelId, timeout.Token);
            ExpectSuccess(report, "channel retrieval/listing", "get publication channel", getPublication, 200);

            object getChannels = await management.GetChannelsAsync(host, timeout.Token);
            ExpectSuccess(report, "channel retrieval/listing", "list channels", getChannels, 200);

            if (channelOnly)
            {
                return;
            }

            AddSecurityTokensOptions addTokens = new AddSecurityTokensOptions();
            addTokens.SecurityTokens.Add(new AddSecurityTokensOptions.SecurityToken
            {
                type = "UsernameToken",
                username = addedUsername,
                password = addedPassword
            });
            object addToken = await management.AddSecurityTokensAsync(host, requestChannelId, addTokens, timeout.Token);
            ExpectSuccess(report, "security-token add/remove", "add generated security token", addToken, 201, 204);

            RemoveSecurityTokensOptions removeTokens = new RemoveSecurityTokensOptions();
            removeTokens.SecurityTokens.Add(new RemoveSecurityTokensOptions.SecurityToken
            {
                type = "UsernameToken",
                username = addedUsername,
                password = addedPassword
            });
            object removeToken = await management.RemoveSecurityTokensAsync(host, requestChannelId, removeTokens, timeout.Token);
            ExpectSuccess(report, "security-token add/remove", "remove generated security token", removeToken, 204);

            ProviderPublicationService badProviderPublication = new ProviderPublicationService();
            SetCredential(badProviderPublication, $"{username}-wrong", $"{password}-wrong");
            object badOpenPublication = await badProviderPublication.OpenPublicationSessionAsync(host, publicationChannelId, timeout.Token);
            ExpectFailure(report, "auth success/failure checks", "reject wrong publication credentials", badOpenPublication);

            OpenSubscriptionSessionOptions listenerOptions = new OpenSubscriptionSessionOptions();
            listenerOptions.ListenerURL = "http://127.0.0.1:9/validation-listener";
            object listenerOpen = await consumerPublication.OpenSubscriptionSessionAsync(host, publicationChannelId, topic, listenerOptions, timeout.Token);
            ExpectSuccess(report, "listenerUrl", "accept listenerUrl option", listenerOpen, 201);
            string? listenerSessionId = GetString(listenerOpen, "SessionID");
            if (!string.IsNullOrWhiteSpace(listenerSessionId))
            {
                cleanupActions.Add(async () => await consumerPublication.CloseSubscriptionSessionAsync(host, listenerSessionId, CancellationToken.None));
            }

            await ValidateFilterExpressionAcceptanceAsync(report, cleanupActions, consumerPublication, host, publicationChannelId, topic, timeout.Token);

            object openSubscription = await consumerPublication.OpenSubscriptionSessionAsync(host, publicationChannelId, topic, timeout.Token);
            ExpectSuccess(report, "subscription workflow", "open subscription session", openSubscription, 201);
            subscriptionSessionId = GetString(openSubscription, "SessionID");

            object openPublication = await providerPublication.OpenPublicationSessionAsync(host, publicationChannelId, timeout.Token);
            ExpectSuccess(report, "publication/session workflow", "open publication session", openPublication, 201);
            publicationSessionId = GetString(openPublication, "SessionID");

            if (!string.IsNullOrWhiteSpace(publicationSessionId))
            {
                PostPublicationOptions postPublicationOptions = new PostPublicationOptions
                {
                    Expiry = "PT5M",
                    MediaType = "application/json"
                };
                object postPublication = await providerPublication.PostPublicationAsync(host, publicationSessionId, topic, JsonPayload("publication", suffix), postPublicationOptions, timeout.Token);
                ExpectSuccess(report, "PostPublication options", "post publication with expiry and mediaType", postPublication, 201);
                ExpectSuccess(report, "expiry", "accept PostPublication expiry option", postPublication, 201);
                ExpectSuccess(report, "MediaType payload modes", "post publication JSON media type", postPublication, 201);

                PostPublicationOptions textPublicationOptions = new PostPublicationOptions
                {
                    MediaType = "text/plain"
                };
                object postTextPublication = await providerPublication.PostPublicationAsync(host, publicationSessionId, topic, $"validation text publication {suffix}", textPublicationOptions, timeout.Token);
                ExpectSuccess(report, "MediaType payload modes", "post publication text media type", postTextPublication, 201);
            }

            if (!string.IsNullOrWhiteSpace(subscriptionSessionId))
            {
                object readPublication = await consumerPublication.ReadPublicationAsync(host, subscriptionSessionId, timeout.Token);
                ExpectSuccess(report, "subscription workflow", "read publication", readPublication, 200);

                object removePublication = await consumerPublication.RemovePublicationAsync(host, subscriptionSessionId, timeout.Token);
                ExpectSuccess(report, "subscription workflow", "remove publication", removePublication, 204);
            }

            object openProviderRequest = await providerRequest.OpenProviderRequestSessionAsync(host, requestChannelId, topic, timeout.Token);
            ExpectSuccess(report, "provider request workflow", "open provider request session", openProviderRequest, 201);
            providerRequestSessionId = GetString(openProviderRequest, "SessionID");

            object openConsumerRequest = await consumerRequest.OpenConsumerRequestSessionAsync(host, requestChannelId, timeout.Token);
            ExpectSuccess(report, "consumer request workflow", "open consumer request session", openConsumerRequest, 201);
            consumerRequestSessionId = GetString(openConsumerRequest, "SessionID");

            string? requestMessageId = null;
            if (!string.IsNullOrWhiteSpace(consumerRequestSessionId))
            {
                PostRequestOptions postRequestOptions = new PostRequestOptions
                {
                    Expiry = "PT5M",
                    MediaType = "application/json"
                };
                object postRequest = await consumerRequest.PostRequestAsync(host, consumerRequestSessionId, topic, JsonPayload("request", suffix), postRequestOptions, timeout.Token);
                ExpectSuccess(report, "PostRequest options", "post request with expiry and mediaType", postRequest, 201);
                ExpectSuccess(report, "expiry", "accept PostRequest expiry option", postRequest, 201);
                ExpectSuccess(report, "MediaType payload modes", "post request JSON media type", postRequest, 201);
                requestMessageId = GetString(postRequest, "MessageID");
            }

            if (!string.IsNullOrWhiteSpace(providerRequestSessionId))
            {
                object readRequest = await providerRequest.ReadRequestAsync(host, providerRequestSessionId, timeout.Token);
                ExpectSuccess(report, "provider request workflow", "read request", readRequest, 200);
                requestMessageId ??= GetString(readRequest, "MessageID");
            }

            if (!string.IsNullOrWhiteSpace(providerRequestSessionId) && !string.IsNullOrWhiteSpace(requestMessageId))
            {
                PostResponseOptions postResponseOptions = new PostResponseOptions
                {
                    MediaType = "application/json"
                };
                object postResponse = await providerRequest.PostResponseAsync(host, providerRequestSessionId, requestMessageId, JsonPayload("response", suffix), postResponseOptions, timeout.Token);
                ExpectSuccess(report, "PostResponse options", "post response with mediaType", postResponse, 201);
                ExpectSuccess(report, "MediaType payload modes", "post response JSON media type", postResponse, 201);
            }

            if (!string.IsNullOrWhiteSpace(consumerRequestSessionId) && !string.IsNullOrWhiteSpace(requestMessageId))
            {
                object readResponse = await consumerRequest.ReadResponseAsync(host, consumerRequestSessionId, requestMessageId, timeout.Token);
                ExpectSuccess(report, "consumer request workflow", "read response", readResponse, 200);

                object removeResponse = await consumerRequest.RemoveResponseAsync(host, consumerRequestSessionId, requestMessageId, timeout.Token);
                ExpectSuccess(report, "consumer request workflow", "remove response", removeResponse, 204);
            }

            if (!string.IsNullOrWhiteSpace(providerRequestSessionId))
            {
                object removeRequest = await providerRequest.RemoveRequestAsync(host, providerRequestSessionId, timeout.Token);
                ExpectSuccess(report, "provider request workflow", "remove request", removeRequest, 204);
            }

            if (!string.IsNullOrWhiteSpace(publicationSessionId))
            {
                object closePublication = await providerPublication.ClosePublicationSessionAsync(host, publicationSessionId, timeout.Token);
                ExpectSuccess(report, "publication/session workflow", "close publication session", closePublication, 204);
                publicationSessionId = null;
            }

            if (!string.IsNullOrWhiteSpace(subscriptionSessionId))
            {
                object closeSubscription = await consumerPublication.CloseSubscriptionSessionAsync(host, subscriptionSessionId, timeout.Token);
                ExpectSuccess(report, "subscription workflow", "close subscription session", closeSubscription, 204);
                subscriptionSessionId = null;
            }

            if (!string.IsNullOrWhiteSpace(consumerRequestSessionId))
            {
                object closeConsumerRequest = await consumerRequest.CloseConsumerRequestSessionAsync(host, consumerRequestSessionId, timeout.Token);
                ExpectSuccess(report, "consumer request workflow", "close consumer request session", closeConsumerRequest, 204);
                consumerRequestSessionId = null;
            }

            if (!string.IsNullOrWhiteSpace(providerRequestSessionId))
            {
                object closeProviderRequest = await providerRequest.CloseProviderRequestSessionAsync(host, providerRequestSessionId, timeout.Token);
                ExpectSuccess(report, "provider request workflow", "close provider request session", closeProviderRequest, 204);
                providerRequestSessionId = null;
            }
        }
        finally
        {
            foreach (Func<Task> cleanupAction in cleanupActions)
            {
                await RunCleanup(report, "temporary subscription session", cleanupAction);
            }

            if (!string.IsNullOrWhiteSpace(publicationSessionId))
            {
                await RunCleanup(report, "publication session", async () => await providerPublication.ClosePublicationSessionAsync(host, publicationSessionId, CancellationToken.None));
            }

            if (!string.IsNullOrWhiteSpace(subscriptionSessionId))
            {
                await RunCleanup(report, "subscription session", async () => await consumerPublication.CloseSubscriptionSessionAsync(host, subscriptionSessionId, CancellationToken.None));
            }

            if (!string.IsNullOrWhiteSpace(consumerRequestSessionId))
            {
                await RunCleanup(report, "consumer request session", async () => await consumerRequest.CloseConsumerRequestSessionAsync(host, consumerRequestSessionId, CancellationToken.None));
            }

            if (!string.IsNullOrWhiteSpace(providerRequestSessionId))
            {
                await RunCleanup(report, "provider request session", async () => await providerRequest.CloseProviderRequestSessionAsync(host, providerRequestSessionId, CancellationToken.None));
            }

            foreach (string channelId in createdChannels.AsEnumerable().Reverse())
            {
                await RunCleanup(report, channelId, async () => await management.DeleteChannelAsync(host, channelId, CancellationToken.None));
            }
        }
    }

    private static async Task RunCleanup(ValidationReport report, string resource, Func<Task> cleanup)
    {
        try
        {
            await cleanup();
            report.Pass("cleanup", $"remove {resource}", null);
        }
        catch (Exception ex)
        {
            report.Fail("cleanup", $"remove {resource}", ex);
        }
    }

    private static async Task ValidateFilterExpressionAcceptanceAsync(
        ValidationReport report,
        List<Func<Task>> cleanupActions,
        ConsumerPublicationService consumerPublication,
        string host,
        string publicationChannelId,
        string topic,
        CancellationToken cancellationToken)
    {
        List<FilterCandidate> candidates = new List<FilterCandidate>
        {
            new FilterCandidate("XPath media filter", "/*", "XPath", "1.0", "application/json"),
            new FilterCandidate("XPath lowercase media filter", "/*", "xpath", "1.0", "application/json"),
            new FilterCandidate("XPath no media filter", "/*", "XPath", "1.0", null),
            new FilterCandidate("XPath descendant media filter", "//*", "XPath", "1.0", "application/json"),
            new FilterCandidate("XPath standard-uri media filter", "/*", "http://www.w3.org/TR/1999/REC-xpath-19991116", "1.0", "application/json")
        };

        object? lastResponse = null;
        foreach (FilterCandidate candidate in candidates)
        {
            OpenSubscriptionSessionOptions filterOptions = new OpenSubscriptionSessionOptions();
            FilterExpression filterExpression = new FilterExpression
            {
                ExpressionString = new ExpressionString
                {
                    Expression = candidate.Expression,
                    Language = candidate.Language,
                    LanguageVersion = candidate.LanguageVersion
                }
            };

            if (!string.IsNullOrWhiteSpace(candidate.MediaType))
            {
                filterExpression.ApplicableMediaTypes.Add(new ApplicableMediaType
                {
                    MediaType = candidate.MediaType
                });
            }

            filterOptions.FilterExpressions.Add(filterExpression);
            object response = await consumerPublication.OpenSubscriptionSessionAsync(host, publicationChannelId, topic, filterOptions, cancellationToken);
            lastResponse = response;

            if (GetInt(response, "StatusCode") == 201)
            {
                report.Pass("filterExpression", $"accept filterExpression option ({candidate.Name})", response);
                string? filterSessionId = GetString(response, "SessionID");
                if (!string.IsNullOrWhiteSpace(filterSessionId))
                {
                    cleanupActions.Add(async () => await consumerPublication.CloseSubscriptionSessionAsync(host, filterSessionId, CancellationToken.None));
                }

                return;
            }
        }

        if (lastResponse != null)
        {
            report.Fail("filterExpression", "accept filterExpression option", lastResponse, "Host rejected all filterExpression candidates.");
        }
        else
        {
            report.Block("filterExpression", "accept filterExpression option", "No filterExpression candidates were attempted.");
        }
    }

    private static void SetCredential(object service, string username, string password)
    {
        object credential = GetRequiredMemberValue(service, "Credential");
        SetRequiredMemberValue(credential, "Username", username);
        SetRequiredMemberValue(credential, "Password", password);
    }

    private static void ExpectSuccess(ValidationReport report, string scenario, string operation, object response, params int[] expectedStatusCodes)
    {
        int statusCode = GetInt(response, "StatusCode");
        if (expectedStatusCodes.Contains(statusCode))
        {
            report.Pass(scenario, operation, response);
            return;
        }

        report.Fail(scenario, operation, response, $"Expected HTTP {string.Join(" or ", expectedStatusCodes)}, received HTTP {statusCode}.");
    }

    private static void ExpectFailure(ValidationReport report, string scenario, string operation, object response)
    {
        int statusCode = GetInt(response, "StatusCode");
        if (statusCode >= 400)
        {
            report.Pass(scenario, operation, response);
            return;
        }

        report.Fail(scenario, operation, response, $"Expected authentication failure, received HTTP {statusCode}.");
    }

    private static bool IsSuccess(object response)
    {
        int statusCode = GetInt(response, "StatusCode");
        return statusCode >= 200 && statusCode < 300;
    }

    private static int GetInt(object source, string name)
    {
        object? value = GetMemberValue(source, name);
        return value is int intValue ? intValue : 0;
    }

    private static string? GetString(object source, string name)
    {
        return GetMemberValue(source, name) as string;
    }

    private static object? GetMemberValue(object source, string name)
    {
        Type? type = source.GetType();
        while (type != null)
        {
            FieldInfo? field = type.GetField(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (field != null)
            {
                return field.GetValue(source);
            }

            PropertyInfo? property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (property != null)
            {
                return property.GetValue(source);
            }

            type = type.BaseType;
        }

        return null;
    }

    private static object GetRequiredMemberValue(object source, string name)
    {
        object? value = GetMemberValue(source, name);
        if (value == null)
        {
            throw new InvalidOperationException($"{source.GetType().Name}.{name} was not found.");
        }

        return value;
    }

    private static void SetRequiredMemberValue(object source, string name, object value)
    {
        Type? type = source.GetType();
        while (type != null)
        {
            FieldInfo? field = type.GetField(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (field != null)
            {
                field.SetValue(source, value);
                return;
            }

            PropertyInfo? property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (property != null)
            {
                property.SetValue(source, value);
                return;
            }

            type = type.BaseType;
        }

        throw new InvalidOperationException($"{source.GetType().Name}.{name} was not found.");
    }

    private static string DescribeResponse(object? response)
    {
        if (response == null)
        {
            return string.Empty;
        }

        int statusCode = GetInt(response, "StatusCode");
        string? reason = GetString(response, "ReasonPhrase");
        string? body = GetString(response, "ISBMHTTPResponse");
        return $"status={statusCode}; reason={reason}; body={body}";
    }

    private static string JsonPayload(string kind, string suffix)
    {
        return $$"""{"kind":"{{kind}}","validationId":"{{suffix}}","createdUtc":"{{DateTime.UtcNow:O}}"}""";
    }

    private static void ValidateCredentialMember<TService>()
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
        public string Scenario { get; private set; } = "all";
        public bool Verbose { get; private set; }
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
                    case "--scenario":
                        options.Scenario = ReadValue();
                        break;
                    case "--verbose":
                        options.Verbose = true;
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

    private sealed class FilterCandidate
    {
        public FilterCandidate(string name, string expression, string language, string languageVersion, string? mediaType)
        {
            Name = name;
            Expression = expression;
            Language = language;
            LanguageVersion = languageVersion;
            MediaType = mediaType;
        }

        public string Name { get; }
        public string Expression { get; }
        public string Language { get; }
        public string LanguageVersion { get; }
        public string? MediaType { get; }
    }

    private sealed class ValidationReport
    {
        private readonly bool verbose;
        private readonly List<Result> results = new List<Result>();

        public ValidationReport(bool verbose)
        {
            this.verbose = verbose;
        }

        public bool HasFailures => results.Any(result => result.Outcome == Outcome.Failed || result.Outcome == Outcome.Blocked);

        public void Pass(string scenario, string operation, object? response)
        {
            Add(Outcome.Passed, scenario, operation, response, null);
        }

        public void Fail(string scenario, string operation, object response, string message)
        {
            Add(Outcome.Failed, scenario, operation, response, message);
        }

        public void Fail(string scenario, string operation, Exception exception)
        {
            Add(Outcome.Failed, scenario, operation, null, exception.Message);
        }

        public void Skip(string scenario, string operation, string message)
        {
            Add(Outcome.Skipped, scenario, operation, null, message);
        }

        public void Block(string scenario, string operation, string message)
        {
            Add(Outcome.Blocked, scenario, operation, null, message);
        }

        public void PrintSummary()
        {
            Console.WriteLine("Scenario summary:");
            Console.WriteLine($"Passed: {results.Count(result => result.Outcome == Outcome.Passed)}");
            Console.WriteLine($"Failed: {results.Count(result => result.Outcome == Outcome.Failed)}");
            Console.WriteLine($"Skipped: {results.Count(result => result.Outcome == Outcome.Skipped)}");
            Console.WriteLine($"Blocked: {results.Count(result => result.Outcome == Outcome.Blocked)}");

            foreach (Result result in results.Where(result => result.Outcome == Outcome.Failed || result.Outcome == Outcome.Blocked))
            {
                Console.WriteLine($"{result.Outcome}: {result.Scenario} / {result.Operation}: {result.Message} {result.ResponseSummary}");
            }
        }

        private void Add(Outcome outcome, string scenario, string operation, object? response, string? message)
        {
            string responseSummary = DescribeResponse(response);
            Result result = new Result(outcome, scenario, operation, message ?? string.Empty, responseSummary);
            results.Add(result);

            if (verbose || outcome != Outcome.Passed)
            {
                Console.WriteLine($"{outcome}: {scenario} / {operation} {responseSummary} {message}");
            }
        }
    }

    private sealed class Result
    {
        public Result(Outcome outcome, string scenario, string operation, string message, string responseSummary)
        {
            Outcome = outcome;
            Scenario = scenario;
            Operation = operation;
            Message = message;
            ResponseSummary = responseSummary;
        }

        public Outcome Outcome { get; }
        public string Scenario { get; }
        public string Operation { get; }
        public string Message { get; }
        public string ResponseSummary { get; }
    }

    private enum Outcome
    {
        Passed,
        Failed,
        Skipped,
        Blocked
    }
}
