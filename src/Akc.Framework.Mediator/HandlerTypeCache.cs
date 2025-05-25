using System.Collections.Concurrent;

namespace Akc.Framework.Mediator;

internal sealed class HandlerTypeCache : ConcurrentDictionary<Type, Type>;
