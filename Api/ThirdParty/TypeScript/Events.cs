
using System;
using System.Collections.Generic;
using System.Text;
using Api.CanvasRenderer;

namespace Api.Eventing
{
    /// <summary>
    /// Events are instanced automatically. 
    /// You can however specify a custom type or instance them yourself if you'd like to do so.
    /// </summary>
    public partial class Events
    {
        /// <summary>
        /// Custom event handlers to handle typescript related events. 
        /// </summary>
        public static TypeScriptEvents TypeScript = new();
    }

    public class TypeScriptEvents
    {
        public ObjectEvent<StringBuilder> TSConfigPaths = new();
        
        public ObjectEvent<SourceFileContainer> ApiContainer = new();
    }

    public class ObjectEvent<T>
    {
        private readonly List<Func<T, T>> Handlers = [];

        public void AddEventListener(Func<T, T> handler)
        {
            Handlers.Add(handler);
        }

        public T Dispatch(T item)
        {
            foreach (var handler in Handlers)
            {
                item = handler(item);
            }

            return item;
        }
    }
}