// Copyright 2026 The AnkhSVN Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.ServiceHub.Framework;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.ServiceBroker;

namespace Ankh.Copilot
{
    /// <summary>
    /// Late-bound bridge to Visual Studio's own Copilot brokered service.
    ///
    /// AnkhSVN deliberately does not ship GitHub's standalone Copilot SDK/CLI.
    /// Instead it resolves the Copilot contract from the running Visual Studio,
    /// so the request uses the same Copilot sign-in and account policy as VS.
    /// </summary>
    public static class VisualStudioCopilot
    {
        const string CopilotNamespace = "Microsoft.VisualStudio.Copilot.";

        const string CommitMessageGuidance =
            "This is a headless, non-interactive commit-message generation request from AnkhSVN. " +
            "The request already contains the complete SVN pending-change data needed for the task. " +
            "Use only that supplied data. Do not ask for Visual Studio editor selections, active-file context, " +
            "error-list context, chat references, files, or any additional workspace context. " +
            "Do not explain limitations or offer choices. " +
            "Place the final commit message, and only the final commit message, between " +
            "<commit-message> and </commit-message> tags. Any planning or reasoning outside those tags is ignored.";

        static readonly string[] CopilotAssemblyNames =
        {
            "Microsoft.VisualStudio.Copilot",
            "Microsoft.VisualStudio.Copilot.Abstractions",
            "Microsoft.VisualStudio.Copilot.Contracts"
        };

        // GitHub uses small utility models for background features such as
        // commit-message generation. Prefer those families when the running
        // Visual Studio Copilot responder reports them as available, rather
        // than inheriting the user's interactive Chat/reasoning model.
        static readonly string[] CommitMessageModelFamilies =
        {
            "gpt-4o-mini",
            "gpt-4o",
            "gpt-4.1",
            "gpt-5.4-nano"
        };

        public static async Task<string> GenerateAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("A Copilot prompt is required.", "prompt");

            Type serviceType = GetRequiredCopilotType("ICopilotService");
            Type descriptorsType = GetRequiredCopilotType("CopilotDescriptors");

            PropertyInfo serviceDescriptorProperty = descriptorsType.GetProperty(
                "CopilotService",
                BindingFlags.Public | BindingFlags.Static);

            if (serviceDescriptorProperty == null)
                throw new MissingMemberException(descriptorsType.FullName, "CopilotService");

            ServiceRpcDescriptor serviceDescriptor =
                serviceDescriptorProperty.GetValue(null, null) as ServiceRpcDescriptor;

            if (serviceDescriptor == null)
                throw new InvalidOperationException("Visual Studio Copilot exposed an invalid service descriptor.");

            IBrokeredServiceContainer container =
                await AsyncServiceProvider.GlobalProvider
                    .GetServiceAsync<SVsBrokeredServiceContainer, IBrokeredServiceContainer>();

            if (container == null)
                throw new InvalidOperationException("Visual Studio's brokered service container is unavailable.");

            IServiceBroker serviceBroker = container.GetFullAccessServiceBroker();
            if (serviceBroker == null)
                throw new InvalidOperationException("Visual Studio's full-access service broker is unavailable.");

            object copilotService = await GetProxyAsync(
                serviceBroker,
                serviceDescriptor,
                serviceType,
                CancellationToken.None);

            if (copilotService == null)
            {
                throw new InvalidOperationException(
                    "Visual Studio Copilot is not available. " +
                    "Make sure GitHub Copilot is installed, enabled, and signed in inside Visual Studio.");
            }

            try
            {
                await VerifyAvailabilityAsync(serviceType, copilotService);

                object session = await StartSessionAsync(serviceType, copilotService);
                if (session == null)
                    throw new InvalidOperationException("Visual Studio Copilot could not start a session.");

                try
                {
                    object response = await SendRequestAsync(session, prompt);
                    return ExtractResponseText(response);
                }
                finally
                {
                    DisposeObject(session);
                }
            }
            finally
            {
                DisposeObject(copilotService);
            }
        }

