using Nancy;
using Nancy.Conventions;
using Nancy.Session;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Nancy.Routing;

namespace corpdb
{

    public class bootstraper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Static", @"Static"));
            base.ConfigureConventions(nancyConventions);
        }
        protected override void ApplicationStartup(TinyIoCContainer container,IPipelines pipelines)
        {
            CookieBasedSessions.Enable(pipelines);
            base.ApplicationStartup(container, pipelines);
            pipelines.BeforeRequest.AddItemToStartOfPipeline(ctx =>
            {
                var resolver = container.Resolve<IRouteResolver>();
                var route = resolver.Resolve(ctx);
                var thingyId = route.Parameters["thingyId"];
                return null;
            });
        }
    }
}