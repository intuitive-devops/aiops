using System.Diagnostics;
using Xunit;

namespace Cartheur.Demo.IntegrationTests;

public class KubernetesDeploymentIntegrationTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public void PodmanDesktopKubernetes_ApplyAndTeardown_Works()
    {
        if (!IntegrationEnabled())
        {
            return;
        }

        var repoRoot = ResolveRepoRoot();
        var namespaceName = "aiops-it";
        var deploymentDir = Path.Combine(repoRoot, "deployment");

        Run("kubectl version --client", repoRoot);

        try
        {
            var namespaceRewriteScript = $"awk -v ns={namespaceName} '/^[[:space:]]*namespace:[[:space:]]*/{{$0=\"  namespace: \" ns}}1'";

            Run($"kubectl create namespace {namespaceName} --dry-run=client -o yaml | kubectl apply -f -", repoRoot);
            Run($"{namespaceRewriteScript} {Path.Combine(deploymentDir, "aiops-noise.configmap.yaml")} | kubectl apply -f -", repoRoot);
            Run($"{namespaceRewriteScript} {Path.Combine(deploymentDir, "aiops-level-1.deployment.yaml")} | kubectl apply -f -", repoRoot);
            Run($"{namespaceRewriteScript} {Path.Combine(deploymentDir, "aiops-level-1.service.yaml")} | kubectl apply -f -", repoRoot);
            Run($"kubectl apply -f {Path.Combine(deploymentDir, "diagnostics.yaml")} -n {namespaceName}", repoRoot);

            var deployment = Run($"kubectl get deployment aipos-level-1 -n {namespaceName} -o name", repoRoot);
            var service = Run($"kubectl get svc aiops-level-one-service -n {namespaceName} -o name", repoRoot);
            var configMap = Run($"kubectl get configmap aiops-noise-configmap -n {namespaceName} -o name", repoRoot);

            Assert.Contains("deployment.apps/aipos-level-1", deployment.StandardOutput);
            Assert.Contains("service/aiops-level-one-service", service.StandardOutput);
            Assert.Contains("configmap/aiops-noise-configmap", configMap.StandardOutput);
        }
        finally
        {
            Run($"kubectl delete namespace {namespaceName} --wait=true --ignore-not-found=true", repoRoot);
        }
    }

    private static bool IntegrationEnabled()
    {
        return string.Equals(Environment.GetEnvironmentVariable("RUN_K8S_INTEGRATION"), "1", StringComparison.Ordinal);
    }

    private static string ResolveRepoRoot()
    {
        var envRoot = Environment.GetEnvironmentVariable("AIOPS_REPO_ROOT");
        if (!string.IsNullOrWhiteSpace(envRoot) && Directory.Exists(envRoot))
        {
            return envRoot;
        }

        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (current != null)
        {
            var deploymentDir = Path.Combine(current.FullName, "deployment");
            var codeTwoDir = Path.Combine(current.FullName, "code-two");
            if (Directory.Exists(deploymentDir) && Directory.Exists(codeTwoDir))
            {
                return current.FullName;
            }
            current = current.Parent;
        }

        throw new InvalidOperationException("Unable to resolve repository root. Set AIOPS_REPO_ROOT.");
    }

    private static CommandResult Run(string command, string workDir)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"-lc \"{command.Replace("\"", "\\\"")}\"",
            WorkingDirectory = workDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi) ?? throw new InvalidOperationException("Failed to start process.");
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        var result = new CommandResult(process.ExitCode, stdout, stderr, command);
        if (result.ExitCode != 0)
        {
            throw new Xunit.Sdk.XunitException($"Command failed: {command}\nExitCode: {result.ExitCode}\nSTDOUT:\n{stdout}\nSTDERR:\n{stderr}");
        }

        return result;
    }

    private sealed record CommandResult(int ExitCode, string StandardOutput, string StandardError, string Command);
}