        static async Task<object> GetProxyAsync(
            IServiceBroker serviceBroker,
            ServiceRpcDescriptor serviceDescriptor,
            Type serviceType,
            CancellationToken cancellationToken)
        {
            MethodInfo getProxy = typeof(IServiceBroker)
                .GetMethods()
                .Where(m => m.Name == "GetProxyAsync" && m.IsGenericMethodDefinition)
                .FirstOrDefault(m =>
                {
                    ParameterInfo[] parameters = m.GetParameters();
                    return parameters.Length > 0 &&
                        parameters[0].ParameterType == typeof(ServiceRpcDescriptor);
                });

            if (getProxy == null)
                throw new MissingMethodException(typeof(IServiceBroker).FullName, "GetProxyAsync");

            MethodInfo closedMethod = getProxy.MakeGenericMethod(serviceType);
            ParameterInfo[] methodParameters = closedMethod.GetParameters();
            object[] arguments = new object[methodParameters.Length];

            arguments[0] = serviceDescriptor;

            for (int i = 1; i < methodParameters.Length; i++)
            {
                Type parameterType = methodParameters[i].ParameterType;

                if (parameterType == typeof(CancellationToken))
                    arguments[i] = cancellationToken;
                else if (parameterType.IsValueType)
                    arguments[i] = Activator.CreateInstance(parameterType);
                else
                    arguments[i] = null;
            }

            object invocation = closedMethod.Invoke(serviceBroker, arguments);
            return await AwaitResultAsync(invocation);
        }

        static async Task VerifyAvailabilityAsync(Type serviceType, object copilotService)
        {
            MethodInfo checkAvailability = serviceType.GetMethod(
                "CheckAvailabilityAsync",
                new[] { typeof(CancellationToken) });

            if (checkAvailability == null)
                return;

            object invocation = checkAvailability.Invoke(
                copilotService,
                new object[] { CancellationToken.None });

            object result = await AwaitResultAsync(invocation);
            if (result is bool && !(bool)result)
            {
                throw new InvalidOperationException(
                    "Visual Studio Copilot is installed but is not currently available. " +
                    "Make sure Copilot is enabled and signed in.");
            }
        }

        static async Task<object> StartSessionAsync(Type serviceType, object copilotService)
        {
            Type clientIdType = GetRequiredCopilotType("CopilotClientId");
            object clientId = Activator.CreateInstance(
                clientIdType,
                new object[] { "AnkhSVN.CommitMessage" });

            Type sessionOptionsType = FindCopilotType("CopilotSessionOptions");
            if (sessionOptionsType != null)
            {
                MethodInfo startSession = serviceType.GetMethod(
                    "StartSessionAsync",
                    new[] { sessionOptionsType, typeof(CancellationToken) });

                if (startSession != null)
                {
                    object options = Activator.CreateInstance(
                        sessionOptionsType,
                        new[] { clientId });

                    object invocation = startSession.Invoke(
                        copilotService,
                        new[] { options, (object)CancellationToken.None });

                    return await AwaitResultAsync(invocation);
                }
            }

            // Older VS 2022 Copilot builds exposed GetCopilotSessionAsync instead.
            Type serviceOptionsType = FindCopilotType("CopilotServiceSessionOptions");
            if (serviceOptionsType != null)
            {
                MethodInfo getSession = serviceType.GetMethod(
                    "GetCopilotSessionAsync",
                    new[] { serviceOptionsType, typeof(CancellationToken) });

                if (getSession != null)
                {
                    object options = Activator.CreateInstance(
                        serviceOptionsType,
                        new[] { clientId });

                    PropertyInfo provideUi = serviceOptionsType.GetProperty("ProvideUI");
                    if (provideUi != null && provideUi.CanWrite)
                        provideUi.SetValue(options, false, null);

                    object invocation = getSession.Invoke(
                        copilotService,
                        new[] { options, (object)CancellationToken.None });

                    return await AwaitResultAsync(invocation);
                }
            }

            throw new MissingMethodException(
                "The installed Visual Studio Copilot contract does not expose a compatible session API.");
        }

