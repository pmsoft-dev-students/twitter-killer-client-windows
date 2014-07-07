using RestSharp;

namespace Twitter_Killer
{
    public interface IRestRequestWrapper
    {
        /// <summary>
        /// IRestRequest wrapper interface.
        /// Fine implementation:
        /// "return new RestRequest(source, method);" 
        /// </summary>
        /// <param name="resource">Resource to use for this request</param>
        /// <param name="method">Method to use for this request</param>
        /// <returns></returns>
        IRestRequest GetRequest(string resource, RestSharp.Method method);
    }
}
