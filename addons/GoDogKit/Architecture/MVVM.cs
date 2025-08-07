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

public class MVVMViewModel
{
    public IMVVMView View { get; protected set; }
    public IMVVMModel Model { get; protected set; }
    public List<(PropertyInfo, PropertyInfo)> PropertiesMap { get; protected set; }

    public MVVMViewModel(IMVVMView view, IMVVMModel model)
    {
        View = view;
        Model = model;
        PropertiesMap = [];

        IEnumerable<PropertyInfo> viewProperties = GetMVVMProperties(View.GetType());
        IEnumerable<PropertyInfo> modelProperties = GetMVVMProperties(Model.GetType());

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

    public void Synchronize()
    {
        foreach ((PropertyInfo viewProperty, PropertyInfo modelProperty) in PropertiesMap)
        {
            viewProperty.SetValue(View, modelProperty.GetValue(Model));
        }

        View.Update();
    }
}