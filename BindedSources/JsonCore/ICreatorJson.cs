namespace BindedSources.JsonCore
{
    public interface ICreatorJson
    {
        void SetJsonPath(string path);
        JType ReadConfig<JType>() where JType : new();
        bool SetConfig<JType>(JType model) where JType : new();
    }
}
