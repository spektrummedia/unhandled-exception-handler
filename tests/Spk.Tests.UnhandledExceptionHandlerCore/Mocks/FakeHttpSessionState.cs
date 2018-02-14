using System.Reflection;
using System.Runtime.Serialization;
using System.Web;
using System.Web.SessionState;

namespace Spk.Tests.UnhandledExceptionHandlerCore.Mocks
{
    public static class FakeHttpSessionState
    {
        public static HttpSessionState Build()
        {
            var state = (HttpSessionState) FormatterServices.GetUninitializedObject(typeof(HttpSessionState));

            var containerFld = typeof(HttpSessionState).GetField(
                "_container", BindingFlags.Instance | BindingFlags.NonPublic);

            containerFld.SetValue(
                state,
                new HttpSessionStateContainer(
                    "1",
                    new SessionStateItemCollection(),
                    new HttpStaticObjectsCollection(),
                    900,
                    true,
                    HttpCookieMode.UseCookies,
                    SessionStateMode.InProc,
                    false
                )
            );

            return state;
        }
    }
}