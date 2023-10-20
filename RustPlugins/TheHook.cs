namespace Oxide.Plugins;

[Info("TheHook", "Fatih Selvi", "0.0.1")]
class TheHook : RustPlugin
{
    object OnConstructionPlace(BaseEntity entity, Construction component, Construction.Target constructionTarget, BasePlayer player)
    {
        Puts("OnConstructionPlace works!");
        return null;
    }
}
