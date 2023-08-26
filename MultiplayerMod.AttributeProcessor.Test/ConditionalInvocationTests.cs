using System;
using NUnit.Framework;

namespace MultiplayerMod.AttributeProcessor.Test;

[TestFixture]
public class ConditionalInvocationTests {

    public static bool AllowInvocation;
    public static bool Invoked;

    [AttributeUsage(AttributeTargets.Method)]
    [ConditionalInvocation(typeof(ControlledInvocationAttribute), nameof(Ignore))]
    public class ControlledInvocationAttribute : Attribute {
        private static bool Ignore() => AllowInvocation;
    }

    [AttributeUsage(AttributeTargets.Method)]
    [ConditionalInvocation(typeof(InvokableAttribute), nameof(Ignore))]
    public class InvokableAttribute : Attribute {
        public bool Allow { get; }

        public InvokableAttribute(bool allow) {
            Allow = allow;
        }

        public static bool Ignore(bool allow) => allow;
    }

    [Test]
    public void InvocationMustBeIgnored() {
        AllowInvocation = false;
        Invoked = false;
        SimpleMethod();
        Assert.False(Invoked);
    }

    [Test]
    public void InvocationMustBeInvoked() {
        AllowInvocation = true;
        Invoked = false;
        SimpleMethod();
        Assert.True(Invoked);
    }

    [Test]
    public void ParametrizedInvocationMustBeIgnored() {
        Invoked = false;
        NonInvokable();
        Assert.False(Invoked);
    }

    [Test]
    public void ParametrizedInvocationMustBeInvoked() {
        Invoked = false;
        Invokable();
        Assert.True(Invoked);
    }

    [Test]
    public void MethodInvocationWithTrailingThrowMustBeIgnored() {
        AllowInvocation = false;
        Invoked = false;
        MethodWithTrailingThrow();
        Assert.False(Invoked);
    }

    [Test]
    public void MethodInvocationWithTrailingThrowMustBeInvoked() {
        AllowInvocation = true;
        Invoked = false;
        Assert.Throws<Exception>(MethodWithTrailingThrow);
    }

    [ControlledInvocation]
    private void SimpleMethod() {
        Invoked = true;
    }

    [Invokable(false)]
    private void NonInvokable() {
        Invoked = true;
    }

    [Invokable(true)]
    private void Invokable() {
        Invoked = true;
    }

    [ControlledInvocation]
    private void MethodWithTrailingThrow() {
        Invoked = true;
        throw new Exception();
    }

}
