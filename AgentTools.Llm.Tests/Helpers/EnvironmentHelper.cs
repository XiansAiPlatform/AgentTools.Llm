using System;
using System.IO;
using System.Linq;

namespace AgentTools.Llm.Tests.Helpers
{
    public static class EnvironmentHelper
    {
        private static bool _isLoaded;

        public static void LoadEnvFile()
        {
            if (_isLoaded) return;

            // Look for .env file in the project directory
            var projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
            var envPath = Path.Combine(projectDir, ".env");

            if (!File.Exists(envPath))
            {
                throw new FileNotFoundException(
                    ".env file not found. Please create one in the test project directory with the required environment variables.");
            }

            Console.WriteLine($"Loading .env file from: {envPath}");
            var lines = File.ReadAllLines(envPath);
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
                    continue;

                var parts = trimmedLine.Split('=', 2);
                if (parts.Length != 2) continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                // Remove quotes if present
                if (value.StartsWith("\"") && value.EndsWith("\""))
                    value = value.Substring(1, value.Length - 2);

                Environment.SetEnvironmentVariable(key, value);
                Console.WriteLine($"Loaded environment variable: {key}");
            }

            _isLoaded = true;
        }
    }
} 