        static async Task<object> SendRequestAsync(object session, string prompt)
        {
            Type requestType = GetRequiredCopilotType("CopilotRequest");
            object request = Activator.CreateInstance(requestType, new object[] { prompt });

            await ConfigureCommitMessageRequestAsync(session, request);

            MethodInfo sendRequest = session.GetType().GetMethod(
                "SendRequestAsync",
                new[] { requestType, typeof(CancellationToken) });

            if (sendRequest == null)
            {
                Type sessionType = GetRequiredCopilotType("ICopilotSession");
                sendRequest = sessionType.GetMethod(
                    "SendRequestAsync",
                    new[] { requestType, typeof(CancellationToken) });
            }

            if (sendRequest == null)
                throw new MissingMethodException(session.GetType().FullName, "SendRequestAsync");

            object invocation = sendRequest.Invoke(
                session,
                new[] { request, (object)CancellationToken.None });

            return await AwaitResultAsync(invocation);
        }

        static async Task ConfigureCommitMessageRequestAsync(object session, object request)
        {
            if (request == null)
                return;

            // Newer and current Copilot contracts expose Guidance, which is
            // equivalent to adding app-specific system guidance. Older VS 2022
            // builds may not have it, so this remains feature-detected.
            PropertyInfo guidanceProperty = request.GetType().GetProperty(
                "Guidance",
                BindingFlags.Public | BindingFlags.Instance);

            if (guidanceProperty != null &&
                guidanceProperty.CanWrite &&
                guidanceProperty.PropertyType == typeof(string))
            {
                guidanceProperty.SetValue(request, CommitMessageGuidance, null);
            }

            // A null Intent asks Copilot to auto-detect intent. VS 2022 can
            // interpret commit generation as an interactive code/chat request
            // and then ask for editor selections or chat references. None means
            // pass the request through without that intent-specific behavior.
            PropertyInfo intentProperty = request.GetType().GetProperty(
                "Intent",
                BindingFlags.Public | BindingFlags.Instance);

            if (intentProperty == null || !intentProperty.CanWrite)
                return;

            Type intentType = Nullable.GetUnderlyingType(intentProperty.PropertyType)
                ?? intentProperty.PropertyType;

            if (!intentType.IsEnum ||
                !Enum.GetNames(intentType).Contains("None"))
            {
                return;
            }

            object noIntent = Enum.Parse(intentType, "None", false);
            intentProperty.SetValue(request, noIntent, null);

            await TryUseCommitMessageUtilityModelAsync(session, request);
        }

        static async Task TryUseCommitMessageUtilityModelAsync(
            object session,
            object request)
        {
            if (session == null || request == null)
                return;

            PropertyInfo modelProperty = request.GetType().GetProperty(
                "Model",
                BindingFlags.Public | BindingFlags.Instance);

            if (modelProperty == null || !modelProperty.CanWrite)
                return;

            IEnumerable models = null;

            try
            {
                MethodInfo getModels = session.GetType().GetMethod(
                    "GetDefaultModelsAsync",
                    new[] { typeof(CancellationToken) });

                if (getModels == null)
                {
                    Type sessionType = FindCopilotType("ICopilotSession");
                    if (sessionType != null)
                    {
                        getModels = sessionType.GetMethod(
                            "GetDefaultModelsAsync",
                            new[] { typeof(CancellationToken) });
                    }
                }

                if (getModels == null)
                    return;

                object invocation = getModels.Invoke(
                    session,
                    new object[] { CancellationToken.None });

                object result = await AwaitResultAsync(invocation);
                models = result as IEnumerable;
            }
            catch
            {
                // Model discovery is an optional optimization. If a particular
                // VS Copilot build cannot enumerate models, keep its default
                // model and rely on the response filtering fallback.
                return;
            }

            string family = SelectCommitMessageModelFamily(models);
            if (string.IsNullOrEmpty(family))
                return;

