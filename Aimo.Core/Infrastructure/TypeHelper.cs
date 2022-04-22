using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Aimo.Core.Infrastructure;

public interface ITypeHelper
{
    /*IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly>? assemblies=null, bool onlyConcreteClasses = true);*/

    IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly>? assemblies = null, bool onlyConcreteClasses = true);
}

public partial class TypeHelper : ITypeHelper
{
    private readonly bool _ignoreReflectionErrors = true;

    private IEnumerable<Assembly>? _appAssemblies;

    private IEnumerable<Assembly> GetAppAssemblies()
    {
            return _appAssemblies ??= AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => a.FullName?.StartsWith(nameof(Aimo)) ?? false);
    }

    public IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly>? assemblies = null, bool onlyConcreteClasses = true)
    {
        return FindClassesOfType(typeof(T), assemblies, onlyConcreteClasses);
    }

    public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly>? assemblies = null, bool onlyConcreteClasses = true)
    {
        var result = new List<Type>();
        try
        {
            assemblies ??= GetAppAssemblies();
            foreach (var a in assemblies)
            {
                Type[] types = null!;
                try
                {
                    types = a.GetTypes();
                }
                catch
                {
                    //Entity Framework 6 doesn't allow getting types (throws an exception)
                    if (!_ignoreReflectionErrors)
                    {
                        throw;
                    }
                }

                if (types is null)
                    continue;

                foreach (var t in types)
                {
                    if (!assignTypeFrom.IsAssignableFrom(t) && (!assignTypeFrom.IsGenericTypeDefinition ||
                                                                !DoesTypeImplementOpenGeneric(t, assignTypeFrom)))
                        continue;

                    if (t.IsInterface)
                        continue;

                    if (onlyConcreteClasses)
                    {
                        if (t.IsClass && !t.IsAbstract)
                        {
                            result.Add(t);
                        }
                    }
                    else
                    {
                        result.Add(t);
                    }
                }
            }
        }
        catch (ReflectionTypeLoadException ex)
        {
            var msg = ex.LoaderExceptions.Aggregate(string.Empty,
                (current, e) => current + e?.Message?.EmptyIfNull() + Environment.NewLine);
            var fail = new Exception(msg, ex);
            Debug.WriteLine(fail.Message, fail);

            throw fail;
        }

        return result;
    }

    protected virtual bool DoesTypeImplementOpenGeneric(Type type, Type openGeneric)
    {
        try
        {
            var genericTypeDefinition = openGeneric.GetGenericTypeDefinition();
            return type.FindInterfaces((objType, criteria) => true, null
                ).Where(implementedInterface => implementedInterface.IsGenericType)
                .Any(implementedInterface =>
                    genericTypeDefinition.IsAssignableFrom(implementedInterface.GetGenericTypeDefinition()));
        }
        catch
        {
            return false;
        }
    }

    public static Assembly GetAssemblyByName(string name)
    {
        return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == name)!;
    }

    public static object To(object value, Type destinationType)
    {
        return To(value, destinationType, CultureInfo.InvariantCulture);
    }

    public static object To(object value, Type destinationType, CultureInfo culture)
    {
        if (value is null)
            return null!;

        var sourceType = value.GetType();

        var destinationConverter = TypeDescriptor.GetConverter(destinationType);
        if (destinationConverter.CanConvertFrom(value.GetType()))
            return destinationConverter.ConvertFrom(null, culture, value)!;

        var sourceConverter = TypeDescriptor.GetConverter(sourceType);
        if (sourceConverter.CanConvertTo(destinationType))
            return sourceConverter.ConvertTo(null, culture, value, destinationType)!;

        if (destinationType.IsEnum && value is int)
            return Enum.ToObject(destinationType, (int)value);

        if (!destinationType.IsInstanceOfType(value))
            return Convert.ChangeType(value, destinationType, culture);

        return value;
    }

    public static T To<T>(object value)
    {
        //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        return (T)To(value, typeof(T));
    }
}