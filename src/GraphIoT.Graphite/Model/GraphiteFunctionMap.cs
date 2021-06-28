using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    public class GraphiteFunctionMap
    {
        public class ParamDef
        {
            public string Name { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public bool Required { get; set; }
            public IList<string> Suggestions { get; set; } = new List<string>();
        }

        public class FunctionDef
        {
            public string Group { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public IList<ParamDef> Params { get; set; } = new List<ParamDef>();
            public Type FunctionExpressionType = typeof(IGraphiteExpression);

            public IGraphiteExpression? Instantiate(object[] parameters)
            {
                var constructor = FunctionExpressionType.GetConstructors().FirstOrDefault();
                if (constructor is null)
                    return null;

                return constructor.Invoke(parameters) as IGraphiteExpression;
            }
        }

        private static Dictionary<string, FunctionDef>? _functionMap = null;

        public static IEnumerable<FunctionDef> GetFunctions()
        {
            FillCache();
            if (_functionMap is null)
                yield break;

            foreach (var def in _functionMap.Values)
                yield return def;
        }

        public static bool TryGetFunction(string functionName, out FunctionDef def)
        {
            FillCache();

            def = new FunctionDef();
            if (_functionMap is null || !_functionMap.TryGetValue(functionName, out FunctionDef? found) || found is null)
                return false;

            def = found;
            return true;
        }

        private static readonly Semaphore _loadCacheSemaphore = new(1, 1);
        private static void FillCache()
        {
            try
            {
                _loadCacheSemaphore.WaitOne();

                if (_functionMap != null)
                    return;

                _functionMap = new Dictionary<string, FunctionDef>();

                var interfaceType = typeof(IGraphiteExpression);
                var types = interfaceType.Assembly.GetTypes().Where(t => t.GetInterfaces().Contains(interfaceType));
                foreach (var type in types)
                {
                    var function = type.GetCustomAttribute<GraphiteFunctionAttribute>();
                    if (function is null)
                        continue;

                    var parameters = type.GetCustomAttributes<GraphiteParamAttribute>(false)
                        .Select(p => new ParamDef() { Name = p.Name, Type = p.Type, Required = p.Required, Suggestions = p.Suggestions }).ToList();

                    _functionMap.Add(function.Name, new FunctionDef() { Name = function.Name, Group = function.Group, Params = parameters, FunctionExpressionType = type });
                }
            }
            finally
            {
                _loadCacheSemaphore.Release();
            }
        }
    }
}