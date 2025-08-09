using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace GoDogKit.Architecture;

[AttributeUsage(AttributeTargets.Property)]
public class MVVMPropertyAttribute : Attribute;

public interface IMVVMModel;

public interface IMVVMView
{
    public void Update();
}

public class MVVMViewModel<TView, TModel> where TView : IMVVMView where TModel : IMVVMModel
{
    static public List<(PropertyInfo, PropertyInfo)> PropertiesMap { get; protected set; }
    public Dictionary<TView, TModel> ViewModels { get; } = [];

    static MVVMViewModel()
    {
        PropertiesMap = [];

        IEnumerable<PropertyInfo> viewProperties = GetMVVMProperties(typeof(TView));
        IEnumerable<PropertyInfo> modelProperties = GetMVVMProperties(typeof(TModel));

        for (int i = 0; i < viewProperties.Count(); i++)
        {
            if (i >= modelProperties.Count()) return;

            PropertyInfo viewProperty = viewProperties.ElementAt(i);
            PropertyInfo modelProperty = modelProperties.ElementAt(i);

            if (viewProperty.PropertyType == modelProperty.PropertyType)
            {
                PropertiesMap.Add((viewProperty, modelProperty));
            }
        }
    }

    public static IEnumerable<PropertyInfo> GetMVVMProperties(Type type)
     =>
     from property in type.GetProperties()
     where property.GetCustomAttribute<MVVMPropertyAttribute>() != null
     select property;

    public void Bind(TView view, TModel model) => ViewModels.Add(view, model);

    public bool UnBind(TView view) => ViewModels.Remove(view);

    public void Synchronize(TView view)
    {
        if (!ViewModels.TryGetValue(view, out TModel model)) return;

        foreach ((PropertyInfo viewProperty, PropertyInfo modelProperty) in PropertiesMap)
        {
            viewProperty.SetValue(view, modelProperty.GetValue(model));
        }

        view.Update();
    }

    public void SynchronizeAll()
    {
        foreach (TView view in ViewModels.Keys)
        {
            Synchronize(view);
        }
    }
}