using System;

namespace MultiplayerMod.AttributeProcessor.MSBuild.Task.Processors;

public class AttributeContractException : Exception {
    public AttributeContractException(string message) : base(message) { }
}
