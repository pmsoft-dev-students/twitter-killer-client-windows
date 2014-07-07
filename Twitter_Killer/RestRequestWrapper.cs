using RestSharp;

namespace Twitter_Killer
{
    class RestRequestWrapper : IRestRequestWrapper
    {
        public IRestRequest GetRequest(string resource, Method method)
        {
            return new RestRequest(resource, method);
        }
    }
}
