# RapidRedPanda ISBM Interface Coding Guide

This guide shows how to use `RapidRedPanda.ISBM.ClientAdapter` 2.2.0 to work with ISBM 2.0 services from C# applications.

It is written for developers who want to get a basic client working quickly, understand the ISBM session and message lifecycle, and then move from beginner JSON payloads to advanced typed payloads using `MediaType`.

## Contents

1. [Introduction](#introduction)
2. [Installation](#installation)
3. [Supported Frameworks](#supported-frameworks)
4. [Core Concepts](#core-concepts)
5. [Authentication](#authentication)
6. [Channel Management](#channel-management)
7. [Provider Publication](#provider-publication)
8. [Consumer Publication](#consumer-publication)
9. [Consumer Request](#consumer-request)
10. [Provider Request](#provider-request)
11. [MediaType and Payload Semantics](#mediatype-and-payload-semantics)
12. [Sync and Async APIs](#sync-and-async-apis)
13. [Session and Message Lifecycle](#session-and-message-lifecycle)
14. [Response and Error Handling](#response-and-error-handling)
15. [Complete End-to-End Workflows](#complete-end-to-end-workflows)
16. [API Quick Reference](#api-quick-reference)
17. [SDK Sample Mapping](#sdk-sample-mapping)
18. [Troubleshooting](#troubleshooting)
19. [Useful Links](#useful-links)

## Introduction

ISBM 2.0 is the ISA-95 Message Service Model. It provides a message-bus style interface for exchanging information between applications, systems, and devices.

`RapidRedPanda.ISBM.ClientAdapter` is a .NET client library that wraps the ISBM service calls behind C# service classes. The adapter handles the HTTP request and response details so your application code can focus on:

- Managing channels.
- Opening and closing sessions.
- Posting and reading publication messages.
- Posting requests and responses.
- Tracking session IDs and message IDs.
- Inspecting response status and diagnostics.

The adapter exposes five main service classes:

| ISBM area | Service class |
|---|---|
| Channel management | `ChannelManagementService` |
| Publication provider | `ProviderPublicationService` |
| Publication consumer | `ConsumerPublicationService` |
| Request consumer | `ConsumerRequestService` |
| Request provider | `ProviderRequestService` |

Most examples in this guide use the asynchronous API first. Synchronous equivalents are available for the same service operations.

## Installation

Install the package from NuGet:

```bash
dotnet add package RapidRedPanda.ISBM.ClientAdapter --version 2.2.0
```

Visual Studio Package Manager Console:

```powershell
NuGet\Install-Package RapidRedPanda.ISBM.ClientAdapter -Version 2.2.0
```

NuGet restores the ClientAdapter package and its dependency:

- `RapidRedPanda.ISBM.ClientAdapter`
- `Newtonsoft.Json`

For projects that use `PackageReference`, the project file entry is:

```xml
<PackageReference Include="RapidRedPanda.ISBM.ClientAdapter" Version="2.2.0" />
```

Common namespaces:

```csharp
using RapidRedPanda.ISBM.ClientAdapter;
using RapidRedPanda.ISBM.ClientAdapter.EndpointOptions;
using RapidRedPanda.ISBM.ClientAdapter.ResponseType;
```

Authentication examples also use:

```csharp
using RapidRedPanda.ISBM.ClientAdapter.Enums;
using RapidRedPanda.ISBM.ClientAdapter.ServerOptions;
```

Examples that read or inspect JSON files may also use:

```csharp
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
```

Examples that pass cancellation tokens or build option lists may also use:

```csharp
using System.Collections.Generic;
using System.Threading;
```

## Supported Frameworks

`RapidRedPanda.ISBM.ClientAdapter` 2.2.0 ships package assets for:

- `netstandard2.0`
- `net8.0`
- `net10.0`

These are package target frameworks. They are different from the SDK sample project target frameworks:

| SDK sample family | Project framework |
|---|---|
| Windows WinForms samples | .NET Framework 4.7.2 |
| Raspberry Pi OS console samples | .NET 8 |

The Windows samples are legacy WinForms projects intended for Visual Studio 2022. The Raspberry Pi OS samples are SDK-style console projects intended for the .NET SDK.

## Core Concepts

### Host Address

The `hostAddress` parameter is the ISBM 2.0 service root. The server address must include the ISBM 2.0 base path `/isbm/2.0`, for example:

```csharp
string hostAddress = "https://your-server-host/isbm/2.0";
```

Use an address reachable from the machine running the client. `https://your-server-host` by itself is incomplete for these samples. `localhost` means the client machine itself.

### Channel ID

A channel is the logical route used for publications or request/response messages. Channel IDs are passed as strings:

```csharp
string channelId = "Example.Channel";
```

Channel management responses expose channel URIs through `URI`. Applications should use the channel ID or URI format expected by the ISBM server they are targeting.

### Topic

A topic identifies the message subject within a channel. Publication and provider request operations support a single topic or multiple topics where the API includes `string[] topics` overloads.

```csharp
string topic = "OIIE:S30:V1.1/CCOM-JSON:SyncMeasurements:V1.0";
string[] topics = { "Topic.One", "Topic.Two" };
```

### Session ID

Open-session calls return `SessionID`. Retain it and pass it to later operations for the same session.

```csharp
OpenPublicationSessionResponse openResponse =
    await service.OpenPublicationSessionAsync(hostAddress, channelId, CancellationToken.None);

string sessionId = openResponse.SessionID;
```

### Message ID

Post and read operations return `MessageID`. Retain it when later calls need to expire a message, correlate a request and response, or display diagnostics.

```csharp
string messageId = postResponse.MessageID;
```

## Authentication

Each service exposes public configuration fields:

```csharp
ProviderPublicationService service = new ProviderPublicationService();

service.Authentication.AuthenticationSchemeType = AuthenticationSchemeType.Basic;
service.Credential.Username = userName;
service.Credential.Password = password;
```

The same pattern applies to all five service classes.

### Basic Authentication

Use Basic authentication when the ISBM server expects a username and password.

```csharp
ConsumerRequestService service = new ConsumerRequestService();

service.Authentication.AuthenticationSchemeType = AuthenticationSchemeType.Basic;
service.Credential.Username = userName;
service.Credential.Password = password;
```

Use HTTPS when sending credentials.

### Custom Authentication

The public API exposes `AuthenticationSchemeType.Custom`, `CustomAuthorizationHeader`, and additional web request headers.

```csharp
ConsumerRequestService service = new ConsumerRequestService();

service.Authentication.AuthenticationSchemeType = AuthenticationSchemeType.Custom;
service.Authentication.CustomAuthorizationHeader = "Bearer eyJhbGciOi...";

service.Authentication.WebRequestHeaders.Add(
    new WebRequestHeader
    {
        Name = "X-Request-Timestamp",
        Value = DateTime.UtcNow.ToString("o")
    });
```

### Unsigned HTTPS Certificates

`Authentication.AcceptUnsignedHttps` is exposed by the public API. Use it only for local development or controlled test environments where accepting an unsigned certificate is intentional.

```csharp
service.Authentication.AcceptUnsignedHttps = true;
```

## Channel Management

Use `ChannelManagementService` for channel discovery and maintenance.

```csharp
ChannelManagementService service = new ChannelManagementService();

service.Credential.Username = userName;
service.Credential.Password = password;
```

### Get Channels

Returns the known channels from the ISBM server.

```csharp
GetChannelsResponse response =
    await service.GetChannelsAsync(hostAddress, CancellationToken.None);

if (response.StatusCode == 200)
{
    foreach (GetChannelsResponse.Channel channel in response.Channels)
    {
        string uri = channel.URI;
        string channelType = channel.ChannelType;
        string description = channel.Description;
    }
}
```

Sync equivalent:

```csharp
GetChannelsResponse response = service.GetChannels(hostAddress);
```

Important fields: `StatusCode`, `ReasonPhrase`, `ISBMHTTPResponse`, `Channels`.

### Get Channel

Returns a specific channel.

```csharp
GetChannelResponse response =
    await service.GetChannelAsync(hostAddress, channelId, CancellationToken.None);

if (response.StatusCode == 200)
{
    foreach (GetChannelResponse.Channel channel in response.Channels)
    {
        string uri = channel.URI;
        string channelType = channel.ChannelType;
        string description = channel.Description;
    }
}
```

Sync equivalent:

```csharp
GetChannelResponse response = service.GetChannel(hostAddress, channelId);
```

### Create Channel

Creates a channel with a channel ID, channel type, and description.

```csharp
CreateChannelResponse response =
    await service.CreateChannelAsync(
        hostAddress,
        channelId,
        channelType,
        description,
        CancellationToken.None);
```

Sync equivalent:

```csharp
CreateChannelResponse response =
    service.CreateChannel(hostAddress, channelId, channelType, description);
```

### Create Channel With UsernameToken Security

`CreateChannelOptions` exposes a `SecurityTokens` list. Current samples use `UsernameToken`.

```csharp
CreateChannelOptions options = new CreateChannelOptions();

options.SecurityTokens.Add(
    new CreateChannelOptions.SecurityToken
    {
        type = "UsernameToken",
        username = userName,
        password = password
    });

CreateChannelResponse response =
    await service.CreateChannelAsync(
        hostAddress,
        channelId,
        channelType,
        description,
        options,
        CancellationToken.None);
```

Sync equivalent:

```csharp
CreateChannelResponse response =
    service.CreateChannel(hostAddress, channelId, channelType, description, options);
```

### Delete Channel

Deletes a channel.

```csharp
DeleteChannelResponse response =
    await service.DeleteChannelAsync(hostAddress, channelId, CancellationToken.None);
```

Sync equivalent:

```csharp
DeleteChannelResponse response = service.DeleteChannel(hostAddress, channelId);
```

### Add Security Tokens

Adds username tokens to an existing channel.

```csharp
AddSecurityTokensOptions options = new AddSecurityTokensOptions();

options.SecurityTokens.Add(
    new AddSecurityTokensOptions.SecurityToken
    {
        type = "UsernameToken",
        username = userName,
        password = password
    });

AddSecurityTokensResponse response =
    await service.AddSecurityTokensAsync(
        hostAddress,
        channelId,
        options,
        CancellationToken.None);
```

Sync equivalent:

```csharp
AddSecurityTokensResponse response =
    service.AddSecurityTokens(hostAddress, channelId, options);
```

### Remove Security Tokens

Removes username tokens from an existing channel.

```csharp
RemoveSecurityTokensOptions options = new RemoveSecurityTokensOptions();

options.SecurityTokens.Add(
    new RemoveSecurityTokensOptions.SecurityToken
    {
        type = "UsernameToken",
        username = userName,
        password = password
    });

RemoveSecurityTokensResponse response =
    await service.RemoveSecurityTokensAsync(
        hostAddress,
        channelId,
        options,
        CancellationToken.None);
```

Sync equivalent:

```csharp
RemoveSecurityTokensResponse response =
    service.RemoveSecurityTokens(hostAddress, channelId, options);
```

## Provider Publication

Use `ProviderPublicationService` when your application publishes messages to a publication channel.

### Open Publication Session

```csharp
ProviderPublicationService service = new ProviderPublicationService();

service.Credential.Username = userName;
service.Credential.Password = password;

OpenPublicationSessionResponse openResponse =
    await service.OpenPublicationSessionAsync(
        hostAddress,
        channelId,
        CancellationToken.None);

string sessionId = openResponse.SessionID;
```

Sync equivalent:

```csharp
OpenPublicationSessionResponse openResponse =
    service.OpenPublicationSession(hostAddress, channelId);
```

### Post Publication

For beginner JSON-object content, leave `MediaType` unset and pass valid JSON object text.

```csharp
string jsonPayload = "{\"measurement\":{\"value\":42}}";

PostPublicationResponse postResponse =
    await service.PostPublicationAsync(
        hostAddress,
        sessionId,
        topic,
        jsonPayload,
        CancellationToken.None);

string publicationMessageId = postResponse.MessageID;
```

Sync equivalent:

```csharp
PostPublicationResponse postResponse =
    service.PostPublication(hostAddress, sessionId, topic, jsonPayload);
```

### Post Publication To Multiple Topics

```csharp
string[] topics = { "Topic.One", "Topic.Two" };

PostPublicationResponse postResponse =
    await service.PostPublicationAsync(
        hostAddress,
        sessionId,
        topics,
        jsonPayload,
        CancellationToken.None);
```

Sync equivalent:

```csharp
PostPublicationResponse postResponse =
    service.PostPublication(hostAddress, sessionId, topics, jsonPayload);
```

### Post Publication With Options

`PostPublicationOptions` exposes `Expiry` and `MediaType`.

```csharp
PostPublicationOptions options = new PostPublicationOptions
{
    Expiry = "P2D"
};

PostPublicationResponse postResponse =
    await service.PostPublicationAsync(
        hostAddress,
        sessionId,
        topic,
        jsonPayload,
        options,
        CancellationToken.None);
```

To send XML, provide XML text and set `MediaType`.

```csharp
PostPublicationOptions options = new PostPublicationOptions
{
    MediaType = "application/xml"
};

string xmlPayload = "<measurement><value>42</value></measurement>";

PostPublicationResponse postResponse =
    await service.PostPublicationAsync(
        hostAddress,
        sessionId,
        topic,
        xmlPayload,
        options,
        CancellationToken.None);
```

### Expire Publication

Expire a posted publication by session ID and publication message ID.

```csharp
ExpirePublicationResponse response =
    await service.ExpirePublicationAsync(
        hostAddress,
        sessionId,
        publicationMessageId,
        CancellationToken.None);
```

Sync equivalent:

```csharp
ExpirePublicationResponse response =
    service.ExpirePublication(hostAddress, sessionId, publicationMessageId);
```

### Close Publication Session

```csharp
ClosePublicationSessionResponse response =
    await service.ClosePublicationSessionAsync(
        hostAddress,
        sessionId,
        CancellationToken.None);
```

Sync equivalent:

```csharp
ClosePublicationSessionResponse response =
    service.ClosePublicationSession(hostAddress, sessionId);
```

## Consumer Publication

Use `ConsumerPublicationService` when your application subscribes to publication messages.

### Open Subscription Session

Open a subscription session for one topic:

```csharp
ConsumerPublicationService service = new ConsumerPublicationService();

service.Credential.Username = userName;
service.Credential.Password = password;

OpenSubscriptionSessionResponse openResponse =
    await service.OpenSubscriptionSessionAsync(
        hostAddress,
        channelId,
        topic,
        CancellationToken.None);

string sessionId = openResponse.SessionID;
```

Open a subscription session for multiple topics:

```csharp
string[] topics = { "Topic.One", "Topic.Two" };

OpenSubscriptionSessionResponse openResponse =
    await service.OpenSubscriptionSessionAsync(
        hostAddress,
        channelId,
        topics,
        CancellationToken.None);
```

Sync equivalents:

```csharp
OpenSubscriptionSessionResponse oneTopic =
    service.OpenSubscriptionSession(hostAddress, channelId, topic);

OpenSubscriptionSessionResponse manyTopics =
    service.OpenSubscriptionSession(hostAddress, channelId, topics);
```

### Listener URL And Filters

`OpenSubscriptionSessionOptions` exposes `ListenerURL` and `FilterExpressions`.

```csharp
OpenSubscriptionSessionOptions options = new OpenSubscriptionSessionOptions
{
    ListenerURL = "http://127.0.0.1:8080"
};

OpenSubscriptionSessionResponse openResponse =
    await service.OpenSubscriptionSessionAsync(
        hostAddress,
        channelId,
        topic,
        options,
        CancellationToken.None);
```

Filter expressions use public fields:

```csharp
OpenSubscriptionSessionOptions options = new OpenSubscriptionSessionOptions();

options.FilterExpressions.Add(
    new FilterExpression
    {
        ApplicableMediaTypes = new List<string> { "application/json" },
        ExpressionString = new ExpressionString
        {
            Expression = "$.DataArea.Show.Measurement[?(@.value > 100)]",
            Language = "JsonPath",
            LanguageVersion = "1.0"
        }
    });
```

### Read Publication

```csharp
ReadPublicationResponse readResponse =
    await service.ReadPublicationAsync(
        hostAddress,
        sessionId,
        CancellationToken.None);

if (readResponse.StatusCode == 200)
{
    string messageId = readResponse.MessageID;
    string[] returnedTopics = readResponse.Topics;
    string messageContent = readResponse.MessageContent;
}
```

Sync equivalent:

```csharp
ReadPublicationResponse readResponse =
    service.ReadPublication(hostAddress, sessionId);
```

### Remove Publication

Remove the first available publication message in the subscription queue.

```csharp
RemovePublicationResponse response =
    await service.RemovePublicationAsync(
        hostAddress,
        sessionId,
        CancellationToken.None);
```

Sync equivalent:

```csharp
RemovePublicationResponse response =
    service.RemovePublication(hostAddress, sessionId);
```

### Close Subscription Session

```csharp
CloseSubscriptionSessionResponse response =
    await service.CloseSubscriptionSessionAsync(
        hostAddress,
        sessionId,
        CancellationToken.None);
```

Sync equivalent:

```csharp
CloseSubscriptionSessionResponse response =
    service.CloseSubscriptionSession(hostAddress, sessionId);
```

## Consumer Request

Use `ConsumerRequestService` when your application sends request messages and later reads responses for those requests.

### Open Consumer Request Session

```csharp
ConsumerRequestService service = new ConsumerRequestService();

service.Credential.Username = userName;
service.Credential.Password = password;

OpenConsumerRequestSessionResponse openResponse =
    await service.OpenConsumerRequestSessionAsync(
        hostAddress,
        channelId,
        CancellationToken.None);

string sessionId = openResponse.SessionID;
```

With a listener URL:

```csharp
OpenConsumerRequestSessionOptions options =
    new OpenConsumerRequestSessionOptions
    {
        ListenerURL = "http://127.0.0.1:8080"
    };

OpenConsumerRequestSessionResponse openResponse =
    await service.OpenConsumerRequestSessionAsync(
        hostAddress,
        channelId,
        options,
        CancellationToken.None);
```

Sync equivalent:

```csharp
OpenConsumerRequestSessionResponse openResponse =
    service.OpenConsumerRequestSession(hostAddress, channelId);
```

### Post Request

```csharp
string requestPayload = "{\"request\":{\"asset\":\"Pump-01\"}}";

PostRequestResponse postResponse =
    await service.PostRequestAsync(
        hostAddress,
        sessionId,
        topic,
        requestPayload,
        CancellationToken.None);

string requestMessageId = postResponse.MessageID;
```

Sync equivalent:

```csharp
PostRequestResponse postResponse =
    service.PostRequest(hostAddress, sessionId, topic, requestPayload);
```

With options:

```csharp
PostRequestOptions options = new PostRequestOptions
{
    Expiry = "P1D"
};

PostRequestResponse postResponse =
    await service.PostRequestAsync(
        hostAddress,
        sessionId,
        topic,
        requestPayload,
        options,
        CancellationToken.None);
```

### Read Response

Use the request message ID returned by `PostRequest`.

```csharp
ReadResponseResponse readResponse =
    await service.ReadResponseAsync(
        hostAddress,
        sessionId,
        requestMessageId,
        CancellationToken.None);

if (readResponse.StatusCode == 200)
{
    string responseMessageId = readResponse.MessageID;
    string responseContent = readResponse.MessageContent;
}
```

Sync equivalent:

```csharp
ReadResponseResponse readResponse =
    service.ReadResponse(hostAddress, sessionId, requestMessageId);
```

### Remove Response

```csharp
RemoveResponseResponse response =
    await service.RemoveResponseAsync(
        hostAddress,
        sessionId,
        requestMessageId,
        CancellationToken.None);
```

Sync equivalent:

```csharp
RemoveResponseResponse response =
    service.RemoveResponse(hostAddress, sessionId, requestMessageId);
```

### Expire Request

Expire a posted request by session ID and request message ID.

```csharp
ExpireRequestResponse response =
    await service.ExpireRequestAsync(
        hostAddress,
        sessionId,
        requestMessageId,
        CancellationToken.None);
```

Sync equivalent:

```csharp
ExpireRequestResponse response =
    service.ExpireRequest(hostAddress, sessionId, requestMessageId);
```

### Close Consumer Request Session

```csharp
CloseConsumerRequestSessionResponse response =
    await service.CloseConsumerRequestSessionAsync(
        hostAddress,
        sessionId,
        CancellationToken.None);
```

Sync equivalent:

```csharp
CloseConsumerRequestSessionResponse response =
    service.CloseConsumerRequestSession(hostAddress, sessionId);
```

## Provider Request

Use `ProviderRequestService` when your application reads incoming requests and posts responses.

### Open Provider Request Session

```csharp
ProviderRequestService service = new ProviderRequestService();

service.Credential.Username = userName;
service.Credential.Password = password;

OpenProviderRequestSessionResponse openResponse =
    await service.OpenProviderRequestSessionAsync(
        hostAddress,
        channelId,
        topic,
        CancellationToken.None);

string sessionId = openResponse.SessionID;
```

Multiple topics are supported:

```csharp
string[] topics = { "Topic.One", "Topic.Two" };

OpenProviderRequestSessionResponse openResponse =
    await service.OpenProviderRequestSessionAsync(
        hostAddress,
        channelId,
        topics,
        CancellationToken.None);
```

With options:

```csharp
OpenProviderRequestSessionOptions options =
    new OpenProviderRequestSessionOptions
    {
        ListenerURL = "http://127.0.0.1:8080"
    };

OpenProviderRequestSessionResponse openResponse =
    await service.OpenProviderRequestSessionAsync(
        hostAddress,
        channelId,
        topic,
        options,
        CancellationToken.None);
```

Sync equivalents:

```csharp
OpenProviderRequestSessionResponse oneTopic =
    service.OpenProviderRequestSession(hostAddress, channelId, topic);

OpenProviderRequestSessionResponse manyTopics =
    service.OpenProviderRequestSession(hostAddress, channelId, topics);
```

### Read Request

```csharp
ReadRequestResponse readResponse =
    await service.ReadRequestAsync(
        hostAddress,
        sessionId,
        CancellationToken.None);

if (readResponse.StatusCode == 200)
{
    string requestMessageId = readResponse.MessageID;
    string requestContent = readResponse.MessageContent;
}
```

Sync equivalent:

```csharp
ReadRequestResponse readResponse =
    service.ReadRequest(hostAddress, sessionId);
```

### Post Response

Use the request message ID returned by `ReadRequest`.

```csharp
string responsePayload = "{\"response\":{\"accepted\":true}}";

PostResponseResponse postResponse =
    await service.PostResponseAsync(
        hostAddress,
        sessionId,
        requestMessageId,
        responsePayload,
        CancellationToken.None);

string responseMessageId = postResponse.MessageID;
```

Sync equivalent:

```csharp
PostResponseResponse postResponse =
    service.PostResponse(hostAddress, sessionId, requestMessageId, responsePayload);
```

With `MediaType`:

```csharp
PostResponseOptions options = new PostResponseOptions
{
    MediaType = "text/plain"
};

PostResponseResponse postResponse =
    await service.PostResponseAsync(
        hostAddress,
        sessionId,
        requestMessageId,
        "Accepted",
        options,
        CancellationToken.None);
```

### Remove Request

After processing a request, remove it from the provider request session queue.

```csharp
RemoveRequestResponse response =
    await service.RemoveRequestAsync(
        hostAddress,
        sessionId,
        CancellationToken.None);
```

Sync equivalent:

```csharp
RemoveRequestResponse response =
    service.RemoveRequest(hostAddress, sessionId);
```

### Close Provider Request Session

```csharp
CloseProviderRequestSessionResponse response =
    await service.CloseProviderRequestSessionAsync(
        hostAddress,
        sessionId,
        CancellationToken.None);
```

Sync equivalent:

```csharp
CloseProviderRequestSessionResponse response =
    service.CloseProviderRequestSession(hostAddress, sessionId);
```

## MediaType and Payload Semantics

`RapidRedPanda.ISBM.ClientAdapter` 2.2.0 supports two payload models for post operations.

### Native JSON Object Mode

When `MediaType` is omitted, empty, or whitespace:

- The payload must be valid JSON object content.
- The adapter sends `messageContent.content` as a JSON object.
- `mediaType` is omitted.
- `contentEncoding` is omitted.

Example:

```csharp
string payload = "{\"measurement\":{\"value\":42}}";

PostPublicationResponse response =
    await service.PostPublicationAsync(
        hostAddress,
        sessionId,
        topic,
        payload,
        CancellationToken.None);
```

Do not use blank `MediaType` for XML, plain text, JSON arrays, or arbitrary strings.

### Typed String Payload Mode

When `MediaType` is supplied:

- The payload is treated as exact string content.
- The adapter includes `mediaType`.
- The adapter does not transform one format into another.

If you set `application/xml`, the payload must already be XML text.

Publication XML example:

```csharp
PostPublicationOptions options = new PostPublicationOptions
{
    MediaType = "application/xml"
};

string payload = "<measurement><value>42</value></measurement>";

PostPublicationResponse response =
    await service.PostPublicationAsync(
        hostAddress,
        sessionId,
        topic,
        payload,
        options,
        CancellationToken.None);
```

Request plain-text example:

```csharp
PostRequestOptions options = new PostRequestOptions
{
    MediaType = "text/plain"
};

PostRequestResponse response =
    await service.PostRequestAsync(
        hostAddress,
        sessionId,
        topic,
        "Start pump",
        options,
        CancellationToken.None);
```

Explicit JSON text example:

```csharp
PostResponseOptions options = new PostResponseOptions
{
    MediaType = "application/json"
};

string payload = "{\"accepted\":true}";

PostResponseResponse response =
    await service.PostResponseAsync(
        hostAddress,
        sessionId,
        requestMessageId,
        payload,
        options,
        CancellationToken.None);
```

Supported public option fields:

| Operation | Options type | MediaType field |
|---|---|---|
| Post publication | `PostPublicationOptions` | `MediaType` |
| Post request | `PostRequestOptions` | `MediaType` |
| Post response | `PostResponseOptions` | `MediaType` |

## Sync and Async APIs

Every major service operation has a synchronous method and an asynchronous method.

Async methods:

- End with `Async`.
- Return `Task<TResponse>`.
- Accept a `CancellationToken` parameter.
- Are the preferred form for UI, web, service, and network-aware applications.

Synchronous methods:

- Use the same operation name without `Async`.
- Return the response object directly.
- Are useful in simple console tools or scripts where blocking is acceptable.

Example pair:

```csharp
OpenPublicationSessionResponse asyncResponse =
    await service.OpenPublicationSessionAsync(
        hostAddress,
        channelId,
        CancellationToken.None);

OpenPublicationSessionResponse syncResponse =
    service.OpenPublicationSession(hostAddress, channelId);
```

## Session and Message Lifecycle

Session and message IDs are application state. Store them deliberately.

| Value | Returned by | Used by |
|---|---|---|
| `SessionID` | Open-session calls | Post, read, remove, expire, close |
| Publication `MessageID` | `PostPublication`, `ReadPublication` | `ExpirePublication`, application diagnostics |
| Request `MessageID` | `PostRequest`, `ReadRequest` | `ReadResponse`, `RemoveResponse`, `ExpireRequest`, `PostResponse` |
| Response `MessageID` | `PostResponse`, `ReadResponse` | Application diagnostics |

General cleanup pattern:

1. Open a session.
2. Retain `SessionID`.
3. Post or read messages.
4. Retain `MessageID` values needed for follow-up operations.
5. Remove or expire messages where appropriate.
6. Close the session.

## Response and Error Handling

All response types inherit common fields from `ISBMResponse`:

```csharp
int statusCode = response.StatusCode;
string reason = response.ReasonPhrase;
string rawResponse = response.ISBMHTTPResponse;
```

Use these fields for normal application decisions and diagnostics.

Common response fields:

| Field | Meaning |
|---|---|
| `StatusCode` | HTTP-style status code returned by the ISBM operation. |
| `ReasonPhrase` | Human-readable reason phrase where available. |
| `ISBMHTTPResponse` | Raw ISBM response body, useful for diagnostics. |
| `SessionID` | Returned by open-session responses. |
| `MessageID` | Returned by post and read-message responses. |
| `MessageContent` | Returned by read-message responses. |
| `Topics` | Returned by `ReadPublicationResponse`. |

Practical guidance:

- Check `StatusCode` before using operation-specific fields.
- Preserve `ISBMHTTPResponse` in logs when diagnosing integration problems.
- Handle invalid or expired sessions by opening a new session.
- Handle missing or no-message responses as normal polling outcomes where the server uses polling.
- Handle authorization failures by checking credentials, channel security tokens, and authentication configuration.
- Do not assume every failure is returned only through response fields; invalid inputs may still throw exceptions.

## Complete End-to-End Workflows

### Publish One JSON Message

```csharp
ProviderPublicationService service = new ProviderPublicationService();

service.Credential.Username = userName;
service.Credential.Password = password;

OpenPublicationSessionResponse openResponse =
    await service.OpenPublicationSessionAsync(
        hostAddress,
        channelId,
        CancellationToken.None);

string sessionId = openResponse.SessionID;

PostPublicationResponse postResponse =
    await service.PostPublicationAsync(
        hostAddress,
        sessionId,
        topic,
        "{\"measurement\":{\"value\":42}}",
        CancellationToken.None);

string messageId = postResponse.MessageID;

ClosePublicationSessionResponse closeResponse =
    await service.ClosePublicationSessionAsync(
        hostAddress,
        sessionId,
        CancellationToken.None);
```

### Subscribe, Read, Remove, Close

```csharp
ConsumerPublicationService service = new ConsumerPublicationService();

OpenSubscriptionSessionResponse openResponse =
    await service.OpenSubscriptionSessionAsync(
        hostAddress,
        channelId,
        topic,
        CancellationToken.None);

string sessionId = openResponse.SessionID;

ReadPublicationResponse readResponse =
    await service.ReadPublicationAsync(
        hostAddress,
        sessionId,
        CancellationToken.None);

if (readResponse.StatusCode == 200)
{
    string content = readResponse.MessageContent;
    string messageId = readResponse.MessageID;
    string[] topics = readResponse.Topics;

    RemovePublicationResponse removeResponse =
        await service.RemovePublicationAsync(
            hostAddress,
            sessionId,
            CancellationToken.None);
}

CloseSubscriptionSessionResponse closeResponse =
    await service.CloseSubscriptionSessionAsync(
        hostAddress,
        sessionId,
        CancellationToken.None);
```

### Send A Request And Read Its Response

```csharp
ConsumerRequestService service = new ConsumerRequestService();

OpenConsumerRequestSessionResponse openResponse =
    await service.OpenConsumerRequestSessionAsync(
        hostAddress,
        channelId,
        CancellationToken.None);

string sessionId = openResponse.SessionID;

PostRequestResponse postResponse =
    await service.PostRequestAsync(
        hostAddress,
        sessionId,
        topic,
        "{\"request\":{\"asset\":\"Pump-01\"}}",
        CancellationToken.None);

string requestMessageId = postResponse.MessageID;

ReadResponseResponse readResponse =
    await service.ReadResponseAsync(
        hostAddress,
        sessionId,
        requestMessageId,
        CancellationToken.None);

if (readResponse.StatusCode == 200)
{
    string responseContent = readResponse.MessageContent;

    RemoveResponseResponse removeResponse =
        await service.RemoveResponseAsync(
            hostAddress,
            sessionId,
            requestMessageId,
            CancellationToken.None);
}

CloseConsumerRequestSessionResponse closeResponse =
    await service.CloseConsumerRequestSessionAsync(
        hostAddress,
        sessionId,
        CancellationToken.None);
```

### Read A Request And Post A Response

```csharp
ProviderRequestService service = new ProviderRequestService();

OpenProviderRequestSessionResponse openResponse =
    await service.OpenProviderRequestSessionAsync(
        hostAddress,
        channelId,
        topic,
        CancellationToken.None);

string sessionId = openResponse.SessionID;

ReadRequestResponse readResponse =
    await service.ReadRequestAsync(
        hostAddress,
        sessionId,
        CancellationToken.None);

if (readResponse.StatusCode == 200)
{
    string requestMessageId = readResponse.MessageID;
    string requestContent = readResponse.MessageContent;

    PostResponseResponse postResponse =
        await service.PostResponseAsync(
            hostAddress,
            sessionId,
            requestMessageId,
            "{\"response\":{\"accepted\":true}}",
            CancellationToken.None);

    RemoveRequestResponse removeResponse =
        await service.RemoveRequestAsync(
            hostAddress,
            sessionId,
            CancellationToken.None);
}

CloseProviderRequestSessionResponse closeResponse =
    await service.CloseProviderRequestSessionAsync(
        hostAddress,
        sessionId,
        CancellationToken.None);
```

### Send Plain Text With MediaType

```csharp
ConsumerRequestService service = new ConsumerRequestService();

PostRequestOptions options = new PostRequestOptions
{
    MediaType = "text/plain"
};

PostRequestResponse postResponse =
    await service.PostRequestAsync(
        hostAddress,
        sessionId,
        topic,
        "Start pump",
        options,
        CancellationToken.None);
```

## API Quick Reference

| Service | Operation | Async method | Sync method | Important returned value |
|---|---|---|---|---|
| `ChannelManagementService` | Get channels | `GetChannelsAsync` | `GetChannels` | `Channels` |
| `ChannelManagementService` | Get channel | `GetChannelAsync` | `GetChannel` | `Channels` |
| `ChannelManagementService` | Create channel | `CreateChannelAsync` | `CreateChannel` | `StatusCode` |
| `ChannelManagementService` | Delete channel | `DeleteChannelAsync` | `DeleteChannel` | `StatusCode` |
| `ChannelManagementService` | Add security tokens | `AddSecurityTokensAsync` | `AddSecurityTokens` | `StatusCode` |
| `ChannelManagementService` | Remove security tokens | `RemoveSecurityTokensAsync` | `RemoveSecurityTokens` | `StatusCode` |
| `ProviderPublicationService` | Open publication session | `OpenPublicationSessionAsync` | `OpenPublicationSession` | `SessionID` |
| `ProviderPublicationService` | Post publication | `PostPublicationAsync` | `PostPublication` | `MessageID` |
| `ProviderPublicationService` | Expire publication | `ExpirePublicationAsync` | `ExpirePublication` | `StatusCode` |
| `ProviderPublicationService` | Close publication session | `ClosePublicationSessionAsync` | `ClosePublicationSession` | `StatusCode` |
| `ConsumerPublicationService` | Open subscription session | `OpenSubscriptionSessionAsync` | `OpenSubscriptionSession` | `SessionID` |
| `ConsumerPublicationService` | Read publication | `ReadPublicationAsync` | `ReadPublication` | `MessageID`, `Topics`, `MessageContent` |
| `ConsumerPublicationService` | Remove publication | `RemovePublicationAsync` | `RemovePublication` | `StatusCode` |
| `ConsumerPublicationService` | Close subscription session | `CloseSubscriptionSessionAsync` | `CloseSubscriptionSession` | `StatusCode` |
| `ConsumerRequestService` | Open consumer request session | `OpenConsumerRequestSessionAsync` | `OpenConsumerRequestSession` | `SessionID` |
| `ConsumerRequestService` | Post request | `PostRequestAsync` | `PostRequest` | `MessageID` |
| `ConsumerRequestService` | Read response | `ReadResponseAsync` | `ReadResponse` | `MessageID`, `MessageContent` |
| `ConsumerRequestService` | Remove response | `RemoveResponseAsync` | `RemoveResponse` | `StatusCode` |
| `ConsumerRequestService` | Expire request | `ExpireRequestAsync` | `ExpireRequest` | `StatusCode` |
| `ConsumerRequestService` | Close consumer request session | `CloseConsumerRequestSessionAsync` | `CloseConsumerRequestSession` | `StatusCode` |
| `ProviderRequestService` | Open provider request session | `OpenProviderRequestSessionAsync` | `OpenProviderRequestSession` | `SessionID` |
| `ProviderRequestService` | Read request | `ReadRequestAsync` | `ReadRequest` | `MessageID`, `MessageContent` |
| `ProviderRequestService` | Post response | `PostResponseAsync` | `PostResponse` | `MessageID` |
| `ProviderRequestService` | Remove request | `RemoveRequestAsync` | `RemoveRequest` | `StatusCode` |
| `ProviderRequestService` | Close provider request session | `CloseProviderRequestSessionAsync` | `CloseProviderRequestSession` | `StatusCode` |

### Options Quick Reference

| Options type | Public fields |
|---|---|
| `CreateChannelOptions` | `SecurityTokens` |
| `AddSecurityTokensOptions` | `SecurityTokens` |
| `RemoveSecurityTokensOptions` | `SecurityTokens` |
| `OpenSubscriptionSessionOptions` | `ListenerURL`, `FilterExpressions` |
| `OpenConsumerRequestSessionOptions` | `ListenerURL` |
| `OpenProviderRequestSessionOptions` | `ListenerURL`, `FilterExpressions` |
| `PostPublicationOptions` | `Expiry`, `MediaType` |
| `PostRequestOptions` | `Expiry`, `MediaType` |
| `PostResponseOptions` | `MediaType` |

## SDK Sample Mapping

| Guide area | SDK sample path |
|---|---|
| Channel Management | `CSharp/Windows/ISBM20ChannelManagementTestCSharp` |
| Provider Publication | `CSharp/Windows/ISBM20ProviderPublicationTestCSharp` |
| Consumer Publication | `CSharp/Windows/ISBM20ConsumerPublicationTestCSharp` |
| Consumer Request | `CSharp/Windows/ISBM20ConsumerRequestTestCSharp` |
| Provider Request | `CSharp/Windows/ISBM20ProviderRequestTestCSharp` |
| Raspberry Pi Publication | `CSharp/Raspberry-Pi-OS/ISBM20Pi3PublicationTestNet8` |
| Raspberry Pi Provider Request | `CSharp/Raspberry-Pi-OS/ISBM20Pi3RequestTestNet8` |

### Running Windows Samples

1. Extract the SDK to a reasonably short local path.
2. Open the sample `.sln` in Visual Studio 2022.
3. Restore NuGet packages.
4. Rebuild the solution.
5. Update host, channel, topic, and authentication fields for your ISBM server.
6. Run the sample.

### Running Raspberry Pi OS Samples

1. Restore NuGet packages with the .NET SDK.
2. Copy `Configs-Example.json` to `Configs.json`.
3. Update `hostName`, `channelId`, `topic`, `authentication`, `userName`, and `password`.
4. Build, run, or publish the selected project.

## Troubleshooting

### Unresolved NuGet References

Restore packages before building. In Visual Studio, use Restore NuGet Packages or rebuild the solution. From a command line:

```bash
dotnet restore
```

The package and dependency expected by these samples are:

- `RapidRedPanda.ISBM.ClientAdapter` 2.2.0
- `Newtonsoft.Json` 13.0.1

### Restore Has Not Completed

If the build fails immediately after opening a sample, wait for Visual Studio package restore to complete, then rebuild.

### Deep Windows Paths

Legacy Windows project tooling can be sensitive to very long paths. Extract the SDK to a reasonably short local path, such as:

```text
C:\ISBM-SDK
```

### Visual Studio Version

Use Visual Studio 2022 for the Windows WinForms samples. The projects target .NET Framework 4.7.2.

### Invalid JSON When MediaType Is Blank

Blank `MediaType` means native JSON object mode. The payload must be a JSON object:

```json
{"value":42}
```

This is not valid for blank `MediaType`:

```text
plain text
```

### MediaType And Payload Mismatch

Setting `MediaType = "application/xml"` does not convert JSON into XML. The payload must already be XML text.

Setting `MediaType = "text/plain"` sends the payload as exact text.

Setting `MediaType = "application/json"` sends JSON text as string content. This is different from leaving `MediaType` blank.

### Expired Session Or Message IDs

If an operation fails because a session or message no longer exists, open a new session or repeat the post/read operation to obtain a current `SessionID` or `MessageID`.

### Authentication Failures

Check:

- `Authentication.AuthenticationSchemeType`
- `Credential.Username`
- `Credential.Password`
- Channel security token configuration
- Custom authorization header and request headers, if using custom authentication

## Useful Links

- [NuGet package: RapidRedPanda.ISBM.ClientAdapter](https://www.nuget.org/packages/RapidRedPanda.ISBM.ClientAdapter/)
- [OpenO&M](https://openoandm.org/)
- [MIMOSA](https://www.mimosa.org/)
- [OIIE](https://www.mimosa.org/open-industrial-interoperability-ecosystem-oiie/)
- [ISBM 2.0](https://openoandm.org/files/standards/ISBM-2.0.pdf)
- [OAGi](https://oagi.org/)
