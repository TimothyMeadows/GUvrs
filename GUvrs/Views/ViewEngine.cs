using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GUvrs.Views
{
    public static class ViewEngine
    {
        private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
        private static readonly Dictionary<string, HandlebarsTemplate<object, object>> _templates = new();

        public static string Render(string name, dynamic data)
        {
            HandlebarsTemplate<object, object> template;
            if (_templates.ContainsKey(name))
            {
                template = _templates[name];
            } else
            {
                var header = GetEmbeddedResource("Shared.header");
                var footer = GetEmbeddedResource("Shared.footer");
                var body = GetEmbeddedResource(name);

                template = Handlebars.Compile($"{header}{body}{footer}");
                _templates.Add(name, template);
            }

            return template(data);
        }

        private static string GetEmbeddedResource(string name)
        {
            using Stream stream = _assembly.GetManifestResourceStream($"GUvrs.Views.{name}.mustache") ?? throw new ArgumentException($"No resource with name {name}", nameof(name));
            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }
    }
}
