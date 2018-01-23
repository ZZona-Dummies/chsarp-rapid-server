using System.Collections;
using System.Linq;

namespace RapidServer.Http
{
    // '' <summary>
    // '' Handlers are external processes or API calls that can be automated to do work or return results. Such handlers include PHP, ASP.NET, Windows Shell, etc. Interpreters are made available through custom definitions specified in the interpreters.xml file.
    // '' </summary>
    // '' <remarks></remarks>
    public class Handlers
    {
        private Hashtable _handlers = new Hashtable();

        private string _webRoot;

        private void Add(Handler h)
        {
            _handlers.Add(h.Name, h);
        }

        private Handler this[int index]
        {
            get => ((Handler)(from DictionaryEntry entry in _handlers.Values select entry.Key).Skip(index).FirstOrDefault()); //(_handlers.Values.ElementAt(index)));
        }

        private Handler this[string name]
        {
            get => ((Handler)(_handlers[name]));
        }
    }
}