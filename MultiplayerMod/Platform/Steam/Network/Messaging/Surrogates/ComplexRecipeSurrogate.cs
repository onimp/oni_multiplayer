using System;
using System.Linq;
using System.Runtime.Serialization;

namespace MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;

public class ComplexRecipeSurrogate : ISerializationSurrogate, ISurrogateType {

    public Type Type => typeof(ComplexRecipe);

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
        var recipe = (ComplexRecipe) obj;
        info.AddValue("id", recipe.id);
    }

    public object SetObjectData(
        object obj,
        SerializationInfo info,
        StreamingContext context,
        ISurrogateSelector selector
    ) {
        var id = info.GetString("id");
        var recipe = ComplexRecipeManager.Get().recipes.Single(recipe => recipe.id == id);
        return recipe;
    }

}
