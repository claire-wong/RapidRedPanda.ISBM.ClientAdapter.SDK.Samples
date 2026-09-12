# ISBM-2.0-Client-SDK

Open Industrial Interoperability Ecosystem (OIIE) a vendor-neutral, interoperable ecosystem to tackle industry-wide data silo challenges. It enables a cluster of IoT/IoE devices and processes to communicate in an interoperable and scalable manner. This Software Development Toolkit is designed to assist anyone interested in trying out the Open Industrial Interoperability Ecosystem (OIIE) with easy to follow examples. 

## Contents
  
   1. [Objectives](#Objectives)
   2. [ISBM Client Adapters Architecture](#ISBM-Client-Adapters-Architecture-Diagram)
   3. [ISBM Message Models](#ISBM-Message-Models)
   4. [Use Cases](#Use-Cases)
   5. [Project Information](#Project-Information)
   6. [Before Running the Program](#Before-Running-the-Program)
   7. [Useful Links](#Useful-Links)
   8. [Quick Reference](#Quick-Reference)

## Objectives

Since OIIE involves multiple industry standards at different levels of the technology stack, it is helpful to have a developer friendly OIIE programming interfaces. This SDK includes sample C# code projects and three flavors of ISBM 2.0 Client Adapters (RapidRedPanda.ISBM.ClientAdapter), .Net Core 3.1, .NET Standard 2.0, and .NET 6. They support a wide range of .Net implementations and OS or hardware platforms; see details in the ISBM Client Adapter Architecture Diagram. By using the adapter, you can deliver and receive CCOM messages (Common Conceptual Object Model) through ISBM services (ISA-95 Message Service Model) with just a few lines of code! It reduces the learning curve, allowing you to explore OIIE without diving into the technical details of the standards

The NuGet package [RapidRedPanda.ISBM.ClientAdapter](https://www.nuget.org/packages/RapidRedPanda.ISBM.ClientAdapter/#readme-body-tab) package is designed to handle all the details of ISBM implementations for communication with ISBM servers. The ISBM interface will be accessible through object classes that developers should find user-friendly and easy to use. This will cut down the learning curve of building ISBM-compliant devices or applications.

      
#### ISBM Client Adapters Architecture Diagram

![image](/Documents/Images/Architecture_Diagram1.jpg)

## ISBM Message Models

An ISBM message bus is a communication platform that allows different devices or processes to communicate with each other. It serves as a centralized hub where messages are sent and received by various systems, enabling asynchronous and decoupled communication. This architecture facilitates scalability, flexibility, and interoperability within complex cluster. ISBM 2.0 (ISA-95 Message Service Model) defines two message bus models: the [publication-subscribe (pub/sub)](#ISBM-Message-Models) model and the [request-response)](#Request-Response-model) model.

### Publication-Subscription model

In this model, providers send messages to a central message broker (ISBM Service Provider), and consumers receive messages based on their interests or subscriptions to specific channels and topics.

#### Breakdown of the key components and concepts of the publication-subscription message bus model

#### Provider Applications:

Provider applications are devices or systems that generate and send messages to the ISBM aervice provider. They produce messages related to specific topics or events without knowing who will receive them.

#### Consumer Applications:

Consumer applications are devices or systems that receive messages from the the ISBM aervice provider based on their subscriptions to specific topics and channels. They express interest in receiving messages about particular topics or events.

#### ISBM Service Provider:

The ISBM service provider acts as an intermediary between provider applications and consumer applications. It receives messages from publishers and distributes them to consumers based on their subscriptions. The ISBM aervice provider manages the routing and delivery of messages, ensuring that each consumer application receives the relevant messages according to its subscriptions.

#### Publication Channels:

A publication channel is a logical conduit which published messages flow between different devices or systems within a cluster. It serves as a means of organizing and directing the transmission of messages, providing a structured communication conduit for exchanging information. Publication channels help to categorize published messages and ensure that they reach their intended consumers efficiently. In messaging systems, channels play a crucial role in facilitating communication, enabling devices and systems to interact and share data seamlessly across distributed environments while maintaining scalability and flexibility in message routing and delivery.

#### Topics:

A topic within the channel represents a distinct subject or category of messages that are published and subscribed to by devices or systems within a cluster. It serves as a container for related messages, organizing them based on common themes or events. Topics enable consumers to express interest in specific types of messages and selectively consume relevant information. Messages published to a topic are broadcasted to all subscribers interested in that topic, facilitating asynchronous communication and event-driven interactions

#### ISBM Publication-Subscription Message Flow Diagram

![image](/Documents/Images/Publication_Flow.jpg)

#### Subscription Mechanism:

Devices or systems register their interest in receiving messages related to specific channels and Topics by subscribing to them. Subscriptions can be dynamic, allowing subscribers to subscribe or unsubscribe from channel at any time.

#### Decoupling:

The publication-subscription model enables loose coupling between providers and consumers. Provider and consumer applications do not need to be aware of each other's existence, promoting scalability, flexibility, and modularity in distributed systems.

#### Asynchronous Communication:

Communication between providers and consumers is asynchronous. Providers produce messages at their own pace, and consumers consume messages as they become available in the message bus.

#### Scalability and Flexibility:

The publication-subscription model is highly scalable and flexible, allowing systems to handle large volumes of messages and accommodate dynamic changes in the number of providers and consumers.

### Request-Response model

In this model, consumers send request messages to a central message broker (ISBM Service Provider), and providers receive request messages and provide response messages based on their services to specific channels and topics. This model is often used in client-server architectures, web services, and microservices-based applications.

#### Breakdown of the key components and concepts of the request-response message bus model

#### Consumer Applications:

The consumer applications are devices or systems that initiates communication by sending request messages to other devices or systems. A request message typically specifies the type of operation to be performed and may include parameters or data needed for the request.

#### Provider Applications:

The provider applications are devices or systems that receive the request messages and perform the requested operations. They processe the requests and generate corresponding response messages containing the result of the operations or any relevant data.

#### ISBM Service Provider:

The ISBM service provider acts as an intermediary between provider applications and consumer applications. It receives request messages from consumers and distributes them to providers based on their subscriptions. The ISBM aervice provider manages the routing and delivery of messages, ensuring that each provider application receives the relevant request messages according to its subscriptions. It may route request messages to the appropriate responders based on filter expression.

#### Request Channels:

A request channel is a logical conduit which request and response messages flow between different devices or systems within a cluster. It serves as a means of organizing and directing the transmission of messages, providing a structured communication conduit for exchanging information. Request channels help to categorize request and response messages and ensure that they reach their intended consumers efficiently. In messaging systems, channels play a crucial role in facilitating communication, enabling devices and systems to interact and share data seamlessly across distributed environments while maintaining scalability and flexibility in message routing and delivery.

#### Topics:

A topic within the channel represents a distinct subject or category of messages that are published and subscribed to by devices or systems within a cluster. It serves as a container for related messages, organizing them based on common themes or events. Topics enable consumers to express interest in specific types of messages and selectively consume relevant information. Messages published to a topic are broadcasted to all subscribers interested in that topic, facilitating asynchronous communication and event-driven interactions

#### ISBM Request-Response Message Flow Diagram

![image](/Documents/Images/Request_Response_Flow.jpg)

#### Subscription Mechanism:

Devices or systems register their interest in receiving request messages related to specific channels and topics by subscribing to them. Subscriptions can be dynamic, allowing subscribers to subscribe or unsubscribe from channel at any time.

#### Decoupling:

The request-response model enables loose coupling between consumers and providers. Consumer and provider applications do not need to be aware of each other's existence, promoting scalability, flexibility, and modularity in distributed systems.

#### Asynchronous Communication:

Unlike typical request-response model, ISBM service provider communications between consumers and providers is asynchronous. The consumer does not wait for a response from the provider(s) before proceeding with other processes. Providers produce response messages at their own pace, and consumers consume response messages as they become available in the message bus.

#### Request Message:

The request message contains information about the operation to be performed, including any necessary parameters or data. It is sent by the consumer to the provider to initiate the communication.

#### Response Message:

The response message contains the result of the operation performed by the provider. It is sent by the provider to the consumer as a reply to the request message.

#### Scalability and Flexibility:

The ISBM request-response model is in asynchronous mode, which means that the sender of a request message does not need to wait for an immediate response from the provider. This asynchronous nature helps to avoid some of the latency and performance overhead associated with synchronous request-response models. In a synchronous model, the sender typically waits for a response from the provider before continuing with further processing, which can introduce delays and impact system performance, especially in distributed environments or when dealing with high message volumes. However, in an asynchronous model like ISBM, the sender can continue with other tasks or processes while waiting for the response, which can help improve system efficiency and responsiveness.

### Use Cases

#### 1. Example of using Publication-Subscription model:  [Smart Agriculture Monitoring System](Documents/Use_Cases/Smart-Agriculture-Monitoring-System.md) 
#### 2. Example of using Request-Response model:  [Fleet Management System for Logistics](Documents/Use_Cases/Fleet_Management.md)
#### 3. Example of using both Publication-Subscription and Request-Response models:  [Flood Management System](Documents/Use_Cases/Flood-Management.md)

### Project Information

The C# toolkit samples in this repository are aligned with the released NuGet package:

```xml
<PackageReference Include="RapidRedPanda.ISBM.ClientAdapter" Version="2.1.1" />
```

The samples are intended to show how real applications consume the public ClientAdapter API from NuGet. They do not require an adjacent ClientAdapter source repository or locally copied ClientAdapter DLLs.

Included sample applications:

1. Windows desktop Provider Publication sample
2. Windows desktop Consumer Publication sample
3. Windows desktop Consumer Request sample
4. Windows desktop Provider Request sample
5. Windows desktop Channel Management sample
6. Raspberry Pi Publication sample
7. Raspberry Pi Request sample
8. ClientAdapter validation runner

The Windows 10 IoT Core / UWP sample has been dropped from the active sample set.

### Before Running the Program

Restore NuGet packages before building or running a sample. Visual Studio can restore automatically, or you can run `dotnet restore` / MSBuild restore for the selected project.

The desktop Provider Publication, Consumer Request, and Provider Request samples include a **Media Type** field for posting payloads:

- Leave **Media Type** blank to use native JSON object mode. The payload must be a JSON object, for example `{"value":42}`.
- Enter `application/xml` to send XML text exactly as typed, for example `<reading><value>42</value></reading>`.
- Enter `text/plain` to send plain text exactly as typed.
- Enter `application/json` to send JSON text as string content. Blank and `application/json` are intentionally different modes.

The validation runner is located at `CSharp/ValidationRunner/ISBM20ClientAdapterValidationRunner`. Running it with no arguments validates that the project resolves `RapidRedPanda.ISBM.ClientAdapter` 2.1.1 and that the expected interfaces, async overloads, and options are present. To validate against a live ISBM server, pass `--host <url>` and, if needed, `--username <user>` and `--password <password>`. Live validation creates temporary channels and security tokens, then removes them before exiting.

### Useful Links

#### Standard Organizations
   1. [OpenO&M](https://openoandm.org/)
   2. [MIMOSA](https://www.mimosa.org/)
   3. [International Society of Automation](https://www.isa.org/)
   4. [OAGi](https://oagi.org/)

#### Development Tools
   1. [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/)
   2. [RapidRedPanda.ISBM.ClientAdapter](https://www.nuget.org/packages/RapidRedPanda.ISBM.ClientAdapter/#readme-body-tab)
   

### Quick Reference

   1. OIIE - [OpenO&M Open Industrial Interoperability Ecosystem](https://www.mimosa.org/open-industrial-interoperability-ecosystem-oiie/)
   2. ISBM - [International Society of Automation ISA-95 Message Service Model](https://openoandm.org/files/standards/ISBM-2.0.pdf)
   3. CCOM - [MIMOSA Common Conceptual Object Model](https://www.mimosa.org/mimosa-ccom/)
   4. BOD - [OAGIS Business Object Document](https://www.oagidocs.org/docs/)

 

