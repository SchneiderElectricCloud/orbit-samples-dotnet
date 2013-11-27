using System;
using System.Linq;
using SE.Orbit.TaskServices;

namespace SampleOrbitEventListenerService.Extensions
{
    static class LinksResourceExtensions
    {
        public static LinkResource FindLink(this LinkResourceCollection resource, string linkRelation)
        {
            if (resource == null) throw new ArgumentNullException("resource");
            if (string.IsNullOrEmpty(linkRelation)) throw new ArgumentException("Must not be empty", "linkRelation");


            StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;

            LinkResource link = null;
            if (resource.Links != null)
            {
                link = resource.Links.First(l => comparer.Compare((string) l.Rel, linkRelation) == 0);
            }

            if (link == null)
            {
                throw new Exception(string.Format("Cannot find link relation '{0}' on {1} resource",
                                                  linkRelation, resource.GetType().Name));
            }

            return link;
        }
    }
}