using System.Collections.Concurrent;
using System.Reflection;

namespace Akc.Framework.Mediator;

internal sealed class HandlerMethodCache : ConcurrentDictionary<Type, MethodInfo>;