            Type modelRequestType = Nullable.GetUnderlyingType(
                modelProperty.PropertyType) ?? modelProperty.PropertyType;

            ConstructorInfo constructor = modelRequestType.GetConstructor(
                new[] { typeof(string) });

            if (constructor == null)
                return;

            object modelRequest = constructor.Invoke(new object[] { family });
            modelProperty.SetValue(request, modelRequest, null);
        }

        static string SelectCommitMessageModelFamily(IEnumerable models)
        {
            if (models == null)
                return null;

            HashSet<string> availableFamilies = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            foreach (object model in models)
            {
                if (model == null)
                    continue;

                PropertyInfo familyProperty = model.GetType().GetProperty(
                    "Family",
                    BindingFlags.Public | BindingFlags.Instance);

                string family = familyProperty != null
                    ? familyProperty.GetValue(model, null) as string
                    : null;

                if (!string.IsNullOrWhiteSpace(family))
                    availableFamilies.Add(family.Trim());
            }

            foreach (string preferredFamily in CommitMessageModelFamilies)
            {
                if (availableFamilies.Contains(preferredFamily))
                    return preferredFamily;
            }

            return null;
        }

        static string ExtractResponseText(object response)
        {
            if (response == null)
                throw new InvalidOperationException("Visual Studio Copilot returned no response.");

            PropertyInfo contentProperty = response.GetType().GetProperty(
                "Content",
                BindingFlags.Public | BindingFlags.Instance);

            IEnumerable content = contentProperty != null
                ? contentProperty.GetValue(response, null) as IEnumerable
                : null;

            List<string> visibleTextParts = new List<string>();

            if (content != null)
            {
                foreach (object part in content)
                {
                    if (part == null)
                        continue;

                    if (!IsUserVisibleContentPart(part))
                        continue;

                    PropertyInfo partContent = part.GetType().GetProperty(
                        "Content",
                        BindingFlags.Public | BindingFlags.Instance);

                    if (partContent == null || partContent.PropertyType != typeof(string))
                        continue;

                    string value = partContent.GetValue(part, null) as string;
                    if (string.IsNullOrWhiteSpace(value))
                        continue;

                    visibleTextParts.Add(value.Trim());
                }
            }

            // Current VS 2026 Copilot builds can return user-visible planning
            // as earlier content parts and the actual answer as the final text
            // part. Prefer an explicitly enveloped part; otherwise prefer the
            // last user-visible text part instead of concatenating every part.
            for (int i = visibleTextParts.Count - 1; i >= 0; i--)
            {
                if (visibleTextParts[i].IndexOf(
                        "<commit-message>",
                        StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return visibleTextParts[i];
                }
            }

            if (visibleTextParts.Count > 0)
                return visibleTextParts[visibleTextParts.Count - 1];

            PropertyInfo statusProperty = response.GetType().GetProperty(
                "Status",
                BindingFlags.Public | BindingFlags.Instance);

            object status = statusProperty != null
                ? statusProperty.GetValue(response, null)
                : null;

            throw new InvalidOperationException(
                "Visual Studio Copilot returned a response without commit-message text" +
                (status != null ? " (status: " + status + ")." : "."));
        }

        static bool IsUserVisibleContentPart(object part)
        {
            PropertyInfo visibilityProperty = part.GetType().GetProperty(
                "Visibility",
                BindingFlags.Public | BindingFlags.Instance);

            if (visibilityProperty == null)
                return true;

            object visibility = visibilityProperty.GetValue(part, null);
            if (visibility == null)
                return true;

            // CopilotContentVisibility.Model is internal/model-only material
            // such as reasoning or planning. User and All are safe to surface.
            // Unknown future values are left visible for compatibility; only
            // the explicitly model-only value is suppressed.
            return !string.Equals(
                visibility.ToString(),
                "Model",
                StringComparison.OrdinalIgnoreCase);
        }

        static async Task<object> AwaitResultAsync(object awaitable)
        {
            if (awaitable == null)
                return null;

            Task task = awaitable as Task;
            if (task != null)
            {
                await task;
                return GetTaskResult(task);
            }

            Type awaitableType = awaitable.GetType();

            // IServiceBroker returns ValueTask<T>. Keep this late-bound as well
            // so the bridge remains compatible with net472 and VS SDK versions.
            MethodInfo asTask = awaitableType.GetMethod(
                "AsTask",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                Type.EmptyTypes,
                null);

            if (asTask != null)
            {
                Task converted = asTask.Invoke(awaitable, null) as Task;
                if (converted == null)
                    throw new InvalidOperationException("Visual Studio returned an invalid asynchronous result.");

                await converted;
                return GetTaskResult(converted);
            }

            return awaitable;
        }

        static object GetTaskResult(Task task)
        {
            PropertyInfo resultProperty = task.GetType().GetProperty(
                "Result",
                BindingFlags.Public | BindingFlags.Instance);

            return resultProperty != null
                ? resultProperty.GetValue(task, null)
                : null;
        }

        static Type GetRequiredCopilotType(string typeName)
        {
            Type type = FindCopilotType(typeName);
            if (type != null)
                return type;

            throw new InvalidOperationException(
                "Visual Studio's Copilot contract type '" + typeName + "' could not be found. " +
                "Install or enable GitHub Copilot in this Visual Studio installation.");
        }

        static Type FindCopilotType(string typeName)
        {
            string fullName = CopilotNamespace + typeName;

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = SafeGetType(assembly, fullName);
                if (type != null)
                    return type;
            }

            foreach (string assemblyName in CopilotAssemblyNames)
            {
                try
                {
                    Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
                    Type type = SafeGetType(assembly, fullName);
                    if (type != null)
                        return type;
                }
                catch (Exception ex) when (
                    ex is FileNotFoundException ||
                    ex is FileLoadException ||
                    ex is BadImageFormatException)
                {
                }
            }

            foreach (string directory in GetCopilotSearchDirectories())
            {
                if (!Directory.Exists(directory))
                    continue;

                IEnumerable<string> files;
                try
                {
                    files = Directory.EnumerateFiles(
                        directory,
                        "Microsoft.VisualStudio.Copilot*.dll",
                        SearchOption.AllDirectories);
                }
                catch (Exception ex) when (
                    ex is IOException ||
                    ex is UnauthorizedAccessException)
                {
                    continue;
                }

                foreach (string file in files)
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFrom(file);
                        Type type = SafeGetType(assembly, fullName);
                        if (type != null)
                            return type;
                    }
                    catch (Exception ex) when (
                        ex is FileNotFoundException ||
                        ex is FileLoadException ||
                        ex is BadImageFormatException)
                    {
                    }
                }
            }

            return null;
        }

        static Type SafeGetType(Assembly assembly, string fullName)
        {
            try
            {
                return assembly.GetType(fullName, false, false);
            }
            catch (Exception ex) when (
                ex is FileNotFoundException ||
                ex is FileLoadException ||
                ex is TypeLoadException)
            {
                return null;
            }
        }

        static IEnumerable<string> GetCopilotSearchDirectories()
        {
            HashSet<string> directories = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            AddCopilotDirectory(directories, baseDirectory);

            string installDirectory = Environment.GetEnvironmentVariable("VSINSTALLDIR");
            if (!string.IsNullOrEmpty(installDirectory))
            {
                AddCopilotDirectory(
                    directories,
                    Path.Combine(installDirectory, "Common7", "IDE"));
            }

            return directories;
        }

        static void AddCopilotDirectory(HashSet<string> directories, string ideDirectory)
        {
            if (string.IsNullOrEmpty(ideDirectory))
                return;

            directories.Add(Path.Combine(
                ideDirectory,
                "Extensions",
                "Microsoft",
                "Copilot"));

            directories.Add(Path.Combine(
                ideDirectory,
                "CommonExtensions",
                "Microsoft",
                "Copilot"));
        }

        static void DisposeObject(object value)
        {
            IDisposable disposable = value as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}
