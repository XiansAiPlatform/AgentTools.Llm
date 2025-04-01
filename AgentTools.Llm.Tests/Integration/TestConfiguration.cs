using System;
using AgentTools.Llm.Configuration;
using AgentTools.Llm.Tests.Helpers;

namespace AgentTools.Llm.Tests.Integration
{
    public static class TestConfiguration
    {
        static TestConfiguration()
        {
            EnvironmentHelper.LoadEnvFile();
        }

        public static OpenAiConfiguration GetOpenAiConfiguration()
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                ?? throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set");

            var modelName = Environment.GetEnvironmentVariable("OPENAI_TEST_MODEL")
                ?? "gpt-3.5-turbo";

            var organizationId = Environment.GetEnvironmentVariable("OPENAI_ORG_ID");

            return new OpenAiConfiguration(apiKey, modelName, organizationId);
        }        
        
        public static ClaudeConfiguration GetClaudeConfiguration()
        {
            var apiKey = Environment.GetEnvironmentVariable("CLAUDE_API_KEY")
                ?? throw new InvalidOperationException("CLAUDE_API_KEY environment variable is not set");
        
            var modelName = Environment.GetEnvironmentVariable("CLAUDE_TEST_MODEL")
                ?? "claude-3-haiku-20240307";
        
            return new ClaudeConfiguration(apiKey, modelName);
        }

        public static void ValidateEnvironment()
        {
            var requiredVars = new[]
            {
                "OPENAI_API_KEY",
                "CLAUDE_API_KEY"
            };

            foreach (var varName in requiredVars)
            {
                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(varName)))
                {
                    throw new InvalidOperationException(
                        $"Required environment variable '{varName}' is not set. " +
                        "Please check your .env file and ensure all required variables are set.");
                }
            }
        }
    }
} 