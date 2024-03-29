﻿using System;
using System.Runtime.Serialization;

namespace MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;

public static class SerializationSurrogates {

    public static readonly SurrogateSelector Selector = new();

    static SerializationSurrogates() {
        Selector.Add(new Vector2SerializationSurrogate());
        Selector.Add(new Vector2fSerializationSurrogate());
        Selector.Add(new Vector3SerializationSurrogate());
        Selector.Add(new PrioritySettingSurrogate());
        Selector.Add(new TagSurrogate());
        Selector.Add(new PathNodeSurrogate());
        Selector.Add(new AssignmentGroupSurrogate());
        Selector.Add(new CarePackageInstanceDataSurrogate());
        Selector.Add(new ChoreTypeSurrogate());
        Selector.Add(new ComplexRecipeSurrogate());
        Selector.Add(new DeathSurrogate());
        Selector.Add(new EmoteSurrogate());
        Selector.Add(new KAnimFileSurrogate());
        Selector.Add(new MinionStartingStatsSurrogate());
        Selector.Add(new RoomSurrogate());
        Selector.Add(new ScheduleBlockTypeSurrogate());
        Selector.Add(new SpaceDestinationSurrogate());
        Selector.Add(new SpiceGrinderSurrogate());
    }

    private static void Add<T>(this SurrogateSelector selector, T surrogate)
        where T : ISerializationSurrogate, ISurrogateType {
        selector.AddSurrogate(surrogate.Type, new StreamingContext(StreamingContextStates.All), surrogate);
    }

    public static bool HasSurrogate(Type type) {
        return Selector.GetSurrogate(type, new StreamingContext(StreamingContextStates.All), out var _) != null;
    }

}
