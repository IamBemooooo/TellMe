
namespace TellMe.Core
{
    // Simplified dependency registration placeholder to avoid requiring
    // Microsoft.Extensions.DependencyInjection at this stage. This keeps
    // the project buildable while folders/files are scaffolded.
    public static class DependencyInjection
    {
        public static object AddCore(this object services)
        {
            // register core services
            return services;
        }
    }
}
